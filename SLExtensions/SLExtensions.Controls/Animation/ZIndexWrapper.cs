namespace SLExtensions.Controls.Animation
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

    public class ZIndexWrapper
    {
        #region Fields

        // Using a DependencyProperty as the backing store for ZIndex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ZIndexProperty = 
            DependencyProperty.RegisterAttached("ZIndex", typeof(double), typeof(ZIndexWrapper), new PropertyMetadata(ZIndexChangedCallback));

        #endregion Fields

        #region Methods

        public static double GetZIndex(DependencyObject obj)
        {
            return (double)obj.GetValue(ZIndexProperty);
        }

        public static void SetZIndex(DependencyObject obj, double value)
        {
            obj.SetValue(ZIndexProperty, value);
        }

        private static void ZIndexChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement elem = d as UIElement;
            if(elem == null)
                return;
            Canvas.SetZIndex(elem, Convert.ToInt32(e.NewValue));
        }

        #endregion Methods
    }
}