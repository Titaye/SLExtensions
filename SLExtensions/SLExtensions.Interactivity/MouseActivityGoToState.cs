namespace SLExtensions.Interactivity
{
    using System;
    using System.Net;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Interactivity;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.Windows.Threading;
    using System.Collections.ObjectModel;
    using System.Windows.Markup;
    using System.Collections.Generic;

    [DefaultTrigger(typeof(UIElement), typeof(System.Windows.Interactivity.EventTrigger), new object[] { "MouseMove" })]
    [ContentProperty("Exclusions")]
    public class MouseActivityGoToState : TargetedTriggerAction<FrameworkElement>
    {
        private class mouseActivityEventHelper
        {
            WeakReference mouseActivity;
            public mouseActivityEventHelper(MouseActivityGoToState mouseActivity)
            {
                this.mouseActivity = new WeakReference(mouseActivity);
                Application.Current.RootVisual.MouseMove += new MouseEventHandler(RootVisual_MouseMove);
            }

            void RootVisual_MouseMove(object sender, MouseEventArgs e)
            {
                MouseActivityGoToState ma = mouseActivity.Target as MouseActivityGoToState;
                if (ma == null)
                {
                    Application.Current.RootVisual.MouseMove -= new MouseEventHandler(RootVisual_MouseMove);
                }
                else
                {
                    ma.lastMousePosition = e.GetPosition(null);
                }
            }

        }

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
                null);

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
        /// State depedency property.
        /// </summary>
        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register(
                "State",
                typeof(string),
                typeof(MouseActivityGoToState),
                null);

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

        private FrameworkElement StateTarget { get; set; }

        protected override void OnTargetChanged(FrameworkElement oldTarget, FrameworkElement newTarget)
        {
            base.OnTargetChanged(oldTarget, newTarget);
            FrameworkElement target = null;
            if (!string.IsNullOrEmpty(base.TargetName))
            {
                target = base.Target;
            }
            else
            {
                target = base.AssociatedObject as FrameworkElement;
                if (target == null)
                {
                    this.StateTarget = null;
                    return;
                }
                for (FrameworkElement element2 = target.Parent as FrameworkElement; element2 != null; element2 = element2.Parent as FrameworkElement)
                {
                    if (element2 is UserControl)
                    {
                        break;
                    }
                    if (element2.Parent == null)
                    {
                        FrameworkElement parent = VisualTreeHelper.GetParent(element2) as FrameworkElement;
                        if ((parent == null) || (!(parent is Control) && !(parent is ContentPresenter)))
                        {
                            break;
                        }
                    }
                    target = element2;
                }
                if (VisualStateManager.GetVisualStateGroups(target).Count != 0)
                {
                    FrameworkElement element4 = VisualTreeHelper.GetParent(target) as FrameworkElement;
                    if ((element4 != null) && (element4 is Control))
                    {
                        target = element4;
                    }
                }
                else
                {
                    this.StateTarget = null;                    
                }
            }
            this.StateTarget = target;

            if (Target != null)
                Target.Loaded += Target_Loaded;
            if (oldTarget != null)
                oldTarget.Loaded -= Target_Loaded;

            RefreshState();
        }


        private Point lastMousePosition;

        #region Constructors
        private mouseActivityEventHelper mousePositionEventHelper;
        public MouseActivityGoToState()
        {
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timer_Tick);
            Exclusions = new List<MouseActivityExclusion>();
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            if (Application.Current != null && Application.Current.RootVisual != null)
            {
                mousePositionEventHelper = new mouseActivityEventHelper(this);
            }
            else
            {
                this.Target.Loaded += delegate { mousePositionEventHelper = new mouseActivityEventHelper(this); };
            }

            if (Attached != null)
            {
                Attached(this, EventArgs.Empty);
            }
        }

        #endregion Constructors

        #region Events

        public event EventHandler IsActiveChanged;

        #endregion Events

        public event EventHandler Attached;


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

        public string State
        {
            get
            {
                return (string)GetValue(StateProperty);
            }

            set
            {
                SetValue(StateProperty, value);
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
            IsActive = true;
            timer.Start();
            RefreshState();
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
            RefreshState();

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

        private void RefreshState()
        {
            Control stateTarget = this.StateTarget as Control;
            if (stateTarget != null)
            {
                stateTarget.ApplyTemplate();
                if (Target != null)
                {
                    if (IsActive)
                    {
                        VisualStateManager.GoToState(stateTarget, State, true);
                    }
                    else
                    {
                        VisualStateManager.GoToState(stateTarget, InactivityState, true);
                    }
                }
            }
        }

        void Target_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshState();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (!ForceShow)
            {
                var excludedElements = (from ex in Exclusions
                                        where ex.Element != null
                                        select ex.Element).ToArray();
                if (excludedElements.Any()
                    && VisualTreeHelper.FindElementsInHostCoordinates(lastMousePosition, Application.Current.RootVisual).Any(_ => excludedElements.Contains(_)))
                {
                    //Exclusion found, keep active
                    IsActive = true;
                    return;
                }

                timer.Stop();
                IsActive = false;
            }
        }

        #endregion Methods

        public List<MouseActivityExclusion> Exclusions { get; set; }
    }
}