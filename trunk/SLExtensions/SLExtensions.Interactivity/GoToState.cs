
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
    public class GoToState : TargetedTriggerAction<FrameworkElement>
    {
        #region Fields

        /// <summary>
        /// Key depedency property.
        /// </summary>
        public static readonly DependencyProperty KeyProperty = 
            DependencyProperty.Register(
                "Key",
                typeof(Key),
                typeof(GoToState),
                new PropertyMetadata(Key.Enter));

        /// <summary>
        /// State depedency property.
        /// </summary>
        public static readonly DependencyProperty StateProperty = 
            DependencyProperty.Register(
                "State",
                typeof(string),
                typeof(GoToState),
                null);

        #endregion Fields

        #region Properties

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

        #endregion Properties

        #region Methods

        protected override void Invoke(object parameter)
        {
            if (parameter is KeyEventArgs && ((KeyEventArgs)parameter).Key != Key)
            {
                return;
            }

            SLExtensions.Controls.Animation.VisualState.GoToState(Target, true, State);
        }

        #endregion Methods
    }
}