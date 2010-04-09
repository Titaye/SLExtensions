﻿namespace SLMedia.Core
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

    public class MarkerThumbnail : MarkerContent
    {
        #region Properties

        public Uri Thumbnail
        {
            get; set;
        }

        #endregion Properties
    }
}