//*@@@*************************************************************************
//
// Microsoft Expression Encoder
// Copyright © Microsoft Corporation. All rights reserved.
//
//*@@@*************************************************************************
namespace MS.Internal.Expression.Encoder.AdaptiveStreaming.Heuristic
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows.Media;
    using Microsoft.Expression.Encoder.AdaptiveStreaming;
    using MS.Internal.Expression.Encoder.AdaptiveStreaming.Logging;
    using MS.Internal.Expression.Encoder.AdaptiveStreaming.Manifest;
    using MS.Internal.Expression.Encoder.AdaptiveStreaming.Network;
    using MS.Internal.Expression.Encoder.AdaptiveStreaming.Utilities;

    /// <summary>
    /// Partial class of the heuristics impl containing the frame rate heuristics.
    /// </summary>
    internal partial class HeuristicsImpl : Heuristics
    {
        /// <summary>
        /// This function takes care of keeping track of dropped frame
        /// rate, deciding when that information is meaningful, and
        /// validating the bit rates that the network heuristics can
        /// use
        /// </summary>
        /// <param name="mediaStreamType">The type of the stream we are processing</param>
        /// <param name="bitRate">Media bit rate in bps</param>
        internal void ProcessFrameRateHeuristcs(MediaStreamType mediaStreamType, ulong bitRate)
        {
            if (mediaStreamType == MediaStreamType.Video)
            {
                // Do this for video only
                StreamInfo stream = m_manifestInfo.GetStreamInfoForStreamType(mediaStreamType);
                int streamIndex = stream.StreamId;
                NetworkMediaInfo networkMediaInfo = m_networkMediaInfo[streamIndex];

                int suspended = networkMediaInfo.TestFrameRate(
                    bitRate,
                    m_playbackInfo.IsMinimizedBrowser,
                    m_playbackInfo.CumulativeTics,
                    m_playbackInfo.GetDroppedFpsHistory(),
                    m_playbackInfo.RenderedFramesPerSecond,
                    m_playbackInfo.SourceFramesPerSecond);

                if (suspended > 0)
                {
                    RequestChunkReplacement();
                }
            }
        }
    }
}