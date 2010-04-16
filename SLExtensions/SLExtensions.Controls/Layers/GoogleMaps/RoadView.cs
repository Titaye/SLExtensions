// <copyright file="RoadView.cs" company="Ucaya">
// Distributed under Microsoft Public License (Ms-PL)
// </copyright>
// <author>Thierry Bouquain</author>
namespace SLExtensions.Controls.Layers.GoogleMaps
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    /// <summary>
    /// Display the Road view for googlemap
    /// </summary>
    public class RoadView : GoogleMapSource
    {
        #region Fields

        /// <summary>
        /// The road view type depency property
        /// </summary>
        public static readonly DependencyProperty RoadViewTypeProperty = 
            DependencyProperty.Register(
                "RoadViewType",
                typeof(RoadViewType),
                typeof(RoadView),
                new PropertyMetadata((s, e) => ((RoadView)s).OnRoadViewTypeChanged((RoadViewType)e.OldValue, (RoadViewType)e.NewValue)));

        /// <summary>
        /// Create a random modulo for server load balancong when accessing tile map
        /// </summary>
        private static int modulo = new Random(DateTime.Now.Second).Next(4);

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RoadView"/> class.
        /// </summary>
        public RoadView()
        {
            DefaultStyleKey = typeof(RoadView);
            GMapAPI.Initialize();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the max level of detail.
        /// </summary>
        /// <value>The max level of detail.</value>
        public override uint MaxLevelOfDetail
        {
            get
            {
                return 17;
            }
        }

        /// <summary>
        /// Gets or sets the type of the road view.
        /// </summary>
        /// <value>The type of the road view.</value>
        public RoadViewType RoadViewType
        {
            get
            {
                return (RoadViewType)GetValue(RoadViewTypeProperty);
            }

            set
            {
                SetValue(RoadViewTypeProperty, value);
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Gets the tile image URL.
        /// </summary>
        /// <param name="tileXY">The tile XY.</param>
        /// <param name="leveOfDetail">The leve of detail.</param>
        /// <returns>the imate uri</returns>
        public override Uri GetTileImageUrl(Point tileXY, uint leveOfDetail)
        {
            if (!GMapAPIReady || GMapAPI.Instance.SatelliteMapUrls == null || GMapAPI.Instance.SatelliteMapUrls.Length == 0)
                return null;

            //http://mt2.google.com/mt?v=w2.86&hl=en&x=730&y=1474&z=12&s=
            //http://mt2.google.com/mt?v=w2t.86&hl=en&x=730&y=1474&z=12&s=
            //http://mt2.google.com/mt?v=w2p.87&hl=en&x=730&y=1474&z=12&s=

            //http://khm3.google.com/kh?v=33&hl=en&x=2281&y=3307&z=13&s=Galile
            // http://kh0.google.com/kh?n=404&v=8&t=tq
            // http://mt0.google.com/mt?n=404&v=&x=0&y=0&zoom=16
            //int srv = (int)(Math.Pow(2, tileXY.X) + tileXY.Y + modulo) % 4;
            //return new Uri(
            //    string.Format(
            //        "http://{0}{1}.google.com/{0}?n=404&v=&t={2}",
            //        "kh",
            //        srv,
            //        this.TileXYToQuadKey(tileXY, leveOfDetail)));

            //Always get the same tile from the same server
            int srv = (int)(Math.Pow(2, tileXY.X) + tileXY.Y + modulo) % GMapAPI.Instance.SatelliteMapUrls.Length;
            string baseUrl;
            switch (RoadViewType)
            {
                default:
                case RoadViewType.Default:
                    baseUrl =GMapAPI.Instance.DefaultMapUrls[srv];
                    break;
                case RoadViewType.RoadOnly:
                    baseUrl =GMapAPI.Instance.RoadOnlyMapUrls[srv];
                    break;
                case RoadViewType.Terrain:
                    baseUrl =GMapAPI.Instance.TerrainUrls[srv];
                    break;
            }

            return new Uri(
                string.Format(
                    "{0}&x={1}&y={2}&z={3}&hl={4}",
                    baseUrl,
                    tileXY.X,
                    tileXY.Y,
                    leveOfDetail,
                    CultureInfo.CurrentCulture.TwoLetterISOLanguageName.ToLower()));

            // http://kh0.google.com/kh?n=404&v=8&t=tq
            // http://mt0.google.com/mt?n=404&v=&x=0&y=0&zoom=16
            //int srv = (int)(Math.Pow(2, tileXY.X) + tileXY.Y + modulo) % 4;
            //string view = null;
            //switch (RoadViewType)
            //{
            //    case RoadViewType.RoadOnly:
            //        view = "&v=w2t.75";
            //        break;
            //    case RoadViewType.Terrain:
            //        view = "&v=w2p.71";
            //        break;
            //}

            //return new Uri(
            //    string.Format(
            //        "http://{0}{1}.google.com/{0}?x={2}&y={3}&z={4}{5}&hl={6}",
            //        "mt",
            //        srv,
            //        tileXY.X,
            //        tileXY.Y,
            //        leveOfDetail,
            //        view,
                    //CultureInfo.CurrentCulture.TwoLetterISOLanguageName.ToLower()));
        }

        /// <summary>
        /// called when the road view type changed.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnRoadViewTypeChanged(RoadViewType oldValue, RoadViewType newValue)
        {
            foreach (var item in TileDataList)
            {
                item.Uri = this.GetTileImageUrl(item.IndexLocation, item.LevelOfDetail);
            }
        }

        #endregion Methods
    }
}