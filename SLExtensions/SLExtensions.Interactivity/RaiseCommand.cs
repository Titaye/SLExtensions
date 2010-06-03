namespace SLExtensions.Interactivity
{
    using System;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Interactivity;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    [DefaultTrigger(typeof(ButtonBase), typeof(System.Windows.Interactivity.EventTrigger), new object[] { "Click" })]
    [DefaultTrigger(typeof(UIElement), typeof(System.Windows.Interactivity.EventTrigger), new object[] { "MouseLeftButtonDown" })]
    [DefaultTrigger(typeof(TextBox), typeof(System.Windows.Interactivity.EventTrigger), new object[] { "KeyDown" })]
    public class RaiseCommand : TargetedTriggerAction<FrameworkElement>
    {
        #region Fields

        /// <summary>
        /// CommandParameter depedency property.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty = 
            DependencyProperty.Register(
                "CommandParameter",
                typeof(object),
                typeof(RaiseCommand),
                new PropertyMetadata((d, e) => ((RaiseCommand)d).OnCommandParameterChanged((object)e.OldValue, (object)e.NewValue)));

        /// <summary>
        /// Command depedency property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty = 
            DependencyProperty.Register(
                "Command",
                typeof(ICommand),
                typeof(RaiseCommand),
                new PropertyMetadata((d, e) => ((RaiseCommand)d).OnCommandChanged((ICommand)e.OldValue, (ICommand)e.NewValue)));

        /// <summary>
        /// Key depedency property.
        /// </summary>
        public static readonly DependencyProperty KeyProperty = 
            DependencyProperty.Register(
                "Key",
                typeof(Key),
                typeof(RaiseCommand),
                new PropertyMetadata(Key.Enter));

        #endregion Fields

        #region Constructors

        public RaiseCommand()
        {
        }

        #endregion Constructors

        #region Properties

        public ICommand Command
        {
            get
            {
                return (ICommand)GetValue(CommandProperty);
            }

            set
            {
                SetValue(CommandProperty, value);
            }
        }

        public object CommandParameter
        {
            get
            {
                return (object)GetValue(CommandParameterProperty);
            }

            set
            {
                SetValue(CommandParameterProperty, value);
            }
        }

        public Key Key
        {
            get
            {
                return (Key)GetValue(KeyProperty);
            }

            set
            {
                SetValue(KeyProperty, value);
            }
        }

        #endregion Properties

        #region Methods

        protected override void Invoke(object parameter)
        {
            if (parameter is KeyEventArgs && ((KeyEventArgs)parameter).Key != Key)
            {
                return;
            }

            var cmd = this.Command;
            if (cmd != null)
            {
                cmd.Execute(this.CommandParameter);
            }
        }

        /// <summary>
        /// handles the CommandProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnCommandChanged(ICommand oldValue, ICommand newValue)
        {
            Invoke(null);
        }

        /// <summary>
        /// handles the CommandParameterProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnCommandParameterChanged(object oldValue, object newValue)
        {
            Invoke(null);
        }

        #endregion Methods
    }
}