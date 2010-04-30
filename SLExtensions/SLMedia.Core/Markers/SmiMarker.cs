namespace SLMedia.Core
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

    using SLMedia.Core;

    public class SmiMarker : MarkerContent
    {
        #region Properties

        public string Language
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }

        #endregion Properties
    }
}