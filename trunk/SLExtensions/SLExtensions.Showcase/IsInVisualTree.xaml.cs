namespace SLExtensions.Showcase
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    using SLExtensions;

    public partial class IsInVisualTree : UserControl
    {
        #region Constructors

        public IsInVisualTree()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LayoutRoot.Children.Remove(rect);
        }

        private void rect_LayoutUpdated(object sender, EventArgs e)
        {
            if (!rect.IsInVisualTree())
            {
                // Handle unload behavior
                Dispatcher.BeginInvoke(new ThreadStart(delegate { btn.Content = "Rect removed"; }));
            }
        }

        #endregion Methods
    }
}