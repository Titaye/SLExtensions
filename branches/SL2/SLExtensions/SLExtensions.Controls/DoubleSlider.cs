namespace SLExtensions.Controls
{
    using System;
    using System.ComponentModel;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    /// <summary>
    /// Double Slider Control. Allow to select a range of value.
    /// </summary>
    [TemplatePart(Name = DoubleSlider.ElementSlider1, Type = typeof(RangeBase))]
    [TemplatePart(Name = DoubleSlider.ElementSlider2, Type = typeof(RangeBase))]
    public class DoubleSlider : Control
    {
        #region Fields

        /// <summary>
        /// LargeChange depedency property.
        /// </summary>
        public static readonly DependencyProperty LargeChangeProperty = 
            DependencyProperty.Register(
                "LargeChange",
                typeof(double),
                typeof(DoubleSlider),
                new PropertyMetadata((d, e) => ((DoubleSlider)d).OnLargeChangeChanged((double)e.OldValue, (double)e.NewValue)));

        /// <summary>
        /// MaxValue depedency property.
        /// </summary>
        public static readonly DependencyProperty MaxValueProperty = 
            DependencyProperty.Register(
                "MaxValue",
                typeof(double),
                typeof(DoubleSlider),
                new PropertyMetadata((d, e) => ((DoubleSlider)d).OnMaxValueChanged((double)e.OldValue, (double)e.NewValue)));

        /// <summary> 
        /// Identifies the Maximum dependency property. 
        /// </summary> 
        public static readonly DependencyProperty MaximumProperty = 
                    DependencyProperty.Register("Maximum", typeof(double),
                          typeof(DoubleSlider), new PropertyMetadata(OnMaximumChanged));

        /// <summary>
        /// MinValue depedency property.
        /// </summary>
        public static readonly DependencyProperty MinValueProperty = 
            DependencyProperty.Register(
                "MinValue",
                typeof(double),
                typeof(DoubleSlider),
                new PropertyMetadata((d, e) => ((DoubleSlider)d).OnMinValueChanged((double)e.OldValue, (double)e.NewValue)));

        /// <summary> 
        /// Identifies the Minimum dependency property. 
        /// </summary> 
        public static readonly DependencyProperty MinimumProperty = 
                    DependencyProperty.Register("Minimum", typeof(double),
                          typeof(DoubleSlider), new PropertyMetadata(OnMinimumChanged));

        /// <summary> 
        /// Identifies the Orientation dependency property. 
        /// </summary> 
        public static readonly DependencyProperty OrientationProperty = 
                    DependencyProperty.Register("Orientation", typeof(Orientation),
                          typeof(DoubleSlider), new PropertyMetadata(OnOrientationChanged));

        /// <summary>
        /// RangeLength depedency property.
        /// </summary>
        public static readonly DependencyProperty RangeLengthProperty = 
            DependencyProperty.Register(
                "RangeLength",
                typeof(double),
                typeof(DoubleSlider),
                null);

        /// <summary> 
        /// Identifies the SmallChange dependency property. 
        /// </summary> 
        public static readonly DependencyProperty SmallChangeProperty = 
                    DependencyProperty.Register("SmallChange", typeof(double),
                          typeof(DoubleSlider), new PropertyMetadata(OnSmallChangeChanged));

        internal const string ElementSlider1 = "Slider1";
        internal const string ElementSlider2 = "Slider2";

        internal RangeBase Slider1;
        internal RangeBase Slider2;

        #endregion Fields

        #region Constructors

        public DoubleSlider()
            : base()
        {
            this.DefaultStyleKey = typeof(DoubleSlider);
        }

        #endregion Constructors

        #region Events

        public event RoutedPropertyChangedEventHandler<double> MaxValueChanged;

        public event RoutedPropertyChangedEventHandler<double> MinValueChanged;

        #endregion Events

        #region Properties

        public double LargeChange
        {
            get
            {
                return (double)GetValue(LargeChangeProperty);
            }

            set
            {
                SetValue(LargeChangeProperty, value);
            }
        }

        public double MaxValue
        {
            get
            {
                return (double)GetValue(MaxValueProperty);
            }

            set
            {
                SetValue(MaxValueProperty, value);
            }
        }

        /// <summary> 
        /// Gets or sets the Maximum possible Value of the double object. 
        /// </summary> 
        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public double MinValue
        {
            get
            {
                return (double)GetValue(MinValueProperty);
            }

            set
            {
                SetValue(MinValueProperty, value);
            }
        }

        /// <summary> 
        /// Gets or sets the Minimum possible Value of the double object. 
        /// </summary> 
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        /// <summary> 
        /// Gets or sets the Orientation possible Value of the double object. 
        /// </summary> 
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public double RangeLength
        {
            get
            {
                return (double)GetValue(RangeLengthProperty);
            }

            private set
            {
                SetValue(RangeLengthProperty, value);
            }
        }

        /// <summary> 
        /// Gets or sets the SmallChange possible Value of the double object. 
        /// </summary> 
        public double SmallChange
        {
            get { return (double)GetValue(SmallChangeProperty); }
            set { SetValue(SmallChangeProperty, value); }
        }

        #endregion Properties

        #region Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.Slider2 = GetTemplateChild(ElementSlider2) as RangeBase;
            this.Slider1 = GetTemplateChild(ElementSlider1) as RangeBase;

            if (this.Slider2 != null)
            {
                this.Slider2.ValueChanged += Slider_ValueChanged;
                this.Slider2.Value = MaxValue;
            }

            if (this.Slider1 != null)
            {
                this.Slider1.ValueChanged += Slider_ValueChanged;
                this.Slider1.Value = MinValue;
            }

            RangeLength = MaxValue - MinValue;
        }

        /// <summary>
        /// handles the MaxValueProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected virtual void OnMaxValueChanged(double oldValue, double newValue)
        {
            if (Slider2 == null
                || Slider1 == null)
                return;

            if (Slider2.Value > Slider1.Value)
            {
                Slider2.Value = newValue;
            }
            else
            {
                Slider1.Value = newValue;
            }

            RangeLength = MaxValue - MinValue;

            if (MaxValueChanged != null)
            {
                MaxValueChanged(this, new RoutedPropertyChangedEventArgs<double>(oldValue, newValue));
            }
        }

        /// <summary>
        /// handles the MinValueProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected virtual void OnMinValueChanged(double oldValue, double newValue)
        {
            if (Slider2 == null
                || Slider1 == null)
                return;

            if (Slider2.Value < Slider1.Value)
            {
                Slider2.Value = newValue;
            }
            else
            {
                Slider1.Value = newValue;
            }

            RangeLength = MaxValue - MinValue;

            if (MinValueChanged != null)
            {
                MinValueChanged(this, new RoutedPropertyChangedEventArgs<double>(oldValue, newValue));
            }
        }

        /// <summary> 
        /// MaximumProperty property changed handler. 
        /// </summary> 
        /// <param name="d">the max Slider</param> 
        /// <param name="e">DependencyPropertyChangedEventArgs.</param> 
        private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary> 
        /// MinimumProperty property changed handler. 
        /// </summary> 
        /// <param name="d">LowHighRangeBase that changed its Minimum.</param> 
        /// <param name="e">DependencyPropertyChangedEventArgs.</param> 
        private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary> 
        /// OrientationProperty property changed handler. 
        /// </summary> 
        /// <param name="d">the min Slider</param> 
        /// <param name="e">DependencyPropertyChangedEventArgs.</param> 
        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary> 
        /// SmallChangeProperty property changed handler. 
        /// </summary> 
        /// <param name="d">the min Slider</param> 
        /// <param name="e">DependencyPropertyChangedEventArgs.</param> 
        private static void OnSmallChangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// handles the LargeChangeProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnLargeChangeChanged(double oldValue, double newValue)
        {
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Slider1 != null && Slider2 != null)
            {
                MinValue = Math.Min(Slider1.Value, Slider2.Value);
                MaxValue = Math.Max(Slider1.Value, Slider2.Value);
            }
        }

        #endregion Methods
    }
}