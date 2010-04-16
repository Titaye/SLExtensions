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
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    public interface IPlaylistSource
    {
        #region Events

        event EventHandler PlaylistChanged;

        #endregion Events

        #region Properties

        IEnumerable<IMediaItem> Playlist
        {
            get;
        }

        #endregion Properties
    }
}