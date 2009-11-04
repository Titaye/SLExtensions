//*@@@*************************************************************************
//
// Microsoft Expression Encoder
// Copyright © Microsoft Corporation. All rights reserved.
//
//*@@@*************************************************************************
namespace MS.Internal.Expression.Encoder.AdaptiveStreaming.Parsing
{
    using System;
    using System.IO;
    using Microsoft.Expression.Encoder.AdaptiveStreaming;

    /// <summary>
    /// Default parser factory class
    /// </summary>
    internal class ChunkParserFactoryImpl : IChunkParserFactory
    {
        /// <summary>
        /// Creates a parser for the given stream
        /// </summary>
        /// <param name="stream">the stream we are trying to parse</param>
        /// <returns>the new parser for this stream</returns>
        public override IChunkParser CreateParserForStream(Stream stream)
        {
            return new FragmentedMp4ParserImpl();
        }
    }
}
