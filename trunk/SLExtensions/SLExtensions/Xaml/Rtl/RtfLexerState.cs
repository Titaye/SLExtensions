using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SLExtensions.Xaml.Rtf
{
    /// <summary>
    /// Defines the set of lexer states.
    /// </summary>
    public enum RtfLexerState
    {
        /// <summary>
        /// The default lexer state.
        /// </summary>
        Default,
        /// <summary>
        /// Control word parameter
        /// </summary>
        ControlWordParameter
    }
}
