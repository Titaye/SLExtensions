namespace SLExtensions.Interactivity
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Interactivity;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.Windows.Threading;

    [DefaultTrigger(typeof(UIElement), typeof(System.Windows.Interactivity.EventTrigger), new object[] { "MouseMove" })]
    [ContentProperty("Parameters")]
    public class MouseActivityCommand : TargetedTriggerAction<FrameworkElement>
    {
        #region Fields

        /// <summary>
        /// ActivityCommandParameter depedency property.
        /// </summary>
        public static readonly DependencyProperty ActivityCommandParameterProperty = 
            DependencyProperty.Register(
                "ActivityCommandParameter",
                typeof(object),
                typeof(MouseActivityCommand),
                new PropertyMetadata((d, e) => ((MouseActivityCommand)d).OnActivityCommandParameterChanged((object)e.OldValue, (object)e.NewValue)));

        /// <summary>
        /// ActivityCommand depedency property.
        /// </summary>
        public static readonly DependencyProperty ActivityCommandProperty = 
            DependencyProperty.Register(
                "ActivityCommand",
                typeof(ICommand),
                typeof(MouseActivityCommand),
                new PropertyMetadata((d, e) => ((MouseActivityCommand)d).OnActivityCommandChanged((ICommand)e.OldValue, (ICommand)e.NewValue)));

        /// <summary>
        /// ForceShow depedency property.
        /// </summary>
        public static readonly DependencyProperty ForceShowProperty = 
            DependencyProperty.Register(
                "ForceShow",
                typeof(bool),
                typeof(MouseActivityCommand),
                new PropertyMetadata((d, e) => ((MouseActivityCommand)d).OnForceShowChanged((bool)e.OldValue, (bool)e.NewValue)));

        /// <summary>
        /// InactivityCommandParameter depedency property.
        /// </summary>
        public static readonly DependencyProperty InactivityCommandParameterProperty = 
            DependencyProperty.Register(
                "InactivityCommandParameter",
                typeof(object),
                typeof(MouseActivityCommand),
                new PropertyMetadata((d, e) => ((MouseActivityCommand)d).OnInactivityCommandParameterChanged((object)e.OldValue, (object)e.NewValue)));

        /// <summary>
        /// InactivityCommand depedency property.
        /// </summary>
        public static readonly DependencyProperty InactivityCommandProperty = 
            DependencyProperty.Register(
                "InactivityCommand",
                typeof(ICommand),
                typeof(MouseActivityCommand),
                new PropertyMetadata((d, e) => ((MouseActivityCommand)d).OnInactivityCommandChanged((ICommand)e.OldValue, (ICommand)e.NewValue)));

        /// <summary>
        /// IsActive depedency property.
        /// </summary>
        public static readonly DependencyProperty IsActiveProperty = 
            DependencyProperty.Register(
                "IsActive",
                typeof(bool),
                typeof(MouseActivityCommand),
                new PropertyMetadata((d, e) => ((MouseActivityCommand)d).OnIsActiveChanged((bool)e.OldValue, (bool)e.NewValue)));

        /// <summary>
        /// State depedency property.
        /// </summary>
        /// <summary>
        /// Timeout depedency property.
        /// </summary>
        public static readonly DependencyProperty TimeoutProperty = 
            DependencyProperty.Register(
                "Timeout",
                typeof(TimeSpan),
                typeof(MouseActivityCommand),
                new PropertyMetadata((d, e) => ((MouseActivityCommand)d).OnTimeoutChanged((TimeSpan)e.OldValue, (TimeSpan)e.NewValue)));

        private Point lastMousePosition;
        private mouseActivityEventHelper mousePositionEventHelper;
        DispatcherTimer timer;

        #endregion Fields

        #region Constructors

        public MouseActivityCommand()
        {
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timer_Tick);
            Parameters = new List<MouseActivityForceActiveElement>();
        }

        #endregion Constructors

        #region Events

        public event EventHandler Attached;

        public event EventHandler IsActiveChanged;

        #endregion Events

        #region Properties

        public ICommand ActivityCommand
        {
            get
            {
                return (ICommand)GetValue(ActivityCommandProperty);
            }

            set
            {
                SetValue(ActivityCommandProperty, value);
            }
        }

        public object ActivityCommandParameter
        {
            get
            {
                return (object)GetValue(ActivityCommandParameterProperty);
            }

            set
            {
                SetValue(ActivityCommandParameterProperty, value);
            }
        }

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

        public ICommand InactivityCommand
        {
            get
            {
                return (ICommand)GetValue(InactivityCommandProperty);
            }

            set
            {
                SetValue(InactivityCommandProperty, value);
            }
        }

        public object InactivityCommandParameter
        {
            get
            {
                return (object)GetValue(InactivityCommandParameterProperty);
            }

            set
            {
                SetValue(InactivityCommandParameterProperty, value);
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

        public List<MouseActivityForceActiveElement> Parameters
        {
            get; set;
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

        private FrameworkElement StateTarget
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        protected override void Invoke(object parameter)
        {
            IsActive = true;
            timer.Start();
            RefreshState();
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

        protected virtual void OnIsActiveChanged()
        {
            if (IsActiveChanged != null)
            {
                IsActiveChanged(this, EventArgs.Empty);
            }
        }

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

        /// <summary>
        /// handles the ActivityCommandProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnActivityCommandChanged(ICommand oldValue, ICommand newValue)
        {
            RefreshState();
        }

        /// <summary>
        /// handles the ActivityCommandParameterProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnActivityCommandParameterChanged(object oldValue, object newValue)
        {
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
        /// handles the InactivityCommandProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnInactivityCommandChanged(ICommand oldValue, ICommand newValue)
        {
            RefreshState();
        }

        /// <summary>
        /// handles the InactivityCommandParameterProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnInactivityCommandParameterChanged(object oldValue, object newValue)
        {
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
                        if (ActivityCommand != null)
                        {
                            ActivityCommand.Execute(ActivityCommandParameter);
                        }
                    }
                    else
                    {
                        if (InactivityCommand != null)
                        {
                            InactivityCommand.Execute(InactivityCommandParameter);
                        }
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
                var excludedElements = (from ex in Parameters.OfType<MouseActivityForceActiveElement>()
                                        let elem = this.Target.FindName(ex.ElementName)
                                        where elem != null
                                        select elem).ToArray();
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

        #region Nested Types

        private class mouseActivityEventHelper
        {
            #region Fields

            WeakReference mouseActivity;

            #endregion Fields

            #region Constructors

            public mouseActivityEventHelper(MouseActivityCommand mouseActivity)
            {
                this.mouseActivity = new WeakReference(mouseActivity);
                Application.Current.RootVisual.MouseMove += new MouseEventHandler(RootVisual_MouseMove);
            }

            #endregion Constructors

            #region Methods

            void RootVisual_MouseMove(object sender, MouseEventArgs e)
            {
                MouseActivityCommand ma = mouseActivity.Target as MouseActivityCommand;
                if (ma == null)
                {
                    Application.Current.RootVisual.MouseMove -= new MouseEventHandler(RootVisual_MouseMove);
                }
                else
                {
                    ma.lastMousePosition = e.GetPosition(null);
                }
            }

            #endregion Methods
        }

        #endregion Nested Types
    }
}