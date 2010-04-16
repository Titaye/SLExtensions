namespace SLMedia.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.Xml.Linq;

    using SLExtensions;
    using SLExtensions.Collections.ObjectModel;

    [ContentProperty("ItemParsers")]
    public class PlaylistSource : NotifyingObject, IPlaylistSource
    {
        #region Fields

        private string content;
        private Uri contentSource;
        private IEnumerable<IMediaItem> playlist;

        #endregion Fields

        #region Constructors

        public PlaylistSource()
        {
            Playlist = new ObservableCollection<IMediaItem>();
            ItemParsers = new List<IXItemParser>();
        }

        #endregion Constructors

        #region Events

        public event EventHandler PlaylistChanged;

        #endregion Events

        #region Properties

        public string Content
        {
            get { return this.content; }
            set
            {
                if (this.content != value)
                {
                    this.content = value;
                    if (this.content != null)
                    {
                        Playlist = ParseContent(this.content);
                    }
                }
            }
        }

        public Uri ContentSource
        {
            get { return this.contentSource; }
            set
            {
                if (this.contentSource != value)
                {
                    this.contentSource = value;

                    if (value != null)
                    {
                        WebClient client = new WebClient();
                        client.DownloadStringCompleted += (snd, e) =>
                        {
                            this.Content = e.Result;
                        };

                        client.DownloadStringAsync(value);
                    }
                }
            }
        }

        public List<IXItemParser> ItemParsers
        {
            get; private set;
        }

        public virtual IEnumerable<IMediaItem> Playlist
        {
            get { return playlist; }
            set
            {
                if (playlist != value)
                {
                    playlist = value;
                    this.OnPlaylistChanged();
                }
            }
        }

        #endregion Properties

        #region Methods

        protected virtual void OnPlaylistChanged()
        {
            if (PlaylistChanged != null)
            {
                PlaylistChanged(this, EventArgs.Empty);
            }
        }

        protected virtual IEnumerable<IMediaItem> ParseContent(string content)
        {
            List<IMediaItem> playlistItems = new List<IMediaItem>();
            XDocument doc = XDocument.Parse(content);
            foreach (var mediaNode in doc.Root.Elements())
            {
                foreach (var parser in ItemParsers)
                {
                    IMediaItem mediaItem;
                    if(parser.TryParseItem(mediaNode, out mediaItem))
                    {
                        playlistItems.Add(mediaItem);
                        break;
                    }
                }
            }

            return playlistItems;
        }

        #endregion Methods
    }
}