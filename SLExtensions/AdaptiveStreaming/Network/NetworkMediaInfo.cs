//*@@@*************************************************************************
//
// Microsoft Expression Encoder
// Copyright © Microsoft Corporation. All rights reserved.
//
//*@@@*************************************************************************
namespace MS.Internal.Expression.Encoder.AdaptiveStreaming.Network
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using MS.Internal.Expression.Encoder.AdaptiveStreaming.Heuristic;
    using MS.Internal.Expression.Encoder.AdaptiveStreaming.Logging;
    using MS.Internal.Expression.Encoder.AdaptiveStreaming.Utilities;

    /// <summary>
    /// This enumeration flags the state of the download queue
    /// </summary>
    internal enum DownloadState
    {
        /// <summary>
        /// Below the lower buffer fullness
        /// </summary>
        Buffering,

        /// <summary>
        /// Above the lower buffer fullness
        /// </summary>
        Steady,

        /// <summary>
        /// Verifying if content was downloaded from cache
        /// </summary>
        ProbingCache,

        /// <summary>
        /// Using content from cache
        /// </summary>
        UsingCache
    }

    /// <summary>
    /// This class holds information about our network statistics and our downloaded chunks.
    /// </summary>
    internal partial class NetworkMediaInfo
    {
        /// <summary>
        /// If new buffer fullness value is less than 20% (0.2) of the current
        /// buffer fullness kernel, reset it to the sample value
        /// </summary>
        private const double BufferFullnessDownFraction = -0.2;

        /// <summary>
        /// If new buffer fullness value is larger than 50% (0.5) of the current
        /// buffer fullness kernel, reset it to the sample value
        /// </summary>
        private const double BufferFullnessUpFraction = 0.5;

        /// <summary>
        /// Buffer fullness is averaged over 3 samples
        /// </summary>
        private const int BufferFullnessWindowSize = 3;

        /// <summary>
        /// If new download bandwidth value is less than 20% (0.2) of the current
        /// download bandwidth kernel, reset it to the sample value
        /// </summary>
        private const double DownloadBandwidthDownFraction = -0.2;

        /// <summary>
        /// If new download bandwidth value is larger than 40% (0.4) of the current
        /// download bandwidth kernel, reset it to the sample value
        /// </summary>
        private const double DownloadBandwidthUpFraction = 4;

        /// <summary>
        /// Download bandwidth is averaged over 3 samples.
        /// </summary>
        private const int DownloadBandwidthWindowSize = 3;

        /// <summary>
        /// If new encoded bitrate value is less than 20% (0.2) of the current
        /// encoded bitrate kernel, reset it to the sample value
        /// </summary>
        private const double EncodedBitrateDownFraction = -0.2;

        /// <summary>
        /// If new encoded bitrate value is larger than 20% (0.2) of the current
        /// encoded bitrate kernel, reset it to the sample value
        /// </summary>
        private const double EncodedBitrateUpFraction = 0.2;

        /// <summary>
        /// The endoded bitrate is averaged over 5 samples.
        /// </summary>
        private const int EncodedBitrateWindowSize = 5;

        /// <summary>
        /// Our current download state
        /// </summary>
        private DownloadState m_downloadState = DownloadState.Buffering;

        /// <summary>
        /// THe bitrate we are locked to
        /// </summary>
        private int m_lockedBitrate = -1;

        /// <summary>
        /// Our maximum bitrate
        /// </summary>
        private ulong m_maxBitrate = Configuration.Heuristics.Network.MaxBitrate;

        /// <summary>
        /// Our minimum bitrate
        /// </summary>
        private ulong m_minBitrate = Configuration.Heuristics.Network.MinBitrate;

        /// <summary>
        /// Points to our network heuristics parameters 
        /// </summary>
        private NetworkHeuristicsParams m_networkHeuristicsParams;

        /// <summary>
        /// The id of the stream we belong to
        /// </summary>
        private int m_streamId = -1;

        /// <summary>
        /// The total stream duration downloaded so far
        /// </summary>
        private double m_totalStreamDownloaded;

        /// <summary>
        /// Initializes a new instance of the NetworkMediaInfo class
        /// </summary>
        /// <param name="index">the index of the stream we belong to</param>
        /// <param name="numBitrates">the number of bitrates we have</param>
        /// <param name="bitrates">the array of bitrate info for this stream</param>
        /// <param name="networkHeuristicsParams">our network heuristics parameters</param>
        public NetworkMediaInfo(int index, int numBitrates, ulong[] bitrates, NetworkHeuristicsParams networkHeuristicsParams)
        {
            m_streamId = index;
            m_networkHeuristicsParams = networkHeuristicsParams;

            BufferFullnessWindow = new SlidingWindow(
                BufferFullnessWindowSize,
                BufferFullnessUpFraction,
                BufferFullnessDownFraction,
                m_networkHeuristicsParams.BufferingSlopeThreshold);

            DownloadBandwidthWindow = new SlidingWindow(
                DownloadBandwidthWindowSize,
                DownloadBandwidthUpFraction,
                DownloadBandwidthDownFraction,
                0 /* Unused */);

            BitratesInfo = new BitrateInfo[numBitrates];

            for (int i = 0; i < numBitrates; i++)
            {
                BitratesInfo[i] = new BitrateInfo(index, EncodedBitrateWindowSize, EncodedBitrateUpFraction, EncodedBitrateDownFraction, bitrates[i]);
            }

            NextBitrate = BitratesInfo[0].NominalBitrate;
            RelativeContentDownloadSpeed = m_networkHeuristicsParams.RelativeContentDownloadSpeed;
            UpdateUsableBitrates();
        }

        /// <summary>
        /// Prevents a default instance of the NetworkMediaInfo class from being created
        /// </summary>
        private NetworkMediaInfo()
        {
        }

        /// <summary>
        /// Gets or sets the nominal media bit rates in bps
        /// </summary>
        public BitrateInfo[] BitratesInfo { get; set; }

        /// <summary>
        /// Gets or sets a summary of up to 8 bit rates validation state
        /// </summary>
        public byte BitrateValidation { get; set; }

        /// <summary>
        /// Gets or sets the buffer fullness
        /// </summary>
        public SlidingWindow BufferFullnessWindow { get; set; }

        /// <summary>
        /// Gets or sets the download bit rate
        /// </summary>
        public SlidingWindow DownloadBandwidthWindow { get; set; }

        /// <summary>
        /// Gets or sets the current state of the download queue for this stream
        /// </summary>
        public DownloadState DownloadState
        {
            get
            {
                return m_downloadState;
            }

            set
            {
                m_downloadState = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether we are limiting the 
        /// number of steps we can in a bitrate jump?
        /// </summary>
        public bool IsLimitBitrateSteps { get; set; }

        /// <summary>
        /// Gets or sets the index within mediaBitRates to which the stream is locked. -1 means not locked
        /// </summary>
        public int LockedBitRate
        {
            get
            {
                return m_lockedBitrate;
            }

            set
            {
                m_lockedBitrate = value;
            }
        }

        /// <summary>
        /// Gets or sets the max bitrate allowed. Having values other than the
        /// defaults will force the heuristics to use only the sub set
        /// of the available bit rates
        /// </summary>
        public ulong MaxBitrate
        {
            get
            {
                return m_maxBitrate;
            }

            set
            {
                m_maxBitrate = value;
            }
        }

        /// <summary>
        /// Gets or sets the min bitrate allowed. Having values other than the
        /// defaults will force the heuristics to use only the sub set
        /// of the available bit rates
        /// </summary>
        public ulong MinBitrate
        {
            get
            {
                return m_minBitrate;
            }

            set
            {
                m_minBitrate = value;
            }
        }

        /// <summary>
        /// Gets or sets the next bit rate to use, in bps
        /// </summary>
        public ulong NextBitrate { get; set; }

        /// <summary>
        /// Gets or sets the the last time we attempted to improve
        /// the bit rate for the next chunk to be downloaded, this is
        /// time is relative to totalStreamDownloaded
        /// </summary>
        public double PreviousAttempt { get; set; }

        /// <summary>
        /// Gets or sets the previous nominal bit rate used in bps
        /// </summary>
        public ulong PreviousBitrate { get; set; }

        /// <summary>
        /// Gets or sets the expected relationship between download speed
        /// (seconds worth of content / second) and real time
        /// playback, it is normally 1.25 and 1.0 after seek, those
        /// values are defined in NetworkHeuristicsParams
        /// </summary>
        public double RelativeContentDownloadSpeed { get; set; }

        /// <summary>
        /// Gets the id of the stream that we belong to
        /// </summary>
        public int StreamId
        {
            get
            {
                return m_streamId;
            }
        }

        /// <summary>
        /// Gets or sets the total stream duration downloaded so for
        /// purposes of pacing the bit rate improvement
        /// </summary>
        public double TotalStreamDownloaded
        {
            get
            {
                return m_totalStreamDownloaded;
            }

            set
            {
                m_totalStreamDownloaded = value;
            }
        }

        /// <summary>
        /// Find the bit rate entry that matches the passed parameter
        /// </summary>
        /// <param name="bitRate">Bit rate to match in bps</param>
        /// <returns>the index of the bitrate</returns>
        public int FindBitRateIndex(ulong bitRate)
        {
            for (int i = 0; i < BitratesInfo.Length; i++)
            {
                if (BitratesInfo[i].NominalBitrate == bitRate)
                {
                    return i;
                }
            }

            Debug.Assert(false, "FindBitRateIndex: Didn't find bit rate:" + bitRate);

            return -1;
        }

        /// <summary>
        /// Find the closest usable bitrate to the given bitrate index
        /// </summary>
        /// <param name="index">bitrate index to search against</param>
        /// <returns>the closest usable bitrate</returns>
        public ulong FindClosestBitrateByIndex(int index)
        {
            // If our index is less than zero, find the first usable bitrate
            if (index < 0)
            {
                // Just get the first usable bitrate
                for (int i = 0; i < BitratesInfo.Length; i++)
                {
                    if (BitratesInfo[i].IsUsable)
                    {
                        return BitratesInfo[i].NominalBitrate;
                    }
                }
            }
            else if (index >= BitratesInfo.Length)
            {
                // If too big, find the last usable bitrate
                for (int i = BitratesInfo.Length - 1; i >= 0; i--)
                {
                    if (BitratesInfo[i].IsUsable)
                    {
                        return BitratesInfo[i].NominalBitrate;
                    }
                }
            }
            else
            {
                // Find the closest usable one less than or equal to the given one
                for (int i = index; i >= 0; i--)
                {
                    if (BitratesInfo[i].IsUsable)
                    {
                        return BitratesInfo[i].NominalBitrate;
                    }
                }
            }

            // All of the above failed, pick a default
            return FindDefaultBitrate();
        }

        /// <summary>
        /// Find the closest bitrate to the given value
        /// </summary>
        /// <param name="bitRate">value to match</param>
        /// <returns>closest bitrate to the input bitrate</returns>
        public ulong FindClosestBitrateByValue(ulong bitRate)
        {
            for (int i = BitratesInfo.Length - 1; i >= 0; i--)
            {
                if (BitratesInfo[i].IsUsable && BitratesInfo[i].NominalBitrate <= bitRate)
                {
                    return BitratesInfo[i].NominalBitrate;
                }
            }

            // Pick a default
            return FindDefaultBitrate();
        }

        /// <summary>
        /// Picks a default bitrate. The default bitrate is the first usable one we find
        /// that is within the min and max rates.
        /// </summary>
        /// <returns>the default bitrate</returns>
        public ulong FindDefaultBitrate()
        {
            // First try to find a default that meets all the
            // criterias
            for (int i = 0; i < BitratesInfo.Length; i++)
            {
                if (BitratesInfo[i].IsUsable)
                {
                    return BitratesInfo[i].NominalBitrate;
                }
            }

            // Now try to find the lowest bit rate that is within the
            // min/max range defined
            for (int i = 0; i < BitratesInfo.Length; i++)
            {
                if (BitratesInfo[i].NominalBitrate >= MinBitrate &&
                    BitratesInfo[i].NominalBitrate <= MaxBitrate)
                {
                    return BitratesInfo[i].NominalBitrate;
                }
            }

            // If all else fails, just pick the lowest bit rate
            // available
            return BitratesInfo[0].NominalBitrate;
        }

        /// <summary>
        /// Resets the last attempt at improving the bitrate
        /// </summary>
        public void ResetImprovingBitRate()
        {
            PreviousAttempt = 0;
        }

        /// <summary>
        /// Resets this object
        /// </summary>
        /// <param name="isSeek">did this happen because of a seek?</param>
        public void ResetMediaInfo(bool isSeek)
        {
            DownloadState = DownloadState.Buffering;
            PreviousBitrate = FindDefaultBitrate();
            NextBitrate = PreviousBitrate;
            TotalStreamDownloaded = 0;
            BufferFullnessWindow.ResetSlidingWindow();
            ResetImprovingBitRate();

            if (isSeek)
            {
                RelativeContentDownloadSpeed = m_networkHeuristicsParams.RelativeContentDownloadSpeedSeek;
            }
            else
            {
                RelativeContentDownloadSpeed = m_networkHeuristicsParams.RelativeContentDownloadSpeed;
            }

            ResetSafetyTics(isSeek);
        }

        /// <summary>
        /// Recompute which bit rates qualify for use depending on the
        /// min/max bit rate limitation and dropped frame rate
        /// observed on it
        /// </summary>
        public void UpdateUsableBitrates()
        {
            byte newbitRateValidation = 0;

            for (int i = 0; i < BitratesInfo.Length; i++)
            {
                bool doUse = BitratesInfo[i].IsBitrateTested ? BitratesInfo[i].IsBitrateSupported : true;

                if (doUse == true)
                {
                    if (BitratesInfo[i].NominalBitrate < MinBitrate ||
                        BitratesInfo[i].NominalBitrate > MaxBitrate)
                    {
                        doUse = false;
                    }
                }

                BitratesInfo[i].IsUsable = doUse;
                if (doUse == true && i < 8)
                {
                    // We can summarize up to 8 bit rates, 0 thru 7
                    newbitRateValidation |= (byte)(1 << i);
                }
            }

            BitrateValidation = newbitRateValidation;
        }

        /// <summary>
        /// Update our list of usable bitrates
        /// </summary>
        /// <param name="minBitrate">new min bitrate</param>
        /// <param name="maxBitrate">new max bitrate</param>
        public void UpdateUsableBitrates(ulong minBitrate, ulong maxBitrate)
        {
            MinBitrate = minBitrate;
            MaxBitrate = maxBitrate;

            UpdateUsableBitrates();

            Tracer.Trace(TraceChannel.NetHeur, "INFO: Stream {0}: Setting minBitrate:{1} maxBitRate:{2}", StreamId, MinBitrate, MaxBitrate);
        }
    }
}
