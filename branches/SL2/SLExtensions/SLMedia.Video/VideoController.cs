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

    using SLMedia.Core;

    public class VideoController : MediaController
    {
        #region Fields

        // Using a DependencyProperty as the backing store for MediaElement.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MediaElementProperty = 
            DependencyProperty.RegisterAttached("MediaElement", typeof(VideoController), typeof(VideoController), new PropertyMetadata(MediaElementChangedCallback));

        private double bufferingProgress;
        private TimeSpan end;
        private double fps;
        private bool inRefreshPosition = false;
        private bool isBuffering;
        private MediaElement mediaElement;
        private TimeSpan start;
        private VideoAdapter videoAdapter;

        #endregion Fields

        #region Constructors

        public VideoController()
        {
            string datatemplate = @"<DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
            xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
            <Rectangle Fill='{Binding VideoBrush}'/>
            </DataTemplate>";

            this.FullscreenPopupTemplate = XamlReader.Load(datatemplate) as DataTemplate;
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

        public virtual MediaElement MediaElement
        {
            get { return mediaElement; }
            set
            {
                if (mediaElement != value)
                {
                    if (this.mediaElement != null)
                    {
                        this.mediaElement.CurrentStateChanged -= new RoutedEventHandler(MediaElement_CurrentStateChanged);
                        this.mediaElement.BufferingProgressChanged -= new RoutedEventHandler(mediaElement_BufferingProgressChanged);
                        this.mediaElement.DownloadProgressChanged -= new RoutedEventHandler(mediaElement_DownloadProgressChanged);
                        this.mediaElement.MediaOpened -= new RoutedEventHandler(mediaElement_MediaOpened);
                        this.mediaElement.MediaEnded -= new RoutedEventHandler(mediaElement_MediaEnded);
                        this.mediaElement.MouseLeftButtonDown -= new MouseButtonEventHandler(mediaElement_MouseLeftButtonDown);
                    }

                    this.mediaElement = value;

                    if (this.mediaElement != null)
                    {
                        this.mediaElement.CurrentStateChanged += new RoutedEventHandler(MediaElement_CurrentStateChanged);
                        this.mediaElement.BufferingProgressChanged += new RoutedEventHandler(mediaElement_BufferingProgressChanged);
                        this.mediaElement.DownloadProgressChanged += new RoutedEventHandler(mediaElement_DownloadProgressChanged);
                        this.mediaElement.MediaOpened += new RoutedEventHandler(mediaElement_MediaOpened);
                        this.mediaElement.MediaEnded += new RoutedEventHandler(mediaElement_MediaEnded);
                        this.mediaElement.MouseLeftButtonDown += new MouseButtonEventHandler(mediaElement_MouseLeftButtonDown);
                    }
                    RefreshStates();
                    this.OnPropertyChanged(this.GetPropertyName(n => n.MediaElement));
                    this.OnPropertyChanged(this.GetPropertyName(n => n.VideoBrush));
                }
            }
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
            get { return this.videoAdapter; }
            protected set
            {
                if (this.videoAdapter != value)
                {
                    if (this.videoAdapter != null)
                    {
                        this.videoAdapter.Controller = null;
                        this.videoAdapter.Dispose();
                    }

                    this.videoAdapter = value;
                    if (this.videoAdapter != null)
                    {
                        this.videoAdapter.Controller = this;
                    }

                    this.OnPropertyChanged(this.GetPropertyName(n => n.VideoAdapter));
                }
            }
        }

        public VideoBrush VideoBrush
        {
            get
            {
                if (MediaElement == null)
                    return null;

                VideoBrush videobrush = new VideoBrush();
                videobrush.Stretch = Stretch.Uniform;
                videobrush.SetSource(MediaElement);
                return videobrush;
            }
        }

        #endregion Properties

        #region Methods

        public static VideoController GetMediaElement(DependencyObject obj)
        {
            return (VideoController)obj.GetValue(MediaElementProperty);
        }

        public static void SetMediaElement(DependencyObject obj, VideoController value)
        {
            obj.SetValue(MediaElementProperty, value);
        }

        protected override void IsPlayingChanged()
        {
            base.IsPlayingChanged();

            if (MediaElement == null)
                return;

            if (IsPlaying == true)
                MediaElement.Play();
            else
                MediaElement.Pause();
        }

        protected override void OnCurrentItemChanged()
        {
            Start = TimeSpan.Zero;
            End = TimeSpan.Zero;

            base.OnCurrentItemChanged();
            if (MediaElement != null)
            {
                VideoAdapter = VideoSourceAdapter.GetVideoAdapter(MediaElement);
            }
        }

        protected override void OnPositionChanged(TimeSpan oldValue, TimeSpan newValue)
        {
            if (!inRefreshPosition && MediaElement != null)
            {
                MediaElement.Position = Position;
                //Debug.WriteLine("Set position " + Position + " " + MediaElement.Position);
                //Debug.WriteLine("Get position " + Position);
            }
            base.OnPositionChanged(oldValue, newValue);
        }

        protected override void RefreshPosition()
        {
            if (MediaElement != null)
            {
                FPS = MediaElement.RenderedFramesPerSecond;
            }

            inRefreshPosition = true;
            try
            {
                base.RefreshPosition();
                if (MediaElement != null)
                {
                    Position = MediaElement.Position;
                    //Debug.WriteLine("GetTime " + MediaElement.Position);                    
                }
            }
            finally
            {
                inRefreshPosition = false;
            }
        }

        protected override void TickScriptCommands()
        {
            double seconds = MediaElement.Position.TotalSeconds;

            var q = from c in ScriptCommands
                    where //c.Type == TypeCommand.Pub &&
                          seconds > c.Time && seconds < (c.Time + c.Duration)
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

        private static void MediaElementChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VideoController controller = e.NewValue as VideoController;
            if (controller != null)
            {
                controller.MediaElement = (MediaElement)d;
            }
        }

        void MediaElement_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            RefreshMediaElementState();
        }

        private void RefreshMediaElementState()
        {
            if (MediaElement.CurrentState == MediaElementState.Buffering)
            {
                IsBuffering = true;
            }
            else
            {
                IsBuffering = false;
            }

            switch (MediaElement.CurrentState)
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
                    if (mediaElement.NaturalDuration.HasTimeSpan)
                    {
                        if (mediaElement.NaturalDuration.HasTimeSpan && mediaElement.Position == mediaElement.NaturalDuration.TimeSpan)
                        {
                            mediaElement.Stop();
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
                    mediaElement.Stop();
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

        void mediaElement_BufferingProgressChanged(object sender, RoutedEventArgs e)
        {
            BufferingProgress = MediaElement.BufferingProgress;
        }

        void mediaElement_DownloadProgressChanged(object sender, RoutedEventArgs e)
        {
            DownloadProgress = this.MediaElement.DownloadProgress;
            if (DownloadProgress > 0 && DownloadProgress < 1)
                IsDownloading = true;
            else
                IsDownloading = false;
        }

        void mediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (IsChaining)
            {
                Next();
            }
        }

        void mediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            Start = TimeSpan.Zero;
            End = Start.Add(MediaElement.NaturalDuration.TimeSpan);
            Duration = MediaElement.NaturalDuration;
        }

        void mediaElement_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SwitchPauseOnClick();
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

        public override void Dispose()
        {
            base.Dispose();
            VideoAdapter = null;
        }
    }
}
