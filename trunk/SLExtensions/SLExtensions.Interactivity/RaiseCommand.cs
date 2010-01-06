
namespace SLExtensions.Interactivity
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
    using System.Windows.Interactivity;

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

        #endregion Fields

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

            var command = SLExtensions.Input.CommandService.FindCommand(CommandName);
            if (command != null)
            {
                command.Execute(CommandParameter);
            }
        }

        #endregion Methods
    }
}