namespace SLMedia.Video
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

    public class MediaElementVideoAdapter : VideoAdapter
    {
        #region Fields

        public static readonly MediaElementVideoAdapter Instance = new MediaElementVideoAdapter();

        private MediaElement mediaElement;

        #endregion Fields

        #region Properties

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
                    return mediaElement.CurrentState;
                }
                return MediaElementState.Closed;
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
                    return mediaElement.NaturalDuration;
                }
                return new Duration();
            }
        }

        public override TimeSpan EndPosition
        {
            get
            {
                if (mediaElement != null && mediaElement.NaturalDuration.HasTimeSpan)
                {
                    return mediaElement.NaturalDuration.TimeSpan;
                }
                return TimeSpan.Zero;
            }
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

                mediaElement.ClearValue(MediaElement.SourceProperty);
                mediaElement.ClearValue(MediaElement.IsMutedProperty);
                mediaElement.ClearValue(MediaElement.VolumeProperty);
                mediaElement.ClearValue(MediaElement.AutoPlayProperty);
                mediaElement.ClearValue(MediaElement.AudioStreamIndexProperty);
            }
            mediaElement = null;
            base.Dispose();
        }

        public override void Pause()
        {
            if (mediaElement != null)
            {
                if(mediaElement.CanPause)
                    mediaElement.Pause();
                else
                {
                    mediaElement.Stop();
                }
            }
        }

        public override void Play()
        {
            if (mediaElement != null)
            {
                mediaElement.Play();
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
            mediaElement = new MediaElement();

            mediaElement.CurrentStateChanged += mediaElement_CurrentStateChanged;
            mediaElement.BufferingProgressChanged += new RoutedEventHandler(mediaElement_BufferingProgressChanged);
            mediaElement.DownloadProgressChanged += new RoutedEventHandler(mediaElement_DownloadProgressChanged);
            mediaElement.MediaOpened += OnMediaOpened;
            mediaElement.MediaEnded += OnMediaEnded;
            mediaElement.MediaFailed += new EventHandler<ExceptionRoutedEventArgs>(mediaElement_MediaFailed);

            mediaElement.SetBinding(MediaElement.SourceProperty, new System.Windows.Data.Binding("Controller.CurrentItem.SourceUri") { Source = this });
            mediaElement.SetBinding(MediaElement.IsMutedProperty, new System.Windows.Data.Binding("Controller.IsMuted") { Source = this });
            mediaElement.SetBinding(MediaElement.VolumeProperty, new System.Windows.Data.Binding("Controller.Volume") { Source = this });
            mediaElement.SetBinding(MediaElement.AutoPlayProperty, new System.Windows.Data.Binding("Controller.AutoPlay") { Source = this });

            mediaElement.SetBinding(MediaElement.AudioStreamIndexProperty, new System.Windows.Data.Binding("AudioStreamIndex") { Source = this });

            return mediaElement;
        }

        protected override void OnMediaOpened(object sender, RoutedEventArgs e)
        {
            this.AudioStreamCount = mediaElement.AudioStreamCount;
            this.NaturalVideoHeight = mediaElement.NaturalVideoHeight;
            this.NaturalVideoWidth = mediaElement.NaturalVideoWidth;
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

        void mediaElement_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            OnMediaFailed(this, e);
        }

        #endregion Methods
    }
}