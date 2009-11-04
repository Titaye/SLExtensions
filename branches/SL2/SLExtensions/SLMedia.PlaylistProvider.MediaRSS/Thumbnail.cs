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
    using System.Xml.Linq;

    using SLExtensions;

    public class Thumbnail
    {
        #region Properties

        public int? Height
        {
            get; set;
        }

        public TimeSpan? Time
        {
            get; set;
        }

        public string Url
        {
            get; set;
        }

        public int? Width
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public static Thumbnail FromXml(XElement elem)
        {
            Thumbnail tn = new Thumbnail();
            tn.Url = elem.GetAttribute("url");

            tn.Width = elem.GetIntAttribute("width");
            tn.Height = elem.GetIntAttribute("height");
            tn.Time = elem.GetTimeSpanAttribute("time");
            return tn;
        }

        #endregion Methods
    }
}