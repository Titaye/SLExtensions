//*@@@*************************************************************************
//
// Microsoft Expression Encoder
// Copyright © Microsoft Corporation. All rights reserved.
//
//*@@@*************************************************************************
namespace MS.Internal.Expression.Encoder.AdaptiveStreaming.Heuristic
{
    using System;

    /// <summary>
    /// This class contains some helper routines used by our NetworkHeuristics class
    /// </summary>
    internal class NetworkHeuristicsHelper
    {
        /// <summary>
        /// Used to log shorter number of seconds
        /// </summary>
        private static DateTime refTime = DateTime.Now;

        /// <summary>
        /// Prevents a default instance of the NetworkHeuristicsHelper
        /// class from being created.
        /// </summary>
        private NetworkHeuristicsHelper()
        {
        }

        /// <summary>
        /// Convert time in 100ns units to seconds
        /// </summary>
        /// <param name="timeHns">time in 100ns units</param>
        /// <returns>time in seconds</returns>
        internal static double ConvertHnsToSeconds(ulong timeHns)
        {
            return (double)timeHns / 10000000;
        }

        /// <summary>
        /// Get the current time that we have been running
        /// </summary>
        /// <returns>Time from when we started</returns>
        internal static double GetTime()
        {
            return (DateTime.Now - refTime).TotalSeconds;
        }
    }
}
