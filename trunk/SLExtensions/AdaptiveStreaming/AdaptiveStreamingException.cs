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
    /// General exception class for derived implementations to use
    /// </summary>
    public class AdaptiveStreamingException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the AdaptiveStreamingException class
        /// </summary>
        public AdaptiveStreamingException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the AdaptiveStreamingException class
        /// </summary>
        /// <param name="message">exception message</param>
        public AdaptiveStreamingException(string message) : base(message)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the AdaptiveStreamingException class
        /// </summary>
        /// <param name="message">exception message</param>
        /// <param name="innerException">exception to wrap</param>
        public AdaptiveStreamingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
