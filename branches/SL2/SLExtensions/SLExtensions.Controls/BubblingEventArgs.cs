// <copyright file="BubblingEventArgs.cs" company="Ucaya">
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
    /// EventArgs for handling bubling events
    /// </summary>
    public class BubblingEventArgs : EventArgs
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BubblingEventArgs"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        public BubblingEventArgs(object source)
        {
            this.Source = source;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="BubblingEventArgs"/> is handled.
        /// </summary>
        /// <value><c>true</c> if handled; otherwise, <c>false</c>.</value>
        public bool Handled
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the source.
        /// </summary>
        /// <value>The source.</value>
        public object Source
        {
            get;
            private set;
        }

        #endregion Properties
    }
}