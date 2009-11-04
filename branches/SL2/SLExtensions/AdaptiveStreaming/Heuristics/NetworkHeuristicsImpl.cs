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
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows.Media;
    using Microsoft.Expression.Encoder.AdaptiveStreaming;
    using MS.Internal.Expression.Encoder.AdaptiveStreaming.Logging;
    using MS.Internal.Expression.Encoder.AdaptiveStreaming.Manifest;
    using MS.Internal.Expression.Encoder.AdaptiveStreaming.Network;
    using MS.Internal.Expression.Encoder.AdaptiveStreaming.Utilities;

    /// <summary>
    /// This half of the HeuristicsImpl class implements the network 
    /// logic for determining which bandwidth to select. It uses the following
    /// goals:
    /// <para>1. Have an initial ramp up as fast as possible</para>
    /// <para>2. Maintain the buffer fullness within lower and upper boundaries</para>
    /// <para>
    /// The algorithm does basically this:
    /// 1. Maintain 2 states, buffering and steady
    /// 2. In buffering state we try to get to steady state as soon as
    /// we can
    /// 3. In steady state we try to maintain the buffer fullness
    /// within certain predefined limits
    /// 4. Under certain conditions, it is possible to move from steady
    /// state to buffering
    /// </para>
    /// </summary>
    internal partial class HeuristicsImpl : Heuristics
    {
        /// <summary>
        /// Used in tracing to help format doubles
        /// </summary>
        private const string DoubleFormat = "0.000";

        /// <summary>
        /// Used to initialize our bandwidth limit, 100M for
        /// the measured bandwidth
        /// </summary>
        private const ulong MaxBandwidth = 100000000;

        /// <summary>
        /// A flag used in printing out our first log
        /// </summary>
        private static bool sm_isFirstLogDone;

        /// <summary>
        /// Keep the network heuristics parameters
        /// </summary>
        private static NetworkHeuristicsParams sm_NetworkHeuristicsParams = new NetworkHeuristicsParams();

        /// <summary>
        /// If perceived bandwdith (per chunk) is higher than this
        /// value, assume it is in cache
        /// </summary>
        private double m_cacheBandwidth = double.MaxValue;

        /// <summary>
        /// Keep per media stream info. We only support 2 active streams right now, 1 audio and 1 video
        /// </summary>
        private NetworkMediaInfo[] m_networkMediaInfo = new NetworkMediaInfo[MS.Internal.Expression.Encoder.AdaptiveStreaming.Utilities.Configuration.Streams.MaxStreams];

        /// <summary>
        /// Max bandwdith perceived during download of first 2 chunks
        /// (making sure they are downloaded from network), used for
        /// cache detection
        /// </summary>
        private ulong m_packetPairBandwidth = MaxBandwidth;

        /// <summary>
        /// Internal tracing utilitiy
        /// </summary>
        /// <param name="label">label of the output</param>
        /// <param name="format">format string</param>
        /// <param name="list">parameters for format string</param>
        internal static void NhTrace(string label, string format, params object[] list)
        {
            string line = NetworkHeuristicsHelper.GetTime().ToString(DoubleFormat, CultureInfo.InvariantCulture) +
                " " + label +
                " " + -1 +
                " " + "?" +
                " " + string.Format(CultureInfo.InvariantCulture, format, list);

            Tracer.Trace(TraceChannel.NetHeur, line);
        }

        /// <summary>
        /// Internal tracing utility
        /// </summary>
        /// <param name="label">label for the trace</param>
        /// <param name="streamIndex">stream index</param>
        /// <param name="format">format string</param>
        /// <param name="list">parameter list for format string</param>
        internal void NhTrace(string label, int streamIndex, string format, params object[] list)
        {
            string type;
            switch (streamIndex)
            {
                case 0: 
                    type = "V"; 
                    break;
                case 1: 
                    type = "A"; 
                    break;
                default: 
                    type = "?"; 
                    break;
            }

            if (sm_isFirstLogDone == false)
            {
                // Do this first so recursive call is fine
                sm_isFirstLogDone = true;

                NhTrace(
                    "INFO", 
                    -1, 
                    "{0} {1} VER:{2} LOGVER:{3}",
                    DateTime.Now.ToString("M/dd/yyyy hh:mm:t", CultureInfo.InvariantCulture),
                    DateTime.Now.ToString("yyyyMMddHHmm", CultureInfo.InvariantCulture),
                    sm_NetworkHeuristicsParams.NetHeuristicsVersion,
                    sm_NetworkHeuristicsParams.NetHeuristicsLogVersion);

                NhTrace("INFO", -1, "HIST logs include:");

                NhTrace("INFO", -1, "1:time 2:label 3:objectID 4:media_type 5:download_size 6:download_duration 7:chunk_id 8:chunk_bitrate 9:chunk_duration ");

                NhTrace(
                    "INFO", 
                    -1,
                    "10:buffer_fullness 11:next_chunk_bitrate 12:smoothed_download_bandwidth 13:smoothed_media_bitrate 14:smoothed_buffer_fullness ");

                NhTrace(
                    "INFO", 
                    -1,
                    "15:buffer_fullness_slope 16:download_state 17:bitrate_validation_state 18:cache_bw");
            }

            string line = NetworkHeuristicsHelper.GetTime().ToString(DoubleFormat, CultureInfo.InvariantCulture) +
                " " + label +
                " " + InstanceId +
                " " + type +
                " " + string.Format(CultureInfo.InvariantCulture, format, list);

            Tracer.Trace(TraceChannel.NetHeur, line);
        }

        /// <summary>
        /// Return the bit rate 1 step above (step > 0) or 1 step
        /// below (step less than 0) the current one
        /// </summary>
        /// <param name="networkMediaInfo">The nextwork media info to query</param>
        /// <param name="step">Increse/decrease bit rate in "step" steps</param>
        /// <returns>next bitrate, in bps</returns>
        private static ulong GetNextBitRate(NetworkMediaInfo networkMediaInfo, int step)
        {
            if (networkMediaInfo.LockedBitRate >= 0)
            {
                return networkMediaInfo.BitratesInfo[networkMediaInfo.LockedBitRate].NominalBitrate;
            }

            int index = 0;

            for (; index < networkMediaInfo.BitratesInfo.Length; index++)
            {
                if (networkMediaInfo.PreviousBitrate == networkMediaInfo.BitratesInfo[index].NominalBitrate)
                {
                    break;
                }
            }

            index += step;

            return networkMediaInfo.FindClosestBitrateByIndex(index);
        }

        /// <summary>
        /// When attempt to move the selected bit rate 1 step up, the
        /// condition is that at least a certain amount of time has
        /// elapsed since the last time bit rate was raised, the
        /// change happens provided the measured network trhoughput is
        /// larger than the measured media bit rate for the potential
        /// new bit rate to select, if no history exists for the
        /// target bit rate, use its nominal value
        /// </summary>
        /// <param name="networkMediaInfo">the network media info to query</param>
        /// <returns>New selected bit rate (may be the one used so far)</returns>
        private ulong AttemptImprovingBitRate(NetworkMediaInfo networkMediaInfo)
        {
            // Locked bitrate scenario is trivial
            if (networkMediaInfo.LockedBitRate >= 0)
            {
                return networkMediaInfo.BitratesInfo[networkMediaInfo.LockedBitRate].NominalBitrate;
            }

            // Try to improve bit rate. First get the elapsed time since
            // we last improved it
            double elapsedTimeSinceLastAttempt = networkMediaInfo.TotalStreamDownloaded - networkMediaInfo.PreviousAttempt;

            // Remember the previous bitrate we used
            ulong newBitRate = networkMediaInfo.PreviousBitrate;

            // Only try to improve it if our period has expired
            if (elapsedTimeSinceLastAttempt >= Configuration.Heuristics.Network.TryImprovingBitratePeriod)
            {
                // Move 1 step above the current one
                newBitRate = GetNextBitRate(networkMediaInfo, 1);

                // Are we actually better than before?
                if (newBitRate > networkMediaInfo.PreviousBitrate)
                {
                    // Get the index for the new bitrate
                    int newBitRateIndex = networkMediaInfo.FindBitRateIndex(newBitRate);

                    // Get the encoded bit rate because we need to check if
                    // the encoded bitrate for the new bitrate is higher than
                    // our bandwidth window
                    double mediaBitRate = networkMediaInfo.BitratesInfo[newBitRateIndex].EncodedBitrateWindow.CurrentKernel;

                    // We might not have any data on this media yet
                    if (mediaBitRate == 0)
                    {
                        // We don't have yet any history for this
                        // particular bit rate, use the nominal media bit
                        // rate
                        mediaBitRate = networkMediaInfo.BitratesInfo[newBitRateIndex].NominalBitrate;
                    }

                    // If our new bitrate is larger than our bandwidth window,
                    // then don't use it
                    if (mediaBitRate >= networkMediaInfo.DownloadBandwidthWindow.CurrentKernel)
                    {
                        // Don't move up as we would exceed the network
                        // capability
                        newBitRate = networkMediaInfo.PreviousBitrate;
                    }
                    else
                    {
                        // Remember the current point only if moving up
                        networkMediaInfo.PreviousAttempt = networkMediaInfo.TotalStreamDownloaded;

                        NhTrace(
                            "INFO", 
                            networkMediaInfo.StreamId,
                            "mediaBitRate:{0}/{1} < downloadBandwidth:{2}, can go there",
                            newBitRate, 
                            mediaBitRate,
                            networkMediaInfo.DownloadBandwidthWindow.CurrentKernel);
                    }
                }
                else
                {
                    // Apparently we weren't, so use the old one
                    newBitRate = networkMediaInfo.PreviousBitrate;
                }
            }

            return networkMediaInfo.FindClosestBitrateByValue(newBitRate);
        }

        /// <summary>
        /// Select the next media bit rate using the minimum value from
        /// 2 criterias:
        /// <para>
        /// Condition 1) The bit rate selected must allow the buffer
        /// fullness to grow at least N times faster than the measured
        /// download bandwidth
        /// </para>
        /// <para>
        /// Condition 2) Using a fraction of the current buffer
        /// fullness, and a fraction of the past download bandwidth
        /// (network bit rate), find the highest bit rate encoding at
        /// which we can download to get as close as possible to the
        /// current target speed content download
        /// </para>
        /// <para>
        /// Condition 3) If in place, will not let the bit rate speed
        /// change be larger than 1 step
        /// </para>
        /// <para>
        /// NOTE speed content download is defined for a given stream
        /// bit rate (actually the bytes size) and network bit rate,
        /// as the amount of seconds worth of content that we can
        /// download per wall clock second
        /// </para>
        /// </summary>
        /// <param name="networkMediaInfo">the network media info to query</param>
        /// <param name="chunkDuration">the duration of the last chunk</param>
        /// <returns>the next bitrate to use</returns>
        private ulong GetNextBitRateUsingBandwidth(NetworkMediaInfo networkMediaInfo, double chunkDuration)
        {
            // If we are using a locked bit rate then our job is easy
            if (networkMediaInfo.LockedBitRate >= 0)
            {
                return networkMediaInfo.BitratesInfo[networkMediaInfo.LockedBitRate].NominalBitrate;
            }

            // Condition 1
            ulong bitRateCond1 = (ulong)(networkMediaInfo.DownloadBandwidthWindow.CurrentKernel / networkMediaInfo.RelativeContentDownloadSpeed);

            // Condition 2
            ulong bitRateCond2 = (ulong)
                (networkMediaInfo.DownloadBandwidthWindow.CurrentKernel
                *
                networkMediaInfo.BufferFullnessWindow.CurrentKernel
                *
                (sm_NetworkHeuristicsParams.DownloadBandwidthFraction *
                sm_NetworkHeuristicsParams.BufferFullnessFraction /
                chunkDuration));

            ulong bitRateFinal =
                bitRateCond2 < bitRateCond1 ? bitRateCond2 : bitRateCond1;

            // Condition 3
            ulong bitRateCond3 = ulong.MaxValue;

            // Only limit bitrate changes to one step at 
            // a time (condition 3)
            if (networkMediaInfo.IsLimitBitrateSteps == true)
            {
                int bitRateIndex = networkMediaInfo.FindBitRateIndex(networkMediaInfo.PreviousBitrate) + 1;

                bitRateCond3 = networkMediaInfo.FindClosestBitrateByIndex(bitRateIndex);

                if (bitRateCond3 < bitRateFinal)
                {
                    bitRateFinal = bitRateCond3;
                }
            }

            NhTrace("INFO", networkMediaInfo.StreamId, "C1({0}):{1} C2:{2} C3:{3} final:{4}", networkMediaInfo.RelativeContentDownloadSpeed, bitRateCond1, bitRateCond2, bitRateCond3, bitRateFinal);

            return bitRateFinal;
        }

        /// <summary>
        /// Gets the total time in the buffer for the given stream type
        /// </summary>
        /// <param name="mediaStreamType">the type of the stream to look at</param>
        /// <returns>the total time of chunks buffered</returns>
        private ulong GetTimeBufferedForStreamType(MediaStreamType mediaStreamType)
        {
            // If we don't have a manifest then we haven't started yet
            if (m_manifestInfo == null)
            {
                return 0;
            }

            // Get the info for the stream
            StreamInfo stream = m_manifestInfo.GetStreamInfoForStreamType(mediaStreamType);

            // If there is no stream, then nothing buffered
            if (stream == null)
            {
                return 0;
            }

            return stream.Queue.BufferTime;
        }

        /// <summary>
        /// Initializes our per stream network information.
        /// </summary>
        /// <param name="streamIndex">The index of the stream to initialize</param>
        /// <param name="maxBufferSize">Maximum buffer size for this stream, in seconds</param>
        /// <param name="numBitRates">The number of supported bitrates for this stream</param>
        /// <param name="bitrates">All of the supported bitrates for this stream</param>
        private void InitializeHeuristicsForStream(int streamIndex, double maxBufferSize, int numBitRates, ulong[] bitrates)
        {
            string allBitRates = string.Empty;
            for (int i = numBitRates - 1; i >= 0; i--)
            {
                if (i < (numBitRates - 1))
                {
                    allBitRates += ";";
                }

                allBitRates += i.ToString(System.Globalization.CultureInfo.InvariantCulture) + ":" + bitrates[i];
            }

            NhTrace("INFO", streamIndex, "Buffer size:{0} bit rates: {1}", maxBufferSize, allBitRates);

            m_networkMediaInfo[streamIndex] = new NetworkMediaInfo(streamIndex, numBitRates, bitrates, sm_NetworkHeuristicsParams);

            // Configure the size limits from the max buffer size
            Debug.Assert(Configuration.Heuristics.Network.MaxBufferSize >= 20, "Buffer size < 20 seconds");
            Debug.Assert((Configuration.Heuristics.Network.PanicBufferFullness < Configuration.Heuristics.Network.LowerBufferFullness) && (Configuration.Heuristics.Network.LowerBufferFullness < Configuration.Heuristics.Network.UpperBufferFullness), "Inconsistent buffer fullness thresholds");
        }

        /// <summary>
        /// This function is called whenever we have received a new chunk
        /// from the download manager.
        /// </summary>
        /// <param name="chunk">The media chunk to process</param>
        private void ProcessChunkDownload(MediaChunk chunk)
        {
            int streamIndex = chunk.StreamId;
            NetworkMediaInfo networkMediaInfo = m_networkMediaInfo[streamIndex];

            // Added for logging purposes only
            int chunkId = chunk.ChunkId;

            // Update prevBitRate to actually be the one used last time
            networkMediaInfo.PreviousBitrate = networkMediaInfo.NextBitrate;

            // Nominal bit rate index
            int nomBitRateIdx = networkMediaInfo.FindBitRateIndex(chunk.Bitrate);

            // Track buffer fullness
            double bufferFullness = NetworkHeuristicsHelper.ConvertHnsToSeconds(GetTimeBufferedForStreamType(chunk.MediaType));
            networkMediaInfo.BufferFullnessWindow.Add(bufferFullness);

            // Track download bit rate
            double downloadDuration = BandwidthCalculator.CalculateDownloadDurationInSeconds(chunk.DownloadStartTime, chunk.DownloadCompleteTime, streamIndex, chunk.ChunkId, chunk.Length);

            // The bit rate at which the chunk was downloaded
            double downloadSize = chunk.Length;
            double downloadBandwidth = downloadSize * 8 / downloadDuration;

            // Cap the bit rate to 1 Gbps
            if (downloadBandwidth > 1e9)
            {
                downloadBandwidth = 1e9;
            }

            // The first 2(PacketPairPacketCount) video chunks are treat 
            // as packetpair packet and will not be cached. 
            // The packetpair bandwidth is used to decide whether the chunk is reading from cache or not. 
            // Better higher than lower.  
            if (chunk.ChunkId < Downloader.PacketPairPacketCount && chunk.MediaType == MediaStreamType.Video)
            {
                if (chunk.ChunkId == 0)
                {
                    m_packetPairBandwidth = (ulong)downloadBandwidth;
                }
                else
                {
                    if ((ulong)downloadBandwidth > m_packetPairBandwidth)
                    {
                        m_packetPairBandwidth = (ulong)downloadBandwidth;
                    }
                }

                m_cacheBandwidth = Configuration.Heuristics.Network.CacheBandwidthFactor * m_packetPairBandwidth;

                if (m_cacheBandwidth < Configuration.Heuristics.Network.CacheBandwidthMin)
                {
                    m_cacheBandwidth = Configuration.Heuristics.Network.CacheBandwidthMin;
                }

                NhTrace("INFO", streamIndex, "Setting m_PacketPairBandwidth:{0} cache:{1}", m_packetPairBandwidth, m_cacheBandwidth);
            }

            // If our download bandwidth is less than the cache bandwidth, then
            // it has not been added to our window yet
            if (downloadBandwidth < m_cacheBandwidth)
            {
                // Not in cache
                networkMediaInfo.DownloadBandwidthWindow.Add(downloadBandwidth);
            }
            else
            {
                // If the current chunk is in cache, use average
                // bandwidth as current bandwidth
                downloadBandwidth = networkMediaInfo.DownloadBandwidthWindow.CurrentKernel;
            }

            // Track actual media content's bit rate
            double chunkDuration = NetworkHeuristicsHelper.ConvertHnsToSeconds(chunk.Duration);
            networkMediaInfo.BitratesInfo[nomBitRateIdx].EncodedBitrateWindow.Add(chunk.Length * 8 / chunkDuration);

            // Update how much has been downloaded
            networkMediaInfo.TotalStreamDownloaded += chunkDuration;

            // Now we are going to do our buffering state logic.
            // We have 2 states, Steady and Buffering.
            ulong currentBitRateSelected = 0;
            switch (networkMediaInfo.DownloadState)
            {
                case DownloadState.Buffering:
                    // If we are Buffering, then we can jump as many steps
                    // as we want
                    networkMediaInfo.IsLimitBitrateSteps = false;

                    // If our download bandwidth is really high, it could
                    // be because we have no history on it yet
                    if (downloadBandwidth >= sm_NetworkHeuristicsParams.AbsoluteHighBandwidth)
                    {
                        // Suspect cache download bandwidth
                        networkMediaInfo.IsLimitBitrateSteps = true;
                    }

                    // Buffering state normal logic. If our buffer fullness is decreasing
                    // or slowly changing, then limit our bitrate step so we don't tweak it too hard
                    if (networkMediaInfo.IsLimitBitrateSteps == false &&
                        (networkMediaInfo.BufferFullnessWindow.IsDecreasing == true || networkMediaInfo.BufferFullnessWindow.IsSlowChanging == true))
                    {
                        networkMediaInfo.IsLimitBitrateSteps = true;
                    }

                    // Find bit rate that satisfies the "conditions"
                    currentBitRateSelected = GetNextBitRateUsingBandwidth(networkMediaInfo, chunkDuration);

                    // Find closest bit rate
                    currentBitRateSelected = networkMediaInfo.FindClosestBitrateByValue(currentBitRateSelected);

                    // If our buffer is more than halfway full, then go steady state
                    if (bufferFullness >=
                        (Configuration.Heuristics.Network.LowerBufferFullness +
                        ((Configuration.Heuristics.Network.UpperBufferFullness - Configuration.Heuristics.Network.LowerBufferFullness) / 2)))
                    {
                        // Before going to steady, re-set this parameter
                        networkMediaInfo.RelativeContentDownloadSpeed = sm_NetworkHeuristicsParams.RelativeContentDownloadSpeed;

                        // Set steady state
                        networkMediaInfo.DownloadState = DownloadState.Steady;
                    }

                    break;

                case DownloadState.Steady:
                    // Default to not limiting bitrate jumps
                    networkMediaInfo.IsLimitBitrateSteps = false;

                    // Check to see if we have enough history
                    if (downloadBandwidth >= sm_NetworkHeuristicsParams.AbsoluteHighBandwidth)
                    {
                        // Suspect cache download bandwidth
                        networkMediaInfo.IsLimitBitrateSteps = true;
                    }

                    // Steady state normal logic. If our buffer fullness gets too
                    // low, then jump back to buffering
                    if (bufferFullness < Configuration.Heuristics.Network.PanicBufferFullness)
                    {
                        // Usually will pick the lowest bit rate
                        currentBitRateSelected = networkMediaInfo.FindDefaultBitrate();

                        // Stop trying to improve our bitrate
                        networkMediaInfo.ResetImprovingBitRate();

                        // Set the buffering state
                        networkMediaInfo.DownloadState = DownloadState.Buffering;
                    }
                    else if (networkMediaInfo.BufferFullnessWindow.IsSlowChanging)
                    {
                        // Smooth change is happening, can control by
                        // adjusting in 1 step
                        if (bufferFullness < Configuration.Heuristics.Network.LowerBufferFullness)
                        {
                            // Reduce bit rate 1 step
                            currentBitRateSelected = GetNextBitRate(networkMediaInfo, -1);

                            // Reset the timer to attempt improving bit rate
                            networkMediaInfo.ResetImprovingBitRate();
                        }
                        else if (bufferFullness > Configuration.Heuristics.Network.UpperBufferFullness)
                        {
                            // Try to go up 1 step
                            currentBitRateSelected = AttemptImprovingBitRate(networkMediaInfo);
                        }
                        else
                        {
                            // Otherwise we are doing all right where we were
                            currentBitRateSelected = networkMediaInfo.FindClosestBitrateByValue(networkMediaInfo.PreviousBitrate);
                        }
                    }
                    else if (networkMediaInfo.BufferFullnessWindow.IsFastDecreasing)
                    {
                        // Buffer fullness decreasing rapidly
                        if (bufferFullness < Configuration.Heuristics.Network.LowerBufferFullness)
                        {
                            // Usually will pick the lowest bit rate
                            currentBitRateSelected = networkMediaInfo.FindDefaultBitrate();

                            // Reset the timer to attempt improving bit rate
                            networkMediaInfo.ResetImprovingBitRate();

                            // Go to buffering state
                            networkMediaInfo.DownloadState = DownloadState.Buffering;
                        }
                        else
                        {
                            // Let's wait until we reach the lower
                            // buffer fullness threshold, might be a
                            // transient condition
                            currentBitRateSelected = networkMediaInfo.FindClosestBitrateByValue(networkMediaInfo.PreviousBitrate);
                        }
                    }
                    else
                    {
                        // Buffer fullness is growing rapidly, so try to go to a better bitrate
                        currentBitRateSelected = AttemptImprovingBitRate(networkMediaInfo);
                    }

                    break;
            }

            // Keep track of the next bitrate we are going to select
            networkMediaInfo.NextBitrate = currentBitRateSelected;

            // Log some information:
            // 1) time 
            // 2) label
            // 3) object ID (instance)
            // 4) streamIndex
            //
            // 5) download size
            // 6) download duration
            // 7) chunk ID
            // 8) chunk bitrate
            // 9) chunk duration
            // 10) buffer fullness 
            // Extended information that could be derived from basic info
            // 11) next bitrate
            // 12) kernel(download bandwidth) 
            // 13) kernel(encoded bitrate)
            // 14) kernel(buffer fullness) 
            // 15) buffer fullness slope
            // 16) download state (buffering, steady)
            // 17) Bit rate validation state
            // 18) Cache bandwidth
            NhTrace(
                "HIST", 
                streamIndex,
                "{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11} {12} {13}",
                /*  5 */downloadSize,
                /*  6 */downloadDuration.ToString(DoubleFormat, CultureInfo.InvariantCulture),
                /*  7 */chunkId,
                /*  8 */networkMediaInfo.BitratesInfo[nomBitRateIdx].NominalBitrate,
                /*  9 */chunkDuration.ToString(DoubleFormat, CultureInfo.InvariantCulture),
                /* 10 */bufferFullness.ToString(DoubleFormat, CultureInfo.InvariantCulture),
                /* 11 */networkMediaInfo.NextBitrate,
                /* 12 */networkMediaInfo.DownloadBandwidthWindow.CurrentKernel.ToString(DoubleFormat, CultureInfo.InvariantCulture),
                /* 13 */networkMediaInfo.BitratesInfo[nomBitRateIdx].EncodedBitrateWindow.CurrentKernel.ToString(DoubleFormat, CultureInfo.InvariantCulture),
                /* 14 */networkMediaInfo.BufferFullnessWindow.CurrentKernel.ToString(DoubleFormat, CultureInfo.InvariantCulture),
                /* 15 */networkMediaInfo.BufferFullnessWindow.CurrentSlope.ToString(DoubleFormat, CultureInfo.InvariantCulture),
                /* 16 */(int)networkMediaInfo.DownloadState,
                /* 17 */0,
                /* 19 */downloadBandwidth < m_cacheBandwidth ? 0 : 1);
        }

        /// <summary>
        /// Clears out the network media info for a particular stream
        /// </summary>
        /// <param name="streamIndex">stream index to clear</param>
        /// <param name="isSeek">are we seeking?</param>
        private void RestartNetMediaInfo(int streamIndex, bool isSeek)
        {
            m_networkMediaInfo[streamIndex].ResetMediaInfo(isSeek);

            NhTrace("INFO", streamIndex, "Restarted, is seek:{0}, next bit rate:{1}", isSeek, m_networkMediaInfo[streamIndex].NextBitrate);
        }
    }
}
