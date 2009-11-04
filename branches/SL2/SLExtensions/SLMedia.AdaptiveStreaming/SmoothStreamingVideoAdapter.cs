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

    using SLMedia.SmoothStreaming;
    using SLMedia.Video;

    public class SmoothStreamingVideoAdapter : VideoAdapter, INotifyPropertyChanged
    {
        #region Fields

        private ulong[] bitrates;
        private double defaultSecondsReview = 8;
        private long downloadBitrate;
        private TimeSpan end;
        private bool isLive;
        private bool isLiveJoined;
        private TimeSpan livePosition;
        private MediaElement mediaElement;
        private bool needJoinLive;
        private long playBitrate;
        private DispatcherTimer timerRefresh;
        private SynchronizationContext uiSyncContext;

        #endregion Fields

        #region Constructors

        public SmoothStreamingVideoAdapter(MediaElement mediaElement, Uri newSource)
        {
            this.mediaElement = mediaElement;
            SmoothStreamingSource = new SmoothStreamingMediaStreamSource(mediaElement, newSource);
            SmoothStreamingSource.DownloadBitrateChange += new EventHandler<BitrateChangedEventArgs>(SmoothStreamingSource_DownloadBitrateChange);
            SmoothStreamingSource.PlayBitrateChange += new EventHandler<BitrateChangedEventArgs>(SmoothStreamingSource_PlayBitrateChange);
            this.mediaElement.MediaOpened += new RoutedEventHandler(mediaElement_MediaOpened);
            this.mediaElement.CurrentStateChanged += new RoutedEventHandler(mediaElement_CurrentStateChanged);

            timerRefresh = new DispatcherTimer();
            timerRefresh.Interval = TimeSpan.FromMilliseconds(100);
            timerRefresh.Tick += RefreshInfo;

            uiSyncContext = SynchronizationContext.Current;
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

        public override VideoController Controller
        {
            get
            {
                return base.Controller;
            }
            set
            {
                if (value != null)
                {
                    value.PositionChanging -= value_PositionChanging;
                }

                base.Controller = value;

                if (value != null)
                {
                    value.PositionChanging += value_PositionChanging;
                }

            }
        }

        public double DefaultSecondsReview
        {
            get { return this.defaultSecondsReview; }
            set
            {
                if (this.defaultSecondsReview != value)
                {
                    this.defaultSecondsReview = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.DefaultSecondsReview));
                }
            }
        }

        public long DownloadBitrate
        {
            get { return this.downloadBitrate; }
            set
            {
                if (this.downloadBitrate != value)
                {
                    this.downloadBitrate = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.DownloadBitrate));
                }
            }
        }


        private double downloadBitrateLimit;
        
        public double DownloadBitrateLimit
        {
            get { return this.downloadBitrateLimit; }
            set
            {
                if (this.downloadBitrateLimit != value)
                {
                    this.downloadBitrateLimit = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.DownloadBitrateLimit));
                    SmoothStreamingSource.SetBitrateRange(MediaStreamType.Video, 0, (long)value * 1000);
                }
            }
        }

        public TimeSpan End
        {
            get { return this.end; }
            set
            {
                if (this.end != value)
                {
                    this.end = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.End));
                }
            }
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

        public bool IsLiveJoined
        {
            get { return this.isLiveJoined; }
            set
            {
                if (this.isLiveJoined != value)
                {
                    this.isLiveJoined = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.IsLiveJoined));
                }
            }
        }

        public TimeSpan LivePosition
        {
            get { return this.livePosition; }
            set
            {
                if (this.livePosition != value)
                {
                    this.livePosition = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.LivePosition));
                }
            }
        }

        public long PlayBitrate
        {
            get { return this.playBitrate; }
            set
            {
                if (this.playBitrate != value)
                {
                    this.playBitrate = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.PlayBitrate));
                }
            }
        }

        public SmoothStreamingMediaStreamSource SmoothStreamingSource
        {
            get;
            private set;
        }

        public override MediaStreamSource Source
        {
            get { return SmoothStreamingSource; }
        }

        #endregion Properties

        #region Methods

        public override void Dispose()
        {
            base.Dispose();
            timerRefresh.Stop();
            this.mediaElement.MediaOpened -= new RoutedEventHandler(mediaElement_MediaOpened);
            this.mediaElement.CurrentStateChanged -= new RoutedEventHandler(mediaElement_CurrentStateChanged);
            SmoothStreamingSource.Dispose();
        }

        public void GoToLive()
        {
            if (IsLive && Controller != null)
            {
                Controller.End = TimeSpan.FromSeconds(SmoothStreamingSource.SuggestedLivePosition);
                Controller.Position = Controller.End;
            }
        }

        private double reviewSeconds = 8;
        public double ReviewSeconds
        {
            get { return this.reviewSeconds; }
            set
            {
                if (this.reviewSeconds != value)
                {
                    this.reviewSeconds = value;
                    this.RaisePropertyChanged(n => n.ReviewSeconds);
                }
            }
        }


        public void Review()
        {
            if (Controller != null)
                Controller.Position = TimeSpan.FromSeconds(Math.Max(Controller.Start.TotalSeconds, Controller.Position.TotalSeconds - Math.Max(ReviewSeconds,0)));
        }

        void RefreshInfo(object sender, EventArgs e)
        {
            //LivePosition = TimeSpan.FromSeconds(SmoothStreamingSource.SuggestedLivePosition);
            if (IsLive)
            {
                //if (Controller.End != TimeSpan.FromSeconds(SmoothStreamingSource.SuggestedLivePosition))
                //{
                //    Debug.WriteLine("livepos " + SmoothStreamingSource.SuggestedLivePosition);
                //}
                if (Controller != null)
                {
                    Controller.End = TimeSpan.FromSeconds(SmoothStreamingSource.SuggestedLivePosition);
                    Controller.Duration = Controller.End - Controller.Start;

                    IsLiveJoined = (TimeSpan.FromSeconds(SmoothStreamingSource.SuggestedLivePosition) - Controller.Position).Duration() <= mediaElement.BufferingTime.Add(TimeSpan.FromSeconds(4));
                }
            }
        }

        void SmoothStreamingSource_DownloadBitrateChange(object sender, BitrateChangedEventArgs e)
        {
            if (e.StreamType == MediaStreamType.Video)
            {
                uiSyncContext.Post(delegate
                {
                    DownloadBitrate = (long)e.Bitrate;
                }, null);
            }
        }

        void SmoothStreamingSource_PlayBitrateChange(object sender, BitrateChangedEventArgs e)
        {
            if (e.StreamType == MediaStreamType.Video)
            {
                uiSyncContext.Post(delegate
                {
                    PlayBitrate = (long)e.Bitrate;
                }, null);
            }
        }

        void mediaElement_CurrentStateChanged(object sender, RoutedEventArgs e)
        {            
            if (needJoinLive && mediaElement.CurrentState == MediaElementState.Playing)
            {
                needJoinLive = false;
                GoToLive();
            }
        }

        void mediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
                TimeSpan end;
                TimeSpan start;

                IsLive = SmoothStreamingSource.IsLive;
                
                if (IsLive)
                    end = TimeSpan.FromSeconds(SmoothStreamingSource.SuggestedLivePosition);
                else
                    end = TimeSpan.FromSeconds(SmoothStreamingSource.ManifestDuration + SmoothStreamingSource.StartTime);
                
                start = TimeSpan.FromSeconds(SmoothStreamingSource.StartTime);

                if (Controller != null)
                {
                    Controller.End = TimeSpan.FromSeconds(Math.Max(end.TotalSeconds, start.TotalSeconds));
                    Controller.Start = start;
                }
                
                Bitrates = (from ul in SmoothStreamingSource.Bitrates(MediaStreamType.Video)
                            select ul).ToArray();

                if (Controller != null)
                {

                    LiveVideoItem item = Controller.CurrentItem as LiveVideoItem;
                    if (item != null)
                    {
                        if (item.JoinLive)
                        {
                            needJoinLive = true;
                            //GoToLive();
                        }
                    }
                }

                if (IsLive)
                {
                    timerRefresh.Start();
                }
        }

        void value_PositionChanging(object sender, RoutedPropertyChangedEventArgs<TimeSpan> e)
        {
            if (SmoothStreamingSource != null && Controller != null && !Controller.InRefreshPosition)
            {
                // Set position directly to streamingsource
                SmoothStreamingSource.SeekTimeOverride = e.NewValue.TotalSeconds;
            }
        }

        #endregion Methods
    }
}
