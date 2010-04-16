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

    //<videoFile formatCode="1002" msnFileId="B193B737-74C0-4602-984A-DA6169862032">
    //<uri>mms://msnvideofr.wmod.llnwd.net/a2926/d1/frfr_cinema/ba_blindness.wmv</uri>
    //</videoFile>
    public class VideoFile
    {
        #region Properties

        public string FormatCode
        {
            get; set;
        }

        public string MsnFileId
        {
            get; set;
        }

        public Uri Url
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        internal static VideoFile FromXml(XElement videoFile)
        {
            VideoFile v = new VideoFile();
            v.FormatCode = videoFile.GetAttribute("formatCode");
            v.MsnFileId = videoFile.GetAttribute("msnFileId");
            v.Url = new Uri(videoFile.GetElementValue(XName.Get("uri", Video.NamespaceMsnVideoCatalog)));
            return v;
        }

        #endregion Methods
    }
}