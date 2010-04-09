namespace SLExtensions.Markup
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    public class MarkupContent
    {
        #region Properties

        public string Content
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }

        public IDictionary<string, string> Parameters
        {
            get; set;
        }

        public MarkupType Type { get; set; }
        #endregion Properties
    }
}