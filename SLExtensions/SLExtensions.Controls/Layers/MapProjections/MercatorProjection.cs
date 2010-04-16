// <copyright file="MercatorProjection.cs" company="Ucaya">
// Distributed under Microsoft Public License (Ms-PL)
// </copyright>
// <author>Thierry Bouquain</author>
namespace SLExtensions.Controls.Layers.MapProjections
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
    /// Mercator map projection.
    /// http://msdn2.microsoft.com/en-us/library/bb259689.aspx
    /// </summary>
    public class MercatorProjection : IMapProjection
    {
        #region Fields

        /// <summary>
        /// max latitude for this projection
        /// </summary>
        private const double MaxLatitude = 85.05112878;

        /// <summary>
        /// max longitude for this projection
        /// </summary>
        private const double MaxLongitude = 180;

        /// <summary>
        /// min latitude for this projection
        /// </summary>
        private const double MinLatitude = -85.05112878;

        /// <summary>
        /// min longitude for this projection
        /// </summary>
        private const double MinLongitude = -180;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MercatorProjection"/> class.
        /// </summary>
        public MercatorProjection()
        {
            this.TileSize = 256;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the size of a tile.
        /// </summary>
        /// <value>The size of a tile.</value>
        public uint TileSize
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Determines the ground resolution (in meters per pixel) at a specified
        /// latitude and level of detail.
        /// </summary>
        /// <param name="latitude">Latitude (in degrees) at which to measure the
        /// ground resolution.</param>
        /// <param name="levelOfDetail">Level of detail, from 1 (lowest detail)
        /// to 23 (highest detail).</param>
        /// <returns>The ground resolution, in meters per pixel.</returns>
        public double GroundResolution(double latitude, uint levelOfDetail)
        {
            latitude = Clip(latitude, MinLatitude, MaxLatitude);
            return Math.Cos(latitude * Math.PI / 180) * 2 * Math.PI * Constants.EarthRadius / this.MapSize(levelOfDetail);
        }

        /// <summary>
        /// Converts a point from latitude/longitude WGS-84 coordinates (in degrees)
        /// into pixel XY coordinates at a specified level of detail.
        /// </summary>
        /// <param name="latitude">Latitude of the point, in degrees.</param>
        /// <param name="longitude">Longitude of the point, in degrees.</param>
        /// <param name="levelOfDetail">Level of detail, from 1 (lowest detail)
        /// to 23 (highest detail).</param>
        /// <returns>The pixel point</returns>
        public Point LatLongToPixelXY(double latitude, double longitude, uint levelOfDetail)
        {
            double factor = (double)this.MapSize(23) / this.MapSize(levelOfDetail);
            Point pt = this.LatLongToPixelXYImpl(latitude, longitude, 23);
            return new Point(pt.X / factor, pt.Y / factor);
        }

        /// <summary>
        /// Converts a point from latitude/longitude WGS-84 coordinates (in degrees)
        /// into pixel XY coordinates at a specified level of detail.
        /// </summary>
        /// <param name="gpsPoint">Longitude = x, Latitude = y point</param>
        /// <param name="levelOfDetail">Level of detail, from 1 (lowest detail)
        /// to 23 (highest detail).</param>
        /// <returns>The pixel point</returns>
        public Point LatLongToPixelXY(Point gpsPoint, uint levelOfDetail)
        {
            return this.LatLongToPixelXY(gpsPoint.Y, gpsPoint.X, levelOfDetail);
        }

        /// <summary>
        /// Determines the map scale at a specified latitude, level of detail,
        /// and screen resolution.
        /// </summary>
        /// <param name="latitude">Latitude (in degrees) at which to measure the
        /// map scale.</param>
        /// <param name="levelOfDetail">Level of detail, from 1 (lowest detail)
        /// to 23 (highest detail).</param>
        /// <param name="screenDpi">Resolution of the screen, in dots per inch.</param>
        /// <returns>The map scale, expressed as the denominator N of the ratio 1 : N.</returns>
        public double MapScale(double latitude, uint levelOfDetail, int screenDpi)
        {
            return this.GroundResolution(latitude, levelOfDetail) * screenDpi / 0.0254;
        }

        /// <summary>
        /// Determines the map width and height (in pixels) at a specified level
        /// of detail.
        /// </summary>
        /// <param name="levelOfDetail">Level of detail, from 1 (lowest detail)
        /// to 23 (highest detail).</param>
        /// <returns>The map width and height in pixels.</returns>
        public uint MapSize(uint levelOfDetail)
        {
            return (uint)this.TileSize << (int)levelOfDetail;
        }

        /// <summary>
        /// Converts a point from latitude/longitude WGS-84 coordinates (in degrees)
        /// into pixel XY coordinates at a specified level of detail.
        /// </summary>
        /// <param name="pt">pixel point</param>
        /// <param name="levelOfDetail">Level of detail, from 1 (lowest detail)
        /// to 23 (highest detail).</param>
        /// <returns>Longitude = x, Latitude = y point</returns>
        public Point PixelXYToLatLong(Point pt, uint levelOfDetail)
        {
            uint mapSize = this.MapSize(levelOfDetail);
            double latitudeRad = Math.Atan(Math.Sinh((mapSize / 2 - pt.Y) / mapSize * 2 * Math.PI));
            double latitude = this.ConvertRadToDeg(latitudeRad);
            double longitude = (pt.X - mapSize / 2) / mapSize * 360;

            return new Point(longitude, latitude);
        }

        /// <summary>
        /// Converts pixel XY coordinates into tile XY coordinates.
        /// </summary>
        /// <param name="pt">Pixel point</param>
        /// <returns>Tile coord</returns>
        public Point PixelXYToTileXY(Point pt)
        {
            return new Point(pt.X / this.TileSize, pt.Y / this.TileSize);
        }

        /// <summary>
        /// Converts pixel XY coordinates into tile XY coordinates.
        /// </summary>
        /// <param name="pt">pixel point</param>
        /// <param name="zoomlevel">zoom level</param>
        /// <returns>Tile coord</returns>
        public Point PixelXYToTileXY(Point pt, uint zoomlevel)
        {
            return new Point(Math.Floor(Clip(0, pt.X / this.TileSize, Math.Pow(2, zoomlevel) - 1)), Math.Floor(Clip(0, pt.Y / this.TileSize, Math.Pow(2, zoomlevel) - 1)));
        }

        /// <summary>
        /// Converts pixel XY coordinates into tile XY coordinates.
        /// </summary>
        /// <param name="pt">tile coord</param>
        /// <returns>pixel coord</returns>
        public Point TileXYToPixelXY(Point pt)
        {
            return new Point(pt.X * this.TileSize, pt.Y * this.TileSize);
        }

        /// <summary>
        /// Clips a number to the specified minimum and maximum values.
        /// </summary>
        /// <param name="n">The number to clip.</param>
        /// <param name="minValue">Minimum allowable value.</param>
        /// <param name="maxValue">Maximum allowable value.</param>
        /// <returns>The clipped value.</returns>
        private static double Clip(double n, double minValue, double maxValue)
        {
            return Math.Min(Math.Max(n, minValue), maxValue);
        }

        /// <summary>
        /// Converts radian to degree.
        /// </summary>
        /// <param name="rad">The radian value.</param>
        /// <returns>the degree value.</returns>
        private double ConvertRadToDeg(double rad)
        {
            return rad / (2 * Math.PI) * 360;
        }

        /// <summary>
        /// convert latitude and longitude to pixel XY.
        /// </summary>
        /// <param name="latitude">The latitude.</param>
        /// <param name="longitude">The longitude.</param>
        /// <param name="levelOfDetail">The level of detail.</param>
        /// <returns>the pixel coord</returns>
        private Point LatLongToPixelXYImpl(double latitude, double longitude, uint levelOfDetail)
        {
            latitude = Clip(latitude, MinLatitude, MaxLatitude);
            longitude = Clip(longitude, MinLongitude, MaxLongitude);

            double x = (longitude + 180) / 360;
            double sinLatitude = Math.Sin(latitude * Math.PI / 180);
            double y = 0.5 - Math.Log((1 + sinLatitude) / (1 - sinLatitude)) / (4 * Math.PI);

            uint mapSize = this.MapSize(levelOfDetail);
            return new Point(Clip(x * mapSize + 0.5, 0, mapSize - 1), Clip(y * mapSize + 0.5, 0, mapSize - 1));
        }

        #endregion Methods
    }
}