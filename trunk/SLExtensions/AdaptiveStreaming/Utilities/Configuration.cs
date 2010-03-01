//*@@@*************************************************************************
//
// Microsoft Expression Encoder
// Copyright © Microsoft Corporation. All rights reserved.
//
//*@@@*************************************************************************
namespace MS.Internal.Expression.Encoder.AdaptiveStreaming.Utilities
{
    using System;

    /// <summary>
    /// This class holds all of the information from our configuration file
    /// </summary>
    internal static class Configuration
    {
        /// <summary>
        /// The major version of the config file
        /// </summary>
        private static int sm_majorVersion = 1;

        /// <summary>
        /// Gets or sets the major version of the config file
        /// </summary>
        public static int MajorVersion
        {
            get
            {
                return sm_majorVersion;
            }

            set
            {
                sm_majorVersion = value;
            }
        }

        /// <summary>
        /// Gets or sets the minor version of the config file
        /// </summary>
        public static int MinorVersion { get; set; }

        /// <summary>
        /// Heuristics related configuration
        /// </summary>
        public static class Heuristics
        {
            /// <summary>
            /// Describes network related configuration parameters
            /// </summary>
            public static class Network
            {
                /// <summary>
                /// If measured bandwidth is 5 time higher than initial bandwidth, 
                /// assume it is in cache.
                /// </summary>
                private static double sm_cacheBandwidthFactor = 5;

                /// <summary>
                /// Minimum value to consider it in cache, in bps
                /// </summary>
                private static double sm_cacheBandwidthMin = 2 * 1000 * 1000;

                /// <summary>
                /// Lower buffer fullness boundary in seconds
                /// <para>
                /// NOTE to avoid oscilations the "band" on which the buffer
                /// fullness is allowed to oscillate should be a few times
                /// larger than the chunk time
                /// </para>
                /// </summary>
                private static double sm_lowerBufferFullness = 12;

                /// <summary>
                /// Max bit rates allowed in bps. Having values other than the
                /// defaults will force the heuristics to use only the sub set
                /// of the available bit rates
                /// </summary>
                private static ulong sm_maxBitrate = 100 * 1000 * 1000;

                /// <summary>
                /// Max download buffer size, in seconds
                /// </summary>
                private static double sm_maxBufferSize = 20;

                /// <summary>
                /// Min bit rates allowed in bps. Having values other than the
                /// defaults will force the heuristics to use only the sub set
                /// of the available bit rates
                /// </summary>
                private static ulong sm_minBitrate;

                /// <summary>
                /// Panic buffer fullness boundary in seconds
                /// <para>
                /// NOTE to avoid oscilations the "band" on which the buffer
                /// fullness is allowed to oscillate should be a few times
                /// larger than the chunk time
                /// </para>
                /// </summary>
                private static double sm_panicBufferFullness = 7;

                /// <summary>
                /// Minimum time period in seconds to allow an attempt to improve (grow)
                /// the bit rate for the next chunk to download
                /// </summary>
                private static double sm_tryImprovingBitratePeriod = 5;

                /// <summary>
                /// Upper buffer fullness boundary in seconds
                /// <para>
                /// NOTE to avoid oscilations the "band" on which the buffer
                /// fullness is allowed to oscillate should be a few times
                /// larger than the chunk time
                /// </para>
                /// </summary>
                private static double sm_upperBufferFullness = 17;

                /// <summary>
                /// Gets or sets the cache bandwidth factor
                /// </summary>
                public static double CacheBandwidthFactor
                {
                    get
                    {
                        return sm_cacheBandwidthFactor;
                    }

                    set
                    {
                        sm_cacheBandwidthFactor = value;
                    }
                }

                /// <summary>
                /// Gets or sets the minimum cache bandwidth
                /// </summary>
                public static double CacheBandwidthMin
                {
                    get
                    {
                        return sm_cacheBandwidthMin;
                    }

                    set
                    {
                        sm_cacheBandwidthMin = value;
                    }
                }

                /// <summary>
                /// Gets or sets the lower buffer fullness
                /// </summary>
                public static double LowerBufferFullness
                {
                    get
                    {
                        return sm_lowerBufferFullness;
                    }

                    set
                    {
                        sm_lowerBufferFullness = value;
                    }
                }

