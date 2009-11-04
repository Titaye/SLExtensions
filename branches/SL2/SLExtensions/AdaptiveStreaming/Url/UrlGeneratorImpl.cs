//*@@@*************************************************************************
//
// Microsoft Expression Encoder
// Copyright © Microsoft Corporation. All rights reserved.
//
//*@@@*************************************************************************
namespace MS.Internal.Expression.Encoder.AdaptiveStreaming.Url
{
    using System;
    using Microsoft.Expression.Encoder.AdaptiveStreaming;
    using MS.Internal.Expression.Encoder.AdaptiveStreaming.Heuristic;

    /// <summary>
    /// Default implementation of the url generator
    /// </summary>
    internal class UrlGeneratorImpl : IUrlGenerator
    {
        /// <summary>
        /// This is the string we look for to replace with our chunk id
        /// </summary>
        private const string ChunkIdArg = "{chunk id}";

        /// <summary>
        /// This is the string we look for to replace with our bitrate
        /// </summary>
        private const string BitrateArg = "{bitrate}";

        /// <summary>
        /// This is the string we look for to replace with our fragment start time
        /// </summary>
        private const string FragmentStartArg = "{start time}";

        /// <summary>
        /// This is the string we look for to replace with our fragment end time
        /// </summary>
        private const string FragmentEndArg = "{end time}";

        /// <summary>
        /// Generates a url to access an adaptive media stream
        /// </summary>
        /// <param name="baseUrl">the base url from the manifest</param>
        /// <param name="streamId">the id of the stream we are generating a url for</param>
        /// <param name="chunkId">the id of the chunk we are generating a url for</param>
        /// <param name="chunkBitrate">the bitrate of the chunk we are generating the url for</param>
        /// <param name="chunkStartTime">the start time of the chunk we are generating the url for</param>
        /// <param name="chunkDuration">the duration of the chunk we are generating the url for</param>
        /// <returns>the url string for the chunk</returns>
        public override string GenerateUrlStringForChunk(string baseUrl, int streamId, int chunkId, ulong chunkBitrate, long chunkStartTime, long chunkDuration)
        {
            string urlString = baseUrl;

            urlString = urlString.Replace(ChunkIdArg, chunkId.ToString(System.Globalization.CultureInfo.InvariantCulture));
            urlString = urlString.Replace(BitrateArg, chunkBitrate.ToString(System.Globalization.CultureInfo.InvariantCulture));
            urlString = urlString.Replace(FragmentStartArg, ((ulong)chunkStartTime).ToString(System.Globalization.CultureInfo.InvariantCulture));
            urlString = urlString.Replace(FragmentEndArg, ((ulong)(chunkStartTime + chunkDuration)).ToString(System.Globalization.CultureInfo.InvariantCulture));

            return urlString;
        }
    }
}
