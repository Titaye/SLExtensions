// <copyright file="ExecutedEventArgs.cs" company="Ucaya">
// Distributed under Microsoft Public License (Ms-PL)
// </copyright>
// <author>Thierry Bouquain</author>
namespace SLExtensions.Input
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    public class ExecutedEventArgs : EventArgs
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutedEventArgs"/> class.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="parameter">The parameter.</param>
        internal ExecutedEventArgs(Command command, object parameter)
        {
            this.Command = command;
            this.Parameter = parameter;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the command that was invoked.
        /// </summary>
        /// <value>The command associated with this event.</value>
        public Command Command
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets data parameter of the command.
        /// </summary>
        /// <value>The command data. The default value is null.</value>
        public object Parameter
        {
            get;
            private set;
        }

        #endregion Properties
    }

    public class ExecutedEventArgs<T> : EventArgs
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutedEventArgs"/> class.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="parameter">The parameter.</param>
        internal ExecutedEventArgs(Command<T> command, T parameter)
            : this(command, parameter, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutedEventArgs"/> class.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="source">The event source.</param>
        internal ExecutedEventArgs(Command<T> command, T parameter, object source)
        {
            this.Command = command;
            this.Parameter = parameter;
            this.Source = source;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the command that was invoked.
        /// </summary>
        /// <value>The command associated with this event.</value>
        public Command<T> Command
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets data parameter of the command.
        /// </summary>
        /// <value>The command data. The default value is null.</value>
        public T Parameter
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the element that raises the event.
        /// </summary>
        /// <value>The event source.</value>
        public object Source
        {
            get;
            private set;
        }

        #endregion Properties
    }
}