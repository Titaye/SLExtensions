﻿namespace SLMedia.Core
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

    public interface IMarkerParser
    {
        #region Methods

        IEnumerable<IMarkerSelector> Parse(string content, IDictionary<string, object> metadata);

        #endregion Methods
    }
}