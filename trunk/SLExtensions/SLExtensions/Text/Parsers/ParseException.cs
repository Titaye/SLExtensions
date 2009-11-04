using System;

namespace SLExtensions.Text.Parsers
{
    /// <summary>
    /// The exception for parse errors.
    /// </summary>
    public class ParseException
        : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:ParseException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ParseException(string message)
            : base(message)
        {
        }
    }
}
