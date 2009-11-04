namespace SLExtensions.Input
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

    public class MouseInactivityCommand : Command
    {
        #region Constructors

        static MouseInactivityCommand()
        {
            MouseInactivity = new MouseInactivityCommand("MouseInactivity");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MouseEnterCommand"/> class.
        /// </summary>
        /// <param name="commandName">The command name used for retreiving command in Xaml.</param>
        public MouseInactivityCommand(string commandName)
            : base(commandName)
        {
        }

        #endregion Constructors

        #region Properties

        public static MouseInactivityCommand MouseInactivity
        {
            get; private set;
        }

        #endregion Properties

        #region Methods

        public static void Initialize()
        {
        }

        public override CommandSubscription CreateCommandSubscription(FrameworkElement element, string commandName)
        {
            return new MouseInactivityCommandSubscription(element, commandName);
        }

        #endregion Methods
    }
}