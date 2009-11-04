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
    using Microsoft.Expression.Encoder.AdaptiveStreaming;
    using MS.Internal.Expression.Encoder.AdaptiveStreaming.Heuristic;
    using MS.Internal.Expression.Encoder.AdaptiveStreaming.Utilities;

    /// <summary>
    /// Partial class implementation of frame rate info.
    /// </summary>
    internal partial class NetworkMediaInfo
    {
        /// <summary>
        /// The revocation period for bitrate suspension (2 hours)
        /// </summary>
        private const double MaxRevocationPeriod = 2 * 60 * 60;

        /// <summary>
        /// Index within mediaBitRates of the bit rate currently
        /// playing
        /// </summary>
        private int m_bitratePlayingIndex = -1;
        
        /// <summary>
        /// When 1 or more bit rates are temporarily suspended,
        /// remember which one is to be checked for revocation the
        /// earliest
        /// </summary>
        private double m_earliestRevocationTime = double.MaxValue;

        /// <summary>
        /// The current value for the safety period before the real
        /// FPS observation begins
        /// </summary>
        private int m_safetyTics = Configuration.Heuristics.FrameRate.SafetyTicsLow;

        /// <summary>
        /// Remember what the last browser state was
        /// </summary>
        private bool m_minimizedBrowserPrev;
        
        /// <summary>
        /// Keep track of recent dropped FPS to be used in deciding
        /// whether or not a suspension may be revoqued
        /// </summary>
        private SlidingWindowSimple m_droppedFpsAverage = new SlidingWindowSimple(4);

        /// <summary>
        /// The average rendered frames per second
        /// </summary>
        private SlidingWindowSimple m_renderedFpsAverage = new SlidingWindowSimple(4);

        /// <summary>
        /// The previous number of rendered tics
        /// </summary>
        private long m_prevCumulativeRenderedTics;

        /// <summary>
        /// Keep track of accumulating the validation play time on a
        /// per bit rate basis, keeping a count of dropped frames and
        /// ultimately deciding which bit rates are supported or not
        /// </summary>
        /// <param name="bitRate">Media bit rate in bps</param>
        /// <param name="minimizedBrowser">Is our browser minimized</param>
        /// <param name="cumulativeRenderedTics">The number of rendered tics</param>
        /// <param name="droppedFpsHistory">History of dropped frames</param>
        /// <param name="renderedFramesPerSecond">Rendered frames per second</param>
        /// <param name="sourceFramesPerSecond">Source frame rate</param>
        /// <returns>Index of suspended frame rates</returns>
        public int TestFrameRate(
            ulong bitRate, 
            bool minimizedBrowser,
            long cumulativeRenderedTics, 
            long[] droppedFpsHistory, 
            double renderedFramesPerSecond,
            double sourceFramesPerSecond)
        {
            int newIndex = FindBitRateIndex(bitRate);

            BitrateInfo bitrateInfo = BitratesInfo[newIndex];

            double currentTime = NetworkHeuristicsHelper.GetTime();

            if (cumulativeRenderedTics > m_prevCumulativeRenderedTics)
            {
                // Keep our dropped FPS average updated
                long droppedFpsIndex = cumulativeRenderedTics % PlaybackInfo.DroppedFpsHistorySize;

                m_droppedFpsAverage.Add(droppedFpsHistory[droppedFpsIndex]);
                m_renderedFpsAverage.Add(renderedFramesPerSecond);

                m_prevCumulativeRenderedTics = cumulativeRenderedTics;
            }

            long elapsedTics;

            int suspended = 0;

            if (minimizedBrowser == true)
            {
                if (m_minimizedBrowserPrev == false)
                {
                    m_minimizedBrowserPrev = true;

                    // The suspension may have been induced by the
                    // recent minimization, so undo any recent
                    // suspensios
                    UndoRecentSuspension(currentTime);

                    // When browser was minimized, we may have got a
                    // bogus reading of dropped fps, in that case
                    // reset the frame rate heuristics and prepare to
                    // ignore readings for a period of time as the
                    // transition back to show on screen will also
                    // return a bogus reading which we don't want to
                    // cause an unnecesary bit rate suspension
                    bitrateInfo.ResetFrameRateTest();

                    m_safetyTics = Configuration.Heuristics.FrameRate.SafetyTicsHigh;
                }
            }
            else if (m_minimizedBrowserPrev == true)
            {
                m_minimizedBrowserPrev = false;
            }
            
            if (bitrateInfo.IsBitrateTested == false)
            {
                if (newIndex != m_bitratePlayingIndex)
                {
                    bitrateInfo.ResetFrameRateTest();

                    m_bitratePlayingIndex = newIndex;
                }
                
                switch (bitrateInfo.FrameRateTestState)
                {
                case FrameRateTestState.Init:
                    bitrateInfo.CumulativeRenderedTics = cumulativeRenderedTics;

                    bitrateInfo.FrameRateTestState = FrameRateTestState.SafetyPeriod;
                    
                    // Fall through next state, SafetyPeriod
                    goto case FrameRateTestState.SafetyPeriod;
                    
                case FrameRateTestState.SafetyPeriod:

                    elapsedTics =
                        cumulativeRenderedTics - bitrateInfo.CumulativeRenderedTics
                        + 1;

                    if (elapsedTics >= m_safetyTics)
                    {
                        // Take a snapshot of current counters as we
                        // begin our obsevation period
                        bitrateInfo.CumulativeRenderedTics = cumulativeRenderedTics;

                        bitrateInfo.FrameRateTestState =
                            FrameRateTestState.ObservationPeriod;
                    }

                    break;
                    
                case FrameRateTestState.ObservationPeriod:

                    elapsedTics =
                        cumulativeRenderedTics - bitrateInfo.CumulativeRenderedTics
                        + 1;
                    
                    long observationTics = ComputeObservationTics();

                    if (elapsedTics >= observationTics)
                    {
                        suspended = ProcessBitRateSuspension(
                                m_bitratePlayingIndex,
                                elapsedTics,
                                cumulativeRenderedTics,
                                droppedFpsHistory,
                                sourceFramesPerSecond);
                    }
                    
                    break;
                }
            }

            if (currentTime >= m_earliestRevocationTime) 
            {
                RevokeSuspendedBitRates(currentTime, sourceFramesPerSecond);
            }

            return suspended;
        }

        /// <summary>
        /// Get the bitrates to suspend
        /// </summary>
        /// <param name="droppedFps">Average dropped frames per second</param>
        /// <param name="renderedFps">Rendered frames per second</param>
        /// <param name="sourceFps">Source frames per second</param>
        /// <returns>The index of the bitrate to suspend (and everything higher)</returns>
        private static int GetBitRatesToSuspend(double droppedFps, double renderedFps, double sourceFps)
        {
            int numToSuspend;

            double averageRate = CalculateRenderedRate(sourceFps - droppedFps, sourceFps);

            if (averageRate <=
                Configuration.Heuristics.FrameRate.FpsMaxDropPercent)
            {
                numToSuspend =
                    Configuration.Heuristics.FrameRate.FpsMaxDropLevels;
            }
            else if (averageRate <=
                     Configuration.Heuristics.FrameRate.FpsLowDropPercent)
            {
                numToSuspend =
                    Configuration.Heuristics.FrameRate.FpsLowDropLevels;
            }
            else
            {
                numToSuspend = 0;
            }

            // Test rendered frames per second vs target frames per second
            int numToSuspendFromRendered = 0;
            double renderedRate = CalculateRenderedRate(renderedFps, sourceFps);

            // 10% drop 1 level, 40% drop 2 levels
            if (renderedRate <= Configuration.Heuristics.FrameRate.RenderedFpsMaxDropPercent)
            {
                numToSuspendFromRendered = Configuration.Heuristics.FrameRate.RenderedFpsMaxDropLevel;
            }
            else if (renderedRate <= Configuration.Heuristics.FrameRate.RenderedFpsLowDropPercent)
            {
                numToSuspendFromRendered = Configuration.Heuristics.FrameRate.RenderedFpsLowDropLevel;
            }
            else
            {
                numToSuspendFromRendered = 0;
            }

            return Math.Max(numToSuspend, numToSuspendFromRendered);
        }

        /// <summary>
        /// Calculate the percentage of rendered to source fps.
        /// </summary>
        /// <param name="renderedFps">Rendered frames per second</param>
        /// <param name="sourceFps">Source frames per second</param>
        /// <returns>Ratio of the two</returns>
        private static double CalculateRenderedRate(double renderedFps, double sourceFps)
        {
            if (renderedFps < 0)
            {
                renderedFps = 0.0;
            }

            if (renderedFps > sourceFps)
            {
                renderedFps = sourceFps;
            }

            if (sourceFps != 0.0)
            {
                return renderedFps / sourceFps;
            }

            return 1.0;
        }

        /// <summary>
        /// Compute the number of tics in the observation window
        /// </summary>
        /// <returns>Number of tics in the observation window</returns>
        private static long ComputeObservationTics()
        {
            return Configuration.Heuristics.FrameRate.ObservationTicsMax;
        }

        /// <summary>
        /// Reset our safety values.
        /// </summary>
        /// <param name="isSeek">Are we in a seek</param>
        private void ResetSafetyTics(bool isSeek)
        {
            m_safetyTics = isSeek == true ?
                Configuration.Heuristics.FrameRate.SafetyTicsLow :
                Configuration.Heuristics.FrameRate.SafetyTicsHigh;
        }

        /// <summary>
        /// Process our suspended bitrates
        /// </summary>
        /// <param name="bitRateIndex">Index of the bitrate we suspended</param>
        /// <param name="elapsedTics">Elapsed tics since last one</param>
        /// <param name="cumulativeTics">Cumulative tics so far</param>
        /// <param name="droppedFpsHistory">Dropped fps history</param>
        /// <param name="sourceFramesPerSecond">Source frame rate</param>
        /// <returns>Index of suspended bitrate</returns>
        private int ProcessBitRateSuspension(
            int bitRateIndex,
            long elapsedTics,
            long cumulativeTics,
            long[] droppedFpsHistory,
            double sourceFramesPerSecond)
        {
            int suspended = 0;

            double currentTime = NetworkHeuristicsHelper.GetTime();

            long sampleIndex;

            sampleIndex = cumulativeTics + 
                PlaybackInfo.DroppedFpsHistorySize - 
                elapsedTics + 1;
            
            sampleIndex %= PlaybackInfo.DroppedFpsHistorySize;

            long[] samples = new long[elapsedTics];

            double droppedFps = 0;

            for (int i = 0; i < elapsedTics; i++)
            {
                samples[i] = droppedFpsHistory[sampleIndex];
                droppedFps += samples[i];

                sampleIndex++;
                sampleIndex %= PlaybackInfo.DroppedFpsHistorySize;
            }

            Array.Sort(samples);

            double droppedFpsBest = samples[0];

            double averageFpsBest = droppedFpsBest;

            int bitRatesToSuspend = GetBitRatesToSuspend(averageFpsBest, m_renderedFpsAverage.CurrentKernel, sourceFramesPerSecond);

            if (bitRatesToSuspend > 0 && bitRateIndex > 0)
            {                
                // We never suspend the lowest bit rate                
                int index = bitRateIndex - bitRatesToSuspend + 1;

                if (index < 1)
                {
                    index = 1;
                }

                // While dealing with transients, a geometric
                // progression (as used here) would lead to very long
                // revocation periods after a few suspensions, for
                // those cases, it might be more appropriate an
                // arithmetic progression, which wouldn't be too bad
                // in case of suspension because of low performance
                // hardware
                m_earliestRevocationTime = currentTime + MaxRevocationPeriod;

                double prevRevocationTime = 0;
                double extraRevocationTime = 
                    Configuration.Heuristics.FrameRate.BaseRevocationPeriod / 4;
                
                for (int i = index; i < BitratesInfo.Length; i++)
                {
                    BitratesInfo[i].IsBitrateTested = true;
                    BitratesInfo[i].IsBitrateSupported = false;

                    double revocationPeriod =
                        Configuration.Heuristics.FrameRate.BaseRevocationPeriod << BitratesInfo[i].NumSuspensions;

                    // Allows to roll back 1 step
                    if (BitratesInfo[i].RevocationTime != 0)
                    {
                        BitratesInfo[i].RevocationTimePrev =
                            BitratesInfo[i].RevocationTime;
                    }
                    
                    BitratesInfo[i].RevocationTime =
                        currentTime + revocationPeriod;

                    if (prevRevocationTime > 0
                        &&
                        BitratesInfo[i].RevocationTime == prevRevocationTime)
                    {
                        // Don't want the same revocation time
                        revocationPeriod += extraRevocationTime;
                        extraRevocationTime /= 2;

                        BitratesInfo[i].RevocationTime =
                            currentTime + revocationPeriod;
                    }
                    
                    prevRevocationTime = BitratesInfo[i].RevocationTime;
                    
                    if (BitratesInfo[i].RevocationTimePrev == 0)
                    {
                        // Just a safety measure
                        BitratesInfo[i].RevocationTimePrev =
                            BitratesInfo[i].RevocationTime;
                    }

                    if (BitratesInfo[i].RevocationTime < m_earliestRevocationTime)
                    {
                        m_earliestRevocationTime = BitratesInfo[i].RevocationTime;
                    }

                    BitratesInfo[i].NumSuspensions++;

                    if (BitratesInfo[i].FrameRateTestState !=
                        FrameRateTestState.Suspended)
                    {
                        // Don't update suspension time for the sake
                        // of an accurate roll back
                        BitratesInfo[i].SuspensionTime = currentTime;

                        BitratesInfo[i].FrameRateTestState =
                            FrameRateTestState.Suspended;
                    }
                    
                    suspended++;
                }
            }
            else
            {
                for (int i = bitRateIndex; i >= 0; i--)
                {
                    // Get ready to repeat the test cycle
                    BitratesInfo[i].ResetFrameRateTest();
                }
            }

            if (suspended > 0)
            {
                m_safetyTics = Configuration.Heuristics.FrameRate.SafetyTicsHigh;
                
                // Bit rates eligibility may have changed, update accordingly
                UpdateUsableBitrates();
            }
            else
            {
                m_safetyTics = Configuration.Heuristics.FrameRate.SafetyTicsLow;
            }

            return suspended;
        }

        /// <summary>
        /// Undo any recent bitrate suspensions.
        /// </summary>
        /// <param name="currentTime">The current time</param>
        private void UndoRecentSuspension(double currentTime)
        {
            bool undo = false;

            for (int i = 0; i < BitratesInfo.Length; i++)
            {
                if (BitratesInfo[i].FrameRateTestState == FrameRateTestState.Suspended)
                {
                    if ((currentTime - BitratesInfo[i].SuspensionTime) < Configuration.Heuristics.FrameRate.UndoTimeWindow)
                    {
                        undo = true;
                        break;
                    }
                }
            }

            if (undo == false)
            {
                // Nothing to undo
                return;
            }

            // Do the actual job here
            for (int i = 0; i < BitratesInfo.Length; i++)
            {
                if (BitratesInfo[i].FrameRateTestState == FrameRateTestState.Suspended)
                {
                    if ((currentTime - BitratesInfo[i].SuspensionTime) < Configuration.Heuristics.FrameRate.UndoTimeWindow)
                    {
                        // If within the window, undo the suspension
                        BitratesInfo[i].NumSuspensions--;
                        BitratesInfo[i].ResetFrameRateTest();
                    }
                    else
                    {
                        // For the others roll-back the revocation
                        // time
                        BitratesInfo[i].RevocationTime =
                            BitratesInfo[i].RevocationTimePrev;
                    }
                }
            }

            // Bit rates eligibility may have changed, update accordingly
            UpdateUsableBitrates();
        }

        /// <summary>
        /// Try to revoke any suspended bitrates
        /// </summary>
        /// <param name="currentTime">The current time</param>
        /// <param name="sourceFps">Source frame rate</param>
        private void RevokeSuspendedBitRates(double currentTime, double sourceFps)
        {
            m_earliestRevocationTime = currentTime + MaxRevocationPeriod;

            if (CalculateRenderedRate(sourceFps - m_droppedFpsAverage.CurrentKernel, sourceFps)
                <=
                Configuration.Heuristics.FrameRate.FpsLowDropPercent || 
                CalculateRenderedRate(m_renderedFpsAverage.CurrentKernel, sourceFps) 
                <= 
                Configuration.Heuristics.FrameRate.RenderedFpsLowDropPercent)                
            {
                int extended = 0;
                
                // Extend revocation time
                for (int i = 0; i < BitratesInfo.Length; i++)
                {
                    if (BitratesInfo[i].FrameRateTestState
                        ==
                        FrameRateTestState.Suspended)
                    {
                        BitratesInfo[i].RevocationTime +=
                            Configuration.Heuristics.FrameRate.BaseRevocationPeriod;

                        extended++;
                    }
                }
            }

            for (int i = 0; i < BitratesInfo.Length; i++)
            {
                if (BitratesInfo[i].FrameRateTestState
                    == FrameRateTestState.Suspended)
                {
                    if (currentTime >= BitratesInfo[i].RevocationTime)
                    {
                        BitratesInfo[i].ResetFrameRateTest();

                        m_safetyTics = Configuration.Heuristics.FrameRate.SafetyTicsLow;
                    }
                    else
                    {
                        if (BitratesInfo[i].RevocationTime <
                            m_earliestRevocationTime)
                        {
                            m_earliestRevocationTime =
                                BitratesInfo[i].RevocationTime;
                        }
                    }
                }
            }

            // Bit rates eligibility may have changed, update accordingly
            UpdateUsableBitrates();
        }
    }
}
