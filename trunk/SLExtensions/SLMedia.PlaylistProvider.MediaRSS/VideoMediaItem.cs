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

    public class VideoMediaItem : MediaItem
    {
        #region Constructors

        public VideoMediaItem()
        {
        }

        public VideoMediaItem(MediaRssItem item)
            : base(item)
        {
        }

        #endregion Constructors
    }
}