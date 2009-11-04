// <copyright file="IItemContainerGenerator.cs" company="Ucaya">
// Distributed under Microsoft Public License (Ms-PL)
// </copyright>
// <author>Thierry Bouquain</author>
namespace SLExtensions.Controls
{
    using System;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    /// <summary>
    /// An interface that is implemented by classes which are responsible for generating user interface (UI) content on behalf of a host.
    /// </summary>
    public interface IItemContainerGenerator
    {
        #region Methods

        /// <summary>
        /// When overridden in a derived class, undoes the effects of the PrepareContainerForItemOverride method.
        /// </summary>
        /// <param name="element">The container element.</param>
        /// <param name="item">Specified item.</param>
        void ClearContainerForItemOverride(DependencyObject element, object item);

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>the container element</returns>
        DependencyObject GetContainerForItemOverride();

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own container.
        /// </summary>
        /// <param name="item">Specified item.</param>
        /// <returns>true if the item is (or is eligible to be) its own container; otherwise, false.</returns>
        bool IsItemItsOwnContainerOverride(object item);

        /// <summary>
        /// Prepares the specified element to display the specified item. 
        /// </summary>
        /// <param name="element">Element used to display the specified item.</param>
        /// <param name="item">Specified item.</param>
        void PrepareContainerForItemOverride(DependencyObject element, object item);

        #endregion Methods
    }
}