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

        public Binding CommandBinding
        {
            get { return this.commandBinding; }
            set
            {
                if (this.commandBinding != value)
                {
                    this.commandBinding = value;
                    if (value == null)
                        bindingListenerCommand.ClearValue(BindingListener.ValueProperty);
                    else
                        bindingListenerCommand.SetBinding(BindingListener.ValueProperty, value);

                    bindingListenerCommand.EnsureBindingSource(Target);
                }
            }
        }

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

        public Binding CommandParameterBinding
        {
            get { return this.commandParameterBinding; }
            set
            {
                if (this.commandParameterBinding != value)
                {
                    this.commandParameterBinding = value;

                    if (value == null)
                        bindingListenerCommandParameter.ClearValue(BindingListener.ValueProperty);
                    else
                        bindingListenerCommandParameter.SetBinding(BindingListener.ValueProperty, value);

                    bindingListenerCommandParameter.EnsureBindingSource(Target);
                }
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

        protected override void OnTargetChanged(FrameworkElement oldTarget, FrameworkElement newTarget)
        {
            base.OnTargetChanged(oldTarget, newTarget);
            bindingListenerCommand.EnsureBindingSource(newTarget);
            bindingListenerCommandParameter.EnsureBindingSource(newTarget);
        }

        #endregion Methods
    }
}