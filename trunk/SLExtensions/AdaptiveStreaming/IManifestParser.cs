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
    /// This class defines the public interface for parsing a manifest file
    /// </summary>
    public abstract class IManifestParser
    {
        /// <summary>
        /// Parse the manifest file for a media stream
        /// </summary>
        /// <param name="manifestStream">the stream to parse the manifest from</param>
        /// <param name="manifestUrl">the url the manifest stream came from</param>
        /// <returns>the manifest info describing the manifest file</returns>
        public abstract ManifestInfo ParseManifest(Stream manifestStream, Uri manifestUrl);
    }
}
