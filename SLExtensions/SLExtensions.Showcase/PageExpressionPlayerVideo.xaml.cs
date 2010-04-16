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

    using ExpressionMediaPlayer;

    using Microsoft.Web.Expression.Encoder.AdaptiveStreaming;

    using SLMedia.Core;
    using SLMedia.PlaylistProvider.MSNVideo;

    /// <summary>
    /// Adpative streaming object factory
    /// </summary>
    public class AdaptiveStreamingSourceFactory : IMediaStreamSourceFactory
    {
        #region Methods

        /// <summary>
        /// Creates a MediaStreamSource object.
        /// </summary>
        /// <param name="mediaElement">The MediaElement to host the stream.</param>
        /// <param name="uri">The string providing the network location of the video to be streamed</param>
        /// <returns>The new MediaStreamSource object</returns>
        public virtual MediaStreamSource Create(MediaElement mediaElement, Uri uri)
        {
            return new AdaptiveStreamingSource(mediaElement, uri);
        }

        #endregion Methods
    }

    public partial class PageExpressionPlayerVideo : UserControl
    {
        #region Constructors

        public PageExpressionPlayerVideo()
        {
            InitializeComponent();
            myPlayer.MediaStreamSourceFactory = new AdaptiveStreamingSourceFactory();
            Uri uri = new Uri("http://www.leprojecteur.fr/smoothhd/ClientBin/Coral_Reef_Adventure_720.ism/Manifest");
            PlaylistItem playlistItem = new PlaylistItem();
            playlistItem.MediaUrl = uri;
            playlistItem.IsAdaptiveStreaming = true;
            playlistItem.ThumbSource = "";
            playlistItem.Title = "Demo SLExtensions";
            playlistItem.Description = "Demo SLExtensions";

            myPlayer.Playlist.Add(playlistItem);
            myPlayer.Play();
        }

        #endregion Constructors

        #region Methods

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        #endregion Methods
    }
}