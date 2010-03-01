namespace SLExtensions.Controls
{
    using System;
    using System.Diagnostics;
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

    public class Timeline : RangeBase
    {
        #region Fields

        /// <summary>
        /// Duration depedency property.
        /// </summary>
        private static readonly DependencyProperty DurationProperty = 
            DependencyProperty.Register(
                "Duration",
                typeof(double),
                typeof(Timeline),
                new PropertyMetadata((d, e) => ((Timeline)d).OnDurationChanged((double)e.OldValue, (double)e.NewValue)));

        /// <summary>
        /// EllapsedTime depedency property.
        /// </summary>
        private static readonly DependencyProperty EllapsedTimeProperty = 
            DependencyProperty.Register(
                "EllapsedTime",
                typeof(double),
                typeof(Timeline),
                null);

        private double distance = 0;
        private bool mouseCaptured;
        private Point mouseDownPoint;
        private double mouseDownPosition;

        #endregion Fields

        #region Constructors

        public Timeline()
        {
            DefaultStyleKey = typeof(Timeline);
        }

        #endregion Constructors

        #region Events

        public event EventHandler DragCompleted;

        public event EventHandler DragStarted;

        #endregion Events

        #region Properties

        public double Duration
        {
            get
            {
                return (double)GetValue(DurationProperty);
            }

            private set
            {
                SetValue(DurationProperty, value);
            }
        }

        public double EllapsedTime
        {
            get
            {
                return (double)GetValue(EllapsedTimeProperty);
            }

            private set
            {
                SetValue(EllapsedTimeProperty, value);
            }
        }

        #endregion Properties

        #region Methods

        protected virtual void OnDragCompleted()
        {
            RefreshEllapsedTime();
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
            RefreshDuration();
        }

        protected override void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
            base.OnMinimumChanged(oldMinimum, newMinimum);

            RefreshDuration();
            RefreshEllapsedTime();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            mouseCaptured = this.CaptureMouse();
            if (mouseCaptured)
                OnDragStarted();

            mouseDownPoint = e.GetPosition(this);
            mouseDownPosition = Value;
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            if (mouseCaptured)
            {
                var pos = e.GetPosition(this);
                distance = Math.Abs(pos.Y);
                mouseCaptured = false;
                if (distance < 30)
                {
                    var delta = (pos.X - mouseDownPoint.X);
                    var ellapsedTime = Math.Min(Duration, Math.Max(0, mouseDownPosition - delta - Minimum));
                    Value = Minimum + ellapsedTime;
                    OnDragCompleted();
                }
            }

            this.ReleaseMouseCapture();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!mouseCaptured)
                return;
            var pos = e.GetPosition(this);
            distance = Math.Abs(pos.Y);

            if (distance > 30)
            {
                RefreshEllapsedTime();
            }
            else
            {
                var delta = (pos.X - mouseDownPoint.X);
                EllapsedTime = Math.Min(Duration, Math.Max(0, mouseDownPosition - delta - Minimum));
            }
        }

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);
            RefreshEllapsedTime();
        }

        /// <summary>
        /// handles the DurationProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnDurationChanged(double oldValue, double newValue)
        {
        }

        private void RefreshDuration()
        {
            Duration = Maximum - Minimum;
        }

        private void RefreshEllapsedTime()
        {
            if (!mouseCaptured
               || distance > 30)
            {
                EllapsedTime = Value - Minimum;
            }
        }

        #endregion Methods
    }
}