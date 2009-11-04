//*@@@*************************************************************************
//
// Microsoft Expression Encoder
// Copyright © Microsoft Corporation. All rights reserved.
//
//*@@@*************************************************************************
namespace Microsoft.Expression.Encoder.AdaptiveStreaming
{
    using System;

    /// <summary>
    /// This class tracks a single add insertion point. We will pause download
    /// new media chunks when this ad starts and for the duration it is shown.
    /// </summary>
    public class AdInsertionPoint
    {
        /// <summary>
        /// Initializes a new instance of the AdInsertionPoint class
        /// </summary>
        /// <param name="timestamp">the time the ad is expected to be shown</param>
        /// <param name="duration">the duration it will be shown for</param>
        public AdInsertionPoint(TimeSpan timestamp, TimeSpan duration)
        {
            Timestamp = timestamp;
            Duration = duration;
        }

        /// <summary>
        /// Gets or sets the time the ad is expected to be shown
        /// </summary>
        public TimeSpan Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the expected duration of the add
        /// </summary>
        public TimeSpan Duration { get; set; }
    }
}
