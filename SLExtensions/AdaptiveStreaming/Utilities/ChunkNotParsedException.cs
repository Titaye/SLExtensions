//*@@@*************************************************************************
//
// Microsoft Expression Encoder
// Copyright © Microsoft Corporation. All rights reserved.
//
//*@@@*************************************************************************
namespace MS.Internal.Expression.Encoder.AdaptiveStreaming.Utilities
{
    using Microsoft.Expression.Encoder.AdaptiveStreaming;

    /// <summary>
    /// Internal exception thrown if a chunk is used before it is parsed
    /// </summary>
    internal class ChunkNotParsedException : AdaptiveStreamingException
    {
        /// <summary>
        /// Initializes a new instance of the ChunkNotParsedException class
        /// </summary>
        public ChunkNotParsedException()
            : base(Errors.ChunkNotParsedError)
        {
        }
    }
}