                /// <summary>
                /// Gets or sets the max bitrate
                /// </summary>
                public static ulong MaxBitrate
                {
                    get
                    {
                        return sm_maxBitrate;
                    }

                    set
                    {
                        sm_maxBitrate = value;
                    }
                }

                /// <summary>
                /// Gets or sets the max download buffer size, in seconds
                /// </summary>
                public static double MaxBufferSize
                {
                    get
                    {
                        return sm_maxBufferSize;
                    }

                    set
                    {
                        sm_maxBufferSize = value;
                    }
                }

                /// <summary>
                /// Gets or sets the min bitrate
                /// </summary>
                public static ulong MinBitrate
                {
                    get
                    {
                        return sm_minBitrate;
                    }

                    set
                    {
                        sm_minBitrate = value;
                    }
                }

                /// <summary>
                /// Gets or sets the panic buffer fullness
                /// </summary>
                public static double PanicBufferFullness
                {
                    get
                    {
                        return sm_panicBufferFullness;
                    }

                    set
                    {
                        sm_panicBufferFullness = value;
                    }
                }

                /// <summary>
                /// Gets or sets the period for trying to improve the bitrate
                /// </summary>
                public static double TryImprovingBitratePeriod
                {
                    get
                    {
                        return sm_tryImprovingBitratePeriod;
                    }

                    set
                    {
                        sm_tryImprovingBitratePeriod = value;
                    }
                }

                /// <summary>
                /// Gets or sets the upper buffer fullness boundary
                /// </summary>
                public static double UpperBufferFullness
                {
                    get
                    {
                        return sm_upperBufferFullness;
                    }

                    set
                    {
                        sm_upperBufferFullness = value;
                    }
                }
            }

            /// <summary>
            /// Frame rate constants
            /// </summary>
            public static class FrameRate
            {
                /// <summary>
                /// Base revocation period, which geometrically scales with
                /// the number of suspensions
                /// </summary>
                private static int sm_baseRevocationPeriod = 60; // seconds
 
                /// <summary>
                /// Maximum number of levels to drop
                /// </summary>
                private static int sm_fpsMaxDropLevels = 2;

                /// <summary>
                /// Percentage of dropped frames to start throttling at
                /// </summary>
                private static double sm_fpsMaxDropPercent = 0.4;

                /// <summary>
                /// Low level to start suspending
                /// </summary>
                private static int sm_fpsLowDropLevels = 1;

                /// <summary>
                /// Percentage at which to start throttling at
                /// </summary>
                private static double sm_fpsLowDropPercent = 0.7;

                /// <summary>
                /// Percentage to start throttling for rendered fps
                /// </summary>
                private static double sm_renderedFpsMaxDropPercent = 0.4;

                /// <summary>
                /// Max drop level
                /// </summary>
                private static int sm_renderedFpsMaxDropLevel = 2;

                /// <summary>
                /// Low level to start throttling at
                /// </summary>
                private static double sm_renderedFpsLowDropPercent = 0.90;

                /// <summary>
                /// Low drop level
                /// </summary>
                private static int sm_renderedFpsLowDropLevel = 1;

                /// <summary>
                /// Low safety value
                /// </summary>
                private static int sm_safetyTicsLow = 0;

                /// <summary>
                /// High safety value
                /// </summary>
                private static int sm_safetyTicsHigh = 2;

                /// <summary>
                /// Variables used to scale the observation period, the worst
                /// things are, the shorter that period
                /// </summary>
                private static int sm_observationTicsMax = 4;

                /// <summary>
                /// Suspensions induced by minimizing browser are undone is
                /// happened within this time window
                /// </summary>
                private static double sm_undoTimeWindow = 5;

                /// <summary>
                /// Gets base revocation period, which geometrically scales with
                /// the number of suspensions
                /// </summary>
                public static int BaseRevocationPeriod
                {
                    get
                    {
                        return sm_baseRevocationPeriod;
                    }
                }

                /// <summary>
                /// Gets maximum number of levels to drop
                /// </summary>
                public static int FpsMaxDropLevels
                {
                    get
                    {
                        return sm_fpsMaxDropLevels;
                    }
                }

                /// <summary>
                /// Gets percentage of dropped frames to start throttling at
                /// </summary>
                public static double FpsMaxDropPercent
                {
                    get
                    {
                        return sm_fpsMaxDropPercent;
                    }
                }

