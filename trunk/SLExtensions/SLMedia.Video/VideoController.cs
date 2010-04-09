namespace SLMedia.Video
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Windows;
    using System.Windows.Browser;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.Windows.Threading;

    using SLExtensions;
    using SLExtensions.Collections.ObjectModel;
    using SLExtensions.Input;

    using SLMedia.Core;

    public class VideoController : MediaController
    {
        #region Fields

        private double bufferingProgress;
        private TimeSpan end;
        private double fps;
        private bool inRefreshPosition = false;
        private bool isBuffering;
        private TimeSpan start;
        private VideoAdapter videoAdapter;
        private object videoContent;

        #endregion Fields

        #region Constructors

        public VideoController()
        {
            string datatemplate = @"<DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
            xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
            <Rectangle Fill='{Binding VideoBrush}'/>
            </DataTemplate>";

            this.FullscreenPopupTemplate = XamlReader.Load(datatemplate) as DataTemplate;
            var setAudioTrack = new Command();
            setAudioTrack.Executed += new EventHandler<ExecutedEventArgs>(setAudioTrack_Executed);
            SetAudioTrack = setAudioTrack;
        }

        #endregion Constructors

        #region Properties

        public double BufferingProgress
        {
            get { return bufferingProgress; }
            set
            {
                if (bufferingProgress != value)
                {
                    bufferingProgress = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.BufferingProgress));
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

        public double FPS
        {
            get { return this.fps; }
            set
            {
                if (this.fps != value)
                {
                    this.fps = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.FPS));
                }
            }
        }

        public bool InRefreshPosition
        {
            get
            {
                return inRefreshPosition;
            }
        }

        public virtual bool IsBuffering
        {
            get { return isBuffering; }
            set
            {
                if (isBuffering != value)
                {
                    isBuffering = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.IsBuffering));
                }
            }
        }

        public ICommand SetAudioTrack
        {
            get; private set;
        }

        public TimeSpan Start
        {
            get { return this.start; }
            set
            {
                if (this.start != value)
                {
                    this.start = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.Start));
                }
            }
        }

        public virtual VideoAdapter VideoAdapter
        {
            get { return videoAdapter; }
            set
            {
                if (videoAdapter != value)
                {
                    if (this.videoAdapter != null)
                    {
                        this.videoAdapter.CurrentStateChanged -= new RoutedEventHandler(VideoAdapter_CurrentStateChanged);
                        this.videoAdapter.BufferingProgressChanged -= new RoutedEventHandler(videoAdapter_BufferingProgressChanged);
                        this.videoAdapter.DownloadProgressChanged -= new RoutedEventHandler(videoAdapter_DownloadProgressChanged);
                        this.videoAdapter.MediaOpened -= new RoutedEventHandler(videoAdapter_MediaOpened);
                        this.videoAdapter.MediaEnded -= new RoutedEventHandler(videoAdapter_MediaEnded);
                    }

                    this.videoAdapter = value;

                    if (this.videoAdapter != null)
                    {
                        this.videoAdapter.CurrentStateChanged += new RoutedEventHandler(VideoAdapter_CurrentStateChanged);
                        this.videoAdapter.BufferingProgressChanged += new RoutedEventHandler(videoAdapter_BufferingProgressChanged);
                        this.videoAdapter.DownloadProgressChanged += new RoutedEventHandler(videoAdapter_DownloadProgressChanged);
                        this.videoAdapter.MediaOpened += new RoutedEventHandler(videoAdapter_MediaOpened);
                        this.videoAdapter.MediaEnded += new RoutedEventHandler(videoAdapter_MediaEnded);
                    }
                    RefreshStates();
                    this.OnPropertyChanged(this.GetPropertyName(n => n.VideoAdapter));
                    //this.OnPropertyChanged(this.GetPropertyName(n => n.VideoBrush));
                }
            }
        }

        public object VideoContent
        {
            get { return this.videoContent; }
            set
            {
                if (this.videoContent != value)
                {
                    this.videoContent = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.VideoContent));
                }
            }
        }

        #endregion Properties

        #region Methods

        protected override void IsPlayingChanged()
        {
            base.IsPlayingChanged();

            SetPlayStateToVideoAdapter();
        }

        protected override void OnCurrentItemChanged()
        {
            Start = TimeSpan.Zero;
            End = TimeSpan.Zero;

            VideoAdapter newAdapter = null;
            IVideoItem videoItem = CurrentItem as IVideoItem;
            if (videoItem != null)
            {
                newAdapter = videoItem.VideoAdapter;
            }

            if (VideoAdapter != null
                && VideoAdapter != newAdapter)
            {
                VideoAdapter.Release();
            }

            if (newAdapter != null)
            {
                newAdapter.Controller = this;
            }
            VideoAdapter = newAdapter;
            if (VideoAdapter != null)
            {
                VideoAdapter.Adapt(this);
            }

            base.OnCurrentItemChanged();
        }

        protected override void OnPositionChanged(TimeSpan oldValue, TimeSpan newValue)
        {
            if (!inRefreshPosition && VideoAdapter != null)
            {
                VideoAdapter.Position = Position;
            }
            base.OnPositionChanged(oldValue, newValue);
        }

        protected override void RefreshPosition()
        {
            if (VideoAdapter != null)
            {
                FPS = VideoAdapter.RenderedFramesPerSecond;
            }

            inRefreshPosition = true;
            try
            {
                base.RefreshPosition();
                if (VideoAdapter != null)
                {
                    Duration = VideoAdapter.Duration;
                    End = VideoAdapter.EndPosition;
                    Start = VideoAdapter.StartPosition;

                    if (VideoAdapter.Position > End)
                        End = VideoAdapter.Position;

                    Position = VideoAdapter.Position;
                }
            }
            finally
            {
                inRefreshPosition = false;
            }
        }

        protected override void TickScriptCommands()
        {
            double seconds = VideoAdapter.Position.TotalSeconds;

            var q = from c in ScriptCommands
                    where seconds > c.Time && seconds < (c.Time + c.Duration)
                    select c;

            if (q.Any())
            {
                ScriptCommandItem command = q.First();
                if (command.Visible == false)
                {
                    //Load XAML and Display
                    command.Visible = true;

                    if (command.Type == TypeCommand.Pub)
                    {
                        WebClient wcLoadContent = new WebClient();
                        wcLoadContent.DownloadStringCompleted += new DownloadStringCompletedEventHandler(wcLoadContent_DownloadStringCompleted);
                        wcLoadContent.DownloadStringAsync(new Uri(command.Command, UriKind.RelativeOrAbsolute));
                    }
                    if (command.Type == TypeCommand.Link)
                    {
                        WebClient wcLoadContent = new WebClient();
                        wcLoadContent.DownloadStringCompleted += new DownloadStringCompletedEventHandler(wcLoadContentLink_DownloadStringCompleted);
                        wcLoadContent.DownloadStringAsync(new Uri("Link/Link.xaml", UriKind.RelativeOrAbsolute), command.Command);
                    }
                    if (command.Type == TypeCommand.Log)
                    {
                        WebClient wcLoadContent = new WebClient();
                        wcLoadContent.DownloadStringAsync(new Uri(command.Command, UriKind.RelativeOrAbsolute));
                    }
                }
            }
            else
            {
                foreach (var item in ScriptCommands)
                {
                    item.Visible = false;
                }
                ContentPub.Content = null;
            }
        }

        private void RefreshMediaElementState()
        {
            if (VideoAdapter.CurrentState == MediaElementState.Buffering)
            {
                IsBuffering = true;
            }
            else
            {
                IsBuffering = false;
            }

            switch (VideoAdapter.CurrentState)
            {
                case MediaElementState.AcquiringLicense:
                    break;
                case MediaElementState.Buffering:
                    break;
                case MediaElementState.Closed:
                    break;
                case MediaElementState.Individualizing:
                    break;
                case MediaElementState.Opening:
                    break;
                case MediaElementState.Paused:
                    if (VideoAdapter.Duration.HasTimeSpan)
                    {
                        if (VideoAdapter.Duration.HasTimeSpan && videoAdapter.Position == VideoAdapter.Duration.TimeSpan)
                        {
                            videoAdapter.Stop();
                            PlayState = PlayStates.Stopped;
                            break;
                        }
                    }

                    PlayState = PlayStates.Paused;
                    break;
                case MediaElementState.Playing:
                    PlayState = PlayStates.Playing;
                    break;
                case MediaElementState.Stopped:
                    videoAdapter.Stop();
                    PlayState = PlayStates.Stopped;
                    break;
                default:
                    break;
            }
        }

        private void RefreshStates()
        {
            RefreshMediaElementState();
        }

        private void SetPlayStateToVideoAdapter()
        {
            if (VideoAdapter == null)
                return;

            if (IsPlaying == true)
                VideoAdapter.Play();
            else
                VideoAdapter.Pause();
        }

        void VideoAdapter_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            RefreshMediaElementState();
        }

        void setAudioTrack_Executed(object sender, ExecutedEventArgs e)
        {
            AudioTrack track = e.Parameter as AudioTrack;
            if (track != null && track.Index >= 0 && track.Index < VideoAdapter.AudioStreamCount)
            {
                VideoAdapter.AudioStreamIndex = track.Index;
            }
            else if (e.Parameter is int)
            {
                int trackIndex = (int)e.Parameter;
                if (trackIndex >= 0 && trackIndex < VideoAdapter.AudioStreamCount)
                {
                    VideoAdapter.AudioStreamIndex = trackIndex;
                }
            }
        }

        void videoAdapter_BufferingProgressChanged(object sender, RoutedEventArgs e)
        {
            BufferingProgress = VideoAdapter.BufferingProgress;
        }

        void videoAdapter_DownloadProgressChanged(object sender, RoutedEventArgs e)
        {
            DownloadProgress = this.VideoAdapter.DownloadProgress;
            if (DownloadProgress > 0 && DownloadProgress < 1)
                IsDownloading = true;
            else
                IsDownloading = false;
        }

        void videoAdapter_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (IsChaining)
            {
                Next();
            }
        }

        void videoAdapter_MediaOpened(object sender, RoutedEventArgs e)
        {
            Start = TimeSpan.Zero;
            if (VideoAdapter.Duration.HasTimeSpan)
            {
                End = Start.Add(VideoAdapter.Duration.TimeSpan);
            }
            Duration = VideoAdapter.Duration;
        }

        void wcLoadContentLink_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                UserControl uc = XamlReader.Load(e.Result) as UserControl;
                Object o = uc.FindName("HLButtonLink");
                if (o != null)
                {
                    HyperlinkButton hlButton = (HyperlinkButton)o;
                    hlButton.Content = (string)e.UserState;
                }
                ContentPub.Content = uc;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error Load Pub : " + ex.Message);
            }
        }

        void wcLoadContent_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                UserControl uc = XamlReader.Load(e.Result) as UserControl;
                ContentPub.Content = uc;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error Load Pub : " + ex.Message);
            }
        }

        #endregion Methods

        #region Other

        //public VideoAdapter VideoAdapter
        //{
        //    get { return this.videoAdapter; }
        //    protected set
        //    {
        //        if (this.videoAdapter != value)
        //        {
        //            if (this.videoAdapter != null)
        //            {
        //                this.videoAdapter.Controller = null;
        //                this.videoAdapter.Dispose();
        //            }
        //            this.videoAdapter = value;
        //            if (this.videoAdapter != null)
        //            {
        //                this.videoAdapter.Controller = this;
        //            }
        //            this.OnPropertyChanged(this.GetPropertyName(n => n.VideoAdapter));
        //        }
        //    }
        //}
        //public VideoBrush VideoBrush
        //{
        //    get
        //    {
        //        if (VideoAdapter == null)
        //            return null;
        //        VideoBrush videobrush = new VideoBrush();
        //        videobrush.Stretch = Stretch.Uniform;
        //        videobrush.SetSource(VideoAdapter);
        //        return videobrush;
        //    }
        //}
        //protected override void GoToPositionExecuted(object sender, TimeSpan position)
        //{
        //    base.GoToPositionExecuted(sender, position);
        //}

        #endregion Other
    }
}