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

    public class MouseEnterCommand : Command, IProvideCommandSubscription
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MouseEnterCommand"/> class.
        /// </summary>
        /// <param name="commandName">The command name used for retreiving command in Xaml.</param>
        public MouseEnterCommand(string commandName)
            : base(commandName)
        {
        }

        #endregion Constructors

        #region Methods

        public CommandSubscription CreateCommandSubscription(FrameworkElement element)
        {
            return  new MouseEnterCommandSubscription(element, this);
        }

        #endregion Methods
    }
}