namespace SLExtensions.Interactivity
{
    using System;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Interactivity;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    using SLExtensions;

    public class ClippingBorder : Behavior<FrameworkElement>
    {
        #region Fields

        private RectangleGeometry clipping;

        #endregion Fields

        #region Methods

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Loaded += new RoutedEventHandler(AssociatedObject_Loaded);
            this.AssociatedObject.SizeChanged += new SizeChangedEventHandler(AssociatedObject_SizeChanged);
            Clip();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.Loaded -= new RoutedEventHandler(AssociatedObject_Loaded);
            this.AssociatedObject.SizeChanged -= new SizeChangedEventHandler(AssociatedObject_SizeChanged);
        }

        void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            Clip();
        }

        void AssociatedObject_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Clip();
        }

        private void Clip()
        {
            if (clipping == null)
            {
                clipping = new RectangleGeometry();
                AssociatedObject.Clip = clipping;
            }

            if (AssociatedObject.ActualWidth.IsRational()
                && AssociatedObject.ActualHeight.IsRational())
            {
                clipping.Rect = new Rect(0, 0, AssociatedObject.ActualWidth, AssociatedObject.ActualHeight);
            }
        }

        #endregion Methods
    }
}