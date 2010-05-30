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

        public Command()
        {
        }

        public Command(string commandName)
        {
            this.Name = commandName;

            if (commandName != null && !CommandService.CommandCache.ContainsKey(commandName))
            {
                CommandService.CommandCache.Add(commandName, this);
            }
        }

        public Command(Action executed = null, Func<bool> canExecute = null, string commandName = null)
            : this(commandName)
        {
            if (canExecute != null)
            {
                CanExecute += (snd, e) =>
                {
                    if (canExecute != null)
                        e.CanExecute = canExecute();
                };
            }

            if (executed != null)
                Executed += (snd, e) => executed();
        }

        public Command(Action<object> executed = null, Func<object, bool> canExecute = null, string commandName = null)
            : this(commandName)
        {
            if (canExecute != null)
            {
                CanExecute += (snd, e) =>
                {
                    if (canExecute != null)
                        e.CanExecute = canExecute(e.Parameter);
                };
            }

            if (executed != null)
                Executed += (snd, e) => executed(e.Parameter);
        }

        #endregion Constructors

        #region Events

        /// <summary>
        /// Check if the specified command can be executed.
        /// </summary>
        public event EventHandler<CanExecuteEventArgs> CanExecute;

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        /// <summary>
        /// Occurs when the command is executed.
        /// </summary>
        public event EventHandler<ExecutedEventArgs> Executed;

        #endregion Events

        #region Properties

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
            if (this.Executed != null)
            {
                ExecutedEventArgs e = new ExecutedEventArgs(this, parameter);
                this.Executed(this, e);
            }
        }

        bool ICommand.CanExecute(object parameter)
        {
            return RaiseCanExecute(parameter);
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

            return result;
        }

        #endregion Methods
    }

    public class Command<T> : ICommand
    {
        #region Constructors

        public Command()
        {
        }

        public Command(Action<T> executed = null, Func<T, bool> canExecute = null, string commandName = null)
            : this(commandName)
        {
            if (canExecute != null)
            {
                CanExecute += (snd, e) =>
                {
                    if (canExecute != null)
                        e.CanExecute = canExecute(e.Parameter);
                };
            }

            if (executed != null)
                Executed += (snd, e) => executed(e.Parameter);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        /// <param name="commandName">The command name used for retreiving command in Xaml.</param>
        public Command(string commandName)
        {
            this.Name = commandName;

            if (commandName != null && !CommandService.CommandCache.ContainsKey(commandName))
            {
                CommandService.CommandCache.Add(commandName, this);
            }
        }

        #endregion Constructors

        #region Events

        /// <summary>
        /// Check if the specified command can be executed.
        /// </summary>
        public event EventHandler<CanExecuteEventArgs<T>> CanExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Occurs when the command is executed.
        /// </summary>
        public event EventHandler<ExecutedEventArgs<T>> Executed;

        #endregion Events

        #region Properties

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

        /// <summary>
        /// Executes the command
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        public virtual void Execute(T parameter)
        {
            if (this.Executed != null)
            {
                this.Executed(this, new ExecutedEventArgs<T>(this, parameter));
            }
        }

        bool ICommand.CanExecute(object parameter)
        {
            if (parameter != null &&
                !typeof(T).IsAssignableFrom(parameter.GetType()))
            {
                throw new ArgumentException("parameter is not cannot be converter to (" + typeof(T) + ")", "parameter");
            }
            T prm = (T)parameter;

            return RaiseCanExecute(prm);
        }

        void ICommand.Execute(object parameter)
        {
            if (parameter != null &&
                !typeof(T).IsAssignableFrom(parameter.GetType()))
            {
                throw new ArgumentException("parameter is not cannot be converter to (" + typeof(T) + ")", "parameter");
            }
            T prm = (T)parameter;

            this.Execute(prm);
        }

        /// <summary>
        /// Raises the can execute event.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>returns <c>true</c> if the command can be executed, otherwise <c>false</c>.</returns>
        public virtual bool RaiseCanExecute(T parameter)
        {
            bool result = true;
            if (this.CanExecute != null)
            {
                CanExecuteEventArgs<T> e = new CanExecuteEventArgs<T>(this, parameter);
                this.CanExecute(this, e);
                result = e.CanExecute;
            }

            return result;
        }

        #endregion Methods
    }
}