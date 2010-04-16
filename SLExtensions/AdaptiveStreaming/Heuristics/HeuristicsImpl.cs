//*@@@*************************************************************************
//
// Microsoft Expression Encoder
// Copyright © Microsoft Corporation. All rights reserved.
//
//*@@@*************************************************************************
namespace MS.Internal.Expression.Encoder.AdaptiveStreaming.Heuristic
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Windows.Controls;
    using System.Windows.Media;
    using Microsoft.Expression.Encoder.AdaptiveStreaming;
    using MS.Internal.Expression.Encoder.AdaptiveStreaming.Advertising;
    using MS.Internal.Expression.Encoder.AdaptiveStreaming.Logging;
    using MS.Internal.Expression.Encoder.AdaptiveStreaming.Manifest;
    using MS.Internal.Expression.Encoder.AdaptiveStreaming.Network;
    using MS.Internal.Expression.Encoder.AdaptiveStreaming.Utilities;

    /// <summary>
    /// This is one half of the Heuristics implementation. This file contains all of the implementations
    /// for scheduling downloads and responding to events. The NetworkHeuristicsImpl file contains
    /// the actual algorithm for selecting the next and current stream to play and download.
    /// </summary>
    internal partial class HeuristicsImpl : Heuristics
    {
        /// <summary>
        /// The minimal video before we must have before pausing for an ad
        /// </summary>
        private const long AdPausableMinimalBuffer = 40000000; // 4 seconds in 100ns units

        /// <summary>
        /// The maximum allowable time after an ad insertion point that we can pause at
        /// </summary>
        private const long AdPausableZoneAfter = 20000000; // 2 seconds in 100ns units

        /// <summary>
        /// The maximum allowable time before an ad insertion point that we can pause at
        /// </summary>
        private const long AdPausableZoneBefore = 30000000; // 3 seconds in 100ns units

        /// <summary>
        /// The maximum number of downloads per stream. We only support 1 download per stream at any one time.
        /// </summary>
        private const int GlobalMaxDownloadsPerStream = 1;

        /// <summary>
        /// SessionId tracker which is useful for tracing
        /// </summary>
        private static Guid sm_sessionId = Guid.NewGuid();

        /// <summary>
        /// The list of ad insertion points we are expecting
        /// </summary>
        private AdInsertionPoints m_adInsertPoints;

        /// <summary>
        /// This is only used if m_HeuristicMode is HeuristicMode.Fixed. It is hardcoded
        /// to use the first bitrate in the bitrate list.
        /// </summary>
        private int[] m_fixedBitrateIndex;

        /// <summary>
        /// The heuristic mode to use. We support two modes, full and fixed. Full performs
        /// the full heuristics algorithm, while fixed uses the the first bitrate in the manifest
        /// file. This is handled on a per stream basis.
        /// </summary>
        private HeuristicsMode[] m_heuristicsMode;

        /// <summary>
        /// This flag keeps track of whether or not we have paused our downloads. Typically,
        /// this occurs because we have received an ad insertion pause request.
        /// </summary>
        private bool m_isDownloadsPaused;

        /// <summary>
        /// Flag which when true, tells us to refresh our next and last ad insertion points
        /// </summary>
        private bool m_isRefreshAdInsertionPoints;

        /// <summary>
        /// This flag is used in the download scheduler to hold off downloading any new chunks
        /// while we are in the middle of a seek.
        /// </summary>
        private bool m_isSeekInProgress;

        /// <summary>
        /// This flag keeps track of whether or not we have been closed. If we have been shutdown,
        /// then we ignore all subsequent calls to schedule new downloads.
        /// </summary>
        private bool m_isShutdown;

        /// <summary>
        /// The last ad insertion point we ran into
        /// </summary>
        private long m_lastAdInsertionPoint = -1;

        /// <summary>
        /// An object to provide strong identity locking
        /// </summary>
        private object m_lock = new object();

        /// <summary>
        /// This is the manifest file for the stream we are reading
        /// </summary>
        private ManifestInfo m_manifestInfo;

        /// <summary>
        /// The max bitrates we are using
        /// </summary>
        private ulong[] m_maxBitRates = new ulong[Configuration.Streams.MaxStreams];

        /// <summary>
        /// This is the maximum buffer size that we will hold. It defaults to 20 seconds.
        /// </summary>
        private ulong m_maxBufferSizeInMs = (ulong)(Configuration.Heuristics.Network.MaxBufferSize * 1000);

        /// <summary>
        /// This defines the maximum number of downloads we can have per stream
        /// </summary>
        private int[] m_maxDownloadsPerStream;

        /// <summary>
        /// These are the video stream types of each stream we are interested in.
        /// </summary>
        private MediaStreamType[] m_mediaStreamTypes;

        /// <summary>
        /// The min bitrates we are using
        /// </summary>
        private ulong[] m_minBitRates = new ulong[Configuration.Streams.MaxStreams];

        /// <summary>
        /// The next ad insertion point we are going to hit
        /// </summary>
        private long m_nextAdInsertionPoint = -1;

        /// <summary>
        /// This is the next chunk to download, per stream
        /// </summary>
        private int[] m_nextChunkId;

        /// <summary>
        /// This is the number of current downloads for each stream
        /// </summary>
        private int[] m_numDownloadsInProgress;

        /// <summary>
        /// The playback info from the media element
        /// </summary>
        private PlaybackInfo m_playbackInfo;

        /// <summary>
        /// Initializes a new instance of the HeuristicsImpl class.
        /// </summary>
        public HeuristicsImpl()
        {
            // Initialize our min and max bitrates.
            // Note min bitrates should already be initialized since they are 0
            for (int i = 0; i < m_maxBitRates.Length; ++i)
            {
                m_maxBitRates[i] = ulong.MaxValue;
            }
        }

        /// <summary>
        /// This enum is used internally to tell us if we should use our full algorithm
        /// for a stream or not. Right now, we use the full algorithm on video streams,
        /// but since we only support 1 audio stream at a single bitrate, we use
        /// the fixed bitrate algorithm on the audio stream.
        /// </summary>
        private enum HeuristicsMode
        {
            /// <summary>
            /// Run this heurisitics module at a fixed bitrate
            /// </summary>
            FixedRate,

            /// <summary>
            /// Run the full heuristics algorithm
            /// </summary>
            Full
        }

        /// <summary>
        /// Implements Heuristics.EndSeek(). We resume scheduling downloads once we have finished
        /// seeking.
        /// </summary>
        /// <param name="seekPosition">the position we just seeked to</param>
        public override void EndSeek(ulong seekPosition)
        {
            lock (m_lock)
            {
                m_isSeekInProgress = false;
            }

            // Seek resets any ad insertion pause
            CancelDownloadPause(true);

            // Go through all our streams and reset our stats
            for (int i = 0; i < m_manifestInfo.NumberOfStreams; i++)
            {
                RestartNetMediaInfo(m_manifestInfo.GetStreamInfoForStream(i).StreamId, true);
            }

            // Schedule any new downloads
            ScheduleDownloads();
        }

        /// <summary>
        /// Tells the heuristics module to override itself and force the next chunk download
        /// for a stream. This is used after a seek to get everything going again.
        /// </summary>
        /// <param name="streamId">the id of the stream to force</param>
        /// <param name="nextChunkId">the id of the chunk to get</param>
        public override void ForceNextDownload(int streamId, int nextChunkId)
        {
            TraceState();
            lock (m_lock)
            {
                m_nextChunkId[streamId] = nextChunkId;

                // Temporary bump up the limit to enable download start before 
                // Silverlight will confirm cancellation of the current one
                m_maxDownloadsPerStream[streamId]++;
            }
        }

        /// <summary>
        /// Implements Heuristics.Initialize(). Called to initialize this heuristics module with
        /// the given manifest file and optional adverting insertion points
        /// </summary>
        /// <param name="manifestInfo">The manifest of the stream we are using</param>
        /// <param name="playbackInfo">The playback info for the media element</param>
        /// <param name="adInsertPoints">Optional ad insertion points. Can be null</param>
        public override void Initialize(ManifestInfo manifestInfo, PlaybackInfo playbackInfo, AdInsertionPoint[] adInsertPoints)
        {
            Tracer.Trace(TraceChannel.MSS, "MediaSource instance {0}, sessionID: {1}", InstanceId.ToString("B"), sm_sessionId.ToString("B"));

            // Store our manifest info
            m_manifestInfo = manifestInfo;
            m_playbackInfo = playbackInfo;

            // We only support 1 audio and 1 video stream right now, so let's throw an exception
            // if someone tries to create us with more than that
            int numStreams = m_manifestInfo.NumberOfStreams;
            if (numStreams > 2)
            {
                throw new AdaptiveStreamingException("We only support 1 audio and 1 video stream at this time");
            }

            // Copy the add insertion points over
            m_adInsertPoints = new AdInsertionPoints();
            foreach (AdInsertionPoint ad in adInsertPoints)
            {
                m_adInsertPoints.Add(ad);
            }

            // Allocate some internal variables
            m_heuristicsMode = new HeuristicsMode[numStreams];
            m_fixedBitrateIndex = new int[numStreams];
            m_mediaStreamTypes = new MediaStreamType[numStreams];
            m_maxDownloadsPerStream = new int[numStreams];
            m_numDownloadsInProgress = new int[numStreams];
            m_nextChunkId = new int[numStreams];

            // Go through each stream in our manifest and do per-stream configuration
            int numVideoStreams = 0, numAudioStreams = 0;
            for (int i = 0; i < numStreams; i++)
            {
                StreamInfo streamInfo = m_manifestInfo.GetStreamInfoForStream(i);

                // Initialize some of the arrays we just created
                m_mediaStreamTypes[i] = streamInfo.MediaType;
                m_fixedBitrateIndex[i] = 0;
                m_maxDownloadsPerStream[i] = GlobalMaxDownloadsPerStream;
                m_numDownloadsInProgress[i] = 0;
                m_nextChunkId[i] = 0;

                // If we are a video stream, then we need to set our heuristics mode
                // to Full
                switch (streamInfo.MediaType)
                {
                    case MediaStreamType.Audio:
                        m_heuristicsMode[i] = HeuristicsMode.FixedRate;
                        ++numAudioStreams;
                        break;
                    case MediaStreamType.Video:
                        m_heuristicsMode[i] = HeuristicsMode.Full;
                        ++numVideoStreams;
                        break;
                    default:
                        throw new AdaptiveStreamingException(Errors.NonVideoOrAudioStreamsNotSupportedError);
                }

                // Configure this stream
                InitializeHeuristicsForStream(
                    streamInfo.StreamId, 
                    (double)m_maxBufferSizeInMs / 1000,
                    m_manifestInfo.GetStreamInfoForStream(i).Bitrates.Count,
                    m_manifestInfo.GetStreamInfoForStream(i).GetBitrateArray());
            }

            // Make sure we found only 1 video and 1 audio stream
            if (numVideoStreams != 1)
            {
                throw new AdaptiveStreamingException(Errors.IncorrectNumberOfVideoStreamsError);
            }

            // Start scheduling our downloads
            ScheduleDownloads();
        }

        /// <summary>
        /// Implements Heuristics.OnDownloadCompleted()
        /// </summary>
        /// <param name="streamId">the id of the stream that the chunk was downloaded for</param>
        /// <param name="chunkId">the id of the chunk downloaded</param>
        /// <param name="bitrate">the bitrate of the chunk downloaded</param>
        /// <param name="downloadStart">the time the chunk started downloading</param>
        /// <param name="downloadComplete">the time the chunk finished downloading</param>
        /// <param name="chunkLength">the length, in bytes of the downloaded chunk</param>
        public override void OnDownloadCompleted(
            int streamId, 
            int chunkId, 
            ulong bitrate,
            DateTime downloadStart, 
            DateTime downloadComplete, 
            long chunkLength)
        {
            // Get the chunk that corresponds to this stream
            MediaChunk chunk = m_manifestInfo.GetStreamInfoForStream(streamId).Queue[chunkId];

            // Update the current downloads counter
            lock (m_lock)
            {
                int streamIndex = chunk.StreamId;
                m_numDownloadsInProgress[streamIndex]--;
            }

            // May be called on cancelled download too because downloadsInProgress must be corrected, check chunk's state
            if (chunk.State == MediaChunk.ChunkState.Loaded || chunk.State == MediaChunk.ChunkState.Parsed)
            {
                ProcessChunkDownload(chunk);
                ScheduleDownloads();
            }
        }

        /// <summary>
        /// Implements Heuristics.OnDownloadProgress()
        /// </summary>
        /// <param name="streamid">the id of the stream we are downloading</param>
        /// <param name="chunkId">the id of the chunk that is downloading</param>
        /// <param name="bitrate">the bitrate of the chunk we are downloading</param>
        /// <param name="bytesReady">the number of bytes that are ready</param>
        /// <param name="percent">the percentage that has been downloaded so far</param>
        public override void OnDownloadProgress(int streamid, int chunkId, int bitrate, long bytesReady, int percent)
        {
            // Ignored for now since Silverlight won't send us these anyway
        }

        /// <summary>
        /// Implements Heuristics.OnSampleDelivered()
        /// </summary>
        /// <param name="mediaStreamType">the type of sample delivered</param>
        /// <param name="chunkId">The chunk we delivered this sample from</param>
        /// <param name="bitrate">The bitrate or the chunk we delivered</param>
        /// <param name="startTime">the time the sample was delivered</param>
        public override void OnSampleDelivered(MediaStreamType mediaStreamType, int chunkId, ulong bitrate, long startTime)
        {
            // Update our frame rate heuristics
            ProcessFrameRateHeuristcs(mediaStreamType, bitrate);

            // Check to see if we need to pause for ad insertion
            CheckForDownloadPause(startTime, mediaStreamType);

            // Re-schedule our downloads
            ScheduleDownloads();
        }

        /// <summary>
        /// Implements Heuristics.OnSampleRequested()
        /// </summary>
        /// <param name="mediaStreamType">the type of sample that was requested</param>
        public override void OnSampleRequested(MediaStreamType mediaStreamType)
        {
            // Recalculate what needs to be downloaded
            ScheduleDownloads();
        }

        /// <summary>
        /// Implements Heuristics.PauseDownloads(). We do not currently support manually
        /// pausing downloads.
        /// </summary>
        public override void PauseDownloads()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Implements Heuristics.ResumeDownloads()
        /// </summary>
        public override void ResumeDownloads()
        {
            CancelDownloadPause(false);
            ScheduleDownloads();
        }

        /// <summary>
        /// Implements Heuristics.ScheduleDownloads(). Recalculates which chunks need to be downloaded
        /// based on what we have and where we are going.
        /// </summary>
        public override void ScheduleDownloads()
        {
            // Flag used internally to tell if we have skipped over our video stream
            bool isVideoSkipped = false;

            // If we are shutdown, then we have nothing to do
            if (!m_isShutdown)
            {
                // Used for synchronization across threads
                lock (m_lock)
                {
                    // If we are in the middle of a seek, then don't schedule anything, since
                    // it's just going to change
                    if (!m_isSeekInProgress)
                    {
                        // Get the first stream in our manifest. This works okay for now because we
                        // only support 1 video stream.
                        StreamInfo videoStream = m_manifestInfo.GetStreamInfoForStreamType(MediaStreamType.Video);

                        // Go through all of our streams and ???
                        for (int streamIndex = 0; streamIndex < m_mediaStreamTypes.Length; streamIndex++)
                        {
                            // What type of stream is this?
                            MediaStreamType streamType = m_mediaStreamTypes[streamIndex];

                            if (m_numDownloadsInProgress[streamIndex] >= m_maxDownloadsPerStream[streamIndex])
                            {
                                // The maximum number of downloads has already been hit for this stream,
                                // so go onto the next one
                                continue;
                            }

                            // Get the info for this stream
                            StreamInfo stream = m_manifestInfo.GetStreamInfoForStreamType(streamType);

                            // Keep track of whether or not we skip this video stream,
                            // because we do not want to load the audio stream too far ahead
                            // of the video
                            if (streamType == MediaStreamType.Video)
                            {
                                isVideoSkipped = true;
                            }

                            // Make sure we got a stream info for this stream. This should never
                            // occur, but we make the check nonetheless
                            if (stream == null)
                            {
                                // The stream is not present for some reason, so just
                                // go onto the next one
                                continue;
                            }

                            // Check to make sure that we still have something left to download
                            if (m_nextChunkId[streamIndex] >= stream.NumberOfChunksInStream)
                            {
                                // We have already loaded everything, so go on to the next stream
                                continue;
                            }

                            // If we get here, then we have not skipped our video streaming
                            // (assuming of course this is a video stream
                            if (streamType == MediaStreamType.Video)
                            {
                                isVideoSkipped = false;
                            }

                            // Check to see how much we have in our buffer. By default, we only keep 20s
                            // of data in the buffer.
                            ulong v = stream.Queue.BufferTime;
                            if (v / 10000 >= m_maxBufferSizeInMs || (m_isDownloadsPaused && v > AdPausableMinimalBuffer))
                            {
                                // Already buffered maximum time or ad download pause mode (with minimal buffer available)
                                continue;
                            }

                            // Make sure we do not load audio too far ahead of video. It's
                            // okay to do it, however, if we don't have a video stream, although in
                            // practice, this will not occur because we do not support audio only streams
                            // right now.
                            if (streamType == MediaStreamType.Audio && videoStream != null && !isVideoSkipped
                                && stream.Queue.BufferTime > videoStream.Queue.BufferTime)
                            {
                                // Don't load audio too far ahead of video, so go to the next stream
                                continue;
                            }

                            // If we have reached this point, then we know we can start downloading the next chunk
                            MediaChunk chunk = stream.Queue[m_nextChunkId[streamIndex]];

                            // Set the bitrate that we are going to use on this chunk
                            chunk.Bitrate = GetNextBitrateForStream(stream);
                            Tracer.Assert(chunk.Bitrate > 0, String.Format(CultureInfo.InvariantCulture, "Download scheduler: Cannot load chunk {0} with zero ({1}) bitrate.", chunk.Sid, chunk.Bitrate));

                            // Tell our engine that we want to download a new chunk
                            RequestChunkDownload(chunk.StreamId, chunk.ChunkId, chunk.Bitrate);

                            // Keep track of the number of downloads we have for this stream
                            m_numDownloadsInProgress[streamIndex]++;

                            // Increment our next chunk Id for this stream
                            m_nextChunkId[streamIndex]++;
                            Tracer.Trace(TraceChannel.DownloadVerbose, "{0} scheduled", chunk.Sid);

                            // Keep track of whether or not we are paused but still downloading
                            if (m_isDownloadsPaused)
                            {
                                Tracer.Trace(TraceChannel.AdsInsert, " * ");
                            }

                            if (m_maxDownloadsPerStream[streamIndex] > GlobalMaxDownloadsPerStream)
                            {
                                // Restore default from forced start after CancelAll
                                m_maxDownloadsPerStream[streamIndex]--;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Limits the range of bitrates that the heuristic module can use
        /// </summary>
        /// <param name="streamType">the type of stream to limit the rate for</param>
        /// <param name="minBitrate">the new minimum bitrate, in bps</param>
        /// <param name="maxBitrate">the new maximum bitrate, in bps</param>
        public override void SetBitrateRange(MediaStreamType streamType, long minBitrate, long maxBitrate)
        {
            if (m_manifestInfo == null)
            {
                return;
            }

            StreamInfo stream = m_manifestInfo.GetStreamInfoForStreamType(streamType);
            int index = stream.StreamId;

            m_maxBitRates[index] = maxBitrate >= 0 ? (ulong)maxBitrate : ulong.MaxValue;
            ulong[] bitrates = stream.GetBitrateArray();

            int count = 0;
            int lastRateIndex = 0;
            for (int i = 0; i < bitrates.Length; i++)
            {
                if (m_minBitRates[index] <= bitrates[i] && bitrates[i] <= m_maxBitRates[index])
                {
                    count++;
                    lastRateIndex = i;
                }
            }

            if (count == 1)
            {
                m_fixedBitrateIndex[index] = lastRateIndex;
                m_heuristicsMode[index] = HeuristicsMode.FixedRate;
            }
            else if (count == 0)
            {
                int ratesCount = bitrates.Length;
                int i = 0;

                // Push limits, better less than more. Both loops won't execute with no limits (0 and MaxUInt64)
                for (; i < ratesCount - 1 && bitrates[i] < m_minBitRates[index]; i++)
                {
                } // Now either i point to the highest bitrate or to the first one that is more than minimal bitarte for the stream

                for (; i > 0 && bitrates[i] > m_maxBitRates[index]; i--)
                {
                } // And now it's pushed down to the first one which is smaller than maximum bitrate, or lowest (0)

                m_fixedBitrateIndex[index] = i;
                m_heuristicsMode[index] = HeuristicsMode.FixedRate;
            }
            else
            {
                m_heuristicsMode[index] = HeuristicsMode.Full;
            }

            // Let network heuristics know about this regardless it
            // will be used or not in deciding about bit rates
            m_networkMediaInfo[index].UpdateUsableBitrates(m_minBitRates[index], m_maxBitRates[index]);
        }

        /// <summary>
        /// Implements Heuristics.Shutdown().
        /// </summary>
        public override void Shutdown()
        {
            m_isShutdown = true;
        }

        /// <summary>
        /// Implements Heuristics.StartSeek(). We hold off scheduling any new downloads
        /// while we are in the middle of a seek.
        /// </summary>
        /// <param name="seekPosition">the position we are seeking too</param>
        public override void StartSeek(ulong seekPosition)
        {
            lock (m_lock)
            {
                m_isSeekInProgress = true;
            }
        }

        /// <summary>
        /// Helper function which cancels any outstanding download pause.
        /// </summary>
        /// <param name="resetLast">if true, resets the last m_NextAdInsertionPoint</param>
        private void CancelDownloadPause(bool resetLast)
        {
            Tracer.Trace(TraceChannel.AdsInsert, m_isDownloadsPaused ? "Cancelling pause mode. " : "No pause mode. ");
            m_isDownloadsPaused = false;

            m_nextAdInsertionPoint = -1;
            if (resetLast)
            {
                m_lastAdInsertionPoint = -1;
            }

            // Make sure to check for the nearest ad insertion point next time
            m_isRefreshAdInsertionPoints = true;
        }

        /// <summary>
        /// Internal helper function which calculates whether or not it is time to pause for an ad insertion
        /// </summary>
        /// <param name="currentPlayTime">the current playing time</param>
        /// <param name="streamType">the stream type to check</param>
        private void CheckForDownloadPause(long currentPlayTime, MediaStreamType streamType)
        {
            // Only check video, since audio buffering is neglible
            if (streamType != MediaStreamType.Video)
            {
                return;
            }

            // Check to see if we need to recalculate the next ad point we are looking for
            if ((m_lastAdInsertionPoint < currentPlayTime && m_isRefreshAdInsertionPoints) || m_adInsertPoints.IsChanged)
            {
                // Get the next ad point
                m_nextAdInsertionPoint = m_adInsertPoints.GetNearest(currentPlayTime);

                // We are up to date, so make it so
                m_isRefreshAdInsertionPoints = false;
                Tracer.Trace(TraceChannel.AdsInsert, "Looking to point {0} at {1}", m_nextAdInsertionPoint / 10000, currentPlayTime / 10000);
            }

            // If we don't have any ad points we are waiting for, 
            // then we are done.
            if (m_nextAdInsertionPoint < 0)
            {
                return;
            }

            // Check to see if we need to pause or not
            if (currentPlayTime > m_nextAdInsertionPoint + AdPausableZoneAfter)
            {
                // We have already passed the time at which we were supposed to pause,
                // so if we are currently paused, cancel it and continue on.
                if (m_isDownloadsPaused)
                {
                    Tracer.Trace(TraceChannel.AdsInsert, "Leaving pause mode at " + (currentPlayTime / 10000) + ". ");
                    CancelDownloadPause(true);
                }
            }
            else if (!m_isDownloadsPaused && (currentPlayTime + AdPausableZoneBefore > m_nextAdInsertionPoint) && IsMediaPausable())
            {
                // If we get here, then we need to pause for an ad
                Tracer.Trace(TraceChannel.AdsInsert, "Entering pause mode at " + (currentPlayTime / 10000) + ". ");
                m_lastAdInsertionPoint = m_nextAdInsertionPoint;
                m_isDownloadsPaused = true;
                NotifyDownloadsPaused(new TimeSpan(m_nextAdInsertionPoint));
            }
        }

        /// <summary>
        /// Calculates the next chunk to get for the given stream
        /// </summary>
        /// <param name="stream">the stream to look at</param>
        /// <returns>the next bitrate for this stream</returns>
        private ulong GetNextBitrateForStream(StreamInfo stream)
        {
            int streamIndex = stream.StreamId;
            if (m_heuristicsMode[streamIndex] == HeuristicsMode.Full)
            {
                // Use the full mode calculation
                // Get our network stats for this stream
                NetworkMediaInfo networkMediaInfo = m_networkMediaInfo[streamIndex];

                // Find the closest bitrate 
                ulong nextBitRate = networkMediaInfo.FindClosestBitrateByValue(networkMediaInfo.NextBitrate);
                NhTrace("INFO", streamIndex, "Next bit rate:{0}", nextBitRate);

                // Return Kilobits per second
                return nextBitRate;
            }
            else
            {
                // There is no other values for now, so that's: if (useHeuristicsMode[streamIndex] == HeuristicsMode.FixedRate)
                // Grab a bitrate from the bitrate array (currently always selects the first bitrate).
                return stream.Bitrates[m_fixedBitrateIndex[streamIndex]];
            }
        }

        /// <summary>
        /// Helper function which tells us if we can pause downloads on any stream
        /// </summary>
        /// <returns>true if the stream is pausable</returns>
        private bool IsMediaPausable()
        {
            return IsStreamPauseable(MediaStreamType.Video) && IsStreamPauseable(MediaStreamType.Audio);
        }

        /// <summary>
        /// Helper function which returns true if an individual stream can be paused. 
        /// </summary>
        /// <param name="mediaStreamType">the type of the stream to check</param>
        /// <returns>true if the given stream type is pausable</returns>
        private bool IsStreamPauseable(MediaStreamType mediaStreamType)
        {
            // If we don't have a manifest yet, then we can pause because we haven't started yet
            if (m_manifestInfo == null)
            {
                return true;
            }

            // Get the info for the given stream type
            StreamInfo stream = m_manifestInfo.GetStreamInfoForStreamType(mediaStreamType);

            // If there is no stream of this type, then we can pause it
            if (stream == null)
            {
                return true;
            }

            // Get the current media chunk
            MediaChunk cur = stream.Queue.Current;

            // If we didn't get one, then the media has ended
            if (cur == null)
            {
                return true;
            }

            // Check the state of the current chunk. If it hasn't been parsed yet, 
            // we cannot pause
            if (cur.State != MediaChunk.ChunkState.Parsed)
            {
                return false;
            }

            // Now check the last chunk in our queue. If current is not null and parsed, 
            // and our last one is null, then that only happens when media ended
            MediaChunk last = stream.Queue.Last;
            if (last == null)
            {
                return true;
            }

            // Make sure we are buffered beyond the ad insertion point plus buffer
            return last.StartTime + (long)last.Duration > m_nextAdInsertionPoint + AdPausableZoneAfter;
        }

        /// <summary>
        /// Internal tracing call
        /// </summary>
        private void TraceState()
        {
            bool lostVideo = m_numDownloadsInProgress[0] < 0 || m_numDownloadsInProgress[0] > GlobalMaxDownloadsPerStream;
            
            bool lostAudio = false;
            if (m_numDownloadsInProgress.Length > 1)
            {
                lostAudio = m_numDownloadsInProgress[1] < 0 || m_numDownloadsInProgress[1] > GlobalMaxDownloadsPerStream;
            }

            Tracer.Trace(lostVideo || lostAudio ? TraceChannel.Error : TraceChannel.DownloadVerbose, "In progress V{0}/A{1}", m_numDownloadsInProgress[0], m_numDownloadsInProgress.Length > 1 ? m_numDownloadsInProgress[1] : 0);
        }
    }
}