                /// <summary>
                /// Gets low level to start suspending
                /// </summary>
                public static int FpsLowDropLevels
                {
                    get
                    {
                        return sm_fpsLowDropLevels;
                    }
                }

                /// <summary>
                /// Gets percentage at which to start throttling at
                /// </summary>
                public static double FpsLowDropPercent
                {
                    get
                    {
                        return sm_fpsLowDropPercent;
                    }
                }

                /// <summary>
                /// Gets percentage to start throttling for rendered fps
                /// </summary>
                public static double RenderedFpsMaxDropPercent
                {
                    get
                    {
                        return sm_renderedFpsMaxDropPercent;
                    }
                }

                /// <summary>
                /// Gets max drop level
                /// </summary>
                public static int RenderedFpsMaxDropLevel
                {
                    get
                    {
                        return sm_renderedFpsMaxDropLevel;
                    }
                }

                /// <summary>
                /// Gets low level to start throttling at
                /// </summary>
                public static double RenderedFpsLowDropPercent
                {
                    get
                    {
                        return sm_renderedFpsLowDropPercent;
                    }
                }

                /// <summary>
                /// Gets low drop level
                /// </summary>
                public static int RenderedFpsLowDropLevel
                {
                    get
                    {
                        return sm_renderedFpsLowDropLevel;
                    }
                }

                /// <summary>
                /// Gets low safety value
                /// </summary>
                public static int SafetyTicsLow
                {
                    get
                    {
                        return sm_safetyTicsLow;
                    }
                }

                /// <summary>
                /// Gets high safety value
                /// </summary>
                public static int SafetyTicsHigh
                {
                    get
                    {
                        return sm_safetyTicsHigh;
                    }
                }

                /// <summary>
                /// Gets variables used to scale the observation period, the worst
                /// things are, the shorter that period
                /// </summary>
                public static int ObservationTicsMax
                {
                    get
                    {
                        return sm_observationTicsMax;
                    }
                }

                /// <summary>
                /// Gets suspensions induced by minimizing browser are undone is
                /// happened within this time window
                /// </summary>
                public static double UndoTimeWindow
                {
                    get
                    {
                        return sm_undoTimeWindow;
                    }
                }
            }
        }

        /// <summary>
        /// Playback related configuration
        /// </summary>
        internal static class Playback
        {
            /// <summary>
            /// Maximum tolerated number of consecutive missing or corrupted chunks
            /// </summary>
            private static int sm_maxMissingOrCorruptedChunks = 10;

            /// <summary>
            /// Gets or sets the max missing or corrupted chunks
            /// </summary>
            public static int MaxMissingOrCorruptedChunks
            {
                get
                {
                    return sm_maxMissingOrCorruptedChunks;
                }

                set
                {
                    sm_maxMissingOrCorruptedChunks = value;
                }
            }
        }

        /// <summary>
        /// Stream related configuration
        /// </summary>
        internal static class Streams
        {
            /// <summary>
            /// We can support up to 10 bitrates per stream
            /// </summary>
            private static int sm_maxBitratesPerStream = 10;

            /// <summary>
            /// Make sure we have room for at least 20 1 second chunks.
            /// </summary>
            private static int sm_maxDownloadQueue = 20;

            /// <summary>
            /// Right now we only support 1 audio and 1 video stream
            /// </summary>
            private static int sm_maxStreams = 2;

            /// <summary>
            /// Gets or sets the max bitrates per stream
            /// </summary>
            public static int MaxBitratesPerStream
            {
                get
                {
                    return sm_maxBitratesPerStream;
                }

                set
                {
                    sm_maxBitratesPerStream = value;
                }
            }

            /// <summary>
            /// Gets or sets the max chunks in the download queue
            /// </summary>
            public static int MaxDownloadQueue
            {
                get
                {
                    return sm_maxDownloadQueue;
                }

                set
                {
                    sm_maxDownloadQueue = value;
                }
            }

            /// <summary>
            /// Gets or sets the max number of streams
            /// </summary>
            public static int MaxStreams
            {
                get
                {
                    return sm_maxStreams;
                }

                set
                {
                    sm_maxStreams = value;
                }
            }
        }
    }
}
