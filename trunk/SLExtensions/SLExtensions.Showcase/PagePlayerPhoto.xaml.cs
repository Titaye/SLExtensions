namespace SLExtensions.Showcase
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    using SLMedia.Core;
    using SLMedia.Picture;
    using SLMedia.PlaylistProvider.MediaRSS;

    public partial class PagePlayerPhoto : UserControl
    {
        #region Constructors

        public PagePlayerPhoto()
        {
            InitializeComponent();

            MediaRssProvider rssProvider = new MediaRssProvider();
            Uri url = new Uri("http://api.flickr.com/services/feeds/photos_public.gne?tags=ninja636&lang=en-us&format=rss_200");
            rssProvider.ContentSource = url;
            player.Controller.PlaylistSource = rssProvider;
        }

        #endregion Constructors
    }
}