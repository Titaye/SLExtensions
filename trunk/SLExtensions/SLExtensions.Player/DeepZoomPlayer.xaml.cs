namespace SLExtensions.Player
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
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
    using SLMedia.Deepzoom;

    public partial class DeepZoomPlayer : UserControl
    {
        #region Constructors

        public DeepZoomPlayer()
        {
            InitializeComponent();
            Controller = Resources["controller"] as DeepZoomController;
        }

        #endregion Constructors

        #region Properties

        public DeepZoomController Controller
        {
            get; private set;
        }

        #endregion Properties

        #region Methods

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
        }

        private void media_ImageOpenSucceeded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("page media_ImageOpenSucceeded");
        }

        #endregion Methods
    }
}
