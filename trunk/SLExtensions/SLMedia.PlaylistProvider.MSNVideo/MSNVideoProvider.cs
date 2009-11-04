namespace SLMedia.PlaylistProvider.MSNVideo
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
    using System.Xml.Linq;

    using SLExtensions;

    using SLMedia.Core;

    public class MSNVideoProvider : NotifyingObject, IPlaylistSource
    {
        #region Fields

        private IEnumerable<IMediaItem> playlist;
        private Uri source;

        #endregion Fields

        #region Properties

        public IEnumerable<IMediaItem> Playlist
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

        public Uri Source
        {
            get { return source; }
            set
            {
                if (source != value)
                {
                    source = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.Source));
                    ReloadPlaylist();
                }
            }
        }

        #endregion Properties

        #region Methods

        public void LoadPlaylist(Action<IEnumerable<IMediaItem>> callback)
        {
            WebClient wcMsnCalagog = new WebClient();
            wcMsnCalagog.DownloadStringCompleted += new DownloadStringCompletedEventHandler(wcMsnCalagog_DownloadStringCompleted);
            wcMsnCalagog.DownloadStringAsync(Source, callback);
        }

        public void ReloadPlaylist()
        {
            LoadPlaylist(pl => Playlist = pl);
        }

        void wcMsnCalagog_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            Action<IEnumerable<IMediaItem>> callback = e.UserState as Action<IEnumerable<IMediaItem>>;
            try
            {
                if (e.Error != null)
                {
                    if (callback != null)
                        callback(null);
                    return;
                }

                XDocument doc = XDocument.Parse(e.Result);
                IEnumerable<Video> result = Video.FromXml(doc);
                if (callback != null)
                    callback(result.Cast<IMediaItem>().ToArray());
            }
            catch
            {
                if (callback != null)
                    callback(null);
            }
        }

        #endregion Methods
    }
}