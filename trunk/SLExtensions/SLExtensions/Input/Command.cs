// <copyright file="Command.cs" company="Ucaya">
// Distributed under Microsoft Public License (Ms-PL)
// </copyright>
// <author>Thierry Bouquain</author>
namespace SLExtensions.Input
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    /// <summary>
    /// Command pattern implementation
    /// </summary>
    public class Command : ICommand
    {
        #region Constructors

        /// <summary>
        /// Static constructor. Initialize static properties
        /// </summary>
        static Command()
        {
            CommandCache = new Dictionary<string, Command>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        /// <param name="commandName">The command name used for retreiving command in Xaml.</param>
        public Command(string commandName)
        {
            if (CommandCache.ContainsKey(commandName))
            {
                // Not throwing exception to prevent error in blend
                return;
                //throw new ArgumentException(Resource.CommandNameAlreadyRegistered, commandName);
            }

            this.Name = commandName;
            CommandCache.Add(commandName, this);
        }

        #endregion Constructors

        #region Events

        /// <summary>
        /// Check if the specified command can be executed.
        /// </summary>
        public event EventHandler<CanExecuteEventArgs> CanExecute;

        /// <summary>
        /// Occurs when the command is executed.
        /// </summary>
        public event EventHandler<ExecutedEventArgs> Executed;

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets the command cache.
        /// </summary>
        /// <value>The command cache.</value>
        public static Dictionary<string, Command> CommandCache
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the name of the command.
        /// </summary>
        /// <value>the name of the command.</value>
        public string Name
        {
            get;
            private set;
        }

        #endregion Properties

        #region Methods

        public virtual CommandSubscription CreateCommandSubscription(FrameworkElement element, string commandName)
        {
            return new CommandSubscription(element, commandName);
        }

        /// <summary>
        /// Executes the command
        /// </summary>
        public virtual void Execute()
        {
            Execute(null);
        }

        /// <summary>
        /// Executes the command
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        public virtual void Execute(object parameter)
        {
            if (this.RaiseCanExecute(parameter))
            {
                if (this.Executed != null)
                {
                    ExecutedEventArgs e = new ExecutedEventArgs(this, parameter);
                    this.Executed(this, e);
                }
            }
        }

        /// <summary>
        /// Executes the command
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="source">The event source.</param>
        public virtual void Execute(object parameter, object source)
        {
            if (this.RaiseCanExecute(parameter))
            {
                if (this.Executed != null)
                {
                    ExecutedEventArgs e = new ExecutedEventArgs(this, parameter, source);
                    this.Executed(this, e);
                }
            }
        }

        /// <summary>
        /// Raises the can execute event.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>returns <c>true</c> if the command can be executed, otherwise <c>false</c>.</returns>
        public virtual bool RaiseCanExecute(object parameter)
        {
            bool result = true;
            if (this.CanExecute != null)
            {
                CanExecuteEventArgs e = new CanExecuteEventArgs(this, parameter);
                this.CanExecute(this, e);
                result = e.CanExecute;
            }

            if (lastCanExecute != result && CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
            return result;
        }

        #endregion Methods

        #region ICommand Members

        private bool lastCanExecute = false;

        bool ICommand.CanExecute(object parameter)
        {
            return RaiseCanExecute(parameter);
        }

        public event EventHandler CanExecuteChanged;

        #endregion
    }
}