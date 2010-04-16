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

    public static class VisualStateCommands
    {
        #region Constructors

        static VisualStateCommands()
        {
            GoToState = new Command("GoToState");
            MouseEnterGoToState = new MouseEnterCommand("MouseEnterGoToState");
            MouseLeaveGoToState = new MouseLeaveCommand("MouseLeaveGoToState");

            GoToState.Executed += new EventHandler<ExecutedEventArgs>(GoToState_Executed);
            MouseEnterGoToState.Executed += new EventHandler<ExecutedEventArgs>(GoToState_Executed);
            MouseLeaveGoToState.Executed += new EventHandler<ExecutedEventArgs>(GoToState_Executed);
        }

        #endregion Constructors

        #region Properties

        public static Command GoToState
        {
            get;
            private set;
        }

        public static MouseEnterCommand MouseEnterGoToState
        {
            get;
            private set;
        }

        public static MouseLeaveCommand MouseLeaveGoToState
        {
            get;
            private set;
        }

        #endregion Properties

        #region Methods

        public static void Initialize()
        {
        }

        static void GoToState_Executed(object sender, ExecutedEventArgs e)
        {
            if (e.Parameter == null)
                return;
            GoToStateCommandParameter prm = e.Parameter as GoToStateCommandParameter;
            if (prm == null)
            {
                GoToStateCommandParameterTypeConverter typeConverter = new GoToStateCommandParameterTypeConverter();
                if (!typeConverter.CanConvertFrom(e.Parameter.GetType()))
                    return;

                prm = (GoToStateCommandParameter)typeConverter.ConvertFrom(e.Parameter);
            }

            FrameworkElement source = e.Source as FrameworkElement;
            if (source == null)
            {
                return;
            }

            FrameworkElement target = source.FindName(prm.ElementName) as FrameworkElement;
            if (target == null)
            {
                target = source;
            }

            VisualState.GoToState(target, true, prm.StateName);
        }

        #endregion Methods
    }
}
