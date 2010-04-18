// <copyright file="TileSource.cs" company="Ucaya">
// Distributed under Microsoft Public License (Ms-PL)
// </copyright>
// <author>Thierry Bouquain</author>
namespace SLExtensions.Controls.Layers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;

    using SLExtensions.Controls.Layers.MapProjections;

    /// <summary>
    /// Display Tiles on a <see cref="LayerHost"/>
    /// </summary>
    public abstract class TileSource : LayerSource
    {
        #region Fields

        /// <summary>
        /// tile data list
        /// </summary>
        private ObservableCollection<TileSourceData> tileDataList;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TileSource"/> class.
        /// </summary>
        public TileSource()
        {
            this.tileDataList = new ObservableCollection<TileSourceData>();
            this.ItemsSource = this.tileDataList;
            this.Width = this.TileSize * 2;
            this.Height = this.TileSize * 2;
        }

        #endregion Constructors

        #region Events

        /// <summary>
        /// Occurs when property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets the layer source.
        /// </summary>
        /// <value>The layer source.</value>
        public LayerSource LayerSource
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// Gets the map projection.
        /// </summary>
        /// <value>The map projection.</value>
        public abstract IMapProjection MapProjection
        {
            get;
        }

        /// <summary>
        /// Gets the max level of detail.
        /// </summary>
        /// <value>The max level of detail.</value>
        public abstract uint MaxLevelOfDetail
        {
            get;
        }

        /// <summary>
        /// Gets the min level of detail.
        /// </summary>
        /// <value>The min level of detail.</value>
        public abstract uint MinLevelOfDetail
        {
            get;
        }

        /// <summary>
        /// Gets the size of the tile.
        /// </summary>
        /// <value>The size of the tile.</value>
        public abstract uint TileSize
        {
            get;
        }

        /// <summary>
        /// Gets the name of the view.
        /// </summary>
        /// <value>The name of the view.</value>
        public string ViewName
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the tile data list.
        /// </summary>
        /// <value>The tile data list.</value>
        protected ObservableCollection<TileSourceData> TileDataList
        {
            get
            {
                return this.tileDataList;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Clips the host.
        /// </summary>
        /// <param name="clipRect">The clip rect.</param>
        /// <param name="zoom">The zoom value.</param>
        public override void ClipHost(Rect clipRect, double zoom)
        {
            // Compute right level of detail
            uint levelOfDetail = 0;
            int tmpZoom = 1;

            while (tmpZoom <= zoom)
            {
                levelOfDetail++;
                tmpZoom = tmpZoom << 1;
            }

            tmpZoom = tmpZoom >> 1;

            if (levelOfDetail < 1)
            {
                levelOfDetail = 1;
                tmpZoom = 1;
            }

            // Perform clipping
            Rect visibleRect = new Rect(0, 0, Width, Height);
            visibleRect.Intersect(clipRect);

            // tmpGuid is used to Tag visible tiles
            Guid tmpGuid = Guid.NewGuid();

            double mapSize = this.MapProjection.MapSize(levelOfDetail);

            // Convert baseClipping in new level of detail
            Rect visibleRect2 = new Rect();
            if (!visibleRect.IsEmpty)
            {
                visibleRect2 = new Rect(visibleRect.Left * tmpZoom, visibleRect.Top * tmpZoom, visibleRect.Width * tmpZoom, visibleRect.Height * tmpZoom);

                // Get tileindex based points
                Point tilePt1 = this.MapProjection.PixelXYToTileXY(new Point(visibleRect2.Left, visibleRect2.Top), levelOfDetail);
                Point tilePt2 = this.MapProjection.PixelXYToTileXY(new Point(visibleRect2.Right, visibleRect2.Bottom), levelOfDetail);

                for (double x = Math.Floor(tilePt1.X); x <= Math.Ceiling(tilePt2.X); x++)
                {
                    for (double y = Math.Floor(tilePt1.Y); y <= Math.Ceiling(tilePt2.Y); y++)
                    {
                        this.UpdateOrCreateTile(new Point(x, y), tmpGuid, levelOfDetail);
                    }
                }
            }

            // Remove tiles not tagged
            List<int> idxToRemove = new List<int>();
            for (int i = 0; i < this.tileDataList.Count; i++)
            {
                if (!tmpGuid.Equals(this.tileDataList[i].Tag))
                {
                    idxToRemove.Add(i);
                }
            }

            for (int i = idxToRemove.Count - 1; i >= 0; i--)
            {
                this.tileDataList.RemoveAt(idxToRemove[i]);
            }
        }

        /// <summary>
        /// Gets the tile image URL.
        /// </summary>
        /// <param name="tileXY">The tile XY.</param>
        /// <param name="leveOfDetail">The leve of detail.</param>
        /// <returns>the imate uri</returns>
        public abstract Uri GetTileImageUrl(Point tileXY, uint leveOfDetail);

        /// <summary>
        /// Called when property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Updates the or create tile.
        /// </summary>
        /// <param name="indexLocation">The index location.</param>
        /// <param name="tmpGuid">The temp GUID for identifying new items.</param>
        /// <param name="levelOfDetail">The level of detail.</param>
        private void UpdateOrCreateTile(Point indexLocation, Guid tmpGuid, uint levelOfDetail)
        {
            TileSourceData tile = (from t in this.tileDataList
                                   where t.IndexLocation == indexLocation
                                    && t.LevelOfDetail == levelOfDetail
                                   select t).FirstOrDefault();

            double zoom = this.MapProjection.MapSize(levelOfDetail) / this.MapProjection.MapSize(this.MinLevelOfDetail);

            double size = (double)this.TileSize / zoom;
            if (tile == null)
            {
                this.tileDataList.Add(new TileSourceData()
                {
                    Tag = tmpGuid,
                    IndexLocation = indexLocation,
                    Height = size,
                    Width = size,
                    Top = size * indexLocation.Y,
                    Left = size * indexLocation.X,
                    Uri = this.GetTileImageUrl(indexLocation, levelOfDetail),
                    LevelOfDetail = levelOfDetail
                });
            }
            else
            {
                tile.Tag = tmpGuid;
            }
        }

        #endregion Methods
    }
}