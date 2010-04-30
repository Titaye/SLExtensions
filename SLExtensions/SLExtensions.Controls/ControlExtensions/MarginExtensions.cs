namespace SLExtensions.Controls.ControlExtensions
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

    using SLExtensions;

    public class MarginExtensions
    {
        #region Fields

        // Using a DependencyProperty as the backing store for MarginBottomPercent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MarginBottomPercentProperty = 
            DependencyProperty.RegisterAttached("MarginBottomPercent", typeof(double), typeof(MarginExtensions), new PropertyMetadata(MarginBottomPercentChangedCallback));

        // Using a DependencyProperty as the backing store for MarginBottom.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MarginBottomProperty = 
            DependencyProperty.RegisterAttached("MarginBottom", typeof(double), typeof(MarginExtensions), new PropertyMetadata(MarginBottomChangedCallback));

        // Using a DependencyProperty as the backing store for MarginLeftPercent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MarginLeftPercentProperty = 
            DependencyProperty.RegisterAttached("MarginLeftPercent", typeof(double), typeof(MarginExtensions), new PropertyMetadata(MarginLeftPercentChangedCallback));

        // Using a DependencyProperty as the backing store for MarginLeft.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MarginLeftProperty = 
            DependencyProperty.RegisterAttached("MarginLeft", typeof(double), typeof(MarginExtensions), new PropertyMetadata(MarginLeftChangedCallback));

        // Using a DependencyProperty as the backing store for MarginRightPercent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MarginRightPercentProperty = 
            DependencyProperty.RegisterAttached("MarginRightPercent", typeof(double), typeof(MarginExtensions), new PropertyMetadata(MarginRightPercentChangedCallback));

        // Using a DependencyProperty as the backing store for MarginRight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MarginRightProperty = 
            DependencyProperty.RegisterAttached("MarginRight", typeof(double), typeof(MarginExtensions), new PropertyMetadata(MarginRightChangedCallback));

        // Using a DependencyProperty as the backing store for MarginTopPercent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MarginTopPercentProperty = 
            DependencyProperty.RegisterAttached("MarginTopPercent", typeof(double), typeof(MarginExtensions), new PropertyMetadata(MarginTopPercentChangedCallback));

        // Using a DependencyProperty as the backing store for MarginTop.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MarginTopProperty = 
            DependencyProperty.RegisterAttached("MarginTop", typeof(double), typeof(MarginExtensions), new PropertyMetadata(MarginTopChangedCallback));

        // Using a DependencyProperty as the backing store for GridRow.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty MarginExtensionsProperty = 
            DependencyProperty.RegisterAttached("MarginExtensions", typeof(MarginExtensions), typeof(MarginExtensions), null);

        private FrameworkElement parent;

        #endregion Fields

        #region Constructors

        public MarginExtensions(FrameworkElement element)
        {
            this.Element = element;
            parent = VisualTreeHelper.GetParent(element) as FrameworkElement;
            if (parent != null)
            {
                parent.SizeChanged += new SizeChangedEventHandler(parent_SizeChanged);
             }
        }

        #endregion Constructors

        #region Properties

        public FrameworkElement Element
        {
            get; private set;
        }

        #endregion Properties

        #region Methods

        public static double GetMarginBottom(DependencyObject obj)
        {
            return (double)obj.GetValue(MarginBottomProperty);
        }

        public static double GetMarginBottomPercent(DependencyObject obj)
        {
            return (double)obj.GetValue(MarginBottomPercentProperty);
        }

        public static double GetMarginLeft(DependencyObject obj)
        {
            return (double)obj.GetValue(MarginLeftProperty);
        }

        public static double GetMarginLeftPercent(DependencyObject obj)
        {
            return (double)obj.GetValue(MarginLeftPercentProperty);
        }

        public static double GetMarginRight(DependencyObject obj)
        {
            return (double)obj.GetValue(MarginRightProperty);
        }

        public static double GetMarginRightPercent(DependencyObject obj)
        {
            return (double)obj.GetValue(MarginRightPercentProperty);
        }

        public static double GetMarginTop(DependencyObject obj)
        {
            return (double)obj.GetValue(MarginTopProperty);
        }

        public static double GetMarginTopPercent(DependencyObject obj)
        {
            return (double)obj.GetValue(MarginTopPercentProperty);
        }

        public static void SetMarginBottom(DependencyObject obj, double value)
        {
            obj.SetValue(MarginBottomProperty, value);
        }

        public static void SetMarginBottomPercent(DependencyObject obj, double value)
        {
            obj.SetValue(MarginBottomPercentProperty, value);
        }

        public static void SetMarginLeft(DependencyObject obj, double value)
        {
            obj.SetValue(MarginLeftProperty, value);
        }

        public static void SetMarginLeftPercent(DependencyObject obj, double value)
        {
            obj.SetValue(MarginLeftPercentProperty, value);
        }

        public static void SetMarginRight(DependencyObject obj, double value)
        {
            obj.SetValue(MarginRightProperty, value);
        }

        public static void SetMarginRightPercent(DependencyObject obj, double value)
        {
            obj.SetValue(MarginRightPercentProperty, value);
        }

        public static void SetMarginTop(DependencyObject obj, double value)
        {
            obj.SetValue(MarginTopProperty, value);
        }

        public static void SetMarginTopPercent(DependencyObject obj, double value)
        {
            obj.SetValue(MarginTopPercentProperty, value);
        }

        private static MarginExtensions GetExtensions(DependencyObject obj)
        {
            FrameworkElement fe = obj as FrameworkElement;
            if (fe == null)
                throw new NotSupportedException("ItemsPresenterExtensions must be assigned to a frameworkelement");

            var ext = fe.GetValue(MarginExtensionsProperty) as MarginExtensions;
            if (ext == null)
            {
                ext = new MarginExtensions(fe);
                fe.SetValue(MarginExtensionsProperty, ext);
            }
            return ext;
        }

        private static void MarginBottomChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GetExtensions(d).UpdateBottom((double)e.NewValue);
        }

        private static void MarginBottomPercentChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GetExtensions(d).UpdatePercentMargins();
        }

        private static void MarginLeftChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GetExtensions(d).UpdateLeft((double)e.NewValue);
        }

        private static void MarginLeftPercentChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GetExtensions(d).UpdatePercentMargins();
        }

        private static void MarginRightChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GetExtensions(d).UpdateRight((double)e.NewValue);
        }

        private static void MarginRightPercentChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GetExtensions(d).UpdatePercentMargins();
        }

        private static void MarginTopChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GetExtensions(d).UpdateTop((double)e.NewValue);
        }

        private static void MarginTopPercentChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GetExtensions(d).UpdatePercentMargins();
        }

        void Element_Loaded(object sender, RoutedEventArgs e)
        {
            if (parent == null)
            {
                parent = VisualTreeHelper.GetParent(this.Element) as FrameworkElement;
                parent.SizeChanged += new SizeChangedEventHandler(parent_SizeChanged);
            }
            UpdateMargin();
            UpdatePercentMargins();
        }

        private bool HasValue(DependencyProperty prop, out double value)
        {
            value = 0;
            object obj = Element.GetValue(prop);
            if (obj != null)
            {
                value = (double)obj;
                return true;
            }
            return false;
        }

        void parent_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdatePercentMargins();
        }

        private void UpdateBottom(double value)
        {
            var m = Element.Margin;
            m.Bottom = value;
            Element.Margin = m;
        }

        private void UpdateLeft(double value)
        {
            var m = Element.Margin;
            m.Left = value;
            Element.Margin = m;
        }

        private void UpdateMargin()
        {
            var m = Element.Margin;

            double value;
            if (HasValue(MarginLeftProperty, out value))
                m.Left = value;
            if (HasValue(MarginTopProperty, out value))
                m.Top = value;
            if (HasValue(MarginRightProperty, out value))
                m.Right = value;
            if (HasValue(MarginBottomProperty, out value))
                m.Bottom = value;

            Element.Margin = m;
        }

        private void UpdatePercentMargins()
        {
            if (parent == null)
                return;
            var m = Element.Margin;

            double value;
            if (HasValue(MarginLeftPercentProperty, out value))
                m.Left = value * parent.RenderSize.Width;
            if (HasValue(MarginTopPercentProperty, out value))
                m.Top = value * parent.RenderSize.Height;
            if (HasValue(MarginRightPercentProperty, out value))
                m.Right = value * parent.RenderSize.Width;
            if (HasValue(MarginBottomPercentProperty, out value))
                m.Bottom = value * parent.RenderSize.Height;

            Element.Margin = m;
        }

        private void UpdateRight(double value)
        {
            var m = Element.Margin;
            m.Right = value;
            Element.Margin = m;
        }

        private void UpdateTop(double value)
        {
            var m = Element.Margin;
            m.Top = value;
            Element.Margin = m;
        }

        #endregion Methods
    }
}