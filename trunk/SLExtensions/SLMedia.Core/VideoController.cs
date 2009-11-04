namespace SLMedia.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Net;
    using System.Windows;
    using System.Windows.Browser;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.Windows.Threading;

    using SLExtensions;
    using SLExtensions.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Windows.Markup;

    public class VideoController : MediaController
    {
        #region Fields

        public const string BufferingProgressPropertyName = "BufferingProgress";

        private double bufferingProgress;

        private bool inRefreshPosition = false;
        private bool isBuffering;
        private MediaElement mediaElement;

        #endregion Fields

        #region Constructors

        public VideoController()
        {
            //timer = new DispatcherTimer();
            //timer.Interval = TimeSpan.FromMilliseconds(200);
            //timer.Tick += new EventHandler(timer_Tick);
            string datatemplate = @"<DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' 
    xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' 
             >
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
                    OnPropertyChanged(BufferingProgressPropertyName);
                }
            }
        }

        public bool IsBuffering
        {
            get { return isBuffering; }
            set
            {
                if (isBuffering != value)
                {
                    isBuffering = value;
                    OnPropertyChanged("IsBuffering");
                }
            }
        }

        public MediaElement MediaElement
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
                    OnPropertyChanged(MediaElementPropertyName);
                    OnPropertyChanged(VideoBrushPropertyName);
                }
            }
        }

        void mediaElement_DownloadProgressChanged(object sender, RoutedEventArgs e)
        {
            DownloadProgress = this.MediaElement.DownloadProgress;
            if (DownloadProgress > 0 && DownloadProgress < 1)
                IsDownloading = true;

            Debug.WriteLine("DownloadProgress " + DownloadProgress + " " + IsDownloading);
        }


        public const string MediaElementPropertyName = "MediaElement";

        #endregion Properties

        #region Methods

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

        public const string VideoBrushPropertyName = "VideoBrush";

        //protected override object GetFullscreenDataContext()
        //{
        //    VideoBrush videobrush = new VideoBrush();
        //    videobrush.Stretch = Stretch.Uniform;
        //    videobrush.SetSource(MediaElement);
        //    return videobrush;            
        //}

        //protected override FrameworkElement CreateFullscreenPopupContent()
        //{
        //    if (FullscreenPopup != null)
        //    {
        //        return base.CreateFullscreenPopupContent();
        //    }

        //    Rectangle rect = new Rectangle();

        //    VideoBrush videobrush = new VideoBrush();
        //    videobrush.Stretch = Stretch.Uniform;
        //    videobrush.SetSource(MediaElement);
        //    rect.Fill = videobrush;
        //    return rect;

        //}

        protected override void IsPlayingChanged()
        {
            base.IsPlayingChanged();

            //if (IsPlaying)
            //    timer.Start();
            //else
            //    timer.Stop();

            if (MediaElement == null)
                return;

            if (IsPlaying == true)
                MediaElement.Play();
            else
                MediaElement.Pause();
        }

        protected override void OnPositionChanged()
        {
            base.OnPositionChanged();

            if (!inRefreshPosition && MediaElement != null)
                MediaElement.Position = Position;
        }

        protected override void RefreshControlState()
        {
            if (StateControl == null || System.ComponentModel.DesignerProperties.GetIsInDesignMode(StateControl))
                return;

            base.RefreshControlState();
            VisualStateManager.GoToState(StateControl, IsBuffering ? "Buffering" : "BufferReady", true);
        }

        //private DispatcherTimer timer;
        //void timer_Tick(object sender, EventArgs e)
        //{
        //    RefreshPosition();
        //}
        protected override void RefreshPosition()
        {
            inRefreshPosition = true;
            try
            {
                base.RefreshPosition();
                if (MediaElement != null)
                    Position = MediaElement.Position;
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
                    where c.Type == TypeCommand.Pub &&
                          seconds > c.Time && seconds < (c.Time + c.Duration)
                    select c;

            if (q.Any())
            {
                ScriptCommandItem command = q.First();
                if (command.Visible == false)
                {
                    //Load XAML and Display
                    command.Visible = true;
                    Debug.WriteLine("ScriptCommand pass to visible:" + command.Command);
                    if (command.Type == TypeCommand.Pub)
                    {
                        WebClient wcLoadContent = new WebClient();
                        wcLoadContent.DownloadStringCompleted += new DownloadStringCompletedEventHandler(wcLoadContent_DownloadStringCompleted);
                        wcLoadContent.DownloadStringAsync(new Uri(command.Command, UriKind.RelativeOrAbsolute));
                    }
                }
            }
            else
            {
                ScriptCommands.Select(c => c.Visible = false);
                ContentPub.Content = null;
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

            Debug.WriteLine(MediaElement.CurrentState.ToString() + " " + MediaElement.Source);
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

            RefreshControlState();
        }

        private void RefreshStates()
        {
            //RefreshPosition();

            RefreshMediaElementState();
        }

        void mediaElement_BufferingProgressChanged(object sender, RoutedEventArgs e)
        {
            BufferingProgress = MediaElement.BufferingProgress;
            //Debug.WriteLine("buffering : " + MediaElement.BufferingProgress);
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
            //RefreshPosition();
            Duration = MediaElement.NaturalDuration;
        }

        void mediaElement_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SwitchPauseOnClick();
        }

        #endregion Methods

        private TimeSpan desiredPosition;
        public TimeSpan DesiredPosition
        {
            get { return desiredPosition; }
            set
            {
                if (desiredPosition != value)
                {
                    desiredPosition = value;
                    OnPropertyChanged(DesiredPositionPropertyName);
                }
            }
        }

        public const string DesiredPositionPropertyName = "DesiredPosition";


    }
}
