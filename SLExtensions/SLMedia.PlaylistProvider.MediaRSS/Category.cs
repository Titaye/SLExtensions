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

    public class Category : Core.Category
    {
        #region Properties

        public string Label
        {
            get; set;
        }

        public string Scheme
        {
            get; set;
        }

        public string Value
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public static Category FromXml(XElement elem)
        {
            Category cat = new Category();
            cat.Scheme = elem.GetAttribute("scheme");
            cat.Label = elem.GetAttribute("label");
            cat.Value = elem.Value;
            return cat;
        }

        #endregion Methods
    }
}