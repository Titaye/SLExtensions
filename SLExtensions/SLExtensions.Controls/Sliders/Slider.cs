namespace SLExtensions.Controls
{
    using System;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    public class Slider : System.Windows.Controls.Slider
    {
        #region Fields

        /// <summary>
        /// MoveValue depedency property.
        /// </summary>
        public static readonly DependencyProperty MoveValueProperty = 
            DependencyProperty.Register(
                "MoveValue",
                typeof(double),
                typeof(Slider),
                new PropertyMetadata((d, e) => ((Slider)d).OnMoveValueChanged((double)e.OldValue, (double)e.NewValue)));

        private FrameworkElement horizontalTemplate;
        private Thumb horizontalThumb;
        private bool isDragging = false;
        private FrameworkElement verticalTemplate;
        private Thumb verticalThumb;

        #endregion Fields

        #region Constructors

        public Slider()
        {
            Minimum = 0;
            Maximum = 0;
        }

        #endregion Constructors

        #region Events

        public event EventHandler DragCompleted;

        public event EventHandler DragStarted;

        #endregion Events

        #region Properties

        public double MoveValue
        {
            get
            {
                return (double)GetValue(MoveValueProperty);
            }

            set
            {
                SetValue(MoveValueProperty, value);
            }
        }

        #endregion Properties

        #region Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            horizontalThumb = base.GetTemplateChild("HorizontalThumb") as Thumb;
            verticalThumb = base.GetTemplateChild("VerticalThumb") as Thumb;

            horizontalTemplate = base.GetTemplateChild("HorizontalTemplate") as FrameworkElement;
            verticalTemplate = base.GetTemplateChild("VerticalTemplate") as FrameworkElement;

            if (horizontalThumb != null)
            {
                horizontalThumb.DragStarted += new DragStartedEventHandler(horizontalThumb_DragStarted);
                horizontalThumb.DragCompleted += new DragCompletedEventHandler(horizontalThumb_DragCompleted);
            }

            if (verticalThumb != null)
            {
                verticalThumb.DragStarted += new DragStartedEventHandler(verticalThumb_DragStarted);
                verticalThumb.DragCompleted += new DragCompletedEventHandler(verticalThumb_DragCompleted);
            }
        }

        protected virtual void OnDragCompleted()
        {
            if (DragCompleted != null)
            {
                DragCompleted(this, EventArgs.Empty);
            }
        }

        protected virtual void OnDragStarted()
        {
            if (DragStarted != null)
            {
                DragStarted(this, EventArgs.Empty);
            }
        }

        protected override void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
            base.OnMaximumChanged(oldMaximum, newMaximum);
        }

        protected override void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
            base.OnMinimumChanged(oldMinimum, newMinimum);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (this.Orientation == Orientation.Horizontal && horizontalTemplate != null)
            {
                var percent = e.GetPosition(horizontalTemplate).X / horizontalTemplate.ActualWidth;
                if (percent >= 0 && percent <= 1)
                {
                    var value = (Maximum - Minimum) * percent + Minimum;
                    MoveValue = value;
                    e.Handled = true;
                }
            }
            else if (this.Orientation == Orientation.Vertical && verticalTemplate != null)
            {
                var percent = e.GetPosition(verticalTemplate).Y / verticalTemplate.ActualHeight;
                if (percent >= 0 && percent <= 1)
                {
                    var value = (Maximum - Minimum) * percent + Minimum;
                    MoveValue = value;
                }
            }

            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);
        }

        void horizontalThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            isDragging = false;
            MoveValue = Value;
            OnDragCompleted();
        }

        void horizontalThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            isDragging = true;
            OnDragStarted();
        }

        /// <summary>
        /// handles the MoveValueProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnMoveValueChanged(double oldValue, double newValue)
        {
            if (!isDragging)
                Value = newValue;
        }

        void verticalThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            isDragging = false;
            MoveValue = Value;
            OnDragCompleted();
        }

        void verticalThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            isDragging = true;
            OnDragStarted();
        }

        #endregion Methods
    }
}