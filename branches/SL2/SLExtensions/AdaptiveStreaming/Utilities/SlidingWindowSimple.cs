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
    /// A simple sliding window for moving averages.
    /// </summary>
    internal class SlidingWindowSimple
    {
        /// <summary>
        /// The samples we have seen
        /// </summary>
        private double[] m_samples;

        /// <summary>
        /// The number of samples we have
        /// </summary>
        private uint m_numSamples;

        /// <summary>
        /// The next sample index
        /// </summary>
        private uint m_nextSample;

        /// <summary>
        /// The sum of our samples
        /// </summary>
        private double m_sum;

        /// <summary>
        /// Our current average
        /// </summary>
        private double m_currentKernel;

        /// <summary>
        /// Initializes a new instance of the SlidingWindowSimple class.
        /// </summary>
        /// <param name="windowSize">Size of the window to average over</param>
        public SlidingWindowSimple(int windowSize)
        {
            Debug.Assert(windowSize > 0, "SlidingWindowSimple: Invalid constructor parameters");
            ResetSlidingWindow();
            m_samples = new double[windowSize];
        }

        /// <summary>
        /// Prevents a default instance of the SlidingWindowSimple class from being created.
        /// </summary>
        private SlidingWindowSimple()
        {
        }

        /// <summary>
        /// Gets our current average
        /// </summary>
        public double CurrentKernel
        {
            get
            {
                return m_currentKernel;
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
        }

        /// <summary>
        /// Add a new value to the window
        /// </summary>
        /// <param name="sample">value to add</param>
        public void Add(double sample)
        {
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

            m_currentKernel = m_sum / m_numSamples;
        }
    }
}
