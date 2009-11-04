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

    using SLMedia.Core;
    using SLMedia.PlaylistProvider.MSNVideo;

    public partial class PagePlayerVideoSmoothHD : UserControl
    {
        #region Constructors

        public PagePlayerVideoSmoothHD()
        {
            InitializeComponent();
            //
            videoPlayer.Controller.Playlist.Add("http://www.leprojecteur.fr/smoothhd/ClientBin/Coral_Reef_Adventure_720.ism/Manifest");
            videoPlayer.Controller.Next();
        }

        #endregion Constructors
    }
}