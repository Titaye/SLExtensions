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

    public static class PlaylistExtensions
    {
        #region Methods

        public static void Add(this ICollection<IMediaItem> playlist, string source)
        {
            playlist.Add(new MediaItem() { Source = source });
        }

        public static void Add(this ICollection<IMediaItem> playlist, string source, string thumbnail)
        {
            playlist.Add(new MediaItem() { Source = source, Thumbnail = thumbnail });
        }

        #endregion Methods
    }
}