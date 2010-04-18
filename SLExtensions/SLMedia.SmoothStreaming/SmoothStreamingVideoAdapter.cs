namespace SLMedia.SmoothStreaming
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.Windows.Threading;

    using Microsoft.Web.Media.SmoothStreaming;

    using SLExtensions;
    using SLExtensions.Input;

    using SLMedia.SmoothStreaming;
    using SLMedia.Video;

    public class SmoothStreamingVideoAdapter : VideoAdapter, INotifyPropertyChanged
    {
        #region Fields

        public static readonly SmoothStreamingVideoAdapter Instance = new SmoothStreamingVideoAdapter();

        private ulong[] bitrates;
        private ulong downloadBitrate;
        private bool isLive;
        private SmoothStreamingMediaElement mediaElement;
        private ulong playbackBitrate;

        #endregion Fields

        #region Constructors

        public SmoothStreamingVideoAdapter()
        {
            GoToLiveCommand = new Command(new Action(GoToLive));
        }

        #endregion Constructors

        #region Properties

        public ulong[] Bitrates
        {
            get { return this.bitrates; }
            set
            {
                if (this.bitrates != value)
                {
                    this.bitrates = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.Bitrates));
                }
            }
        }

        public override double BufferingProgress
        {
            get
            {
                if (mediaElement != null)
                {
                    return mediaElement.BufferingProgress;
                }
                return 0;
            }
        }

        public override MediaElementState CurrentState
        {
            get
            {
                if (mediaElement != null)
                {
                    switch (mediaElement.CurrentState)
                    {
                        case SmoothStreamingMediaElementState.AcquiringLicense:
                            return MediaElementState.AcquiringLicense;
                        case SmoothStreamingMediaElementState.Buffering:
                            return MediaElementState.AcquiringLicense;
                        case SmoothStreamingMediaElementState.ClipPlaying:
                        case SmoothStreamingMediaElementState.Playing:
                            return MediaElementState.Playing;
                        case SmoothStreamingMediaElementState.Closed:
                            return MediaElementState.Closed;
                        case SmoothStreamingMediaElementState.Individualizing:
                            return MediaElementState.Individualizing;
                        case SmoothStreamingMediaElementState.Opening:
                            return MediaElementState.Opening;
                        case SmoothStreamingMediaElementState.Paused:
                            return MediaElementState.Paused;
                        case SmoothStreamingMediaElementState.Stopped:
                            return MediaElementState.Stopped;
                    }
                }
                return MediaElementState.Closed;
            }
        }

        public ulong DownloadBitrate
        {
            get { return this.downloadBitrate; }
            private set
            {
                if (this.downloadBitrate != value)
                {
                    this.downloadBitrate = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.DownloadBitrate));
                }
            }
        }

        public override double DownloadProgress
        {
            get
            {
                if (mediaElement != null)
                {
                    return mediaElement.DownloadProgress;
                }
                return 0;
            }
        }

        public override Duration Duration
        {
            get
            {
                if (mediaElement != null)
                {
                    return mediaElement.Duration;
                }
                return new Duration();
            }
        }

        public override TimeSpan EndPosition
        {
            get
            {
                if (mediaElement != null)
                    return mediaElement.EndPosition;
                return TimeSpan.Zero;
            }
        }

        public ICommand GoToLiveCommand
        {
            get; private set;
        }

        public bool IsLive
        {
            get { return this.isLive; }
            set
            {
                if (this.isLive != value)
                {
                    this.isLive = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.IsLive));
                }
            }
        }

        public SmoothStreamingMediaElement MediaElement
        {
            get { return mediaElement; }
        }

        public override Duration NaturalDuration
        {
            get
            {
                if (mediaElement != null)
                {
                    return mediaElement.NaturalDuration;
                }
                return new Duration();
            }
        }

        public ulong PlaybackBitrate
        {
            get { return this.playbackBitrate; }
            private set
            {
                if (this.playbackBitrate != value)
                {
                    this.playbackBitrate = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.PlaybackBitrate));
                }
            }
        }

        public override TimeSpan Position
        {
            get
            {
                if (mediaElement != null)
                {
                    return mediaElement.Position;
                }
                else
                    return TimeSpan.Zero;
            }
            set
            {
                if (mediaElement != null)
                {
                    mediaElement.Position = value;
                }
            }
        }

        public override double RenderedFramesPerSecond
        {
            get
            {
                if (mediaElement != null)
                {
                    return mediaElement.RenderedFramesPerSecond;
                }
                return 0;
            }
        }

        public override TimeSpan StartPosition
        {
            get
            {
                if(mediaElement != null)
                    return mediaElement.StartPosition;
                return TimeSpan.Zero;
            }
        }

        #endregion Properties

        #region Methods

        public override void Dispose()
        {
            if (mediaElement != null)
            {
                mediaElement.CurrentStateChanged -= OnCurrentStateChanged;
                mediaElement.BufferingProgressChanged -= OnBufferingProgressChanged;
                mediaElement.DownloadProgressChanged -= OnDownloadProgressChanged;
                mediaElement.MediaOpened -= OnMediaOpened;
                mediaElement.MediaEnded -= OnMediaEnded;

                mediaElement.ClearValue(SmoothStreamingMediaElement.SourceProperty);
                mediaElement.ClearValue(SmoothStreamingMediaElement.IsMutedProperty);
                mediaElement.ClearValue(SmoothStreamingMediaElement.VolumeProperty);
                mediaElement.ClearValue(SmoothStreamingMediaElement.AutoPlayProperty);
            }
            mediaElement = null;
            base.Dispose();
        }

        public void GoToLive()
        {
            if (mediaElement != null && mediaElement.IsLive)
                mediaElement.StartSeekToLive();
        }

        public override void Pause()
        {
            if (mediaElement != null)
            {
                mediaElement.Pause();
            }
        }

        public override void Play()
        {
            if (mediaElement != null)
            {
                mediaElement.Play();
            }
        }

        public void Review()
        {
            if (mediaElement != null)
            {
                var position = Math.Max(mediaElement.StartPosition.TotalSeconds, mediaElement.Position.TotalSeconds - 6);
                mediaElement.Position = TimeSpan.FromSeconds(position);
            }
        }

        public override void Stop()
        {
            if (mediaElement != null)
            {
                mediaElement.Stop();
            }
        }

        protected override object CreateDisplayControl()
        {
            mediaElement = new SmoothStreamingMediaElement();
            mediaElement.EnableGPUAcceleration = true;
            mediaElement.CurrentStateChanged += mediaElement_CurrentStateChanged;
            mediaElement.BufferingProgressChanged += mediaElement_BufferingProgressChanged;
            mediaElement.DownloadProgressChanged += mediaElement_DownloadProgressChanged;
            mediaElement.MediaOpened += OnMediaOpened;
            mediaElement.MediaEnded += OnMediaEnded;
            mediaElement.MediaFailed += new EventHandler<ExceptionRoutedEventArgs>(mediaElement_MediaFailed);
            mediaElement.DownloadTrackChanged += new EventHandler<TrackChangedEventArgs>(mediaElement_DownloadTrackChanged);
            mediaElement.DurationExtended += new EventHandler<DurationExtendedEventArgs>(mediaElement_DurationExtended);
            mediaElement.PlaybackTrackChanged += new EventHandler<TrackChangedEventArgs>(mediaElement_PlaybackTrackChanged);
            SmoothVideoItem svi = Controller.CurrentItem as SmoothVideoItem;
            if (svi != null && svi.JoinLive)
            {
                mediaElement.LivePlaybackStartPosition = PlaybackStartPosition.End;
            }

            mediaElement.SetBinding(SmoothStreamingMediaElement.SmoothStreamingSourceProperty, new System.Windows.Data.Binding("Controller.CurrentItem.SourceUri") { Source = this });
            mediaElement.SetBinding(SmoothStreamingMediaElement.IsMutedProperty, new System.Windows.Data.Binding("Controller.IsMuted") { Source = this });
            mediaElement.SetBinding(SmoothStreamingMediaElement.VolumeProperty, new System.Windows.Data.Binding("Controller.Volume") { Source = this });
            mediaElement.SetBinding(SmoothStreamingMediaElement.AutoPlayProperty, new System.Windows.Data.Binding("Controller.AutoPlay") { Source = this });

            return mediaElement;
        }

        protected override void OnMediaEnded(object sender, RoutedEventArgs e)
        {
            base.OnMediaEnded(sender, e);
            IsLive = false;
        }

        protected override void OnMediaOpened(object sender, RoutedEventArgs e)
        {
            //SmoothVideoItem svi = Controller.CurrentItem as SmoothVideoItem;
            //if (svi != null && MediaElement.IsLive)
            //{
            //    GoToLive();
            //}
            NaturalVideoHeight = mediaElement.NaturalVideoHeight;
            NaturalVideoWidth = mediaElement.NaturalVideoWidth;
            IsLive = MediaElement.IsLive;

            var info = mediaElement.GetStreamInfoForStreamType("video");
            if(info != null)
            {
                Bitrates = (from i in info.SelectedTracks
                           select i.Bitrate).ToArray();

            }
            //Bitrates
            base.OnMediaOpened(sender, e);
        }

        void mediaElement_BufferingProgressChanged(object sender, RoutedEventArgs e)
        {
            OnPropertyChanged(this.GetPropertyName(o => o.BufferingProgress));
            OnBufferingProgressChanged(this, e);
        }

        void mediaElement_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            OnPropertyChanged(this.GetPropertyName(o => o.CurrentState));
            OnCurrentStateChanged(this, e);
        }

        void mediaElement_DownloadProgressChanged(object sender, RoutedEventArgs e)
        {
            OnPropertyChanged(this.GetPropertyName(o => o.DownloadProgress));
            OnDownloadProgressChanged(this, e);
        }

        void mediaElement_DownloadTrackChanged(object sender, TrackChangedEventArgs e)
        {
            if (e.StreamType == MediaStreamType.Video)
                DownloadBitrate = e.Track.Bitrate;
        }

        void mediaElement_DurationExtended(object sender, DurationExtendedEventArgs e)
        {
            //EndPosition = MediaElement.EndPosition;
            //Controller.End = Controller.Start.Add(NaturalDuration.TimeSpan);
        }

        void mediaElement_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            OnMediaFailed(this, e);
        }

        void mediaElement_PlaybackTrackChanged(object sender, TrackChangedEventArgs e)
        {
            if (e.StreamType == MediaStreamType.Video)
                PlaybackBitrate = e.Track.Bitrate;
        }

        #endregion Methods
    }
}
