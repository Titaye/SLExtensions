// <copyright file="BubblingEventDelegate.cs" company="Ucaya">
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
    /// Bubbling event delegate
    /// </summary>
    /// <typeparam name="T">A <see cref="BubblingEventArgs"/> type</typeparam>
    public class BubblingEventDelegate<T>
        where T : BubblingEventArgs
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BubblingEventDelegate&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="registration">The registration.</param>
        public BubblingEventDelegate(object source, BubblingEventRegistration<T> registration)
        {
            this.Source = source;
            this.EventRegistration = registration;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the event registration.
        /// </summary>
        /// <value>The event registration.</value>
        public BubblingEventRegistration<T> EventRegistration
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>The source.</value>
        public object Source
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Invokes the specified args.
        /// </summary>
        /// <param name="args">The args to invoke.</param>
        public void Invoke(T args)
        {
            if (!args.Handled || this.EventRegistration.HandledEvents)
            {
                this.EventRegistration.Handler(this.Source, args);
            }
        }

        #endregion Methods
    }
}