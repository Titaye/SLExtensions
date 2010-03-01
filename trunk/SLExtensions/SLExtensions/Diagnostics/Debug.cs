// <copyright file="Debug.cs" company="Ucaya">
// Distributed under Microsoft Public License (Ms-PL)
// </copyright>
// <author>Thierry Bouquain</author>
namespace SLExtensions.Diagnostics
{
    using System;
    using System.Net;
    using System.Windows;
    using System.Windows.Browser;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    /// <summary>
    /// Debug helper
    /// </summary>
    public static class Debug
    {
        #region Fields

        /// <summary>
        /// Browser debug javascript
        /// </summary>
        private const string JavascriptDebug = @"function _dbg(text)
                {        // VS script debugger output window.
                        if ((typeof(Debug) !== 'undefined') && Debug.writeln) {
                            Debug.writeln(text);
                        }
                        // FF firebug and Safari console.
                        if (window.console && window.console.log) {
                            window.console.log(text);
                        }
                        // Opera console.
                        if (window.opera) {
                            window.opera.postError(text);
                        }
                        // WebDevHelper console.
                        if (window.debugService) {
                            window.debugService.trace(text);
                        }
                };";

        /// <summary>
        /// Browser debug javascript call
        /// </summary>
        private const string JavascriptDebugExec = "_dbg('{0}');";

        #endregion Fields

        #region Properties

        private static ScriptObject debugFunction
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Trace a debug in Browser output
        /// </summary>
        /// <param name="text">The text to display in debug output</param>
        public static void WriteLine(string text)
        {
            if (debugFunction == null)
            {
                HtmlElement elem = HtmlPage.Document.CreateElement("script");
                elem.SetAttribute("type", "text/javascript");
                elem.SetProperty("text", JavascriptDebug);
                HtmlPage.Document.Body.AppendChild(elem);
                debugFunction = HtmlPage.Window.GetProperty("_dbg") as ScriptObject;
            }
            System.Diagnostics.Debug.WriteLine(text);
            //HtmlPage.Window.Eval(JavascriptDebug + string.Format(JavascriptDebugExec, (text ?? "").Replace("\\", "\\\\").Replace("'", "\\'")));
            debugFunction.InvokeSelf(text);
        }

        /// <summary>
        /// Trace a debug in Browser output
        /// </summary>
        /// <param name="text">The text to display in debug output</param>
        public static void WriteLine(object value)
        {
            WriteLine(Convert.ToString(value));
        }

        #endregion Methods
    }
}