﻿namespace SLMedia.Video
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

    using SLExtensions;

    using SLMedia.Core;

    public class VideoItem : MediaItem, IVideoItem
    {
        #region Fields

        private AudioTrack[] audioTracks;

        #endregion Fields

        #region Constructors

        public VideoItem()
        {
            //VideoAdapter = MediaElementVideoAdapter.Instance;
            //VideoAdapter = new MediaElementVideoAdapter();
        }

        #endregion Constructors

        #region Properties

        public AudioTrack[] AudioTracks
        {
            get { return this.audioTracks; }
            set
            {
                if (this.audioTracks != value)
                {
                    this.audioTracks = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.AudioTracks));
                }
            }
        }

        private VideoAdapter videoAdapter;
        public VideoAdapter VideoAdapter
        {
            get { return this.videoAdapter; }
            set
            {
                if (this.videoAdapter != value)
                {
                    this.videoAdapter = value;
                    this.RaisePropertyChanged(n => n.VideoAdapter);
                }
            }
        }

        public virtual VideoAdapter GetVideoAdapter(VideoAdapter adapter)
        {
            VideoAdapter = new MediaElementVideoAdapter();
            return VideoAdapter;
        }

        #endregion Properties
    }
}