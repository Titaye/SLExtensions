//*@@@*************************************************************************
//
// Microsoft Expression Encoder
// Copyright © Microsoft Corporation. All rights reserved.
//
//*@@@*************************************************************************
namespace Microsoft.Expression.Encoder.AdaptiveStreaming
{
    using System;
    using System.Windows.Media;
    using MS.Internal.Expression.Encoder.AdaptiveStreaming.Manifest;

    /// <summary>
    /// This class defines the base interface for all Heuristics modules.
    /// </summary>
    public abstract class Heuristics
    {
        /// <summary>
        /// The bandwidth calculator
        /// </summary>
        private BandwidthCalculator m_bandwidthCalculator = new BandwidthCalculator();

        /// <summary>
        /// Internal instance id
        /// </summary>
        private Guid m_instanceId = Guid.NewGuid();

        /// <summary>
        /// Chunk replacement event, used to request a forced download
        /// </summary>
        public event EventHandler<EventArgs> ChunkReplacementSuggested;

        /// <summary>
        /// The DownloadsPaused event.
        /// </summary>
        internal event EventHandler<DownloadsPausedEventArgs> DownloadsPaused;

        /// <summary>
        /// This event is fired whenever a Heuristics class wishes to start downloading a
        /// new media chunk. The encoder media stream source listens for this event.
        /// </summary>
        internal event EventHandler<RequestDownloadEventArgs> RequestDownload;

        /// <summary>
        /// Gets or sets the bandwidth calculation module used by this heuristics instance.
        /// It can be overridden to insert phony data into the module for testing purposes.
        /// </summary>
        public BandwidthCalculator BandwidthCalculator
        {
            get
            {
                return m_bandwidthCalculator;
            }

            set
            {
                m_bandwidthCalculator = value;
            }
        }

        /// <summary>
        /// Gets an internal guid which is used for tracking downloads in the
        /// download manager.
        /// </summary>
        internal Guid InstanceId
        {
            get
            {
                return m_instanceId;
            }
        }

        /// <summary>
        /// Notify the heuristics module that we have finished seeking to the given position
        /// </summary>
        /// <param name="seekPosition">the position we seeked to</param>
        public abstract void EndSeek(ulong seekPosition);

        /// <summary>
        /// Force the heuristics module to schedule the given chunk for downloading.
        /// This is used after a seek to jump to the right place.
        /// </summary>
        /// <param name="streamId">int streamId</param>
        /// <param name="chunkId">int chunkId</param>
        public abstract void ForceNextDownload(int streamId, int chunkId);        
        
        /// <summary>
        /// This function is used to intialize a heuristics module. It must be implemented by all derived classes.
        /// </summary>
        /// <param name="manifestInfo">The manifest for the stream we are reading</param>
        /// <param name="playbackInfo">The playback info for the media element</param>
        /// <param name="adInsertPoints">Optional expected points for advertising. Can be null</param>
        public abstract void Initialize(ManifestInfo manifestInfo, PlaybackInfo playbackInfo, AdInsertionPoint[] adInsertPoints);

        /// <summary>
        /// This function is called by the downloader to let the heuristics module
        /// know that the requested chunk has been downloaded.
        /// </summary>
        /// <param name="streamId">the id of the stream that the chunk was downloaded for</param>
        /// <param name="chunkId">the id of the chunk downloaded</param>
        /// <param name="bitrate">the bitrate of the chunk downloaded</param>        
        /// <param name="downloadStart">the time the chunk started downloading</param>
        /// <param name="downloadComplete">the time the chunk finished downloading</param>
        /// <param name="chunkLength">the length, in bytes of the downloaded chunk</param>
        public abstract void OnDownloadCompleted(
            int streamId, 
            int chunkId, 
            ulong bitrate,
            DateTime downloadStart, 
            DateTime downloadComplete, 
            long chunkLength);

        /// <summary>
        /// This function is called by the downloader to report the status of the current chunk it is downloading.
        /// Silverlight does not currently support reporting download progress, so this interface call is currently
        /// not used. However, once Silverlight has been updated, this function will be called rom the engine.
        /// </summary>
        /// <param name="streamId">the id of the stream we are downloading</param>
        /// <param name="chunkId">the id of the chunk that is downloading</param>
        /// <param name="bitrate">the bitrate of the chunk we are downloading</param>
        /// <param name="bytesReady">the number of bytes that are ready</param>
        /// <param name="percent">the percentage that has been downloaded so far</param>
        public abstract void OnDownloadProgress(int streamId, int chunkId, int bitrate, long bytesReady, int percent);

        /// <summary>
        /// This function is called when the media stream source has delivered a sample
        /// to it's hosting media element.
        /// </summary>
        /// <param name="streamType">The type of sample delivered</param>
        /// <param name="chunkId">The id of the chunk from the sample we delivered</param>
        /// <param name="bitrate">The bitrate of the chunk we delivered</param>
        /// <param name="timestamp">The time the sample was delivered</param>
        public abstract void OnSampleDelivered(MediaStreamType streamType, int chunkId, ulong bitrate, long timestamp);

