//*@@@*************************************************************************
//
// Microsoft Expression Encoder
// Copyright © Microsoft Corporation. All rights reserved.
//
//*@@@*************************************************************************
namespace Microsoft.Expression.Encoder.AdaptiveStreaming
{
    using System;
    using System.Globalization;
    using MS.Internal.Expression.Encoder.AdaptiveStreaming.Logging;

    /// <summary>
    /// This class calculates the time it took to download a media chunk. It can be extended
    /// or overridden if you want to insert fake bandwidth statistics into the heuristics modules. 
    /// This is useful for testing and other external control of a heuristics module.
    /// </summary>
    public class BandwidthCalculator
    {
        /// <summary>
        /// The last value we calculated.
        /// </summary>
        private double m_lastDownloadCalculation;

        /// <summary>
        /// Calculate the time it took to download a media chunk. Override this if you
        /// want to fill the heuristics module with artificial data.
        /// </summary>
        /// <param name="downloadStartTime">The time the download started</param>
        /// <param name="downloadCompleteTime">The time the download ended</param>
        /// <param name="streamIndex">the index of the stream that the chunk came from</param>
        /// <param name="chunkId">the id of the chunk that was downloaded</param>
        /// <param name="chunkLength">the length, in bytes, of the downloaded chunk</param>
        /// <returns>the time in seconds it took to download the chunk</returns>
        public virtual double CalculateDownloadDurationInSeconds(DateTime downloadStartTime, DateTime downloadCompleteTime, int streamIndex, int chunkId, long chunkLength)
        {
            double downloadDuration;

            if (downloadCompleteTime == downloadStartTime)
            {
                // Some times this actually do happens, pick the midle
                // point in the resolution timer (which is 1/64 of a
                // second)
                downloadDuration = 1.0 / (64 * 2);
                Tracer.Trace(TraceChannel.NetHeur, "downloadDuration for stream {0} was 0, set it to: {1}", streamIndex, downloadDuration.ToString("0.000", CultureInfo.InvariantCulture));
            }
            else
            {
                downloadDuration = (downloadCompleteTime - downloadStartTime).TotalSeconds;
            }

            m_lastDownloadCalculation = downloadDuration;
            return downloadDuration;
        }

        /// <summary>
        /// Gets the last time calculated by this class. Useful for calculator that
        /// depend on previous values.
        /// </summary>
        /// <returns>The time, in seconds, of the last calculation</returns>
        public virtual double GetLastCalculation()
        {
            return m_lastDownloadCalculation;
        }

        /// <summary>
        /// Resets this class. Useful for calculators that depend on time.
        /// </summary>
        public virtual void Reset()
        {
            m_lastDownloadCalculation = 0.0;
        }
    }
}
