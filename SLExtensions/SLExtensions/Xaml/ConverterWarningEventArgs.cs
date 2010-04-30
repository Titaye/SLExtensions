namespace SLExtensions.Xaml
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Provides data for the Warning event
    /// </summary>
    public class ConverterWarningEventArgs : EventArgs
    {
        #region Fields

        private string message;

        #endregion Fields

        #region Constructors

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

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the warning message.
        /// </summary>
        /// <value>The warning.</value>
        public string Message
        {
            get { return this.message; }
        }

        #endregion Properties
    }
}