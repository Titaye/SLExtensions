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

    using SLExtensions;
    using SLExtensions.Collections.ObjectModel;

    public class PlaylistSource : NotifyingObject, IPlaylistSource
    {
        #region Fields

        private IEnumerable<IMediaItem> playlist;

        #endregion Fields

        #region Constructors

        public PlaylistSource()
        {
            Playlist = new ObservableCollection<IMediaItem>();
        }

        #endregion Constructors

        #region Properties

        public virtual IEnumerable<IMediaItem> Playlist
        {
            get { return playlist; }
            set
            {
                if (playlist != value)
                {
                    playlist = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.Playlist));
                }
            }
        }

        #endregion Properties

        #region Methods

        public void LoadPlaylist(Action<IEnumerable<IMediaItem>> callback)
        {
            if (callback != null)
                callback(Playlist);
        }

        public void ReloadPlaylist()
        {
        }

        #endregion Methods
    }
}