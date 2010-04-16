namespace SLMedia.Core
{
    using System;
    using System.Collections.Generic;
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

    public static class XItemParserMediaItem
    {
        #region Methods

        public static void ReadItem(MediaItem item, XElement element)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            item.Description = element.GetAttribute("description");
            item.Title = element.GetAttribute("title");
            item.Thumbnail = element.GetAttribute("thumbnail");
            item.Source = element.GetAttribute("source");

            List<IMarkerSource> markerSources = new List<IMarkerSource>();
            foreach (var caption in element.Elements("captions").Elements("caption"))
            {
                var src = caption.GetAttribute("source");
                var language = caption.GetAttribute("language");
                Dictionary<string, object> metadata = new Dictionary<string, object>();

                if (!string.IsNullOrEmpty(language))
                {
                    metadata.Add(MarkerMetadata.Language, language);
                }

                Uri uriSrc;
                if (Uri.TryCreate(src, UriKind.RelativeOrAbsolute, out uriSrc))
                {
                    markerSources.Add(new WebClientMarkerSource { Source = uriSrc, Metadata = metadata });
                }
            }

            var chapters = element.Element("chapters");
            if (chapters != null)
            {
                MarkerSelector chapterSelector = new MarkerSelector();
                chapterSelector.Metadata.Add(MarkerMetadata.Chapter, string.Empty);
                foreach (var chapter in chapters.Elements("chapter"))
                {
                    MarkerThumbnail marker = new MarkerThumbnail();
                    TimeSpan start;
                    if (TimeSpan.TryParse(chapter.GetAttribute("start"), out start))
                    {
                        marker.Position = start;
                        chapterSelector.Markers.Add(marker);
                    }
                    marker.Content = chapter.GetAttribute("title");
                    var thumb = chapter.GetAttribute("thumbnail");
                    Uri uriThumbnail;
                    if (!string.IsNullOrEmpty(thumb) && Uri.TryCreate(thumb, UriKind.RelativeOrAbsolute, out uriThumbnail))
                        marker.Thumbnail = uriThumbnail;
                }
                item.MarkerSelectors.Add(chapterSelector);
            }

            item.MarkerSources = markerSources.ToArray();
        }

        #endregion Methods
    }
}