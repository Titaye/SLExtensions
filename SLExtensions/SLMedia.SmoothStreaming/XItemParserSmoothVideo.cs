namespace SLMedia.SmoothStreaming
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

    public class XItemParserSmoothVideo : IXItemParser
    {
        #region Methods

        public bool TryParseItem(XElement element, out IMediaItem item)
        {
            item = null;
            if (element.Name != "smoothvideo")
            {
                return false;
            }

            SmoothVideoItem videoItem = new SmoothVideoItem();
            item = videoItem;

            XItemParserMediaItem.ReadItem(videoItem, element);
            //videoItem.Description = element.GetAttribute("description");
            //videoItem.Title = element.GetAttribute("title");
            //videoItem.Thumbnail = element.GetAttribute("thumbnail");
            //videoItem.Source = element.GetAttribute("source");

            //List<IMarkerSource> markerSources = new List<IMarkerSource>();
            //foreach (var caption in element.Elements("captions").Elements("caption"))
            //{
            //    var src = caption.GetAttribute("source");
            //    var language = caption.GetAttribute("language");
            //    Dictionary<string, object> metadata = new Dictionary<string, object>();

            //    if (!string.IsNullOrEmpty(language))
            //    {
            //        metadata.Add(MarkerMetadata.Language, language);
            //    }

            //    Uri uriSrc;
            //    if (Uri.TryCreate(src, UriKind.RelativeOrAbsolute, out uriSrc))
            //    {
            //        markerSources.Add(new WebClientMarkerSource { Source = uriSrc, Metadata = metadata });
            //    }
            //}
            //videoItem.MarkerSources = markerSources.ToArray();
            videoItem.ItemType = "SmoothVideo";

            var joinLive = element.GetBoolAttribute("joinlive");
            if (joinLive.HasValue)
                videoItem.JoinLive = joinLive.Value;

            return true;
        }

        #endregion Methods
    }
}