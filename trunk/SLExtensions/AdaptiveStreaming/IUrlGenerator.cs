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
    /// This class defines the interface for generating a custom Url for a chunk
    /// </summary>
    public abstract class IUrlGenerator
    {
        /// <summary>
        /// Generates a url to access an adaptive media stream
        /// </summary>
        /// <param name="baseUrl">the base url from the manifest</param>
        /// <param name="streamId">the id of the stream we are generating a url for</param>
        /// <param name="chunkId">the id of the chunk we are generating a url for</param>
        /// <param name="chunkBitrate">the bitrate, in kbps, of the chunk we are generating the url for</param>
        /// <param name="chunkStartTime">the start time, in 100ns units, of the chunk we are generating the url for</param>
        /// <param name="chunkDuration">the duration of the chunk, in 100ns units, we are generating the url for</param>
        /// <returns>a new string describing the chunk url</returns>
        public abstract string GenerateUrlStringForChunk(string baseUrl, int streamId, int chunkId, ulong chunkBitrate, long chunkStartTime, long chunkDuration);
    }
}
