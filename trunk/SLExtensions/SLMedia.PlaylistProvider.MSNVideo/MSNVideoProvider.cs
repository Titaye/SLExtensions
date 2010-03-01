namespace SLMedia.PlaylistProvider.MSNVideo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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

    using SLMedia.Core;

    public class MSNVideoProvider : PlaylistSource
    {
        #region Methods

        protected override IEnumerable<IMediaItem> ParseContent(string content)
        {
            XDocument doc = XDocument.Parse(content);
            return Video.FromXml(doc);
        }

        #endregion Methods
    }
}