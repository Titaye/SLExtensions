// <copyright file="RoadViewType.cs" company="Ucaya">
// Distributed under Microsoft Public License (Ms-PL)
// </copyright>
// <author>Thierry Bouquain</author>
namespace SLExtensions.Controls.Layers.GoogleMaps
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
    /// Road view types
    /// </summary>
    public enum RoadViewType
    {
        /// <summary>
        /// Default map view
        /// </summary>
        Default = 0,

        /// <summary>
        /// Terrain map view
        /// </summary>
        Terrain = 1,

        /// <summary>
        /// Road map view
        /// </summary>
        RoadOnly = 2
    }

    #endregion Enumerations
}