//*@@@*************************************************************************
//
// Microsoft Expression Encoder
// Copyright © Microsoft Corporation. All rights reserved.
//
//*@@@*************************************************************************
namespace MS.Internal.Expression.Encoder.AdaptiveStreaming.Utilities
{
    /// <summary>
    /// Error strings used internally.
    /// </summary>
    internal static class Errors
    {
        /// <summary>
        /// Null media element error
        /// </summary>
        public const string NullMediaElementOnMSSError = "Media Element cannot be null on media stream source.";

        /// <summary>
        /// Null url error
        /// </summary>
        public const string NullUrlOnMSSError = "Source URL cannot be null on media stream source.";

        /// <summary>
        /// Cannot load manifest error
        /// </summary>
        public const string CannotLoadManifestError = "Cannot load media manifest {0}: {1}";

        /// <summary>
        /// Manifest load failure error
        /// </summary>
        public const string ManifestFailureError = "Failed to read media manifest from the URL {0}";

        /// <summary>
        /// Unparsed chunk error
        /// </summary>
        public const string ChunkNotParsedError = "Tried to operate on a media chunk that has not been parsed yet.";

        /// <summary>
        /// Incorrect number of video streams
        /// </summary>
        public const string IncorrectNumberOfVideoStreamsError = "Did not find the correct number of media streams. We require video stream.";

        /// <summary>
        /// Must have video or audio only streams
        /// </summary>
        public const string NonVideoOrAudioStreamsNotSupportedError = "Tried to create our heuristics module with a stream other than video or audio";
    }
}
