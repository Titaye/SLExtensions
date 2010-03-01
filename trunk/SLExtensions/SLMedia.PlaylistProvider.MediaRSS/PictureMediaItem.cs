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

    public class PictureMediaItem : MediaItem
    {
        #region Constructors

        public PictureMediaItem()
        {
        }

        public PictureMediaItem(MediaRssItem item)
            : base(item)
        {
        }

        #endregion Constructors

        #region Properties

        public override string Thumbnail
        {
            get
            {
                if (string.IsNullOrEmpty(Thumbnail) || Thumbnails == null || Thumbnails.Count == 0)
                {
                    return Source;
                }

                return base.Thumbnail;
            }
            set
            {
                base.Thumbnail = value;
            }
        }

        #endregion Properties
    }
}