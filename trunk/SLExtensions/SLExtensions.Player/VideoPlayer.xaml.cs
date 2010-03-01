namespace SLExtensions.Player
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
    using SLMedia.Video;

    public partial class VideoPlayer : UserControl
    {
        #region Constructors

        public VideoPlayer()
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

        #endregion Properties

        #region Methods

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
        }

        private void media_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
        }

        #endregion Methods
    }
}
