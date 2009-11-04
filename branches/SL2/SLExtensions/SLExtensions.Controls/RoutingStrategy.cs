// <copyright file="RoutingStrategy.cs" company="Ucaya">
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

    #region Enumerations

    /// <summary>
    /// Routing strategy for <see cref="BubblingEvent"/>
    /// </summary>
    public enum RoutingStrategy
    {
        /// <summary>
        /// Tunnel strategy
        /// </summary>
        Tunnel,

        /// <summary>
        /// Bubble strategy
        /// </summary>
        Bubble,

        /// <summary>
        /// Direct startegy
        /// </summary>
        Direct,
    }

    #endregion Enumerations
}