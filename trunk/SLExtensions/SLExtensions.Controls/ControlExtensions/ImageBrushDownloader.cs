namespace SLExtensions.Controls
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
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;

    public static class ImageBrushDownloader
    {
        #region Fields

        // Using a DependencyProperty as the backing store for Stretch.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StretchProperty = 
            DependencyProperty.RegisterAttached("Stretch", typeof(Stretch), typeof(ImageBrushDownloader), new PropertyMetadata(Stretch.Uniform, StretchChangedCallback));

        // Using a DependencyProperty as the backing store for SyncSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SyncSourceProperty = 
            DependencyProperty.RegisterAttached("SyncSource", typeof(string), typeof(ImageBrushDownloader), new PropertyMetadata(SyncSourceChanged));

        // Using a DependencyProperty as the backing store for SyncBitmapImage.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty SyncBitmapImageProperty = 
            DependencyProperty.RegisterAttached("SyncBitmapImage", typeof(BitmapImage), typeof(ImageBrushDownloader), new PropertyMetadata(SyncBitmapImageChangedCallback));

        #endregion Fields

        #region Methods

        public static Stretch GetStretch(DependencyObject obj)
        {
            return (Stretch)obj.GetValue(StretchProperty);
        }

        public static BitmapImage GetSyncBitmapImage(DependencyObject obj)
        {
            return (BitmapImage)obj.GetValue(SyncBitmapImageProperty);
        }

        public static string GetSyncSource(DependencyObject obj)
        {
            return (string)obj.GetValue(SyncSourceProperty);
        }

        public static void SetStretch(DependencyObject obj, Stretch value)
        {
            obj.SetValue(StretchProperty, value);
        }

        public static void SetSyncSource(DependencyObject obj, string value)
        {
            obj.SetValue(SyncSourceProperty, value);
        }

        private static void SetSyncBitmapImage(DependencyObject obj, BitmapImage value)
        {
            obj.SetValue(SyncBitmapImageProperty, value);
        }

        private static void StretchChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Shape shape = d as Shape;
            if (shape == null)
                return;

            ImageBrush ib = shape.Fill as ImageBrush;
            if (ib != null)
                ib.Stretch = (Stretch)e.NewValue;
        }

        private static void SyncBitmapImageChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        private static void SyncSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Shape targetShape = d as Shape;

            string url = e.NewValue as string;
            internalDownloader dldr = new internalDownloader();
            dldr.Shape = targetShape;
            dldr.Url = url;
            dldr.Download();
        }

        #endregion Methods

        #region Nested Types

        private class internalDownloader
        {
            #region Fields

            private BitmapImage bmpImage;
            private ImageBrush brush;
            private Panel pnl;
            private Rectangle rect;

            #endregion Fields

            #region Properties

            public Shape Shape
            {
                get; set;
            }

            public string Url
            {
                get; set;
            }

            #endregion Properties

            #region Methods

            public void Download()
            {
                if (Shape == null)
                    return;

                Uri uri;
                if (!Uri.TryCreate(Url, UriKind.RelativeOrAbsolute, out uri))
                    return;

                bmpImage = new BitmapImage(uri);
                brush = new ImageBrush() { ImageSource = bmpImage };
                brush.ImageFailed += new EventHandler<ExceptionRoutedEventArgs>(brush_ImageFailed);
                brush.Stretch = GetStretch(Shape);

                SetSyncBitmapImage(Shape, bmpImage);

                FrameworkElement elem = Shape as FrameworkElement;
                while (elem != null && !(elem is Panel))
                {
                    elem = elem.Parent as FrameworkElement;
                }
                if (elem == null)
                    return;

                bmpImage.DownloadProgress += new EventHandler<DownloadProgressEventArgs>(bmpImage_DownloadProgress);

                pnl = elem as Panel;
                rect = new Rectangle();
                rect.Width = 0;
                rect.Height = 0;
                rect.Opacity = 0;
                rect.Fill = brush;
                pnl.Children.Add(rect);
            }

            void bmpImage_DownloadProgress(object sender, DownloadProgressEventArgs e)
            {
                if (e.Progress == 100)
                {
                    bmpImage.DownloadProgress -= new EventHandler<DownloadProgressEventArgs>(bmpImage_DownloadProgress);
                    pnl.Children.Remove(rect);
                    Shape.Fill = brush;
                }
            }

            void brush_ImageFailed(object sender, ExceptionRoutedEventArgs e)
            {
                brush.ImageFailed -= new EventHandler<ExceptionRoutedEventArgs>(brush_ImageFailed);
                bmpImage.DownloadProgress -= new EventHandler<DownloadProgressEventArgs>(bmpImage_DownloadProgress);
            }

            #endregion Methods
        }

        #endregion Nested Types
    }
}