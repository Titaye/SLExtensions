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
    using System.Windows.Shapes;

    public static class ImageExtensions
    {
        #region Fields

        // Using a DependencyProperty as the backing store for IgnoreImageFailed.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IgnoreImageFailedProperty = 
            DependencyProperty.RegisterAttached("IgnoreImageFailed", typeof(bool), typeof(ImageExtensions), new PropertyMetadata(IgnoreImageFailedChanged));

        internal static readonly DependencyProperty imageExtenderProperty = 
            DependencyProperty.RegisterAttached("imageExtenderProperty", typeof(internalImageExtender), typeof(ImageExtensions), null);

        #endregion Fields

        #region Methods

        public static bool GetIgnoreImageFailed(DependencyObject obj)
        {
            return (bool)obj.GetValue(IgnoreImageFailedProperty);
        }

        public static void SetIgnoreImageFailed(DependencyObject obj, bool value)
        {
            obj.SetValue(IgnoreImageFailedProperty, value);
        }

        private static void IgnoreImageFailedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Image target = d as Image;
            if (target == null)
                return;

            internalImageExtender extender = target.GetValue(imageExtenderProperty) as internalImageExtender;
            if (extender == null)
            {
                extender = new internalImageExtender(target);
                target.SetValue(imageExtenderProperty, extender);
            }

            extender.IgnoreImageFailed = (bool)e.NewValue;
        }

        #endregion Methods

        #region Nested Types

        private class internalImageExtender
        {
            #region Fields

            private bool ignoreImageFailed;

            #endregion Fields

            #region Constructors

            public internalImageExtender(Image image)
            {
                Image = image;
            }

            #endregion Constructors

            #region Properties

            public bool IgnoreImageFailed
            {
                get
                {
                    return this.ignoreImageFailed;
                }

                set
                {
                    if (this.ignoreImageFailed != value)
                    {
                        this.Image.ImageFailed -= new EventHandler<ExceptionRoutedEventArgs>(Image_IgnoreImageFailed);

                        this.ignoreImageFailed = value;

                        if (this.ignoreImageFailed)
                            this.Image.ImageFailed += new EventHandler<ExceptionRoutedEventArgs>(Image_IgnoreImageFailed);
                    }
                }
            }

            public Image Image
            {
                get; private set;
            }

            #endregion Properties

            #region Methods

            private void Image_IgnoreImageFailed(object sender, ExceptionRoutedEventArgs e)
            {
            }

            #endregion Methods
        }

        #endregion Nested Types
    }
}