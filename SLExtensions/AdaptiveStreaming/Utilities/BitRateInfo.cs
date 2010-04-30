//*@@@*************************************************************************
//
// Microsoft Expression Encoder
// Copyright © Microsoft Corporation. All rights reserved.
//
//*@@@*************************************************************************
namespace MS.Internal.Expression.Encoder.AdaptiveStreaming.Utilities
{
    /// <summary>
    /// The current state of our frame rate test on a bitrate.
    /// </summary>
    internal enum FrameRateTestState
    {
        /// <summary>
        /// Initializing the bitrate test
        /// </summary>
        Init,

        /// <summary>
        /// Safety period where FPS are ignored
        /// </summary>
        SafetyPeriod,

        /// <summary>
        /// Keeping track of FPS to get average over
        /// the period, at the end we decide whether
        /// or not the bit rate can be used or needs
        /// to be temporarily suspended        
        /// </summary>
        ObservationPeriod,

        /// <summary>
        /// Bitrate has been suspended
        /// </summary>
        Suspended
    }

    /// <summary>
    /// Used to maintain information relevant to a specific bit rate
    /// </summary>
    internal class BitrateInfo
    {
        /// <summary>
        /// Whether or not this bit rate has been tested
        /// </summary>
        private bool m_isBitrateTested;

        /// <summary>
        /// Frame rate test state
        /// </summary>
        private FrameRateTestState m_frameRateTestState = FrameRateTestState.Init;

        /// <summary>
        /// This is the scheduled time at which a temporarily suspended
        /// bit rate will be tested again
        /// </summary>
        private double m_revocationTime;

        /// <summary>
        /// Allows rolling back the revocation time
        /// </summary>
        private double m_revocationTimePrev;

        /// <summary>
        /// When the last bit rate suspension happened
        /// </summary>
        private double m_suspensionTime = double.MinValue;

        /// <summary>
        /// Number of times this bit rate has been temporarily
        /// suspended
        /// </summary>
        private int m_numSuspensions;

        /// <summary>
        /// Snapshot of the cumulative tics used as the time base for
        /// cumulative rendered FPS
        /// </summary>
        private long m_cumulativeRenderedTics;

        /// <summary>
        /// Is this bitrate supported?
        /// </summary>
        private bool m_isBitrateSupported = true;

        /// <summary>
        /// Is this bitrate usable?
        /// </summary>
        private bool m_isUsable = true;

        /// <summary>
        /// The stream index that this bitrate belongs to
        /// </summary>
        private int m_streamIndex = -1;

        /// <summary>
        /// Initializes a new instance of the BitrateInfo class
        /// </summary>
        /// <param name="strmIndex">index of the stream</param>
        /// <param name="encodedBitrateWindowSize">encoded bitrate window size</param>
        /// <param name="encodedBitrateUpFraction">bitrate up fraction</param>
        /// <param name="encodedBitrateDownFraction">bitrate down fraction</param>
        /// <param name="bitRate">the bitrate we are storing</param>
        internal BitrateInfo(
            int strmIndex,
            int encodedBitrateWindowSize,
            double encodedBitrateUpFraction,
            double encodedBitrateDownFraction,
            ulong bitRate)
        {
            StreamIndex = strmIndex;

            EncodedBitrateWindow = new SlidingWindow(
                encodedBitrateWindowSize, 
                encodedBitrateUpFraction, 
                encodedBitrateDownFraction, 
                0.0 /* unused */);

            NominalBitrate = bitRate;
        }

        /// <summary>
        /// Prevents a default instance of the BitrateInfo class from being created
        /// </summary>
        private BitrateInfo()
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not this bit rate has been tested
        /// </summary>
        public bool IsBitrateTested
        {
            get
            {
                return m_isBitrateTested;
            }

            set
            {
                m_isBitrateTested = value;
            }
        }

        /// <summary>
        /// Gets or sets frame rate test state
        /// </summary>
        public FrameRateTestState FrameRateTestState
        {
            get
            {
                return m_frameRateTestState;
            }

            set
            {
                m_frameRateTestState = value;
            }
        }

        /// <summary>
        /// Gets or sets the scheduled time at which a temporarily suspended
        /// bit rate will be tested again
        /// </summary>
        public double RevocationTime
        {
            get
            {
                return m_revocationTime;
            }

            set
            {
                m_revocationTime = value;
            }
        }

        /// <summary>
        /// Gets or sets allow rolling back the revocation time
        /// </summary>
        public double RevocationTimePrev
        {
            get
            {
                return m_revocationTimePrev;
            }

            set
            {
                m_revocationTimePrev = value;
            }
        }

        /// <summary>
        /// Gets or sets when the last bit rate suspension happened
        /// </summary>
        public double SuspensionTime
        {
            get
            {
                return m_suspensionTime;
            }

            set
            {
                m_suspensionTime = value;
            }
        }

        /// <summary>
        /// Gets or sets number of times this bit rate has been temporarily
        /// suspended
        /// </summary>
        public int NumSuspensions
        {
            get
            {
                return m_numSuspensions;
            }

            set
            {
                m_numSuspensions = value;
            }
        }

        /// <summary>
        /// Gets or sets snapshot of the cumulative tics used as the time base for
        /// cumulative rendered FPS
        /// </summary>
        public long CumulativeRenderedTics
        {
            get
            {
                return m_cumulativeRenderedTics;
            }

            set
            {
                m_cumulativeRenderedTics = value;
            }
        }

        /// <summary>
        /// Gets or sets the actual encoded media bit rate
        /// </summary>
        public SlidingWindow EncodedBitrateWindow { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether when tested, the test's result, if not tested
        /// remains 'true'
        /// </summary>
        public bool IsBitrateSupported
        {
            get
            {
                return m_isBitrateSupported;
            }

            set
            {
                m_isBitrateSupported = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the result of combining the logic
        /// from min/max bit rates allowed and dropped frames rate
        /// </summary>
        public bool IsUsable
        {
            get
            {
                return m_isUsable;
            }

            set
            {
                m_isUsable = value;
            }
        }

        /// <summary>
        /// Gets or sets the nominal bit rate
        /// </summary>
        public ulong NominalBitrate { get; set; }

        /// <summary>
        /// Gets or sets the stream we belong to
        /// </summary>
        public int StreamIndex
        {
            get
            {
                return m_streamIndex;
            }

            set
            {
                m_streamIndex = value;
            }
        }

        /// <summary>
        /// Causes the bit rate to be tested again
        /// </summary>
        internal void ResetFrameRateTest()
        {
            IsBitrateTested = false;
            IsBitrateSupported = true;

            FrameRateTestState = FrameRateTestState.Init;
        }
    }
}
