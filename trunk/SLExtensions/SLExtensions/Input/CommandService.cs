// <copyright file="CommandService.cs" company="Ucaya">
// Distributed under Microsoft Public License (Ms-PL)
// </copyright>
// <author>Thierry Bouquain</author>
namespace SLExtensions.Input
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
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
    /// Service class that provides the system implementation for linking Command
    /// </summary>
    public static class CommandService
    {
        #region Fields

        /// <summary>
        ///     The DependencyProperty for the CommandParameter property.
        /// </summary> 
        public static readonly DependencyProperty CommandParameterProperty =
                DependencyProperty.RegisterAttached(
                "CommandParameter",         // Name
                typeof(object),            // Type
                typeof(CommandService), // Owner
                null);

        /// <summary>
        ///     The DependencyProperty for the Command property.
        /// </summary> 
        public static readonly DependencyProperty CommandProperty =
                DependencyProperty.RegisterAttached(
                "Command",         // Name
                typeof(object),            // Type
                typeof(CommandService), // Owner
                new PropertyMetadata(CommandChanged));

        #endregion Fields

        #region Methods

        /// <summary>
        /// Finds a command from its name.
        /// </summary>
        /// <param name="commandName">The command name.</param>
        /// <returns>returns the command for a given commandName</returns>
        public static ICommand FindCommand(string commandName)
        {
            if (string.IsNullOrEmpty(commandName))
            {
                return null;
            }

            // Check from cache
            ICommand cmd = null;
            if (CommandCache.TryGetValue(commandName, out cmd))
            {
                return cmd;
            }

            return null;
        }

        /// <summary>
        ///     Gets the value of the Command property. 
        /// </summary>
        /// <param name="element">The object on which to query the property.</param>
        /// <returns>The value of the property.</returns> 
        public static object GetCommand(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return (object)element.GetValue(CommandProperty);
        }

        /// <summary>
        ///     Gets the value of the CommandParameter property. 
        /// </summary>
        /// <param name="element">The object on which to query the property.</param>
        /// <returns>The value of the property.</returns> 
        public static object GetCommandParameter(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return element.GetValue(CommandParameterProperty);
        }

        /// <summary> 
        ///     Sets the value of the Command property.
        /// </summary>
        /// <param name="element">The object on which to set the value.</param> 
        /// <param name="value">The desired value of the property.</param> 
        public static void SetCommand(DependencyObject element, object value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            element.SetValue(CommandProperty, value);
        }

        /// <summary> 
        ///     Sets the value of the CommandParameter property.
        /// </summary>
        /// <param name="element">The object on which to set the value.</param> 
        /// <param name="value">The desired value of the property.</param> 
        public static void SetCommandParameter(DependencyObject element, object value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            element.SetValue(CommandParameterProperty, value);
        }

        public static void UnregisterCommands(FrameworkElement obj)
        {
            CommandSubscription.UnregisterAllSubscriptions(obj);
        }

        public static void UnregisterCommandsRecursive(FrameworkElement obj)
        {
            Panel pnl = obj as Panel;
            if (pnl != null)
            {
                foreach (var item in pnl.Children)
                {
                    UnregisterCommandsRecursive(item as FrameworkElement);
                }
            }
            CommandSubscription.UnregisterAllSubscriptions(obj);
        }

        /// <summary>
        /// occurs when the command change on a <see cref="DependencyObject"/>
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void CommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement)
            {
                FrameworkElement elem = (FrameworkElement)d;
                string oldCommands = e.OldValue as string;
                if (!string.IsNullOrEmpty(oldCommands))
                {
                    CommandSubscription.UnregisterSubscriptions(elem, (from s in oldCommands.Split(' ') select CommandService.FindCommand(s)).ToArray());
                }
                else
                {
                    ICommand oldCommand = e.OldValue as ICommand;
                    if (oldCommand != null)
                        CommandSubscription.UnregisterSubscriptions(elem, oldCommand);
                }

                string newCommands = e.NewValue as string;
                if (!string.IsNullOrEmpty(newCommands))
                {
                    foreach (var item in newCommands.Split(' '))
                    {
                        CommandSubscription.RegisterCommand(item, elem);
                    }
                }
                else
                {
                    ICommand newCommand = e.NewValue as ICommand;
                    if (newCommand != null)
                        CommandSubscription.RegisterCommand(newCommand, elem);
                }
            }
        }

        #endregion Methods

        /// <summary>
        /// Static constructor. Initialize static properties
        /// </summary>
        static CommandService()
        {
            CommandCache = new Dictionary<string, ICommand>();
        }

        /// <summary>
        /// Gets the command cache.
        /// </summary>
        /// <value>The command cache.</value>
        public static Dictionary<string, ICommand> CommandCache
        {
            get;
            private set;
        }

    }
}