namespace SLExtensions.Text.Parsers
{
    using System;

    /// <summary>
    /// The exception for parse errors.
    /// </summary>
    public class ParseException : Exception
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ParseException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ParseException(string message)
            : base(message)
        {
        }

        #endregion Constructors
    }
}