        /// <summary>
        /// This function is called when our media stream source has been requested to download
        /// a new sample of the given type. The heuristics module can use this to do any per sample
        /// calculations.
        /// </summary>
        /// <param name="streamType">The type of sample requested</param>
        public abstract void OnSampleRequested(MediaStreamType streamType);

        /// <summary>
        /// Used to manually tell this object to pause downloading any more chunks. Make sure
        /// to call ResumeDownloads() eventually otherwise they will never continue.
        /// </summary>
        public abstract void PauseDownloads();

        /// <summary>
        /// Used to manually tell this object to resume downloading.
        /// </summary>
        public abstract void ResumeDownloads();

        /// <summary>
        /// Tell our heuristics module that is ok to recalculate which
        /// chunks need to be downloaded. This gets called frequently, for example
        /// whenever a download is complete, whenever we get a new sample, etc.
        /// </summary>
        public abstract void ScheduleDownloads();

        /// <summary>
        /// Limits the range of bitrates that the heuristic module can use
        /// </summary>
        /// <param name="streamType">the type of stream to limit the rate for</param>
        /// <param name="minBitrate">the new minimum bitrate</param>
        /// <param name="maxBitrate">the new maximum bitrate</param>
        public abstract void SetBitrateRange(MediaStreamType streamType, long minBitrate, long maxBitrate);

        /// <summary>
        /// Shutdowns the heuristics module. Do any per instance clean up here. This will be called
        /// when our media stream is closed.
        /// </summary>
        public abstract void Shutdown();

        /// <summary>
        /// Notify the heuristics module that we are beginning a seek to a new position
        /// </summary>
        /// <param name="seekPosition">the position that the player is seeking to</param>
        public abstract void StartSeek(ulong seekPosition);

        /// <summary>
        /// Notify anyone listening that we have paused our downloads
        /// </summary>
        /// <param name="startTime">the time which the downloads were paused</param>
        protected void NotifyDownloadsPaused(TimeSpan startTime)
        {
            if (DownloadsPaused != null)
            {
                DownloadsPaused(this, new DownloadsPausedEventArgs(startTime));
            }
        }

        /// <summary>
        /// Requests a chunk replacement, based on updated heuristics. Eventually
        /// ends up going through ForceNextDownload.
        /// </summary>
        protected void RequestChunkReplacement()
        {
            if (ChunkReplacementSuggested != null)
            {
                ChunkReplacementSuggested(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// This function is used by derived classes to request a download from the engine
        /// </summary>
        /// <param name="streamId">the stream id requested</param>
        /// <param name="chunkId">the chunk id requested</param>
        /// <param name="bitrate">the bitrate requested</param>
        protected void RequestChunkDownload(int streamId, int chunkId, ulong bitrate)
        {
            if (RequestDownload != null)
            {
                RequestDownload(this, new RequestDownloadEventArgs(streamId, chunkId, bitrate));
            }
        }

        /// <summary>
        /// This class is used as the argument for a DownloadsPaused event,
        /// which is fired from a Heuristics module whenever downloads
        /// are paused for any reason (typically, for ad insertion)
        /// </summary>
        internal class DownloadsPausedEventArgs : EventArgs
        {
            /// <summary>
            /// Initializes a new instance of the DownloadsPausedEventArgs class
            /// </summary>
            /// <param name="startTime">time which the downloads started pausing</param>
            public DownloadsPausedEventArgs(TimeSpan startTime)
            {
                StartTime = startTime;
            }

            /// <summary>
            /// Gets or sets the time which the downloads started pausing
            /// </summary>
            public TimeSpan StartTime { get; set; }
        }

        /// <summary>
        /// This is the event arguments for the RequestDownload event
        /// </summary>
        internal class RequestDownloadEventArgs : EventArgs
        {
            /// <summary>
            /// Initializes a new instance of the RequestDownloadEventArgs class
            /// </summary>
            /// <param name="streamId">the id of the stream we want to download</param>
            /// <param name="chunkId">the chunk in the stream we want to download</param>
            /// <param name="bitrate">the bitrate of the chunk to download</param>
            public RequestDownloadEventArgs(int streamId, int chunkId, ulong bitrate)
            {
                StreamId = streamId;
                ChunkId = chunkId;
                Bitrate = bitrate;
            }

            /// <summary>
            /// Gets or sets the bitrate of the chunk to download
            /// </summary>
            public ulong Bitrate { get; set; }

            /// <summary>
            /// Gets or sets the id of the chunk to download
            /// </summary>
            public int ChunkId { get; set; }

            /// <summary>
            /// Gets or sets the id of the stream to download from
            /// </summary>
            public int StreamId { get; set; }
        }
    }
}
