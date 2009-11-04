// <copyright file="CommandSubscription.cs" company="Ucaya">
// Distributed under Microsoft Public License (Ms-PL)
// </copyright>
// <author>Thierry Bouquain</author>
namespace SLExtensions.Input
{
    using System;
    using System.Collections.Generic;
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

        /// <summary>
        /// CommandSubscription depedency property.
        /// </summary>
        public static readonly DependencyProperty CommandSubscriptionProperty = 
            DependencyProperty.RegisterAttached(
                "CommandSubscription",
                typeof(Dictionary<string, CommandSubscription>),
                typeof(CommandSubscription),
                null);


        // Using a DependencyProperty as the backing store for ICommandSubscription.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ICommandSubscriptionProperty =
            DependencyProperty.RegisterAttached("ICommandSubscription", typeof(Dictionary<ICommand,CommandSubscription>), typeof(CommandSubscription), null);

        
        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandSubscription"/> class.
        /// </summary>
        /// <param name="element">The element attached to the command.</param>
        /// <param name="commandName">Name of the command.</param>
        public CommandSubscription(FrameworkElement element, string commandName)
        {
            this.Element = element;
            this.CommandName = commandName;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the Command name
        /// </summary>
        /// <value>The name of the command.</value>
        public string CommandName
        {
            get;
            private set;
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
            Command cmd = CommandService.FindCommand(commandName);
            if (cmd != null)
            {
                CommandSubscription subscription = cmd.CreateCommandSubscription(element, commandName);

                Dictionary<string, CommandSubscription> elementSubscriptions = element.GetValue(CommandSubscriptionProperty) as Dictionary<string, CommandSubscription>;
                if (elementSubscriptions == null)
                {
                    elementSubscriptions = new Dictionary<string, CommandSubscription>();
                    element.SetValue(CommandSubscriptionProperty, elementSubscriptions);
                }

                subscription.HookEvents();
                elementSubscriptions[commandName] = subscription;
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
                CommandSubscription subscription = new CommandSubscription(element, command.ToString());

                var elementSubscriptions = element.GetValue(ICommandSubscriptionProperty) as Dictionary<ICommand, CommandSubscription>;
                if (elementSubscriptions == null)
                {
                    elementSubscriptions = new Dictionary<ICommand, CommandSubscription>();
                    element.SetValue(ICommandSubscriptionProperty, elementSubscriptions);
                }

                subscription.HookEvents();
                elementSubscriptions[command] = subscription;
            }
        }

        internal static void UnregisterAllSubscriptions(FrameworkElement element)
        {
            Dictionary<string, CommandSubscription> elementSubscriptions = element.GetValue(CommandSubscriptionProperty) as Dictionary<string, CommandSubscription>;
            if (elementSubscriptions == null)
                return;

            foreach (var item in new List<CommandSubscription>(elementSubscriptions.Values))
            {
                item.Unregister();
            }

            Dictionary<ICommand, CommandSubscription> elementISubscriptions = element.GetValue(ICommandSubscriptionProperty) as Dictionary<ICommand, CommandSubscription>;
            if (elementISubscriptions == null)
                return;

            foreach (var item in new List<CommandSubscription>(elementISubscriptions.Values))
            {
                item.Unregister();
            }
        }

        /// <summary>
        /// Unregister a command from an element
        /// </summary>
        /// <param name="commandName">The command name to remove</param>
        /// <param name="element">The element to be detached</param>
        internal static void UnregisterSubscription(string commandName, FrameworkElement element)
        {
            Dictionary<string, CommandSubscription> elementSubscriptions = element.GetValue(CommandSubscriptionProperty) as Dictionary<string, CommandSubscription>;
            if (elementSubscriptions == null)
            {
                return;
            }

            CommandSubscription currentSubscription;
            if (!elementSubscriptions.TryGetValue(commandName, out currentSubscription))
            {
                return;
            }

            currentSubscription.Unregister();
        }

        /// <summary>
        /// Unregister a command from an element
        /// </summary>
        /// <param name="commandName">The command name to remove</param>
        /// <param name="element">The element to be detached</param>
        internal static void UnregisterSubscription(ICommand command, FrameworkElement element)
        {
            Dictionary<ICommand, CommandSubscription> elementSubscriptions = element.GetValue(ICommandSubscriptionProperty) as Dictionary<ICommand, CommandSubscription>;
            if (elementSubscriptions == null)
            {
                return;
            }

            CommandSubscription currentSubscription;
            if (!elementSubscriptions.TryGetValue(command, out currentSubscription))
            {
                return;
            }

            currentSubscription.Unregister();
        }

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
                
            string commandName = command as string;
            if (commandName != null)
            {
                Command cmd = CommandService.FindCommand(commandName);
                cmd.Execute(parameter, sender);
            }
            else
            {
                ICommand cmd = command as ICommand;
                if (cmd != null)
                {
                    if (cmd.CanExecute(parameter))
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

            Dictionary<string, CommandSubscription> elementSubscriptions = this.Element.GetValue(CommandSubscriptionProperty) as Dictionary<string, CommandSubscription>;
            if (elementSubscriptions == null)
            {
                return;
            }

            elementSubscriptions.Remove(this.CommandName);
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