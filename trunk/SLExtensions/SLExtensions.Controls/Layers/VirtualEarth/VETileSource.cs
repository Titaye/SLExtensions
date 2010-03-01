// <copyright file="VETileSource.cs" company="Ucaya">
// Distributed under Microsoft Public License (Ms-PL)
// </copyright>
// <author>Thierry Bouquain</author>
namespace SLExtensions.Controls.Layers.VirtualEarth
{
    using System;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    using SLExtensions.Controls.Layers.MapProjections;

    /// <summary>
    /// Base tile source for VirtualEarth
    /// </summary>
    public abstract class VETileSource : TileSource
    {
        #region Fields

        /// <summary>
        /// Current map projection
        /// </summary>
        private IMapProjection mapProjection;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="VETileSource"/> class.
        /// </summary>
        public VETileSource()
        {
            ItemTemplate = (DataTemplate)XamlReader.Load(Resource.VELayersDataTemplate);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the map projection.
        /// </summary>
        /// <value>The map projection.</value>
        public override IMapProjection MapProjection
        {
            get
            {
                if (this.mapProjection == null)
                {
                    this.mapProjection = new MercatorProjection();
                    this.mapProjection.TileSize = this.TileSize;
                }

                return this.mapProjection;
            }
        }

        /// <summary>
        /// Gets the max level of detail.
        /// </summary>
        /// <value>The max level of detail.</value>
        public override uint MaxLevelOfDetail
        {
            get
            {
                return 23;
            }
        }

        /// <summary>
        /// Gets the min level of detail.
        /// </summary>
        /// <value>The min level of detail.</value>
        public override uint MinLevelOfDetail
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// Gets the size of the tile.
        /// </summary>
        /// <value>The size of the tile.</value>
        public override uint TileSize
        {
            get
            {
                return 256;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Convert a VirtualEarth quadkey to a tile point.
        /// </summary>
        /// <param name="quadkey">The quadkey.</param>
        /// <returns>the tile coord</returns>
        public Point QuadKeyToTileXY(string quadkey)
        {
            int x = 0;
            int y = 0;
            for (int i = 0; i < quadkey.Length; i++)
            {
                x = x << 1;
                y = y << 1;
                switch (quadkey[i])
                {
                    case '0':
                        break;
                    case '1':
                        x += 1;
                        break;
                    case '2':
                        y += 1;
                        break;
                    case '3':
                        x += 1;
                        y += 1;
                        break;
                }
            }

            return new Point(x, y);
        }

        /// <summary>
        /// Converts tile XY coordinates into a QuadKey at a specified level of detail.
        /// </summary>
        /// <param name="tileX">Tile X coordinate.</param>
        /// <param name="tileY">Tile Y coordinate.</param>
        /// <param name="levelOfDetail">Level of detail, from 1 (lowest detail)
        /// to 23 (highest detail).</param>
        /// <returns>A string containing the QuadKey.</returns>
        public string TileXYToQuadKey(int tileX, int tileY, uint levelOfDetail)
        {
            StringBuilder quadKey = new StringBuilder();
            for (int i = (int)levelOfDetail; i > 0; i--)
            {
                char digit = '0';
                int mask = 1 << (i - 1);
                if ((tileX & mask) != 0)
                {
                    digit++;
                }

                if ((tileY & mask) != 0)
                {
                    digit++;
                    digit++;
                }

                quadKey.Append(digit);
            }

            return quadKey.ToString();
        }

        /// <summary>
        /// Convert tile point to a VirtualEarth quadkey.
        /// </summary>
        /// <param name="pt">The tile point.</param>
        /// <param name="levelOfDetail">The level of detail.</param>
        /// <returns>the quadkey string</returns>
        public string TileXYToQuadKey(Point pt, uint levelOfDetail)
        {
            StringBuilder quadKey = new StringBuilder();
            for (int i = (int)levelOfDetail; i > 0; i--)
            {
                char digit = '0';
                int mask = 1 << (i - 1);
                if (((int)pt.X & mask) != 0)
                {
                    digit++;
                }

                if (((int)pt.Y & mask) != 0)
                {
                    digit++;
                    digit++;
                }

                quadKey.Append(digit);
            }

            return quadKey.ToString();
        }

        #endregion Methods
    }
}