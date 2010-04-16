namespace SLMedia.PlaylistProvider.MediaRSS
{
    using System;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    /// <summary>
    /// <media:rating scheme="urn:simple">adult</media:rating>       
    /// <media:rating scheme="urn:icra">r (cz 1 lz 1 nz 1 oz 1 vz 1)</media:rating>
    /// <media:rating scheme="urn:mpaa">pg</media:rating>
    /// <media:rating scheme="urn:v-chip">tv-y7-fv</media:rating>
    /// </summary>
    public class Rating
    {
        #region Fields

        public const string RatingSchemeICRA = "urn:icra";
        public const string RatingSchemeMPAA = "urn:mpaa";
        public const string RatingSchemeSimple = "urn:simple";
        public const string RatingSchemeVChip = "urn:v-chip";
        public const string SimpleRatingAdult = "adult";
        public const string SimpleRatingNonAdult = "nonadult";

        #endregion Fields

        #region Properties

        public string Scheme
        {
            get; set;
        }

        public string Value
        {
            get; set;
        }

        #endregion Properties
    }
}