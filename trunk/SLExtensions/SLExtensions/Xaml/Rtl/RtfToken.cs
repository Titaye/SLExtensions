namespace SLExtensions.Xaml.Rtf
{
    using System;

    using SLExtensions.Text.Parsers;

    /// <summary>
    /// Represents an RTF token.
    /// </summary>
    public struct RtfToken : IToken
    {
        #region Properties

        /// <summary>
        /// Gets a value indicating whether this instance is EOF.
        /// </summary>
        /// <value><c>true</c> if this instance is EOF; otherwise, <c>false</c>.</value>
        public bool IsEof
        {
            get { return this.Type == RtfTokenType.Eof; }
        }

        /// <summary>
        /// Gets or sets the control word parameter.
        /// </summary>
        /// <value>The parameter.</value>
        public int? Parameter
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public RtfTokenType Type
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public string Value
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> containing a fully qualified type name.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}: {1}{2}", this.Type, this.Value, this.Parameter.HasValue ? this.Parameter.ToString() : "");
        }

        #endregion Methods
    }
}