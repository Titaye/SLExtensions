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

    public class MarginWrapper
    {
        #region Fields

        // Using a DependencyProperty as the backing store for MarginBottom.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty marginBottomProperty = 
            DependencyProperty.RegisterAttached("MarginBottom", typeof(double), typeof(MarginWrapper), new PropertyMetadata(double.NaN, MarginChangedCallback));

        // Using a DependencyProperty as the backing store for MarginHorizontalPercentShift.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty marginHorizontalPercentShiftProperty = 
            DependencyProperty.RegisterAttached("MarginHorizontalPercentShift", typeof(double), typeof(MarginWrapper), new PropertyMetadata(double.NaN, MarginChangedCallback));

        // Using a DependencyProperty as the backing store for MarginHorizontalShift.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty marginHorizontalShiftProperty = 
            DependencyProperty.RegisterAttached("MarginHorizontalShift", typeof(double), typeof(MarginWrapper), new PropertyMetadata(double.NaN, MarginChangedCallback));

        // Using a DependencyProperty as the backing store for MarginLeft.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty marginLeftProperty = 
            DependencyProperty.RegisterAttached("MarginLeft", typeof(double), typeof(MarginWrapper), new PropertyMetadata(double.NaN, MarginChangedCallback));

        // Using a DependencyProperty as the backing store for MarginPercentBottom.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty marginPercentBottomProperty = 
            DependencyProperty.RegisterAttached("MarginPercentBottom", typeof(double), typeof(MarginWrapper), new PropertyMetadata(double.NaN, MarginChangedCallback));

        // Using a DependencyProperty as the backing store for MarginPercentLeft.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty marginPercentLeftProperty = 
            DependencyProperty.RegisterAttached("MarginPercentLeft", typeof(double), typeof(MarginWrapper), new PropertyMetadata(double.NaN, MarginChangedCallback));

        // Using a DependencyProperty as the backing store for MarginPercentRight.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty marginPercentRightProperty = 
            DependencyProperty.RegisterAttached("MarginPercentRight", typeof(double), typeof(MarginWrapper), new PropertyMetadata(double.NaN, MarginChangedCallback));

        // Using a DependencyProperty as the backing store for MarginPercentTop.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty marginPercentTopProperty = 
            DependencyProperty.RegisterAttached("MarginPercentTop", typeof(double), typeof(MarginWrapper), new PropertyMetadata(double.NaN, MarginChangedCallback));

        // Using a DependencyProperty as the backing store for MarginRight.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty marginRightProperty = 
            DependencyProperty.RegisterAttached("MarginRight", typeof(double), typeof(MarginWrapper), new PropertyMetadata(double.NaN, MarginChangedCallback));

        // Using a DependencyProperty as the backing store for MarginTop.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty marginTopProperty = 
            DependencyProperty.RegisterAttached("MarginTop", typeof(double), typeof(MarginWrapper), new PropertyMetadata(double.NaN, MarginChangedCallback));

        // Using a DependencyProperty as the backing store for MarginVerticalPercentShift.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty marginVerticalPercentShiftProperty = 
            DependencyProperty.RegisterAttached("MarginVerticalPercentShift", typeof(double), typeof(MarginWrapper), new PropertyMetadata(double.NaN, MarginChangedCallback));

        // Using a DependencyProperty as the backing store for MarginVerticalShift.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty marginVerticalShiftProperty = 
            DependencyProperty.RegisterAttached("MarginVerticalShift", typeof(double), typeof(MarginWrapper), new PropertyMetadata(double.NaN, MarginChangedCallback));

        #endregion Fields

        #region Properties

        public static DependencyProperty MarginBottomProperty
        {
            get { return marginBottomProperty; }
        }

        public static DependencyProperty MarginHorizontalPercentShiftProperty
        {
            get { return marginHorizontalPercentShiftProperty; }
        }

        public static DependencyProperty MarginHorizontalShiftProperty
        {
            get { return marginHorizontalShiftProperty; }
        }

        public static DependencyProperty MarginLeftProperty
        {
            get { return marginLeftProperty; }
        }

        public static DependencyProperty MarginPercentBottomProperty
        {
            get { return marginPercentBottomProperty; }
        }

        public static DependencyProperty MarginPercentLeftProperty
        {
            get { return marginPercentLeftProperty; }
        }

        public static DependencyProperty MarginPercentRightProperty
        {
            get { return marginPercentRightProperty; }
        }

        public static DependencyProperty MarginPercentTopProperty
        {
            get { return marginPercentTopProperty; }
        }

        public static DependencyProperty MarginRightProperty
        {
            get { return marginRightProperty; }
        }

        public static DependencyProperty MarginTopProperty
        {
            get { return marginTopProperty; }
        }

        public static DependencyProperty MarginVerticalPercentShiftProperty
        {
            get { return marginVerticalPercentShiftProperty; }
        }

        public static DependencyProperty MarginVerticalShiftProperty
        {
            get { return marginVerticalShiftProperty; }
        }

        #endregion Properties

        #region Methods

        public static double GetMarginBottom(DependencyObject obj)
        {
            return (double)obj.GetValue(MarginBottomProperty);
        }

        public static double GetMarginHorizontalPercentShift(DependencyObject obj)
        {
            return (double)obj.GetValue(MarginHorizontalPercentShiftProperty);
        }

        public static double GetMarginHorizontalShift(DependencyObject obj)
        {
            return (double)obj.GetValue(MarginHorizontalShiftProperty);
        }

        public static double GetMarginLeft(DependencyObject obj)
        {
            return (double)obj.GetValue(MarginLeftProperty);
        }

        public static double GetMarginPercentBottom(DependencyObject obj)
        {
            return (double)obj.GetValue(MarginPercentBottomProperty);
        }

        public static double GetMarginPercentLeft(DependencyObject obj)
        {
            return (double)obj.GetValue(MarginPercentLeftProperty);
        }

        public static double GetMarginPercentRight(DependencyObject obj)
        {
            return (double)obj.GetValue(MarginPercentRightProperty);
        }

        public static double GetMarginPercentTop(DependencyObject obj)
        {
            return (double)obj.GetValue(MarginPercentTopProperty);
        }

        public static double GetMarginRight(DependencyObject obj)
        {
            return (double)obj.GetValue(MarginRightProperty);
        }

        public static double GetMarginTop(DependencyObject obj)
        {
            return (double)obj.GetValue(MarginTopProperty);
        }

        public static double GetMarginVerticalPercentShift(DependencyObject obj)
        {
            return (double)obj.GetValue(MarginVerticalPercentShiftProperty);
        }

        public static double GetMarginVerticalShift(DependencyObject obj)
        {
            return (double)obj.GetValue(MarginVerticalShiftProperty);
        }

        public static void SetMarginBottom(DependencyObject obj, double value)
        {
            obj.SetValue(MarginBottomProperty, value);
        }

        public static void SetMarginHorizontalPercentShift(DependencyObject obj, double value)
        {
            obj.SetValue(MarginHorizontalPercentShiftProperty, value);
        }

        public static void SetMarginHorizontalShift(DependencyObject obj, double value)
        {
            obj.SetValue(MarginHorizontalShiftProperty, value);
        }

        public static void SetMarginLeft(DependencyObject obj, double value)
        {
            obj.SetValue(MarginLeftProperty, value);
        }

        public static void SetMarginPercentBottom(DependencyObject obj, double value)
        {
            obj.SetValue(MarginPercentBottomProperty, value);
        }

        public static void SetMarginPercentLeft(DependencyObject obj, double value)
        {
            obj.SetValue(MarginPercentLeftProperty, value);
        }

        public static void SetMarginPercentRight(DependencyObject obj, double value)
        {
            obj.SetValue(MarginPercentRightProperty, value);
        }

        public static void SetMarginPercentTop(DependencyObject obj, double value)
        {
            obj.SetValue(MarginPercentTopProperty, value);
        }

        public static void SetMarginRight(DependencyObject obj, double value)
        {
            obj.SetValue(MarginRightProperty, value);
        }

        public static void SetMarginTop(DependencyObject obj, double value)
        {
            obj.SetValue(MarginTopProperty, value);
        }

        public static void SetMarginVerticalPercentShift(DependencyObject obj, double value)
        {
            obj.SetValue(MarginVerticalPercentShiftProperty, value);
        }

        public static void SetMarginVerticalShift(DependencyObject obj, double value)
        {
            obj.SetValue(MarginVerticalShiftProperty, value);
        }

        private static void ApplyMargin(FrameworkElement frameworkElement)
        {
            Thickness margin = frameworkElement.Margin;

            var left = GetMarginLeft(frameworkElement);
            if (!double.IsNaN(left))
                margin.Left = left;

            var top = GetMarginTop(frameworkElement);
            if (!double.IsNaN(top))
                margin.Top = top;

            var right = GetMarginRight(frameworkElement);
            if (!double.IsNaN(right))
                margin.Right = right;

            var bottom = GetMarginBottom(frameworkElement);
            if (!double.IsNaN(bottom))
                margin.Bottom = bottom;

            var verticalShift = GetMarginVerticalShift(frameworkElement);
            if (!double.IsNaN(verticalShift))
            {
                margin.Top = verticalShift;
                margin.Bottom = -verticalShift;
            }

            var horizontalShift = GetMarginHorizontalShift(frameworkElement);
            if (!double.IsNaN(horizontalShift))
            {
                margin.Left = horizontalShift;
                margin.Right = -horizontalShift;
            }

            if (frameworkElement.ActualHeight != 0
                && frameworkElement.ActualWidth != 0)
            {
                var pcleft = GetMarginPercentLeft(frameworkElement);
                if (!double.IsNaN(pcleft))
                {
                    left = frameworkElement.ActualWidth * pcleft;
                    margin.Left = left;
                }

                var pctop = GetMarginPercentTop(frameworkElement);
                if (!double.IsNaN(pctop))
                {
                    top = frameworkElement.ActualHeight * pctop;
                    margin.Top = top;
                }

                var pcright = GetMarginPercentRight(frameworkElement);
                if (!double.IsNaN(pcright))
                {
                    right = frameworkElement.ActualWidth * pcright;
                    margin.Right = right;
                }

                var pcbottom = GetMarginPercentBottom(frameworkElement);
                if (!double.IsNaN(pcbottom))
                {
                    bottom = frameworkElement.ActualHeight * pcbottom;
                    margin.Bottom = pcbottom;
                }

                var verticalPercentShift = GetMarginVerticalPercentShift(frameworkElement);
                if (!double.IsNaN(verticalPercentShift))
                {
                    verticalShift = frameworkElement.ActualWidth * verticalPercentShift;
                    margin.Top = verticalShift;
                    margin.Bottom = -verticalShift;
                }

                var horizontalPercentShift = GetMarginHorizontalPercentShift(frameworkElement);
                if (!double.IsNaN(horizontalPercentShift))
                {
                    horizontalShift = frameworkElement.ActualWidth * horizontalPercentShift;
                    margin.Left = horizontalShift;
                    margin.Right = -horizontalShift;
                }
            }

            frameworkElement.Margin = margin;
        }

        static void fe_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ApplyMargin((FrameworkElement)sender);
        }

        private static void MarginChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var fe = d as FrameworkElement;
            ApplyMargin(fe);
            if (fe != null)
                fe.SizeChanged += new SizeChangedEventHandler(fe_SizeChanged);
        }

        #endregion Methods

        #region Other

        //////////////////////////////////////////
        //        MarginBottom = double.NaN;
        //MarginBottomPercent = double.NaN;
        //MarginHorizontalPercentShift = double.NaN;
        //MarginHorizontalShift = double.NaN;
        //MarginLeft = double.NaN;
        //MarginLeftPercent = double.NaN;
        //MarginRight = double.NaN;
        //MarginRightPercent = double.NaN;
        //MarginTop = double.NaN;
        //MarginTopPercent = double.NaN;
        //MarginVerticalPercentShift = double.NaN;
        //MarginVerticalShift = double.NaN;

        #endregion Other
    }
}