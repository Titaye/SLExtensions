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

    public interface IMarkerSource
    {
        #region Properties

        IDictionary<string, object> Metadata
        {
            get;
        }

        #endregion Properties

        #region Methods

        void LoadAsync(Action<IEnumerable<IMarkerSelector>> endLoadCallback, IDictionary<string, object> metadata);

        #endregion Methods
    }
}