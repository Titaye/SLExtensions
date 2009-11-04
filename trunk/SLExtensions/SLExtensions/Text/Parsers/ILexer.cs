using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SLExtensions.Text.Parsers
{
    /// <summary>
    /// Defines the contract for a lexer.
    /// </summary>
    public interface ILexer
    {
        /// <summary>
        /// Gets the next token.
        /// </summary>
        /// <returns></returns>
        IToken NextToken();
    }
}
