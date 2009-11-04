// <copyright file="BubblingEventRegistration.cs" company="Ucaya">
// Distributed under Microsoft Public License (Ms-PL)
// </copyright>
// <author>Thierry Bouquain</author>
namespace SLExtensions.Controls
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

    /// <summary>
    /// Bubbling event registration
    /// </summary>
    /// <typeparam name="T">A <see cref="BubblingEventArgs"/> type</typeparam>
    public class BubblingEventRegistration<T>
        where T : BubblingEventArgs
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BubblingEventRegistration&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="classType">Type of the class.</param>
        /// <param name="handler">The handler.</param>
        /// <param name="handledEvents">if set to <c>true</c> [handled events].</param>
        public BubblingEventRegistration(Type classType, EventHandler<T> handler, bool handledEvents)
        {
            this.ClassType = classType;
            this.Handler = handler;
            this.HandledEvents = handledEvents;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the type of the class.
        /// </summary>
        /// <value>The type of the class.</value>
        public Type ClassType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the event is handled.
        /// </summary>
        /// <value><c>true</c> if the event is handled otherwise, <c>false</c>.</value>
        public bool HandledEvents
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the handler.
        /// </summary>
        /// <value>The handler.</value>
        public EventHandler<T> Handler
        {
            get;
            set;
        }

        #endregion Properties
    }
}