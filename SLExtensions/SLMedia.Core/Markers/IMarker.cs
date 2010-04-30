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

    public interface IMarker
    {
        #region Properties

        Duration Duration
        {
            get;
        }

        bool IsActive
        {
            get; set;
        }

        TimeSpan Position
        {
            get;
        }

        #endregion Properties
    }
}