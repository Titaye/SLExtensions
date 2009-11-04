namespace SLMedia.PlaylistProvider.MediaRSS
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Windows;
    using System.Windows.Browser;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    public class MediaRssItem
    {
        #region Constructors

        public MediaRssItem()
        {
            Content = new Content[0];
            Keywords = new string[0];
            Thumbnails = new List<Thumbnail>();
            Categories = new List<Category>();
        }

        #endregion Constructors

        #region Properties

        [ScriptableMember]
        public List<Category> Categories
        {
            get; protected set;
        }

        public IEnumerable<Content> Content
        {
            get; set;
        }

        [ScriptableMember]
        public string Description
        {
            get; set;
        }

        [ScriptableMember]
        public string Id
        {
            get; set;
        }

        [ScriptableMember]
        public IEnumerable<string> Keywords
        {
            get; set;
        }

        [ScriptableMember]
        public Rating Rating
        {
            get; set;
        }

        public IList<Thumbnail> Thumbnails
        {
            get; set;
        }

        [ScriptableMember]
        public string Title
        {
            get; set;
        }

        #endregion Properties
    }
}