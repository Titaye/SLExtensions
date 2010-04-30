//*@@@*************************************************************************
//
// Microsoft Expression Encoder
// Copyright © Microsoft Corporation. All rights reserved.
//
//*@@@*************************************************************************
namespace MS.Internal.Expression.Encoder.AdaptiveStreaming.Heuristic
{
    /// <summary>
    /// This class is used to contain all the parameters that play a
    /// role in the network heuristics
    /// </summary>
    internal class NetworkHeuristicsParams
    {
        /// <summary>
        /// Version 1: Expression encoder heuristics
        /// </summary>
        private int m_netHeuristicsVersion = 1;

        /// <summary>
        /// Version 1: Expression encoder log version
        /// </summary>
        private int m_netHeuristicsLogVersion = 1;

        /// <summary>
        /// If the buffer fullness is changing with values +/- within
        /// this slope, the adjustment is allowed in steps of 1,
        /// otherwise larger steps can be used
        /// </summary>
        private double m_bufferingSlopeThreshold = 0.5;

        /// <summary>
        /// Fraction of the measured bit rate to use when
        /// selecting the bit rate of the next chunk
        /// </summary>
        private double m_downloadBandwidthFraction = 0.8;

        /// <summary>
        /// Fraction of the buffer fullness to use when
        /// selecting the bit rate of the next chunk
        /// </summary>
        private double m_bufferFullnessFraction = 0.8;

        /// <summary>
        /// This is the expected relationship between
        /// download speed (seconds worth of content / second) and
        /// real time playback, currently defined as 1.25
        /// </summary> 
        private double m_relativeContentDownloadSpeed = 1.25;

        /// <summary>
        /// This is the expected relationship between
        /// download speed (seconds worth of content / second) and
        /// real time playback after seeking, currently defined as 1.0
        /// </summary> 
        private double m_relativeContentDownloadSpeedSeek = 1.0;

        /// <summary>
        /// What is considered high bandwidth when there is no history
        /// </summary>
        private ulong m_absoluteHighBandwidth = 20 * 1000 * 1000;

        /// <summary>
        /// Gets or sets the version of the heuristics module
        /// </summary>
        public int NetHeuristicsVersion
        {
            get
            {
                return m_netHeuristicsVersion;
            }

            set
            {
                m_netHeuristicsVersion = value;
            }
        }

        /// <summary>
        /// Gets or sets the heuristics log version
        /// </summary>
        public int NetHeuristicsLogVersion
        {
            get
            {
                return m_netHeuristicsLogVersion;
            }

            set
            {
                m_netHeuristicsLogVersion = value;
            }
        }

        /// <summary>
        /// Gets or sets the buffering slope threshold
        /// </summary>
        public double BufferingSlopeThreshold
        {
            get
            {
                return m_bufferingSlopeThreshold;
            }

            set
            {
                m_bufferingSlopeThreshold = value;
            }
        }

        /// <summary>
        /// Gets or sets the download bandwidth fraction
        /// </summary>
        public double DownloadBandwidthFraction
        {
            get
            {
                return m_downloadBandwidthFraction;
            }

            set
            {
                m_downloadBandwidthFraction = value;
            }
        }

        /// <summary>
        /// Gets or sets the buffer fullness fraction
        /// </summary>
        public double BufferFullnessFraction
        {
            get
            {
                return m_bufferFullnessFraction;
            }

            set
            {
                m_bufferFullnessFraction = value;
            }
        }

        /// <summary>
        /// Gets or sets the relative content download speed
        /// </summary>
        public double RelativeContentDownloadSpeed
        {
            get
            {
                return m_relativeContentDownloadSpeed;
            }

            set
            {
                m_relativeContentDownloadSpeed = value;
            }
        }

        /// <summary>
        /// Gets or sets the relative content download speed for seeking
        /// </summary>
        public double RelativeContentDownloadSpeedSeek
        {
            get
            {
                return m_relativeContentDownloadSpeedSeek;
            }

            set
            {
                m_relativeContentDownloadSpeedSeek = value;
            }
        }

        /// <summary>
        /// Gets or sets the absolute highest bandwidth
        /// </summary>
        public ulong AbsoluteHighBandwidth
        {
            get
            {
                return m_absoluteHighBandwidth;
            }

            set
            {
                m_absoluteHighBandwidth = value;
            }
        }
    }
}
