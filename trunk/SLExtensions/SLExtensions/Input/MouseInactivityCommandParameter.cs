namespace SLExtensions.Input
{
    using System;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    public class MouseInactivityCommandParameter
    {
        #region Properties

        public string ActiveCommandName
        {
            get; set;
        }

        public object ActiveCommandParameter
        {
            get; set;
        }

        public string InActiveCommandName
        {
            get; set;
        }

        public object InActiveCommandParameter
        {
            get; set;
        }

        public TimeSpan InactivityTimeSpan
        {
            get; set;
        }

        #endregion Properties
    }
}