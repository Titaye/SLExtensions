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

    using SLExtensions.Input;

    public class VisualStateCommandListener : Panel, IDisposable
    {
        #region Fields

        /// <summary>
        /// Command depedency property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty = 
            DependencyProperty.Register(
                "Command",
                typeof(string),
                typeof(VisualStateCommandListener),
                new PropertyMetadata((d, e) => ((VisualStateCommandListener)d).OnCommandChanged((string)e.OldValue, (string)e.NewValue)));

        /// <summary>
        /// ElementName depedency property.
        /// </summary>
        public static readonly DependencyProperty ElementNameProperty = 
            DependencyProperty.Register(
                "ElementName",
                typeof(string),
                typeof(VisualStateCommandListener),
                new PropertyMetadata((d, e) => ((VisualStateCommandListener)d).OnElementNameChanged((string)e.OldValue, (string)e.NewValue)));

        /// <summary>
        /// GoToState depedency property.
        /// </summary>
        public static readonly DependencyProperty GoToStateProperty = 
            DependencyProperty.Register(
                "GoToState",
                typeof(string),
                typeof(VisualStateCommandListener),
                new PropertyMetadata((d, e) => ((VisualStateCommandListener)d).OnGoToStateChanged((string)e.OldValue, (string)e.NewValue)));

        private FrameworkElement element;
        private bool isLoaded;

        #endregion Fields

        #region Constructors

        public VisualStateCommandListener()
        {
            this.Visibility = Visibility.Collapsed;
            this.Width = 0;
            this.Height = 0;
            this.Loaded += new RoutedEventHandler(VisualStateCommandListener_Loaded);
        }

        #endregion Constructors

        #region Properties

        public string Command
        {
            get
            {
                return (string)GetValue(CommandProperty);
            }

            set
            {
                SetValue(CommandProperty, value);
            }
        }

        public string ElementName
        {
            get
            {
                return (string)GetValue(ElementNameProperty);
            }

            set
            {
                SetValue(ElementNameProperty, value);
            }
        }

        public string GoToState
        {
            get
            {
                return (string)GetValue(GoToStateProperty);
            }

            set
            {
                SetValue(GoToStateProperty, value);
            }
        }

        #endregion Properties

        #region Methods

        public void Dispose()
        {
            Command = null;
        }

        /// <summary>
        /// handles the CommandProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnCommandChanged(string oldValue, string newValue)
        {
            if (!string.IsNullOrEmpty(oldValue))
            {
                Command cmd = Input.Command.CommandCache.TryGetValue(oldValue);
                if (cmd != null)
                    cmd.Executed -= new EventHandler<ExecutedEventArgs>(cmd_Executed);
            }

            if (!string.IsNullOrEmpty(newValue))
            {
                Command cmd = Input.Command.CommandCache.TryGetValue(newValue);
                if (cmd != null)
                    cmd.Executed += new EventHandler<ExecutedEventArgs>(cmd_Executed);
            }
        }

        /// <summary>
        /// handles the ElementNameProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnElementNameChanged(string oldValue, string newValue)
        {
            if (isLoaded)
                element = FindName(newValue) as FrameworkElement;
        }

        /// <summary>
        /// handles the GoToStateProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnGoToStateChanged(string oldValue, string newValue)
        {
        }

        void VisualStateCommandListener_Loaded(object sender, RoutedEventArgs e)
        {
            isLoaded = true;
            element = FindName(ElementName) as FrameworkElement;
        }

        void cmd_Executed(object sender, ExecutedEventArgs e)
        {
            string stateName = GoToState;
            if (!string.IsNullOrEmpty(stateName) && element != null)
            {
                VisualState.GoToState(element, true, stateName);
            }
        }

        #endregion Methods
    }
}