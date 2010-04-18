namespace SLMedia.Core
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

    using SLMedia.Core;

    public interface IMarkerSelector
    {
        #region Events

        event EventHandler IsActiveChanged;

        #endregion Events

        #region Properties

        IMarker ActiveMarker
        {
            get; set;
        }

        bool IsActive
        {
            get; set;
        }

        IList<IMarker> Markers
        {
            get;
        }

        IDictionary<string, object> Metadata
        {
            get;
        }

        #endregion Properties
    }
}