namespace SLExtensions.Xaml.Rtf
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

    #region Enumerations

    /// <summary>
    /// Defines the set of RTF token types.
    /// </summary>
    public enum RtfTokenType
    {
        /// <summary>
        /// No token
        /// </summary>
        None,
        /// <summary>
        /// Group start
        /// </summary>
        GroupStart,
        /// <summary>
        /// Group end
        /// </summary>
        GroupEnd,
        /// <summary>
        /// Keyword
        /// </summary>
        Keyword,
        /// <summary>
        /// Control word
        /// </summary>
        ControlWord,
        /// <summary>
        /// Control symbol
        /// </summary>
        ControlSymbol,
        /// <summary>
        /// Text
        /// </summary>
        Text,
        /// <summary>
        /// End of file token
        /// </summary>
        Eof
    }

    #endregion Enumerations
}