namespace SLMedia.PlaylistProvider.MediaRSS
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.ServiceModel.Syndication;
    using System.Windows;
    using System.Windows.Browser;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.Xml;
    using System.Xml.Linq;

    using SLExtensions;
    using SLExtensions.Collections.ObjectModel;
    using SLExtensions.Diagnostics;

    using SLMedia.Core;

    public class MediaRssProvider : PlaylistSource, IPlaylistSource
    {
        #region Fields

        private const string ContentType = "type";
        private const string HtmlType = "html";
        private const string MediaAdult = "adult";
        private const string MediaCategory = "category";
        private const string MediaContent = "content";
        private const string MediaContentAttributeUrl = "url";
        private const string MediaCopyright = "copyright";
        private const string MediaCredit = "credit";
        private const string MediaDescription = "description";
        private const string MediaGroup = "group";
        private const string MediaHash = "hash";
        private const string MediaKeywords = "keywords";
        private const string MediaPlayer = "player";
        private const string MediaRating = "rating";
        private const string MediaRatingAttributeScheme = "scheme";
        private const string MediaRssNamespace = "http://search.yahoo.com/mrss/";
        private const string MediaText = "text";
        private const string MediaThumbnail = "thumbnail";
        private const string MediaTitle = "title";
        private const string PlainType = "plain";

        private string[] extensionArrays;
        private string extensionFilter;

        #endregion Fields

        #region Constructors

        public MediaRssProvider()
        {
            //Playlist = new Playlist();
            ExtensionFilter = ".jpg, *.jpeg, .png, .wmv, .asx, .wsx";
        }

        #endregion Constructors

        #region Properties

        public string ExtensionFilter
        {
            get { return extensionFilter; }
            set
            {
                if (extensionFilter != value)
                {
                    extensionFilter = value;
                    if (value != null)
                    {
                        extensionArrays = (from k in value.Split(',')
                                           select k.Trim().ToLower()).ToArray();
                    }
                    else
                        extensionArrays = null;

                    this.OnPropertyChanged(this.GetPropertyName(n => n.ExtensionFilter));
                }
            }
        }

        #endregion Properties

        #region Methods

        public static IEnumerable<MediaRssItem> FromFeed(SyndicationFeed feed)
        {
            return from item in feed.Items
                   let mediaRssVideo = FromFeedItem(item)
                   where mediaRssVideo != null
                   select mediaRssVideo;
        }

        public static MediaRssItem FromFeedItem(SyndicationItem feedItem)
        {
            MediaRssItem item = new MediaRssItem();
            item.Id = item.Id;

            foreach (var extensions in feedItem.ElementExtensions)
            {
                if (extensions.OuterNamespace == MediaRssNamespace)
                {
                    if (extensions.OuterName == MediaGroup)
                    {
                        XElement groupNode = (XElement)XDocument.ReadFrom(extensions.GetReader());
                        item.Content = (from g in groupNode.Elements(XName.Get(MediaContent, MediaRssNamespace))
                                        select MediaRSS.Content.FromXml(g)).ToArray();
                    }
                    else if (extensions.OuterName == MediaContent)
                    {
                        XElement contentNode = (XElement)XDocument.ReadFrom(extensions.GetReader());
                        item.Content = new Content[] { MediaRSS.Content.FromXml(contentNode) };
                    }
                    else if (extensions.OuterName == MediaTitle)
                    {
                        XElement titleNode = (XElement)extensions.GetReader().GetXNode();

                        string type = titleNode.GetAttribute(ContentType) ?? PlainType;

                        if (type == HtmlType)
                            item.Title = HttpUtility.HtmlDecode(titleNode.Value);
                        else
                            item.Title = titleNode.Value;
                    }
                    else if (extensions.OuterName == MediaDescription)
                    {
                        XElement descriptionNode = (XElement)extensions.GetReader().GetXNode();

                        string type = descriptionNode.GetAttribute(ContentType) ?? PlainType;

                        if (type == HtmlType)
                            item.Description = HttpUtility.HtmlDecode(descriptionNode.Value);
                        else
                            item.Description = descriptionNode.Value;
                    }
                    else if (extensions.OuterName == MediaAdult)
                    {
                        item.Rating = new Rating { Scheme = Rating.RatingSchemeSimple, Value = Rating.SimpleRatingAdult };
                    }
                    else if (extensions.OuterName == MediaRating)
                    {
                        XElement rating = (XElement)extensions.GetReader().GetXNode();
                        item.Rating = new Rating { Scheme = rating.GetAttribute(MediaRatingAttributeScheme), Value = rating.Value };
                    }
                    else if (extensions.OuterName == MediaKeywords)
                    {
                        string keywords = extensions.GetReader().ReadInnerXml();

                        item.Keywords = (from kw in keywords.Split(',')
                                         select kw.Trim()).ToArray();

                    }
                    else if (extensions.OuterName == MediaThumbnail)
                    {
                        XElement thumbnail = (XElement)extensions.GetReader().GetXNode();
                        item.Thumbnails.Add(Thumbnail.FromXml(thumbnail));
                    }
                    else if (extensions.OuterName == MediaCategory)
                    {
                        XElement category = (XElement)extensions.GetReader().GetXNode();
                        item.Categories.Add(MediaRSS.Category.FromXml(category));
                    }

                    //MediaHash
                    //MediaCredit
                    //MediaPlayer
                    //MediaCopyright
                    //MediaText

                }
            }

            if (string.IsNullOrEmpty(item.Title) && feedItem.Title != null)
                item.Title = feedItem.Title.Text;

            if (string.IsNullOrEmpty(item.Description) && feedItem.Summary != null)
                item.Description = feedItem.Summary.Text;

            return item;
        }

        public static IMediaItem GetMediaItemFromRssItem(MediaRssItem item, string[] extensionsFilter)
        {
            item.Content = (from c in item.Content
                            let uri = new Uri(c.Url, UriKind.RelativeOrAbsolute)
                            where extensionsFilter.Contains(System.IO.Path.GetExtension(uri.LocalPath).ToLower())
                            select c).ToArray();
            var content = item.Content.FirstOrDefault();

            if (content == null)
                return null;

            if ((content.Medium.HasValue && content.Medium.Value == Medium.Video)
                || (content.Type != null && content.Type.StartsWith("video")))
                return new VideoMediaItem(item);

            if ((content.Medium.HasValue && content.Medium.Value == Medium.Image)
                || (content.Type != null && content.Type.StartsWith("image")))
                return new PictureMediaItem(item);

            return null;
        }

        protected override IEnumerable<IMediaItem> ParseContent(string content)
        {
            StringReader stringReader = new StringReader(content);
            Rss20FeedFormatter feedformatter = new Rss20FeedFormatter();
            feedformatter.PreserveAttributeExtensions = true;
            feedformatter.PreserveElementExtensions = true;
            XmlReader reader = XmlReader.Create(stringReader);
            feedformatter.ReadFrom(reader);

            var result = from item in FromFeed(feedformatter.Feed)
                         let media = GetMediaItemFromRssItem(item, extensionArrays)
                         where media != null
                         select media;

            return result.ToArray();
        }

        #endregion Methods
    }
}