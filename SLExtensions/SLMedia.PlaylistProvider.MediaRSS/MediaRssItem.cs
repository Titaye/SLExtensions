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

    public class MediaRssItem : SLMedia.Core.MediaItem
    {
        #region Constructors

        public MediaRssItem()
        {
            Content = new Content[0];
            Keywords = new string[0];
            Thumbnails = new List<Thumbnail>();
        }

        #endregion Constructors

        #region Properties

      

        public IEnumerable<Content> Content
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

        #endregion Properties
    }
}