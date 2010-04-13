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
        /// CommandName depedency property.
        /// </summary>
        public static readonly DependencyProperty CommandNameProperty = 
            DependencyProperty.Register(
                "CommandName",
                typeof(string),
                typeof(RaiseCommand),
                null);

        /// <summary>
        /// CommandParameter depedency property.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty = 
            DependencyProperty.Register(
                "CommandParameter",
                typeof(object),
                typeof(RaiseCommand),
                null);

        /// <summary>
        /// Key depedency property.
        /// </summary>
        public static readonly DependencyProperty KeyProperty = 
            DependencyProperty.Register(
                "Key",
                typeof(Key),
                typeof(RaiseCommand),
                new PropertyMetadata(Key.Enter));

        private BindingListener bindingListenerCommand;
        private BindingListener bindingListenerCommandParameter;
        private ICommand command;
        private Binding commandBinding;
        private Binding commandParameterBinding;

        #endregion Fields

        #region Constructors

        public RaiseCommand()
        {
            bindingListenerCommand = new BindingListener();
            bindingListenerCommand.ValueChanged += delegate { this.command = bindingListenerCommand.Value as ICommand; };
            bindingListenerCommandParameter = new BindingListener();
            bindingListenerCommandParameter.ValueChanged += delegate { this.CommandParameter = bindingListenerCommandParameter.Value; };
        }

        #endregion Constructors

        #region Properties

        public string CommandName
        {
            get
            {
                return (string)GetValue(CommandNameProperty);
            }

            set
            {
                SetValue(CommandNameProperty, value);
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



        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(RaiseCommand), null);
        
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

            var cmd = this.command;
            if (cmd == null)
            {
                cmd = SLExtensions.Input.CommandService.FindCommand(CommandName);
            }

            if (cmd != null)
            {
                cmd.Execute(CommandParameter);
            }
        }

        #endregion Methods
    }
}