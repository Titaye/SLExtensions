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

    public class MouseLeaveCommandSubscription : CommandSubscription
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MouseLeaveCommandSubscription"/> class.
        /// </summary>
        /// <param name="element">The element attached to the command.</param>
        /// <param name="commandName">The command attached to the element.</param>
        public MouseLeaveCommandSubscription(FrameworkElement element, ICommand command)
            : base(element, command)
        {
        }

        #endregion Constructors

        #region Methods

        protected override void HookEvents()
        {
            Element.MouseLeave += new MouseEventHandler(Element_MouseLeave);
        }

        protected override void UnhookEvents()
        {
            Element.MouseLeave -= new MouseEventHandler(Element_MouseLeave);
        }

        void Element_MouseLeave(object sender, MouseEventArgs e)
        {
            ExecuteCommand(sender);
        }

        #endregion Methods
    }
}