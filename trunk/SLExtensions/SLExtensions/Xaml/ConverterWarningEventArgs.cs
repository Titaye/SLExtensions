using System;
using System.Collections.Generic;
using System.Text;

namespace SLExtensions.Xaml
{
    /// <summary>
    /// Provides data for the Warning event
    /// </summary>
    public class ConverterWarningEventArgs
        : EventArgs
    {
        private string message;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConverterWarningEventArgs"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ConverterWarningEventArgs(string message)
        {
            if (message == null) {
                throw new ArgumentNullException("message");
            }
            this.message = message;
        }

        /// <summary>
        /// Gets the warning message.
        /// </summary>
        /// <value>The warning.</value>
        public string Message
        {
            get { return this.message; }
        }
    }
}
