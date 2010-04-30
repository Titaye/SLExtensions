namespace SLMedia.PlaylistProvider.MSNVideo
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

    //<file formatCode="2009">
    //<uri>http://img4.catalog.video.msn.com/image.aspx?uuid=be5bf55e-2a89-4314-91ff-f44cbcade1fe&w=400&h=300</uri>
    //</file>
    public class File
    {
        #region Properties

        public string FormatCode
        {
            get; set;
        }

        public Uri Url
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        internal static File FromXml(XElement file)
        {
            File f = new File();
            f.FormatCode = file.GetAttribute("formatCode");
            f.Url = new Uri(file.GetElementValue(XName.Get("uri", Video.NamespaceMsnVideoCatalog)));
            return f;
        }

        #endregion Methods
    }
}