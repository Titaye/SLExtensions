// <copyright file="AerialView.cs" company="Ucaya">
// Distributed under Microsoft Public License (Ms-PL)
// </copyright>
// <author>Thierry Bouquain</author>
namespace SLExtensions.Controls.Layers.VirtualEarth
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
    /// Aerial map view
    /// </summary>
    public class AerialView : VETileSource
    {
        #region Fields

        /// <summary>
        /// Create a random modulo for server load balancong when accessing tile map
        /// </summary>
        private static int modulo = new Random(DateTime.Now.Second).Next(4);

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AerialView"/> class.
        /// </summary>
        public AerialView()
        {
            DefaultStyleKey = typeof(AerialView);
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Gets the tile image URL.
        /// </summary>
        /// <param name="tileXY">The tile XY.</param>
        /// <param name="leveOfDetail">The leve of detail.</param>
        /// <returns>the image uri</returns>
        public override Uri GetTileImageUrl(Point tileXY, uint leveOfDetail)
        {
            int srv = (int)(tileXY.X + tileXY.Y + modulo) % 4;
            return new Uri(string.Format("http://{0}{3}.ortho.tiles.virtualearth.net/tiles/{0}{1}{2}", "a", TileXYToQuadKey(tileXY, leveOfDetail), ".jpeg?g=131", srv));

            // http://a0.ortho.tiles.virtualearth.net/tiles/a0230.jpeg?g=131
            // http://r0.ortho.tiles.virtualearth.net/tiles/r0230.png?g=131&shading=hill
        }

        #endregion Methods
    }
}