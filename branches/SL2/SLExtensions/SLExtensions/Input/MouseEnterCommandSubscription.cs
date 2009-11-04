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

    using SLExtensions.Diagnostics;

    public class MouseEnterCommandSubscription : CommandSubscription
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MouseCommandSubscription"/> class.
        /// </summary>
        /// <param name="element">The element attached to the command.</param>
        /// <param name="commandName">Name of the command.</param>
        public MouseEnterCommandSubscription(FrameworkElement element, string commandName)
            : base(element, commandName)
        {
        }

        #endregion Constructors

        #region Methods

        protected override void HookEvents()
        {
            Element.MouseEnter += new MouseEventHandler(Element_MouseEnter);
        }

        protected override void UnhookEvents()
        {
            Element.MouseEnter -= new MouseEventHandler(Element_MouseEnter);
        }

        void Element_MouseEnter(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("Cmd MouseEnter ");
            ExecuteCommand(sender);
        }

        #endregion Methods
    }
}