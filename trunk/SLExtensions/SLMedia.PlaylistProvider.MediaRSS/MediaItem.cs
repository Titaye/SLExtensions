namespace SLMedia.PlaylistProvider.MediaRSS
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

    using SLMedia.Core;

    public class MediaItem : MediaRssItem, IMediaItem
    {
        #region Fields

        private List<Core.Category> coreCategories;
        private string[] picturesExtensions = new string[] { ".jpg", ".jpeg", ".png" };

        #endregion Fields

        #region Constructors

        public MediaItem()
        {
            coreCategories = new List<SLMedia.Core.Category>();
        }

        public MediaItem(MediaRssItem item)
        {
            coreCategories = new List<SLMedia.Core.Category>();
            if (item.Categories != null)
                coreCategories.AddRange(item.Categories.OfType<MediaRSS.Category>().Select(c => new Core.Category { Name = c.Label }));

            this.Categories = item.Categories;
            this.Content = item.Content;
            this.Description = item.Description;
            this.Id = item.Id;
            this.Keywords = item.Keywords;
            this.Rating = item.Rating;
            this.Thumbnails = item.Thumbnails;
            this.Title = item.Title;
        }

        #endregion Constructors
    }
}