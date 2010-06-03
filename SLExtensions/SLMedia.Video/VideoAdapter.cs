namespace SLMedia.Video
{
    using System;
    using System.Linq;
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
    using System.Windows.Data;

    public abstract class VideoAdapter : NotifyingObject, IDisposable
    {
        #region Fields

        private int audioStreamCount;
        private int audioStreamIndex;
        private AudioTrack audioTrack;
        private object content;
        private int naturalVideoHeight;
        private int naturalVideoWidth;

        #endregion Fields

        #region Constructors

        public VideoAdapter()
        {
        }

        #endregion Constructors

        #region Events

        public event RoutedEventHandler BufferingProgressChanged;

        public event RoutedEventHandler CurrentStateChanged;

        public event RoutedEventHandler DownloadProgressChanged;

        public event RoutedEventHandler MediaEnded;

        public event EventHandler<ExceptionRoutedEventArgs> MediaFailed;

        public event RoutedEventHandler MediaOpened;

        #endregion Events

        #region Properties

        public int AudioStreamCount
        {
            get { return this.audioStreamCount; }
            protected set
            {
                if (this.audioStreamCount != value)
                {
                    this.audioStreamCount = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.AudioStreamCount));
                }
            }
        }

        public int AudioStreamIndex
        {
            get { return this.audioStreamIndex; }
            set
            {
                if (this.audioStreamIndex != value)
                {
                    this.audioStreamIndex = value;

                    SetAudioTrackFromStreamIndex(value);

                    this.OnPropertyChanged(this.GetPropertyName(n => n.AudioStreamIndex));
                }
            }
        }

        //public virtual Binding SourceBinding { get; set; }
        //public virtual DependencyProperty SourceBindingProperty { get; set; }

        public AudioTrack AudioTrack
        {
            get { return this.audioTrack; }
            set
            {
                if (this.audioTrack != value)
                {
                    this.audioTrack = value;
                    if (value != null && value.Index < AudioStreamCount)
                    {
                        AudioStreamIndex = value.Index;
                    }
                    else
                    {
                        AudioStreamIndex = 0;
                    }

                    if (this.audioTrack != null)
                    {
                        this.audioTrack.IsActive = true;
                        VideoItem vi = CurrentItem as VideoItem;
                        if (vi != null)
                        {
                            foreach (var at in vi.AudioTracks)
                            {
                                if (at != this.audioTrack)
                                    at.IsActive = false;
                            }
                        }
                    }

                    this.OnPropertyChanged(this.GetPropertyName(n => n.AudioTrack));
                }
            }
        }

        public abstract double BufferingProgress
        {
            get;
        }

        public object Content
        {
            get
            {
                if (this.content == null)
                {
                    this.Content = CreateDisplayControl();
                    OnDisplayControlCreated();
                }
                return this.content;
            }
            protected set
            {
                if (this.content != value)
                {
                    this.content = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.Content));
                }
            }
        }

        public abstract MediaElementState CurrentState
        {
            get;
        }

        public abstract double DownloadProgress
        {
            get;
        }

        public abstract Duration Duration
        {
            get;
        }

        public abstract TimeSpan EndPosition
        {
            get;
        }

        public abstract Duration NaturalDuration
        {
            get;
        }

        public int NaturalVideoHeight
        {
            get { return this.naturalVideoHeight; }
            set
            {
                if (this.naturalVideoHeight != value)
                {
                    this.naturalVideoHeight = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.NaturalVideoHeight));
                }
            }
        }

        public int NaturalVideoWidth
        {
            get { return this.naturalVideoWidth; }
            set
            {
                if (this.naturalVideoWidth != value)
                {
                    this.naturalVideoWidth = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.NaturalVideoWidth));
                }
            }
        }

        public abstract TimeSpan Position
        {
            get;
            set;
        }

        public abstract double RenderedFramesPerSecond
        {
            get;
        }

        public abstract TimeSpan StartPosition
        {
            get;
        }

        public abstract bool CanPause
        {
            get;
        }

        public abstract bool CanSeek
        {
            get;
        }

        public bool ReuseDisplayControl { get; set; }

        private IVideoItem currentItem;
        public IVideoItem CurrentItem
        {
            get { return this.currentItem; }
            set
            {
                if (this.currentItem != value)
                {
                    this.currentItem = value;
                    this.RaisePropertyChanged(n => n.CurrentItem);
                }
            }
        }


        private VideoController controller;
        public VideoController Controller
        {
            get { return this.controller; }
            set
            {
                if (this.controller != value)
                {
                    this.controller = value;
                    this.RaisePropertyChanged(n => n.Controller);
                }
            }
        }


        #endregion Properties

        #region Methods        

        //public virtual void Adapt(VideoController videoController)
        //{
        //    if (!ReuseDisplayControl || Content == null)
        //    {
        //        Content = CreateDisplayControl();
        //        OnDisplayControlCreated();
        //    }

        //    Controller = videoController;
        //}

        public virtual void Dispose()
        {
            Stop();
            if (!ReuseDisplayControl)
            {
                DisposeDisplayControl();
                Content = null;
            }
            //Controller = null;
        }

        public abstract void Pause();

        public abstract void Play();

        public abstract void Stop();

        protected abstract object CreateDisplayControl();
        protected abstract void DisposeDisplayControl();

        public event EventHandler DisplayControlCreated;

        protected virtual void OnDisplayControlCreated()
        {
            if (DisplayControlCreated != null)
            {
                DisplayControlCreated(this, EventArgs.Empty);
            }
        }

        public event EventHandler DisplayControlDisposed;

        protected virtual void OnDisplayControlDisposed()
        {
            if (DisplayControlDisposed != null)
            {
                DisplayControlDisposed(this, EventArgs.Empty);
            }
        }

        protected virtual void OnBufferingProgressChanged(object sender, RoutedEventArgs e)
        {
            if (BufferingProgressChanged != null)
                BufferingProgressChanged(sender, e);
        }

        protected virtual void OnCurrentStateChanged(object sender, RoutedEventArgs e)
        {
            if (CurrentStateChanged != null)
                CurrentStateChanged(sender, e);
        }

        protected virtual void OnDownloadProgressChanged(object sender, RoutedEventArgs e)
        {
            if (DownloadProgressChanged != null)
                DownloadProgressChanged(sender, e);
        }

        protected virtual void OnMediaEnded(object sender, RoutedEventArgs e)
        {
            if (MediaEnded != null)
                MediaEnded(sender, e);
        }

        protected virtual void OnMediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            if (MediaFailed != null)
                MediaFailed(sender, e);
        }

        protected virtual void OnMediaOpened(object sender, RoutedEventArgs e)
        {
            SetAudioTrackFromStreamIndex(AudioStreamIndex);

            this.RaisePropertyChanged(_ => _.CanPause);
            this.RaisePropertyChanged(_ => _.CanSeek);

            if (MediaOpened != null)
                MediaOpened(sender, e);
        }

        private void SetAudioTrackFromStreamIndex(int value)
        {
            if (CurrentItem != null)
            {
                VideoItem videoItem = CurrentItem as VideoItem;
                if (videoItem != null && videoItem.AudioTracks != null)
                {
                    var track = (from at in videoItem.AudioTracks
                                 where at.Index == value
                                 select at).FirstOrDefault();
                    AudioTrack = track;
                }
            }
        }

        #endregion Methods
    }
}