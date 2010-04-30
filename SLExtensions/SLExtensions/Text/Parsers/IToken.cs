namespace SLExtensions.Text.Parsers
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    /// <summary>
    /// Defines the contract for tokens.
    /// </summary>
    public interface IToken
    {
        #region Properties

        /// <summary>
        /// Gets a value indicating whether this instance is the end of file instance.
        /// </summary>
        /// <value><c>true</c> if this instance is EOF; otherwise, <c>false</c>.</value>
        bool IsEof
        {
            get;
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        string Value
        {
            get; set;
        }

        #endregion Properties
    }
}