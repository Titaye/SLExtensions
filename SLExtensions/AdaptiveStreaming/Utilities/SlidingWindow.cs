//*@@@*************************************************************************
//
// Microsoft Expression Encoder
// Copyright © Microsoft Corporation. All rights reserved.
//
//*@@@*************************************************************************
namespace MS.Internal.Expression.Encoder.AdaptiveStreaming.Utilities
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// This class computes an average over the window size.
    /// The slope at which the parameter evolves is a double.
    /// The last kernel value is a uint.
    /// </summary>
    internal class SlidingWindow
    {
        /// <summary>
        /// The current kernel value
        /// </summary>
        private double m_currentKernel;

        /// <summary>
        /// The current slope over our window
        /// </summary>
        private double m_currentSlope;

        /// <summary>
        /// The current time we are calculating for
        /// </summary>
        private DateTime m_currentTime = DateTime.Now;

        /// <summary>
        ///  The max percentage by which we can decrease the value
        /// </summary>
        private double m_downFraction;

        /// <summary>
        /// The index of the next sample
        /// </summary>
        private uint m_nextSample;

        /// <summary>
        /// The total number of samples we have so far
        /// </summary>
        private uint m_numSamples;

        /// <summary>
        /// The previous kernel value
        /// </summary>
        private double m_previousKernel;

        /// <summary>
        /// The previous time we calculated a value
        /// </summary>
        private DateTime m_previousTime = DateTime.MinValue;

        /// <summary>
        /// The samples we are tracking, sized by windowSize
        /// </summary>
        private double[] m_samples;

        /// <summary>
        /// Used to declare slope changing slow or fast
        /// </summary>
        private double m_slopeThreshold;

        /// <summary>
        /// The running total of our sample values
        /// </summary>
        private double m_sum;

        /// <summary>
        /// The max percentage by which we can increase the kernel
        /// </summary>
        private double m_upFraction;

        /// <summary>
        /// Initializes a new instance of the SlidingWindow class
        /// </summary>
        /// <param name="windowSize">the number of samples we want to compute over</param>
        /// <param name="upFraction">increasing percentage</param>
        /// <param name="downFraction">decreasing percentage</param>
        /// <param name="slopeThreshold">threshold for fast/slow changing</param>
        public SlidingWindow(int windowSize, double upFraction, double downFraction, double slopeThreshold)
        {
            Debug.Assert(
                windowSize > 0 && upFraction > 0 && downFraction < 0 && slopeThreshold >= 0 && slopeThreshold < 1, 
                "SlidingWindow: Invalid constructor parameters");

            ResetSlidingWindow();

            m_upFraction = upFraction;
            m_downFraction = downFraction;
            m_slopeThreshold = slopeThreshold;

            m_samples = new double[windowSize];
        }

        /// <summary>
        /// Prevents a default instance of the SlidingWindow class from being created
        /// </summary>
        private SlidingWindow()
        {
        }

        /// <summary>
        /// Gets the current average value over the window
        /// </summary>
        public double CurrentKernel
        {
            get
            {
                return m_currentKernel;
            }
        }

        /// <summary>
        /// Gets the current slope over our window
        /// </summary>
        public double CurrentSlope
        {
            get
            {
                return m_currentSlope;
            }
        }

        /// <summary>
        /// Gets a value indicating whether we are an increasing or a decreasing trend
        /// </summary>
        public bool IsDecreasing
        {
            get
            {
                return m_currentSlope < 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether we are fast decreasing
        /// </summary>
        public bool IsFastDecreasing
        {
            get
            {
                return m_currentSlope < -m_slopeThreshold;
            }
        }

        /// <summary>
        /// Gets a value indicating whether we are fast increasing
        /// </summary>
        public bool IsFastIncreasing
        {
            get
            {
                return m_currentSlope > m_slopeThreshold;
            }
        }

        /// <summary>
        /// Gets a value indicating whether we are an increasing trend
        /// </summary>
        public bool IsIncreasing
        {
            get
            {
                return m_currentSlope > 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether we are slow changing
        /// </summary>
        public bool IsSlowChanging
        {
            get
            {
                return m_currentSlope >= -m_slopeThreshold && m_currentSlope <= m_slopeThreshold;
            }
        }

        /// <summary>
        /// Gets a value indicating whether we are slowly decreasing
        /// </summary>
        public bool IsSlowDecreasing
        {
            get
            {
                return m_currentSlope < 0 && m_currentSlope >= -m_slopeThreshold;
            }
        }

        /// <summary>
        /// Gets a value indicating whether we are slowly increasing
        /// </summary>
        public bool IsSlowIncreasing
        {
            get
            {
                return m_currentSlope > 0 && m_currentSlope <= m_slopeThreshold;
            }
        }

        /// <summary>
        /// Adds a new sample to the window
        /// </summary>
        /// <param name="sample">new sample value</param>
        public void Add(double sample)
        {
            if (m_numSamples > 0 && m_currentKernel != 0)
            {
                double fraction = (sample - m_currentKernel) / m_currentKernel;

                if (fraction > m_upFraction || fraction < m_downFraction)
                {
                    ResetSlidingWindow();
                }
            }

            if (m_numSamples < m_samples.Length)
            {
                m_numSamples++;
                m_sum += sample;
            }
            else
            {
                m_sum -= m_samples[m_nextSample];
                m_sum += sample;
            }

            m_samples[m_nextSample] = sample;

            m_nextSample++;
            if (m_nextSample >= m_samples.Length)
            {
                m_nextSample = 0;
            }

            m_previousTime = m_currentTime;
            m_currentTime = DateTime.Now;

            m_previousKernel = m_currentKernel;
            m_currentKernel = m_sum / m_numSamples;

            double elapsedTime = (m_currentTime - m_previousTime).TotalSeconds;

            if (elapsedTime > 0)
            {
                m_currentSlope = (m_currentKernel - m_previousKernel) / elapsedTime;
            }
        }

        /// <summary>
        /// Brings the sliding window back to its initial state
        /// </summary>
        public void ResetSlidingWindow()
        {
            m_numSamples = 0;
            m_nextSample = 0;
            m_sum = 0;
            m_currentSlope = 0;
            m_currentKernel = 0;
            m_currentTime = DateTime.Now;
        }
    }
}
