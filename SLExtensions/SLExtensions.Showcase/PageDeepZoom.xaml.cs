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

    using SLExtensions.DeepZoom;

    public partial class PageDeepZoom : UserControl
    {
        #region Fields

        private int collectionIndex = -1;

        #endregion Fields

        #region Constructors

        public PageDeepZoom()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        private void All_Click(object sender, RoutedEventArgs e)
        {
            msi.ZoomCenter();
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            DZContext context = msi.EnsureContext();
            collectionIndex = (collectionIndex + 1) % context.ImagesToShow.Count;
            msi.ZoomFullAndCenterImage(collectionIndex);
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            DZContext context = msi.EnsureContext();
            collectionIndex = (collectionIndex + context.ImagesToShow.Count - 1) % context.ImagesToShow.Count;
            msi.ZoomFullAndCenterImage(collectionIndex);
        }

        #endregion Methods
    }
}