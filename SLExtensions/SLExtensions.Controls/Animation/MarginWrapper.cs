// <copyright file="MarginWrapper.cs" company="Ucaya">
// Distributed under Microsoft Public License (Ms-PL)
// </copyright>
// <author>Thierry Bouquain</author>
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

    /// <summary>
    /// Class wrapper for animating a Margin property with a storyboard.
    /// </summary>
    public class MarginWrapper : Panel
    {
        #region Fields

        /// <summary>
        /// MarginBottomPercent depedency property.
        /// </summary>
        public static readonly DependencyProperty MarginBottomPercentProperty = 
            DependencyProperty.Register(
                "MarginBottomPercent",
                typeof(double),
                typeof(MarginWrapper),
                new PropertyMetadata((d, e) => ((MarginWrapper)d).OnMarginBottomPercentChanged((double)e.OldValue, (double)e.NewValue)));

        /// <summary>
        /// MarginBottom depedency property.
        /// </summary>
        public static readonly DependencyProperty MarginBottomProperty = 
            DependencyProperty.Register(
                "MarginBottom",
                typeof(double),
                typeof(MarginWrapper),
                new PropertyMetadata((d, e) => ((MarginWrapper)d).OnMarginBottomChanged((double)e.OldValue, (double)e.NewValue)));

        /// <summary>
        /// MarginHorizontalPercentShift depedency property.
        /// </summary>
        public static readonly DependencyProperty MarginHorizontalPercentShiftProperty = 
            DependencyProperty.Register(
                "MarginHorizontalPercentShift",
                typeof(double),
                typeof(MarginWrapper),
                new PropertyMetadata((d, e) => ((MarginWrapper)d).OnMarginHorizontalPercentShiftChanged((double)e.OldValue, (double)e.NewValue)));

        /// <summary>
        /// MarginHorizontalShift depedency property.
        /// </summary>
        public static readonly DependencyProperty MarginHorizontalShiftProperty = 
            DependencyProperty.Register(
                "MarginHorizontalShift",
                typeof(double),
                typeof(MarginWrapper),
                new PropertyMetadata((d, e) => ((MarginWrapper)d).OnMarginHorizontalShiftChanged((double)e.OldValue, (double)e.NewValue)));

        /// <summary>
        /// MarginLeftPercent depedency property.
        /// </summary>
        public static readonly DependencyProperty MarginLeftPercentProperty = 
            DependencyProperty.Register(
                "MarginLeftPercent",
                typeof(double),
                typeof(MarginWrapper),
                new PropertyMetadata((d, e) => ((MarginWrapper)d).OnMarginLeftPercentChanged((double)e.OldValue, (double)e.NewValue)));

        /// <summary>
        /// MarginLeft depedency property.
        /// </summary>
        public static readonly DependencyProperty MarginLeftProperty = 
            DependencyProperty.Register(
                "MarginLeft",
                typeof(double),
                typeof(MarginWrapper),
                new PropertyMetadata((d, e) => ((MarginWrapper)d).OnMarginLeftChanged((double)e.OldValue, (double)e.NewValue)));

        /// <summary>
        /// MarginRightPercent depedency property.
        /// </summary>
        public static readonly DependencyProperty MarginRightPercentProperty = 
            DependencyProperty.Register(
                "MarginRightPercent",
                typeof(double),
                typeof(MarginWrapper),
                new PropertyMetadata((d, e) => ((MarginWrapper)d).OnMarginRightPercentChanged((double)e.OldValue, (double)e.NewValue)));

        /// <summary>
        /// MarginRight depedency property.
        /// </summary>
        public static readonly DependencyProperty MarginRightProperty = 
            DependencyProperty.Register(
                "MarginRight",
                typeof(double),
                typeof(MarginWrapper),
                new PropertyMetadata((d, e) => ((MarginWrapper)d).OnMarginRightChanged((double)e.OldValue, (double)e.NewValue)));

        /// <summary>
        /// MarginTopPercent depedency property.
        /// </summary>
        public static readonly DependencyProperty MarginTopPercentProperty = 
            DependencyProperty.Register(
                "MarginTopPercent",
                typeof(double),
                typeof(MarginWrapper),
                new PropertyMetadata((d, e) => ((MarginWrapper)d).OnMarginTopPercentChanged((double)e.OldValue, (double)e.NewValue)));

        /// <summary>
        /// MarginTop depedency property.
        /// </summary>
        public static readonly DependencyProperty MarginTopProperty = 
            DependencyProperty.Register(
                "MarginTop",
                typeof(double),
                typeof(MarginWrapper),
                new PropertyMetadata((d, e) => ((MarginWrapper)d).OnMarginTopChanged((double)e.OldValue, (double)e.NewValue)));

        /// <summary>
        /// MarginVerticalPercentShift depedency property.
        /// </summary>
        public static readonly DependencyProperty MarginVerticalPercentShiftProperty = 
            DependencyProperty.Register(
                "MarginVerticalPercentShift",
                typeof(double),
                typeof(MarginWrapper),
                new PropertyMetadata((d, e) => ((MarginWrapper)d).OnMarginVerticalPercentShiftChanged((double)e.OldValue, (double)e.NewValue)));

        /// <summary>
        /// MarginVerticalShift depedency property.
        /// </summary>
        public static readonly DependencyProperty MarginVerticalShiftProperty = 
            DependencyProperty.Register(
                "MarginVerticalShift",
                typeof(double),
                typeof(MarginWrapper),
                new PropertyMetadata((d, e) => ((MarginWrapper)d).OnMarginVerticalShiftChanged((double)e.OldValue, (double)e.NewValue)));

        private bool initialized = false;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MarginWrapper"/> class.
        /// </summary>
        public MarginWrapper()
        {
            // Hide this control. It's just a wrapper and does not need to be shown in the GUI
            this.Visibility = Visibility.Collapsed;
            this.Loaded += this.MarginWrapper_Loaded;
            MarginBottom = double.NaN;
            MarginBottomPercent = double.NaN;
            MarginHorizontalPercentShift = double.NaN;
            MarginHorizontalShift = double.NaN;
            MarginLeft = double.NaN;
            MarginLeftPercent = double.NaN;
            MarginRight = double.NaN;
            MarginRightPercent = double.NaN;
            MarginTop = double.NaN;
            MarginTopPercent = double.NaN;
            MarginVerticalPercentShift = double.NaN;
            MarginVerticalShift = double.NaN;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// The FrameworkElement that will have its margins animated
        /// </summary>
        public FrameworkElement Element
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the FrameworkElement to have its margin animated.
        /// </summary>
        /// <value>The name of the element.</value>
        public string ElementName
        {
            get;
            set;
        }

        public bool ForceOnLoad
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the bottom margin.
        /// </summary>
        /// <value>The bottom margin.</value>
        public double MarginBottom
        {
            get
            {
                return (double)GetValue(MarginBottomProperty);
            }

            set
            {
                SetValue(MarginBottomProperty, value);
            }
        }

        public double MarginBottomPercent
        {
            get
            {
                return (double)GetValue(MarginBottomPercentProperty);
            }

            set
            {
                SetValue(MarginBottomPercentProperty, value);
            }
        }

        public double MarginHorizontalPercentShift
        {
            get
            {
                return (double)GetValue(MarginHorizontalPercentShiftProperty);
            }

            set
            {
                SetValue(MarginHorizontalPercentShiftProperty, value);
            }
        }

        public double MarginHorizontalShift
        {
            get
            {
                return (double)GetValue(MarginHorizontalShiftProperty);
            }

            set
            {
                SetValue(MarginHorizontalShiftProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the left margin.
        /// </summary>
        /// <value>The left margin.</value>
        public double MarginLeft
        {
            get
            {
                return (double)GetValue(MarginLeftProperty);
            }

            set
            {
                SetValue(MarginLeftProperty, value);
            }
        }

        public double MarginLeftPercent
        {
            get
            {
                return (double)GetValue(MarginLeftPercentProperty);
            }

            set
            {
                SetValue(MarginLeftPercentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the right margin.
        /// </summary>
        /// <value>The right margin.</value>
        public double MarginRight
        {
            get
            {
                return (double)GetValue(MarginRightProperty);
            }

            set
            {
                SetValue(MarginRightProperty, value);
            }
        }

        public double MarginRightPercent
        {
            get
            {
                return (double)GetValue(MarginRightPercentProperty);
            }

            set
            {
                SetValue(MarginRightPercentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the top margin.
        /// </summary>
        /// <value>The top margin.</value>
        public double MarginTop
        {
            get
            {
                return (double)GetValue(MarginTopProperty);
            }

            set
            {
                SetValue(MarginTopProperty, value);
            }
        }

        public double MarginTopPercent
        {
            get
            {
                return (double)GetValue(MarginTopPercentProperty);
            }

            set
            {
                SetValue(MarginTopPercentProperty, value);
            }
        }

        public double MarginVerticalPercentShift
        {
            get
            {
                return (double)GetValue(MarginVerticalPercentShiftProperty);
            }

            set
            {
                SetValue(MarginVerticalPercentShiftProperty, value);
            }
        }

        public double MarginVerticalShift
        {
            get
            {
                return (double)GetValue(MarginVerticalShiftProperty);
            }

            set
            {
                SetValue(MarginVerticalShiftProperty, value);
            }
        }

        #endregion Properties

        #region Methods

        void Element_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RefreshPercentMargins();
        }

        /// <summary>
        /// Ensures the FrameworkElement is found.
        /// </summary>
        private void EnsureElement()
        {
            if (string.IsNullOrEmpty(ElementName))
                return;

            if (this.Element == null)
            {
                this.Element = this.FindName(this.ElementName) as FrameworkElement;
            }

            if (!this.initialized && this.Element != null)
            {
                this.initialized = true;

                if (!double.IsNaN(MarginBottom))
                    OnMarginBottomChanged(double.NaN, MarginBottom);

                if (!double.IsNaN(MarginHorizontalShift))
                    OnMarginHorizontalShiftChanged(double.NaN, MarginHorizontalShift);

                if (!double.IsNaN(MarginLeft))
                    OnMarginLeftChanged(double.NaN, MarginLeft);

                if (!double.IsNaN(MarginRight))
                    OnMarginRightChanged(double.NaN, MarginRight);

                if (!double.IsNaN(MarginTop))
                    OnMarginTopChanged(double.NaN, MarginTop);

                if (!double.IsNaN(MarginVerticalShift))
                    OnMarginVerticalShiftChanged(double.NaN, MarginVerticalShift);

                RefreshPercentMargins();

                this.Element.SizeChanged += new SizeChangedEventHandler(Element_SizeChanged);

            }
        }

        /// <summary>
        /// Handles the Loaded event of the MarginWrapper control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void MarginWrapper_Loaded(object sender, RoutedEventArgs e)
        {
            this.EnsureElement();
            RefreshPercentMargins();
        }

        /// <summary>
        /// handles the MarginBottomProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnMarginBottomChanged(double oldValue, double newValue)
        {
            this.EnsureElement();
            if (this.Element != null)
            {

                Thickness margin = this.Element.Margin;

                margin.Bottom = newValue;
                this.Element.Margin = margin;
            }
        }

        /// <summary>
        /// handles the MarginBottomPercentProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnMarginBottomPercentChanged(double oldValue, double newValue)
        {
            this.EnsureElement();
            if (this.Element != null)
            {
                FrameworkElement parentElement = this.Element.Parent as FrameworkElement;
                if (parentElement == null)
                {
                    return;
                }

                Thickness margin = this.Element.Margin;

                margin.Bottom = parentElement.ActualHeight * newValue;
                this.Element.Margin = margin;
            }
        }

        /// <summary>
        /// handles the MarginHorizontalPercentShiftProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnMarginHorizontalPercentShiftChanged(double oldValue, double newValue)
        {
            this.EnsureElement();
            if (this.Element != null)
            {
                FrameworkElement parentElement = this.Element.Parent as FrameworkElement;
                if (parentElement == null)
                {
                    return;
                }

                Thickness margin = this.Element.Margin;
                double newMarginLeft = parentElement.ActualWidth * newValue;
                double delta = newMarginLeft - margin.Left;
                margin.Left = newMarginLeft;
                margin.Right -= delta;
                this.Element.Margin = margin;
            }
        }

        /// <summary>
        /// handles the MarginHorizontalShiftProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnMarginHorizontalShiftChanged(double oldValue, double newValue)
        {
            this.EnsureElement();
            if (this.Element != null)
            {
                FrameworkElement parentElement = this.Element.Parent as FrameworkElement;
                if (parentElement == null)
                {
                    return;
                }

                if (double.IsNaN(oldValue))
                    oldValue = 0;

                double delta = newValue - oldValue;
                Thickness margin = this.Element.Margin;
                margin.Left = newValue;
                margin.Right -= delta;
                this.Element.Margin = margin;
            }
        }

        /// <summary>
        /// handles the MarginLeftProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnMarginLeftChanged(double oldValue, double newValue)
        {
            this.EnsureElement();
            if (this.Element != null)
            {

                Thickness margin = this.Element.Margin;
                margin.Left = newValue;
                this.Element.Margin = margin;
            }
        }

        /// <summary>
        /// handles the MarginLeftPercentProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnMarginLeftPercentChanged(double oldValue, double newValue)
        {
            this.EnsureElement();
            if (this.Element != null)
            {
                FrameworkElement parentElement = this.Element.Parent as FrameworkElement;
                if (parentElement == null)
                {
                    return;
                }

                Thickness margin = this.Element.Margin;

                margin.Left = parentElement.ActualWidth * newValue;
                this.Element.Margin = margin;
            }
        }

        /// <summary>
        /// handles the MarginRightProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnMarginRightChanged(double oldValue, double newValue)
        {
            this.EnsureElement();
            if (this.Element != null)
            {

                Thickness margin = this.Element.Margin;

                margin.Right = newValue;
                this.Element.Margin = margin;
            }
        }

        /// <summary>
        /// handles the MarginRightPercentProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnMarginRightPercentChanged(double oldValue, double newValue)
        {
            this.EnsureElement();
            if (this.Element != null)
            {
                FrameworkElement parentElement = this.Element.Parent as FrameworkElement;
                if (parentElement == null)
                {
                    return;
                }

                Thickness margin = this.Element.Margin;

                margin.Right = parentElement.ActualWidth * newValue;
                this.Element.Margin = margin;
            }
        }

        /// <summary>
        /// handles the MarginTopProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnMarginTopChanged(double oldValue, double newValue)
        {
            this.EnsureElement();
            if (this.Element != null)
            {

                Thickness margin = this.Element.Margin;

                margin.Top = newValue;
                this.Element.Margin = margin;
            }
        }

        /// <summary>
        /// handles the MarginTopPercentProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnMarginTopPercentChanged(double oldValue, double newValue)
        {
            this.EnsureElement();
            if (this.Element != null)
            {
                FrameworkElement parentElement = this.Element.Parent as FrameworkElement;
                if (parentElement == null)
                {
                    return;
                }

                Thickness margin = this.Element.Margin;

                margin.Top = parentElement.ActualHeight * newValue;
                this.Element.Margin = margin;
            }
        }

        /// <summary>
        /// handles the MarginVerticalPercentShiftProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnMarginVerticalPercentShiftChanged(double oldValue, double newValue)
        {
            this.EnsureElement();
            if (this.Element != null)
            {
                FrameworkElement parentElement = this.Element.Parent as FrameworkElement;
                if (parentElement == null)
                {
                    return;
                }

                Thickness margin = this.Element.Margin;
                double newMarginTop = parentElement.ActualHeight * newValue;
                double delta = newMarginTop - margin.Top;
                margin.Top = newMarginTop;
                margin.Bottom -= delta;
                this.Element.Margin = margin;
            }
        }

        /// <summary>
        /// handles the MarginVerticalShiftProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnMarginVerticalShiftChanged(double oldValue, double newValue)
        {
            this.EnsureElement();
            if (this.Element != null)
            {
                FrameworkElement parentElement = this.Element.Parent as FrameworkElement;
                if (parentElement == null)
                {
                    return;
                }

                if (double.IsNaN(oldValue))
                    oldValue = 0;

                double delta = newValue - oldValue;

                Thickness margin = this.Element.Margin;
                margin.Top = newValue;
                margin.Bottom -= delta;
                this.Element.Margin = margin;
            }
        }

        private void RefreshPercentMargins()
        {
            if (!double.IsNaN(MarginBottomPercent))
                OnMarginBottomPercentChanged(double.NaN, MarginBottomPercent);

            if (!double.IsNaN(MarginHorizontalPercentShift))
                OnMarginHorizontalPercentShiftChanged(double.NaN, MarginHorizontalPercentShift);

            if (!double.IsNaN(MarginLeftPercent))
                OnMarginLeftPercentChanged(double.NaN, MarginLeftPercent);

            if (!double.IsNaN(MarginRightPercent))
                OnMarginRightPercentChanged(double.NaN, MarginRightPercent);

            if (!double.IsNaN(MarginTopPercent))
                OnMarginTopPercentChanged(double.NaN, MarginTopPercent);

            if (!double.IsNaN(MarginVerticalPercentShift))
                OnMarginVerticalPercentShiftChanged(double.NaN, MarginVerticalPercentShift);
        }

        #endregion Methods
    }
}