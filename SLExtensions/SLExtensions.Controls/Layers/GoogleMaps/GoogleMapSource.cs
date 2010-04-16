// <copyright file="GoogleMapSource.cs" company="Ucaya">
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
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    using SLExtensions.Controls.Layers.MapProjections;

    /// <summary>
    /// Base tile source for GoogleMap
    /// </summary>
    public abstract class GoogleMapSource : TileSource
    {
        #region Fields

        private bool gMapAPIReady;

        /// <summary>
        /// Current map projection
        /// </summary>
        private IMapProjection mapProjection;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleMapSource"/> class.
        /// </summary>
        public GoogleMapSource()
        {
            ItemTemplate = (DataTemplate)XamlReader.Load(Resource.GLayersDataTemplate);
            GMapAPI.Instance.Ready += delegate { GMapAPIReady = true; };
        }

        #endregion Constructors

        #region Properties

        public bool GMapAPIReady
        {
            get
            {
                return this.gMapAPIReady;
            }

            private set
            {
                this.gMapAPIReady = value;
                if (value)
                {
                    foreach (var item in TileDataList)
                    {
                        item.Uri = GetTileImageUrl(item.IndexLocation, item.LevelOfDetail);
                    }
                }
            }
        }

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
    }
}