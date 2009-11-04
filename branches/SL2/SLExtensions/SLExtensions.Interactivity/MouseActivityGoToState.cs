namespace SLExtensions.Interactivity
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
    using System.Windows.Threading;

    using Microsoft.Expression.Interactivity;

    using SLExtensions.Collections.ObjectModel;

    [DefaultTrigger(typeof(UIElement), typeof(Microsoft.Expression.Interactivity.EventTrigger), new object[] { "MouseMove" })]
    public class MouseActivityGoToState : GoToState
    {
        #region Fields

        /// <summary>
        /// ForceShow depedency property.
        /// </summary>
        public static readonly DependencyProperty ForceShowProperty = 
            DependencyProperty.Register(
                "ForceShow",
                typeof(bool),
                typeof(MouseActivityGoToState),
                new PropertyMetadata((d, e) => ((MouseActivityGoToState)d).OnForceShowChanged((bool)e.OldValue, (bool)e.NewValue)));

        /// <summary>
        /// InactivityState depedency property.
        /// </summary>
        public static readonly DependencyProperty InactivityStateProperty = 
            DependencyProperty.Register(
                "InactivityState",
                typeof(string),
                typeof(MouseActivityGoToState),
                new PropertyMetadata((d, e) => ((MouseActivityGoToState)d).OnInactivityStateChanged((string)e.OldValue, (string)e.NewValue)));

        /// <summary>
        /// IsActive depedency property.
        /// </summary>
        public static readonly DependencyProperty IsActiveProperty = 
            DependencyProperty.Register(
                "IsActive",
                typeof(bool),
                typeof(MouseActivityGoToState),
                new PropertyMetadata((d, e) => ((MouseActivityGoToState)d).OnIsActiveChanged((bool)e.OldValue, (bool)e.NewValue)));

        /// <summary>
        /// Timeout depedency property.
        /// </summary>
        public static readonly DependencyProperty TimeoutProperty = 
            DependencyProperty.Register(
                "Timeout",
                typeof(TimeSpan),
                typeof(MouseActivityGoToState),
                new PropertyMetadata((d, e) => ((MouseActivityGoToState)d).OnTimeoutChanged((TimeSpan)e.OldValue, (TimeSpan)e.NewValue)));

        DispatcherTimer timer;

        #endregion Fields

        #region Constructors

        public MouseActivityGoToState()
        {
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timer_Tick);
        }

        #endregion Constructors

        #region Events

        public event EventHandler IsActiveChanged;

        #endregion Events

        #region Properties

        public bool ForceShow
        {
            get
            {
                return (bool)GetValue(ForceShowProperty);
            }

            set
            {
                SetValue(ForceShowProperty, value);
            }
        }

        public string InactivityState
        {
            get
            {
                return (string)GetValue(InactivityStateProperty);
            }

            set
            {
                SetValue(InactivityStateProperty, value);
            }
        }

        public bool IsActive
        {
            get
            {
                return (bool)GetValue(IsActiveProperty);
            }

            set
            {
                SetValue(IsActiveProperty, value);
            }
        }

        public TimeSpan Timeout
        {
            get
            {
                return (TimeSpan)GetValue(TimeoutProperty);
            }

            set
            {
                SetValue(TimeoutProperty, value);
            }
        }

        #endregion Properties

        #region Methods

        protected override void Invoke(object parameter)
        {
            base.Invoke(parameter);
            IsActive = true;
            timer.Start();
        }

        protected virtual void OnIsActiveChanged()
        {
            if (IsActiveChanged != null)
            {
                IsActiveChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// handles the ForceShowProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnForceShowChanged(bool oldValue, bool newValue)
        {
            if (newValue)
            {
                timer.Stop();
                IsActive = true;
            }
            else
                timer.Start();
        }

        /// <summary>
        /// handles the InactivityStateProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnInactivityStateChanged(string oldValue, string newValue)
        {
        }

        /// <summary>
        /// handles the IsActiveProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnIsActiveChanged(bool oldValue, bool newValue)
        {
            if(newValue)
                SLExtensions.Controls.Animation.VisualState.GoToState(Target, true, State);
            else
                SLExtensions.Controls.Animation.VisualState.GoToState(Target, true, InactivityState);

            OnIsActiveChanged();
        }

        /// <summary>
        /// handles the TimeoutProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnTimeoutChanged(TimeSpan oldValue, TimeSpan newValue)
        {
            timer.Interval = newValue;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (!ForceShow)
            {
                timer.Stop();
                IsActive = false;
            }
        }

        #endregion Methods
    }
}