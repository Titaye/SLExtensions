namespace SLExtensions.Xaml
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Represents a converter exception.
    /// </summary>
    public class ConverterException : Exception
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConverterException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ConverterException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConverterException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ConverterException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        #endregion Constructors
    }
}