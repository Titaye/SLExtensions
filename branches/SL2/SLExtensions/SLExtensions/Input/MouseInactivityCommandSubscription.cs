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
    using System.Windows.Threading;

    using SLExtensions.Diagnostics;

    public class MouseInactivityCommandSubscription : CommandSubscription
    {
        #region Fields

        private MouseInactivityCommandParameter commandParameter;
        private bool isActive;
        private DispatcherTimer timer;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MouseLeaveCommandSubscription"/> class.
        /// </summary>
        /// <param name="element">The element attached to the command.</param>
        /// <param name="commandName">Name of the command.</param>
        public MouseInactivityCommandSubscription(FrameworkElement element, string commandName)
            : base(element, commandName)
        {
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timer_Tick);
        }

        #endregion Constructors

        #region Properties

        public bool IsActive
        {
            get
            {
                return this.isActive;
            }

            set
            {
                if (this.isActive != value)
                {
                    this.isActive = value;

                    if (commandParameter != null)
                    {
                        if (value)
                        {
                            Command cmd = CommandService.FindCommand(commandParameter.ActiveCommandName);
                            if (cmd != null)
                            {
                                cmd.Execute(commandParameter.ActiveCommandParameter, Element);
                            }
                        }
                        else
                        {
                            Command cmd = CommandService.FindCommand(commandParameter.InActiveCommandName);
                            if (cmd != null)
                            {
                                cmd.Execute(commandParameter.InActiveCommandParameter, Element);
                            }
                        }
                    }
                }
            }
        }

        #endregion Properties

        #region Methods

        protected override void HookEvents()
        {
            Element.MouseMove += new MouseEventHandler(Element_MouseMove);
            Element.Loaded += new RoutedEventHandler(Element_Loaded);
        }

        void Element_Loaded(object sender, RoutedEventArgs e)
        {
            commandParameter = CommandService.GetCommandParameter(Element) as MouseInactivityCommandParameter;
            if (commandParameter != null)
            {
                timer.Interval = commandParameter.InactivityTimeSpan;
            }
        }

        void Element_MouseMove(object sender, MouseEventArgs e)
        {
            if (commandParameter == null)
                return;

            IsActive = true;
            timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            IsActive = false;
        }

        #endregion Methods
    }
}