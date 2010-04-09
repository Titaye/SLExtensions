#region Header

// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

#endregion Header

namespace SLExtensions.Controls.Primitives
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Automation.Peers;
    using System.Windows.Controls;

    /// <summary> 
    /// Represents the base class for elements that have a specific range. 
    /// Examples of such elements are ScrollBar, ProgressBar and Slider.  This
    /// class defines the relevant events and properties, and provides handlers 
    /// for the events.
    /// </summary>
    public abstract class Range2DBase : Control
    {
        #region Fields

        /// <summary>
        /// Identifies the LargeChange dependency property. 
        /// </summary> 
        public static readonly DependencyProperty LargeChangeXProperty = 
            DependencyProperty.Register(
                "LargeChangeX",
                typeof(double),
                typeof(Range2DBase),
                new PropertyMetadata(1.0d, OnLargeChangeXPropertyChanged));

        /// <summary>
        /// Identifies the LargeChange dependency property. 
        /// </summary> 
        public static readonly DependencyProperty LargeChangeYProperty = 
            DependencyProperty.Register(
                "LargeChangeY",
                typeof(double),
                typeof(Range2DBase),
                new PropertyMetadata(1.0d, OnLargeChangeYPropertyChanged));

        /// <summary> 
        /// Identifies the MaximumX dependency property.
        /// </summary>
        public static readonly DependencyProperty MaximumXProperty = 
            DependencyProperty.Register(
                "MaximumX",
                typeof(double),
                typeof(Range2DBase),
                new PropertyMetadata(1.0d, OnMaximumXPropertyChanged));

        /// <summary> 
        /// Identifies the MaximumY dependency property.
        /// </summary>
        public static readonly DependencyProperty MaximumYProperty = 
            DependencyProperty.Register(
                "MaximumY",
                typeof(double),
                typeof(Range2DBase),
                new PropertyMetadata(1.0d, OnMaximumYPropertyChanged));

        /// <summary> 
        /// Identifies the MinimumX dependency property.
        /// </summary>
        public static readonly DependencyProperty MinimumXProperty = 
            DependencyProperty.Register(
                "MinimumX",
                typeof(double),
                typeof(Range2DBase),
                new PropertyMetadata(0.0d, OnMinimumXPropertyChanged));

        /// <summary> 
        /// Identifies the MinimumY dependency property.
        /// </summary>
        public static readonly DependencyProperty MinimumYProperty = 
            DependencyProperty.Register(
                "MinimumY",
                typeof(double),
                typeof(Range2DBase),
                new PropertyMetadata(0.0d, OnMinimumYPropertyChanged));

        /// <summary>
        /// Identifies the SmallChange dependency property. 
        /// </summary>
        public static readonly DependencyProperty SmallChangeXProperty = 
            DependencyProperty.Register(
                "SmallChangeX",
                typeof(double),
                typeof(Range2DBase),
                new PropertyMetadata(0.1d, OnSmallChangeXPropertyChanged));

        /// <summary>
        /// Identifies the SmallChange dependency property. 
        /// </summary>
        public static readonly DependencyProperty SmallChangeYProperty = 
            DependencyProperty.Register(
                "SmallChangeY",
                typeof(double),
                typeof(Range2DBase),
                new PropertyMetadata(0.1d, OnSmallChangeYPropertyChanged));

        /// <summary> 
        /// Identifies the Value dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueXProperty = 
            DependencyProperty.Register(
                "ValueX",
                typeof(double),
                typeof(Range2DBase),
                new PropertyMetadata(0.0d, OnValueXPropertyChanged));

        /// <summary> 
        /// Identifies the Value dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueYProperty = 
            DependencyProperty.Register(
                "ValueY",
                typeof(double),
                typeof(Range2DBase),
                new PropertyMetadata(0.0d, OnValueYPropertyChanged));

        internal double _initialMaxX;
        internal double _initialMaxY;
        internal double _initialValX;
        internal double _initialValY;

        /// This section contains static helper variables used to keep track 
        /// of which variable values have been updated/coerced, and when to 
        /// call the corresponding property changed methods.
        internal int _levelsFromRootCall;
        internal double _requestedMaxX;
        internal double _requestedMaxY;
        internal double _requestedValX;
        internal double _requestedValY;

        /// <summary>
        /// Format string for Range2DBase
        /// </summary> 
        private const string FormatString = "{0} MinimumX:{1} MaximumX:{2} ValueX:{3} MinimumY:{4} MaximumY:{5} ValueY:{6}";

        #endregion Fields

        #region Constructors

        /// <summary> 
        /// Initializes a new instance of the Range2DBase class.
        /// </summary>
        protected Range2DBase()
        {
        }

        #endregion Constructors

        #region Events

        /// <summary>
        /// Occurs when the range value changes. 
        /// </summary> 
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly", Justification = "Compat with WPF.")]
        public event RoutedPropertyChangedEventHandler<double> ValueXChanged;

        /// <summary>
        /// Occurs when the range value changes. 
        /// </summary> 
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly", Justification = "Compat with WPF.")]
        public event RoutedPropertyChangedEventHandler<double> ValueYChanged;

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets or sets a value to be added to or subtracted from the Value of
        /// a Range2DBase control.  The default values is one. 
        /// </summary> 
        public double LargeChangeX
        {
            get { return (double)GetValue(LargeChangeXProperty); }
            set { SetValue(LargeChangeXProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value to be added to or subtracted from the Value of
        /// a Range2DBase control.  The default values is one. 
        /// </summary> 
        public double LargeChangeY
        {
            get { return (double)GetValue(LargeChangeYProperty); }
            set { SetValue(LargeChangeYProperty, value); }
        }

        /// <summary>
        /// Gets or sets the MaximumX possible Value of the range element. 
        /// The default values is one.
        /// </summary>
        public double MaximumX
        {
            get { return (double)GetValue(MaximumXProperty); }
            set { SetValue(MaximumXProperty, value); }
        }

        /// <summary>
        /// Gets or sets the MaximumY possible Value of the range element. 
        /// The default values is one.
        /// </summary>
        public double MaximumY
        {
            get { return (double)GetValue(MaximumYProperty); }
            set { SetValue(MaximumYProperty, value); }
        }

        /// <summary>
        /// Gets or sets the MinimumX possible Value of the range element.  The 
        /// default value is zero.
        /// </summary>
        public double MinimumX
        {
            get { return (double)GetValue(MinimumXProperty); }
            set { SetValue(MinimumXProperty, value); }
        }

        /// <summary>
        /// Gets or sets the MinimumY possible Value of the range element.  The 
        /// default value is zero.
        /// </summary>
        public double MinimumY
        {
            get { return (double)GetValue(MinimumYProperty); }
            set { SetValue(MinimumYProperty, value); }
        }

        /// <summary> 
        /// Gets or sets a value to be added to or subtracted from the Value of
        /// a Range2DBase control.  The default values is 0.1. 
        /// </summary>
        public double SmallChangeX
        {
            get { return (double)GetValue(SmallChangeXProperty); }
            set { SetValue(SmallChangeXProperty, value); }
        }

        /// <summary> 
        /// Gets or sets a value to be added to or subtracted from the Value of
        /// a Range2DBase control.  The default values is 0.1. 
        /// </summary>
        public double SmallChangeY
        {
            get { return (double)GetValue(SmallChangeYProperty); }
            set { SetValue(SmallChangeYProperty, value); }
        }

        /// <summary>
        /// Gets or sets the current Value of the range element.  The default
        /// value is zero. 
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "Compatibility with WPF")]
        public double ValueX
        {
            get { return (double)GetValue(ValueXProperty); }
            set { SetValue(ValueXProperty, value); }
        }

        /// <summary>
        /// Gets or sets the current Value of the range element.  The default
        /// value is zero. 
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "Compatibility with WPF")]
        public double ValueY
        {
            get { return (double)GetValue(ValueYProperty); }
            set { SetValue(ValueYProperty, value); }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Provides a string representation of a Range2DBase object. 
        /// </summary> 
        /// <returns>
        /// Returns the string representation of a Range2DBase object. 
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, FormatString, base.ToString(), MinimumX, MaximumX, ValueX, MinimumY, MaximumY, ValueY);
        }

        internal bool GoToState(bool useTransitions, string stateName)
        {
            Debug.Assert(stateName != null);
            return VisualStateManager.GoToState(this, stateName, useTransitions);
        }

        /// <summary>
        /// Called when the MaximumX property changes.
        /// </summary> 
        /// <param name="oldMaximumX">Old value of the MaximumX property.</param>
        /// <param name="newMaximumX">New value of the MaximumX property.</param>
        protected virtual void OnMaximumXChanged(double oldMaximumX, double newMaximumX)
        {
        }

        /// <summary>
        /// Called when the MaximumY property changes.
        /// </summary> 
        /// <param name="oldMaximumY">Old value of the MaximumY property.</param>
        /// <param name="newMaximumY">New value of the MaximumY property.</param>
        protected virtual void OnMaximumYChanged(double oldMaximumY, double newMaximumY)
        {
        }

        /// <summary>
        /// Called when the MinimumX property changes.
        /// </summary> 
        /// <param name="oldMinimumX">Old value of the MinimumX property.</param>
        /// <param name="newMinimumX">New value of the MinimumX property.</param>
        protected virtual void OnMinimumXChanged(double oldMinimumX, double newMinimumX)
        {
        }

        /// <summary>
        /// Called when the MinimumY property changes.
        /// </summary> 
        /// <param name="oldMinimumY">Old value of the MinimumY property.</param>
        /// <param name="newMinimumY">New value of the MinimumY property.</param>
        protected virtual void OnMinimumYChanged(double oldMinimumY, double newMinimumY)
        {
        }

        /// <summary>
        /// Called when the Value property changes.
        /// </summary> 
        /// <param name="oldValue">Old value of the Value property.</param>
        /// <param name="newValue">New value of the Value property.</param>
        protected virtual void OnValueXChanged(double oldValue, double newValue)
        {
            RoutedPropertyChangedEventHandler<double> handler = ValueXChanged;
            if (handler != null)
            {
                handler(this, new RoutedPropertyChangedEventArgs<double>(oldValue, newValue));
            }
        }

        /// <summary>
        /// Called when the Value property changes.
        /// </summary> 
        /// <param name="oldValue">Old value of the Value property.</param>
        /// <param name="newValue">New value of the Value property.</param>
        protected virtual void OnValueYChanged(double oldValue, double newValue)
        {
            RoutedPropertyChangedEventHandler<double> handler = ValueYChanged;
            if (handler != null)
            {
                handler(this, new RoutedPropertyChangedEventArgs<double>(oldValue, newValue));
            }
        }

        /// <summary>
        /// Check if a value is a valid change for the two change properties.
        /// </summary> 
        /// <param name="value">Value.</param> 
        /// <returns>true if a valid value; false otherwise.</returns>
        private static bool IsValidChange(object value)
        {
            return IsValidDoubleValue(value) && (((double)value) >= 0);
        }

        /// <summary> 
        /// Check if a value is a value double. 
        /// </summary>
        /// <param name="value">Value.</param> 
        /// <returns>true if a valid double; false otherwise.</returns>
        /// <remarks>
        /// This method is set to private, and is only expected to be 
        /// called from our property changed handlers.
        /// </remarks>
        private static bool IsValidDoubleValue(object value)
        {
            Debug.Assert(typeof(double).IsInstanceOfType(value));
            double number = (double)value;
            return !double.IsNaN(number) && !double.IsInfinity(number);
        }

        /// <summary> 
        /// LargeChangeProperty property changed handler. 
        /// </summary>
        /// <param name="d">Range2DBase that changed its LargeChange.</param> 
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnLargeChangeXPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Range2DBase range = d as Range2DBase;
            Debug.Assert(range != null);

            // Ensure it's a valid value
            if (!IsValidChange(e.NewValue))
            {
                throw new ArgumentException();
            }
        }

        /// <summary> 
        /// LargeChangeProperty property changed handler. 
        /// </summary>
        /// <param name="d">Range2DBase that changed its LargeChange.</param> 
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnLargeChangeYPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Range2DBase range = d as Range2DBase;
            Debug.Assert(range != null);

            // Ensure it's a valid value
            if (!IsValidChange(e.NewValue))
            {
                throw new ArgumentException();
            }
        }

        /// <summary>
        /// MaximumXProperty property changed handler.
        /// </summary> 
        /// <param name="d">Range2DBase that changed its MaximumX.</param> 
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnMaximumXPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Range2DBase range = d as Range2DBase;
            Debug.Assert(range != null);

            // Ensure it's a valid value
            if (!IsValidDoubleValue(e.NewValue))
            {
                throw new ArgumentException();
            }

            // Note: this section is a workaround, containing my
            // logic to hold all calls to the property changed
            // methods until after all coercion has completed
            // ----------
            if (range._levelsFromRootCall == 0)
            {
                range._requestedMaxX = (double)e.NewValue;
                range._initialMaxX = (double)e.OldValue;
                range._initialValX = range.ValueX;
            }
            range._levelsFromRootCall++;
            // ----------

            range.CoerceMaximumX();
            range.CoerceValueX();

            // Note: this section completes my workaround to call
            // the property changed logic if all coercion has completed
            // ----------
            range._levelsFromRootCall--;
            if (range._levelsFromRootCall == 0)
            {
                double maximumX = range.MaximumX;
                if (range._initialMaxX != maximumX)
                {
                    range.OnMaximumXChanged(range._initialMaxX, maximumX);
                }
                double value = range.ValueX;
                if (range._initialValX != value)
                {
                    range.OnValueXChanged(range._initialValX, value);
                }
            }
            // ----------
        }

        /// <summary>
        /// MaximumYProperty property changed handler.
        /// </summary> 
        /// <param name="d">Range2DBase that changed its MaximumY.</param> 
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnMaximumYPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Range2DBase range = d as Range2DBase;
            Debug.Assert(range != null);

            // Ensure it's a valid value
            if (!IsValidDoubleValue(e.NewValue))
            {
                throw new ArgumentException();
            }

            // Note: this section is a workaround, containing my
            // logic to hold all calls to the property changed
            // methods until after all coercion has completed
            // ----------
            if (range._levelsFromRootCall == 0)
            {
                range._requestedMaxY = (double)e.NewValue;
                range._initialMaxY = (double)e.OldValue;
                range._initialValY = range.ValueY;
            }
            range._levelsFromRootCall++;
            // ----------

            range.CoerceMaximumY();
            range.CoerceValueY();

            // Note: this section completes my workaround to call
            // the property changed logic if all coercion has completed
            // ----------
            range._levelsFromRootCall--;
            if (range._levelsFromRootCall == 0)
            {
                double maximumY = range.MaximumY;
                if (range._initialMaxY != maximumY)
                {
                    range.OnMaximumYChanged(range._initialMaxY, maximumY);
                }
                double value = range.ValueY;
                if (range._initialValY != value)
                {
                    range.OnValueYChanged(range._initialValY, value);
                }
            }
            // ----------
        }

        /// <summary>
        /// MinimumXProperty property changed handler.
        /// </summary> 
        /// <param name="d">Range2DBase that changed its MinimumX.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        /// 
        private static void OnMinimumXPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Range2DBase range = d as Range2DBase;
            Debug.Assert(range != null);

            // Ensure it's a valid value
            if (!IsValidDoubleValue(e.NewValue))
            {
                throw new ArgumentException();
            }

            // Note: this section is a workaround, containing my
            // logic to hold all calls to the property changed
            // methods until after all coercion has completed
            // ----------
            if (range._levelsFromRootCall == 0)
            {
                range._initialMaxX = range.MaximumX;
                range._initialValX = range.ValueX;
            }
            range._levelsFromRootCall++;
            // ----------

            range.CoerceMaximumX();
            range.CoerceValueX();

            // Note: this section completes my workaround to call
            // the property changed logic if all coercion has completed
            // ----------
            range._levelsFromRootCall--;
            if (range._levelsFromRootCall == 0)
            {
                range.OnMinimumXChanged((double)e.OldValue, (double)e.NewValue);
                double maximumX = range.MaximumX;
                if (range._initialMaxX != maximumX)
                {
                    range.OnMaximumXChanged(range._initialMaxX, maximumX);
                }
                double value = range.ValueX;
                if (range._initialValX != value)
                {
                    range.OnValueXChanged(range._initialValX, value);
                }
            }
            // ----------
        }

        /// <summary>
        /// MinimumYProperty property changed handler.
        /// </summary> 
        /// <param name="d">Range2DBase that changed its MinimumY.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        /// 
        private static void OnMinimumYPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Range2DBase range = d as Range2DBase;
            Debug.Assert(range != null);

            // Ensure it's a valid value
            if (!IsValidDoubleValue(e.NewValue))
            {
                throw new ArgumentException();
            }

            // Note: this section is a workaround, containing my
            // logic to hold all calls to the property changed
            // methods until after all coercion has completed
            // ----------
            if (range._levelsFromRootCall == 0)
            {
                range._initialMaxY = range.MaximumY;
                range._initialValY = range.ValueY;
            }
            range._levelsFromRootCall++;
            // ----------

            range.CoerceMaximumY();
            range.CoerceValueY();

            // Note: this section completes my workaround to call
            // the property changed logic if all coercion has completed
            // ----------
            range._levelsFromRootCall--;
            if (range._levelsFromRootCall == 0)
            {
                range.OnMinimumYChanged((double)e.OldValue, (double)e.NewValue);
                double maximumY = range.MaximumY;
                if (range._initialMaxY != maximumY)
                {
                    range.OnMaximumYChanged(range._initialMaxY, maximumY);
                }
                double value = range.ValueY;
                if (range._initialValY != value)
                {
                    range.OnValueYChanged(range._initialValY, value);
                }
            }
            // ----------
        }

        /// <summary> 
        /// SmallChangeProperty property changed handler.
        /// </summary>
        /// <param name="d">Range2DBase that changed its SmallChange.</param> 
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnSmallChangeXPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Range2DBase range = d as Range2DBase;
            Debug.Assert(range != null);

            // Ensure it's a valid value
            if (!IsValidChange(e.NewValue))
            {
                throw new ArgumentException();
            }
        }

        /// <summary> 
        /// SmallChangeProperty property changed handler.
        /// </summary>
        /// <param name="d">Range2DBase that changed its SmallChange.</param> 
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnSmallChangeYPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Range2DBase range = d as Range2DBase;
            Debug.Assert(range != null);

            // Ensure it's a valid value
            if (!IsValidChange(e.NewValue))
            {
                throw new ArgumentException();
            }
        }

        /// <summary>
        /// ValueProperty property changed handler.
        /// </summary> 
        /// <param name="d">Range2DBase that changed its Value.</param> 
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnValueXPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Range2DBase range = d as Range2DBase;
            Debug.Assert(range != null);

            // Ensure it's a valid value
            if (!IsValidDoubleValue(e.NewValue))
            {
                throw new ArgumentException();
            }

            // Note: this section is a workaround, containing my
            // logic to hold all calls to the property changed
            // methods until after all coercion has completed
            // ----------
            if (range._levelsFromRootCall == 0)
            {
                range._requestedValX = (double)e.NewValue;
                range._initialValX = (double)e.OldValue;
            }
            range._levelsFromRootCall++;
            // ----------

            range.CoerceValueX();

            // Note: this section completes my workaround to call
            // the property changed logic if all coercion has completed
            // ----------
            range._levelsFromRootCall--;
            if (range._levelsFromRootCall == 0)
            {
                double value = range.ValueX;
                if (range._initialValX != value)
                {
                    range.OnValueXChanged(range._initialValX, value);
                }
            }
            // ----------
        }

        /// <summary>
        /// ValueProperty property changed handler.
        /// </summary> 
        /// <param name="d">Range2DBase that changed its Value.</param> 
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnValueYPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Range2DBase range = d as Range2DBase;
            Debug.Assert(range != null);

            // Ensure it's a valid value
            if (!IsValidDoubleValue(e.NewValue))
            {
                throw new ArgumentException();
            }

            // Note: this section is a workaround, containing my
            // logic to hold all calls to the property changed
            // methods until after all coercion has completed
            // ----------
            if (range._levelsFromRootCall == 0)
            {
                range._requestedValY = (double)e.NewValue;
                range._initialValY = (double)e.OldValue;
            }
            range._levelsFromRootCall++;
            // ----------

            range.CoerceValueY();

            // Note: this section completes my workaround to call
            // the property changed logic if all coercion has completed
            // ----------
            range._levelsFromRootCall--;
            if (range._levelsFromRootCall == 0)
            {
                double value = range.ValueY;
                if (range._initialValY != value)
                {
                    range.OnValueYChanged(range._initialValY, value);
                }
            }
            // ----------
        }

        /// <summary> 
        /// Ensure the MaximumX is greater than or equal to the minimumX.
        /// </summary> 
        private void CoerceMaximumX()
        {
            double minimumX = MinimumX;
            double maximumX = MaximumX;
            if (_requestedMaxX != maximumX && _requestedMaxX >= minimumX)
            {
                SetValue(MaximumXProperty, _requestedMaxX);
            }
            else if (maximumX < minimumX)
            {
                SetValue(MaximumXProperty, minimumX);
            }
        }

        /// <summary> 
        /// Ensure the MaximumY is greater than or equal to the minimumY.
        /// </summary> 
        private void CoerceMaximumY()
        {
            double minimumY = MinimumY;
            double maximumY = MaximumY;
            if (_requestedMaxY != maximumY && _requestedMaxY >= minimumY)
            {
                SetValue(MaximumYProperty, _requestedMaxY);
            }
            else if (maximumY < minimumY)
            {
                SetValue(MaximumYProperty, minimumY);
            }
        }

        /// <summary> 
        /// Ensure the value falls between the MinimumX and MaximumX values. 
        /// This function assumes that (MaximumX >= MinimumX)
        /// </summary> 
        private void CoerceValueX()
        {
            double minimumX = MinimumX;
            double maximumX = MaximumX;
            double value = ValueX;

            if (_requestedValX != value && _requestedValX >= minimumX && _requestedValX <= maximumX)
            {
                SetValue(ValueXProperty, _requestedValX);
            }
            else
            {
                if (value < minimumX)
                {
                    SetValue(ValueXProperty, minimumX);
                }
                if (value > maximumX)
                {
                    SetValue(ValueXProperty, maximumX);
                }
            }
        }

        /// <summary> 
        /// Ensure the value falls between the MinimumY and MaximumY values. 
        /// This function assumes that (MaximumY >= MinimumY)
        /// </summary> 
        private void CoerceValueY()
        {
            double minimumY = MinimumY;
            double maximumY = MaximumY;
            double value = ValueY;

            if (_requestedValY != value && _requestedValY >= minimumY && _requestedValY <= maximumY)
            {
                SetValue(ValueYProperty, _requestedValY);
            }
            else
            {
                if (value < minimumY)
                {
                    SetValue(ValueYProperty, minimumY);
                }
                if (value > maximumY)
                {
                    SetValue(ValueYProperty, maximumY);
                }
            }
        }

        #endregion Methods
    }
}