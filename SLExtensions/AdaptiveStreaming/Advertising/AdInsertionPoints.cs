//*@@@*************************************************************************
//
// Microsoft Expression Encoder
// Copyright © Microsoft Corporation. All rights reserved.
//
//*@@@*************************************************************************
namespace MS.Internal.Expression.Encoder.AdaptiveStreaming.Advertising
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Expression.Encoder.AdaptiveStreaming;
    using MS.Internal.Expression.Encoder.AdaptiveStreaming.Logging;

    /// <summary>
    /// This class holds a list of points used for inserting ads at a given time
    /// </summary>
    internal class AdInsertionPoints
    {
        /// <summary>
        /// Our list of ad insertion points
        /// </summary>
        private List<AdInsertionPoint> m_adInsertionPoints = new List<AdInsertionPoint>(30);

        /// <summary>
        /// Backing store for IsChanged property
        /// </summary>
        private bool m_isChanged = true;

        /// <summary>
        /// Gets a value indicating whether the array changed 
        /// since the last time we got an item.
        /// </summary>
        public bool IsChanged
        {
            get
            {
                return m_isChanged;
            }
        }

        /// <summary>
        /// Add a new ad notification point
        /// </summary>
        /// <param name="point">the point to</param>
        public void Add(AdInsertionPoint point)
        {
            lock (m_adInsertionPoints)
            {
                m_adInsertionPoints.Add(point);
                m_isChanged = true;
            }

            Tracer.Trace(TraceChannel.AdsInsert, "Added ad point {0}", point.Timestamp);
        }

        /// <summary>
        /// Get the closest insertion point to the given time
        /// </summary>
        /// <param name="currentPlayTime">current time</param>
        /// <returns>the closest time to the current time</returns>
        public long GetNearest(long currentPlayTime)
        {
            long res = long.MaxValue;
            lock (m_adInsertionPoints)
            {
                foreach (AdInsertionPoint point in m_adInsertionPoints)
                {
                    long ticks = point.Timestamp.Ticks;
                    if (currentPlayTime < ticks && ticks < res)
                    {
                        res = ticks;
                    }
                }

                if (res == long.MaxValue)
                {
                    res = -1;
                }

                m_isChanged = false;
            }

            Tracer.Trace(TraceChannel.AdsInsert, "Reported ad point {0}", res);
            return res;
        }

        /// <summary>
        /// Retrieve the list of insertion points as an array
        /// </summary>
        /// <returns>an array of the stored insertion points</returns>
        public AdInsertionPoint[] ToArray()
        {
            AdInsertionPoint[] ads;

            lock (m_adInsertionPoints)
            {
                ads = new AdInsertionPoint[m_adInsertionPoints.Count];
                for (int i = 0; i < m_adInsertionPoints.Count; ++i)
                {
                    ads[i] = m_adInsertionPoints[i];
                }
            }

            return ads;
        }
    }
}
