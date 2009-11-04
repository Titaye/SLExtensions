namespace SLExtensions.Showcase
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    using SLExtensions.Diagnostics;

    using SLMedia.Core;
    using SLMedia.Deepzoom;

    public partial class PagePlayerDeepZoom : UserControl
    {
        #region Constructors

        public PagePlayerDeepZoom()
        {
            InitializeComponent();

            player.Controller.Playlist.Add("dznantes/dzc_output.xml");
        }

        #endregion Constructors
    }
}