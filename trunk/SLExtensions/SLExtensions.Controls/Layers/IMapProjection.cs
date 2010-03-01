// <copyright file="IMapProjection.cs" company="Ucaya">
// Distributed under Microsoft Public License (Ms-PL)
// </copyright>
// <author>Thierry Bouquain</author>
namespace SLExtensions.Controls.Layers
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
    /// Defines a map projection
    /// </summary>
    public interface IMapProjection
    {
        #region Properties

        /// <summary>
        /// Gets or sets the size of a tile.
        /// </summary>
        /// <value>The size of a tile.</value>
        uint TileSize
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Get ground resolution for a given level of detai.
        /// </summary>
        /// <param name="latitude">The latitude.</param>
        /// <param name="levelOfDetail">The level of detail.</param>
        /// <returns>the resolution.</returns>
        double GroundResolution(double latitude, uint levelOfDetail);

        /// <summary>
        /// Convert latitude and longitude to pixel point.
        /// </summary>
        /// <param name="latitude">The latitude.</param>
        /// <param name="longitude">The longitude.</param>
        /// <param name="levelOfDetail">The level of detail.</param>
        /// <returns>The pixel point</returns>
        System.Windows.Point LatLongToPixelXY(double latitude, double longitude, uint levelOfDetail);

        /// <summary>
        /// Convert latitude and longitude to pixel point.
        /// </summary>
        /// <param name="gpsPoint">The GPS point.</param>
        /// <param name="levelOfDetail">The level of detail.</param>
        /// <returns>The pixel point</returns>
        System.Windows.Point LatLongToPixelXY(System.Windows.Point gpsPoint, uint levelOfDetail);

        /// <summary>
        /// Get map scale
        /// </summary>
        /// <param name="latitude">The latitude.</param>
        /// <param name="levelOfDetail">The level of detail.</param>
        /// <param name="screenDpi">The screen dpi.</param>
        /// <returns>the map scale</returns>
        double MapScale(double latitude, uint levelOfDetail, int screenDpi);

        /// <summary>
        /// Get the pixel size of a map at a given level of detail.
        /// </summary>
        /// <param name="levelOfDetail">The level of detail.</param>
        /// <returns>the map size</returns>
        uint MapSize(uint levelOfDetail);

        /// <summary>
        /// Convert pixel point to latitude and longitude.
        /// </summary>
        /// <param name="pt">The pixel point.</param>
        /// <param name="levelOfDetail">The level of detail.</param>
        /// <returns>The GPS coord point</returns>
        System.Windows.Point PixelXYToLatLong(System.Windows.Point pt, uint levelOfDetail);

        /// <summary>
        /// Get the tile index for a pixel point
        /// </summary>
        /// <param name="pt">The tile point.</param>
        /// <param name="levelOfDetail">The level of detail.</param>
        /// <returns>the tile coord</returns>
        System.Windows.Point PixelXYToTileXY(System.Windows.Point pt, uint levelOfDetail);

        /// <summary>
        /// Get the tile index for a pixel point
        /// </summary>
        /// <param name="pt">The pixel point.</param>
        /// <returns>the tile coord</returns>
        System.Windows.Point PixelXYToTileXY(System.Windows.Point pt);

        /// <summary>
        /// Get the topleft pixel point for a tile coord
        /// </summary>
        /// <param name="pt">The tile coord.</param>
        /// <returns>the pixel point</returns>
        System.Windows.Point TileXYToPixelXY(System.Windows.Point pt);

        #endregion Methods
    }
}