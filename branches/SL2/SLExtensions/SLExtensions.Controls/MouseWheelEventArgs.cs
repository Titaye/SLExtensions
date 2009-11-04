// <copyright file="MouseWheelEventArgs.cs" company="Ucaya">
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
    /// Mouse event args
    /// </summary>
    public class MouseWheelEventArgs : BubblingEventArgs
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MouseWheelEventArgs"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="delta">The delta.</param>
        public MouseWheelEventArgs(FrameworkElement source, double delta)
            : base(source)
        {
            this.Delta = delta;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets mouse wheel delta.
        /// </summary>
        /// <value>The delta.</value>
        public double Delta
        {
            get;
            private set;
        }

        #endregion Properties
    }
}