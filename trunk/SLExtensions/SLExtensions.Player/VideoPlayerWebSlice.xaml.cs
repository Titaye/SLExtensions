namespace SLExtensions.Player
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Windows;
    using System.Windows.Browser;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    using SLMedia.Core;
    using SLMedia.PlaylistProvider.MSNVideo;
    using SLMedia.Video;

    public partial class VideoPlayerWebSlice : UserControl
    {
        #region Constructors

        public VideoPlayerWebSlice()
        {
            InitializeComponent();
            Controller = Resources["controller"] as VideoController;
        }

        #endregion Constructors

        #region Properties

        public VideoController Controller
        {
            get;
            private set;
        }

        public Uri VideoLinkUri
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var scale = 1d;
            var top = 0d;
            g.Width = e.NewSize.Width;
            g.Height = e.NewSize.Height;
            if (e.NewSize.Width < g.MinWidth)
            {
                scale = e.NewSize.Width / g.MinWidth;
                top = (e.NewSize.Height - (scale * g.ActualHeight)) / 2;
            }

            scale1.ScaleX = scale;
            scale1.ScaleY = scale;
            Canvas.SetTop(g, top);
        }

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.VideoLinkUri != null)
                hlVideoLink.NavigateUri = this.VideoLinkUri;
        }

        private void media_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
        }

        #endregion Methods
    }
}
