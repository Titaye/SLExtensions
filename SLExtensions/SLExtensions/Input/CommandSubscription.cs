// <copyright file="CommandSubscription.cs" company="Ucaya">
// Distributed under Microsoft Public License (Ms-PL)
// </copyright>
// <author>Thierry Bouquain</author>
namespace SLExtensions.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    /// <summary>
    /// Handles command subscription of a FrameworkElement
    /// </summary>
    public class CommandSubscription
    {
        #region Fields

        // Using a DependencyProperty as the backing store for ICommandSubscription.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandSubscriptionsProperty = 
            DependencyProperty.RegisterAttached("CommandSubscriptions", typeof(List<CommandSubscription>), typeof(CommandSubscription), null);

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandSubscription"/> class.
        /// </summary>
        /// <param name="element">The element attached to the command.</param>
        /// <param name="command">The command attached to the element.</param>
        public CommandSubscription(FrameworkElement element, ICommand command)
        {
            this.Element = element;
            this.Command = command;
        }

        #endregion Constructors

        #region Properties

        public ICommand Command
        {
            get; private set;
        }

        /// <summary>
        /// Gets the element attached to the command
        /// </summary>
        /// <value>The element.</value>
        public FrameworkElement Element
        {
            get;
            private set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Registers the command.
        /// </summary>
        /// <param name="commandName">The command name.</param>
        /// <param name="element">The element.</param>
        internal static void RegisterCommand(string commandName, FrameworkElement element)
        {
            ICommand cmd = CommandService.FindCommand(commandName);
            if (cmd != null)
            {
                RegisterCommand(cmd, element);
            }
        }

        /// <summary>
        /// Registers the command.
        /// </summary>
        /// <param name="commandName">The command.</param>
        /// <param name="element">The element.</param>
        internal static void RegisterCommand(ICommand command, FrameworkElement element)
        {
            if (command != null)
            {
                CommandSubscription subscription = new CommandSubscription(element, command);

                var elementSubscriptions = element.GetValue(CommandSubscriptionsProperty) as List<CommandSubscription>;
                if (elementSubscriptions == null)
                {
                    elementSubscriptions = new List<CommandSubscription>();
                    element.SetValue(CommandSubscriptionsProperty, elementSubscriptions);
                }

                subscription.HookEvents();
                elementSubscriptions.Add(subscription);
            }
        }

        internal static void UnregisterAllSubscriptions(FrameworkElement element)
        {
            List<CommandSubscription> elementISubscriptions = element.GetValue(CommandSubscriptionsProperty) as List<CommandSubscription>;
            if (elementISubscriptions == null)
                return;

            foreach (var item in elementISubscriptions.ToArray())
            {
                item.Unregister();
            }
        }

        /// <summary>
        /// Unregister a command from an element
        /// </summary>
        /// <param name="commandName">The command name to remove</param>
        /// <param name="element">The element to be detached</param>
        internal static void UnregisterSubscriptions(FrameworkElement element, params ICommand[] commmands)
        {
            if (commmands == null)
            {
                return;
            }

            List<CommandSubscription> elementSubscriptions = element.GetValue(CommandSubscriptionsProperty) as List<CommandSubscription>;
            if (elementSubscriptions == null)
            {
                return;
            }

            var subscribtionsToRemove = (from s in elementSubscriptions
                                         where commmands.Contains(s.Command)
                                         select s).ToArray();
            foreach (var item in subscribtionsToRemove)
            {
                item.Unregister();
            }
        }

        ///// <summary>
        ///// Unregister a command from an element
        ///// </summary>
        ///// <param name="commandName">The command name to remove</param>
        ///// <param name="element">The element to be detached</param>
        //internal static void UnregisterSubscription(ICommand command, FrameworkElement element)
        //{
        //    Dictionary<ICommand, CommandSubscription> elementSubscriptions = element.GetValue(ICommandSubscriptionProperty) as Dictionary<ICommand, CommandSubscription>;
        //    if (elementSubscriptions == null)
        //    {
        //        return;
        //    }
        //    CommandSubscription currentSubscription;
        //    if (!elementSubscriptions.TryGetValue(command, out currentSubscription))
        //    {
        //        return;
        //    }
        //    currentSubscription.Unregister();
        //}
        /// <summary>
        /// Executes the command
        /// </summary>
        /// <param name="sender">The command sender</param>
        protected virtual void ExecuteCommand(object sender)
        {
            Control ctrl = sender as Control;
            if (ctrl != null)
            {
                if (!ctrl.IsEnabled)
                    return;
            }

            object parameter = CommandService.GetCommandParameter(this.Element);
            parameter = PreProcessParameter(parameter);

            object command = CommandService.GetCommand(this.Element);

            var ICmd = command as ICommand;
            if (ICmd != null)
            {
                if (ICmd.CanExecute(parameter))
                    ICmd.Execute(parameter);
            }
            else
            {
                string commandName = command as string;
                if (commandName != null)
                {
                    ICommand cmd = CommandService.FindCommand(commandName);
                    cmd.Execute(parameter);
                }
            }
        }

        protected virtual void HookEvents()
        {
            FrameworkElement element = Element;

            if (element is ButtonBase)
            {
                ((ButtonBase)element).Click += CommandService_Click;
            }
            else if (element is TextBox)
            {
                ((TextBox)element).KeyDown += CommandService_KeyDown;
            }
            else
            {
                element.MouseLeftButtonUp += CommandService_Click;
            }
        }

        protected virtual object PreProcessParameter(object parameter)
        {
            return parameter;
        }

        protected virtual void UnhookEvents()
        {
            FrameworkElement element = this.Element;

            if (element is ButtonBase)
            {
                ((ButtonBase)element).Click -= this.CommandService_Click;
            }
            else if (element is TextBox)
            {
                ((TextBox)element).KeyDown -= CommandService_KeyDown;
            }
            else
            {
                element.MouseLeftButtonUp -= this.CommandService_Click;
            }
        }

        /// <summary>
        /// Unregister the current CommandSubscription
        /// </summary>
        protected virtual void Unregister()
        {
            UnhookEvents();

            List<CommandSubscription> elementSubscriptions = this.Element.GetValue(CommandSubscriptionsProperty) as List<CommandSubscription>;
            if (elementSubscriptions == null)
            {
                return;
            }

            elementSubscriptions.Remove(this);
        }

        /// <summary>
        /// Handles the Click event of the CommandService control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void CommandService_Click(object sender, EventArgs e)
        {
            ExecuteCommand(sender);
        }

        /// <summary>
        /// Handles the KeyDown event of the CommandService control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void CommandService_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ExecuteCommand(sender);
            }
        }

        #endregion Methods
    }
}