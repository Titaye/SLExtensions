//*@@@*************************************************************************
//
// Microsoft Expression Encoder
// Copyright © Microsoft Corporation. All rights reserved.
//
//*@@@*************************************************************************
namespace Microsoft.Expression.Encoder.AdaptiveStreaming
{
    using System;
    using System.IO;

    /// <summary>
    /// Interface for creating a new chunk parser object
    /// </summary>
    public abstract class IChunkParserFactory
    {
        /// <summary>
        /// Creates a new chunk parser object. This should always create
        /// a new object rather than returning the same instance of one. The factory
        /// method includes the stream we are trying to parse, in case your factory
        /// method wants to read in the first couple of bytes from the stream to help
        /// determine what kind of parser to create.
        /// </summary>
        /// <param name="stream">the stream we are trying to pass</param>
        /// <returns>a new chunk parser instance</returns>
        public abstract IChunkParser CreateParserForStream(Stream stream);
    }
}
