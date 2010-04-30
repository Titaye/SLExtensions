// <copyright file="AerialView.cs" company="Ucaya">
// Distributed under Microsoft Public License (Ms-PL)
// </copyright>
// <author>Thierry Bouquain</author>
namespace SLExtensions.Controls.Layers.GoogleMaps
{
    using System;
    using System.Globalization;
    using System.Text;
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
    public class AerialView : GoogleMapSource
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
                return 20;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Gets the tile image URL.
        /// </summary>
        /// <param name="tileXY">The tile XY.</param>
        /// <param name="leveOfDetail">The leve of detail.</param>
        /// <returns>the image uri</returns>
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
            string baseUrl = GMapAPI.Instance.SatelliteMapUrls[srv];

            return new Uri(
                string.Format(
                    "{0}&x={1}&y={2}&z={3}&hl={4}",
                    baseUrl,
                    tileXY.X,
                    tileXY.Y,
                    leveOfDetail,
                    CultureInfo.CurrentCulture.TwoLetterISOLanguageName.ToLower()));
        }

        /// <summary>
        /// Convert a tile coord to a quadkey
        /// </summary>
        /// <param name="pt">the tile coord</param>
        /// <param name="levelOfDetail">the level of detail</param>
        /// <returns>the quadkey</returns>
        public string TileXYToQuadKey(Point pt, uint levelOfDetail)
        {
            StringBuilder quadKey = new StringBuilder();
            for (int i = (int)levelOfDetail; i > 0; i--)
            {
                char digit = 'q';
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

            quadKey.Replace('t', 'z');
            quadKey.Replace('s', 't');
            quadKey.Replace('z', 's');
            quadKey.Insert(0, new char[] { 't' });
            return quadKey.ToString();
        }

        #endregion Methods
    }
}