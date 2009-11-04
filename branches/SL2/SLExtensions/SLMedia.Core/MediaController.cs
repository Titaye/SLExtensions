namespace SLMedia.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Windows;
    using System.Windows.Browser;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;
    using System.Windows.Threading;
    using System.Xml.Linq;

    using SLExtensions;
    using SLExtensions.Collections.ObjectModel;

    public abstract class MediaController : NotifyingObject, IDisposable
    {
        #region Fields

        // Using a DependencyProperty as the backing store for ContentPup.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentPubProperty = 
            DependencyProperty.RegisterAttached("ContentPub", typeof(MediaController), typeof(MediaController), new PropertyMetadata(ContentPubChangedCallback));

        private bool autoPlay;
        private ContentControl contentPub;
        private IMediaItem currentItem;
        private int currentItemIndex = -1;
        private double downloadProgress;
        private Duration duration;
        private Popup fullscreenPopup = null;
        private DataTemplate fullscreenPopupTemplate;
        private bool isChaining;
        private bool isDownloading;
        private bool isFullscreen;
        private bool isMuted = false;
        private bool isNextEnabled;
        private bool isPlaying;
        private bool isPopupFullscreen;
        private bool isPreviousEnabled;
        private IMediaItem lastSelectedItem;
        private string mediaName;
        private IEnumerable<IMediaItem> nextItems;
        private bool pauseOnClick = true;
        private PlayStates playState;
        private ObservableCollection<IMediaItem> playlist;
        private IPlaylistSource playlistSource;
        private TimeSpan position;
        private IEnumerable<IMediaItem> previousItems;
        private List<ScriptCommandItem> scriptCommands;
        private string scriptCommandsUrl;
        private int? selectedIndex;
        private DispatcherTimer timerTick;
        private double volume = 0.5;
        private bool wasFullscreenBeforePopup;

        #endregion Fields

        #region Constructors

        public MediaController()
        {
            Playlist = new ObservableCollection<IMediaItem>();
            timerTick = new DispatcherTimer();
            timerTick.Interval = TimeSpan.FromMilliseconds(100);
            timerTick.Tick += new EventHandler(timerTick_Tick);

            Application.Current.Host.Content.FullScreenChanged += new EventHandler(Content_FullScreenChanged);
            IsFullscreen = Application.Current.Host.Content.IsFullScreen;
        }

        #endregion Constructors

        #region Events

        [ScriptableMemberAttribute]
        public event EventHandler CurrentItemChanged;

        public event RoutedPropertyChangedEventHandler<TimeSpan> PositionChanged;

        public event RoutedPropertyChangedEventHandler<TimeSpan> PositionChanging;

        #endregion Events

        #region Properties

        public bool AutoPlay
        {
            get { return this.autoPlay; }
            set
            {
                if (this.autoPlay != value)
                {
                    this.autoPlay = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.AutoPlay));
                }
            }
        }

        [ScriptableMemberAttribute]
        public ContentControl ContentPub
        {
            get
            {
                return contentPub;
            }
            set
            {
                if (contentPub != value)
                {
                    contentPub = value;
                }
            }
        }

        [ScriptableMemberAttribute]
        public IMediaItem CurrentItem
        {
            get { return currentItem; }
            set
            {
                if (currentItem != value)
                {
                    LastSelectedItem = currentItem;
                    currentItem = value;
                    OnPropertyChanged(this.GetPropertyName(n => n.CurrentItem));
                    OnCurrentItemChanged();
                }
            }
        }

        public int CurrentItemIndex
        {
            get { return currentItemIndex; }
            set
            {
                if (currentItemIndex != value)
                {
                    currentItemIndex = value;

                    if (Playlist != null && currentItemIndex >= 0 && currentItemIndex < Playlist.Count)
                    {
                        CurrentItem = Playlist[currentItemIndex];
                    }
                    else
                    {
                        currentItemIndex = -1;
                        CurrentItem = null;
                    }

                    if (Playlist != null)
                    {
                        PreviousItems = Playlist.Take(Math.Max(0, currentItemIndex)).ToArray();
                        NextItems = Playlist.Skip(currentItemIndex + 1).ToArray();
                    }
                    else
                    {
                        PreviousItems = null;
                        NextItems = null;
                    }

                    OnPropertyChanged(this.GetPropertyName(n => n.CurrentItemIndex));
                }
            }
        }

        [ScriptableMemberAttribute]
        public double DownloadProgress
        {
            get { return downloadProgress; }
            set
            {
                if (downloadProgress != value)
                {
                    downloadProgress = value;
                    OnPropertyChanged(this.GetPropertyName(n => n.DownloadProgress));
                }
            }
        }

        [ScriptableMemberAttribute]
        public Duration Duration
        {
            get { return duration; }
            set
            {
                if (duration != value)
                {
                    duration = value;
                    OnPropertyChanged(this.GetPropertyName(n => n.Duration));
                    OnDurationChanged();
                }
            }
        }

        public DataTemplate FullscreenPopupTemplate
        {
            get { return fullscreenPopupTemplate; }
            set
            {
                if (fullscreenPopupTemplate != value)
                {
                    fullscreenPopupTemplate = value;
                    OnPropertyChanged(this.GetPropertyName(n => n.FullscreenPopupTemplate));
                }
            }
        }

        [ScriptableMemberAttribute]
        public bool IsChaining
        {
            get { return isChaining; }
            set
            {
                if (isChaining != value)
                {
                    isChaining = value;
                    OnPropertyChanged(this.GetPropertyName(n => n.IsChaining));
                }
            }
        }

        [ScriptableMemberAttribute]
        public bool IsDownloading
        {
            get { return isDownloading; }
            set
            {
                if (isDownloading != value)
                {
                    isDownloading = value;
                    OnPropertyChanged(this.GetPropertyName(n => n.IsDownloading));
                }
            }
        }

        [ScriptableMemberAttribute]
        public bool IsFullscreen
        {
            get { return isFullscreen; }
            set
            {
                if (isFullscreen != value)
                {
                    isFullscreen = value;
                    OnPropertyChanged(this.GetPropertyName(n => n.IsFullscreen));

                    Application.Current.Host.Content.IsFullScreen = value;
                    OnFullscreenChanged();
                }
            }
        }

        [ScriptableMemberAttribute]
        public bool IsMuted
        {
            get { return isMuted; }
            set
            {
                if (isMuted != value)
                {
                    isMuted = value;
                    OnPropertyChanged(this.GetPropertyName(n => n.IsMuted));
                }
            }
        }

        [ScriptableMemberAttribute]
        public bool IsNextEnabled
        {
            get { return isNextEnabled; }
            set
            {
                if (isNextEnabled != value)
                {
                    isNextEnabled = value;
                    OnPropertyChanged(this.GetPropertyName(n => n.IsNextEnabled));
                }
            }
        }

        [ScriptableMemberAttribute]
        public bool IsPlaying
        {
            get { return isPlaying; }
            set
            {
                if (isPlaying != value)
                {
                    isPlaying = value;
                    OnPropertyChanged(this.GetPropertyName(n => n.IsPlaying));
                    IsPlayingChanged();
                }
            }
        }

        [ScriptableMemberAttribute]
        public bool IsPopupFullscreen
        {
            get { return isPopupFullscreen; }
            set
            {
                if (isPopupFullscreen != value)
                {
                    isPopupFullscreen = value;
                    OnPropertyChanged(this.GetPropertyName(n => n.IsPopupFullscreen));

                    if (value)
                    {
                        wasFullscreenBeforePopup = IsFullscreen;
                        IsFullscreen = true;
                    }
                    else
                    {
                        if (!wasFullscreenBeforePopup)
                            IsFullscreen = false;
                    }
                    SwitchFullscreenPopup();
                }
            }
        }

        [ScriptableMemberAttribute]
        public bool IsPreviousEnabled
        {
            get { return isPreviousEnabled; }
            set
            {
                if (isPreviousEnabled != value)
                {
                    isPreviousEnabled = value;
                    OnPropertyChanged(this.GetPropertyName(n => n.IsPreviousEnabled));
                }
            }
        }

        [ScriptableMemberAttribute]
        public IMediaItem LastSelectedItem
        {
            get { return lastSelectedItem; }
            set
            {
                if (lastSelectedItem != value)
                {
                    lastSelectedItem = value;
                    OnPropertyChanged(this.GetPropertyName(n => n.LastSelectedItem));
                }
            }
        }

        public string MediaName
        {
            get { return mediaName; }
            set
            {
                if (mediaName != value)
                {
                    mediaName = value;
                    OnPropertyChanged(this.GetPropertyName(n => n.MediaName));
                }
            }
        }

        //[ScriptableMemberAttribute]
        //public FrameworkElement NextElement
        //{
        //    get { return nextElement; }
        //    set
        //    {
        //        if (nextElement != value)
        //        {
        //            if (this.nextElement != null)
        //            {
        //                ButtonBase nextButton = this.nextElement as ButtonBase;
        //                if (nextButton != null)
        //                    nextButton.Click -= new RoutedEventHandler(nextButton_Click);
        //                else
        //                    nextButton.MouseLeftButtonDown -= new MouseButtonEventHandler(nextButton_MouseLeftButtonDown);
        //            }
        //            nextElement = value;
        //            OnPropertyChanged(this.GetPropertyName(n => n.NextElement));
        //            if (this.nextElement != null)
        //            {
        //                ButtonBase nextButton = this.nextElement as ButtonBase;
        //                if (nextButton != null)
        //                    nextButton.Click += new RoutedEventHandler(nextButton_Click);
        //                else
        //                    nextButton.MouseLeftButtonDown += new MouseButtonEventHandler(nextButton_MouseLeftButtonDown);
        //            }
        //        }
        //    }
        //}
        public IEnumerable<IMediaItem> NextItems
        {
            get { return nextItems; }
            set
            {
                if (nextItems != value)
                {
                    nextItems = value;
                    OnPropertyChanged(this.GetPropertyName(n => n.NextItems));
                }
            }
        }

        [ScriptableMemberAttribute]
        public bool PauseOnClick
        {
            get { return pauseOnClick; }
            set
            {
                if (pauseOnClick != value)
                {
                    pauseOnClick = value;
                    OnPropertyChanged(this.GetPropertyName(n => n.PauseOnClick));
                }
            }
        }

        [ScriptableMemberAttribute]
        public PlayStates PlayState
        {
            get { return playState; }
            set
            {
                if (playState != value)
                {
                    playState = value;
                    IsPlaying = PlayState == PlayStates.Playing;
                    OnPropertyChanged(this.GetPropertyName(n => n.PlayState));
                }
            }
        }

        [ScriptableMemberAttribute]
        public ObservableCollection<IMediaItem> Playlist
        {
            get { return playlist; }
            set
            {
                if (playlist != value)
                {
                    playlist = value;
                    OnPropertyChanged(this.GetPropertyName(n => n.Playlist));

                    if (playlist != null)
                    {
                        SelectedIndex = -1;
                        Next();
                    }

                    RefreshNexPrevious();
                }
            }
        }

        [ScriptableMemberAttribute]
        public IPlaylistSource PlaylistSource
        {
            get { return playlistSource; }
            set
            {
                if (playlistSource != value)
                {
                    if (playlistSource != null)
                    {
                        UnbindPlaylistSource();
                    }

                    playlistSource = value;

                    if (playlistSource != null)
                    {
                        BindPlaylistSource();
                    }

                    OnPropertyChanged(this.GetPropertyName(n => n.PlaylistSource));
                }
            }
        }

        [ScriptableMemberAttribute]
        public virtual TimeSpan Position
        {
            get { return position; }
            set
            {
                if (position != value)
                {
                    var oldValue = position;
                    OnPositionChanging(position, value);
                    position = value;
                    OnPropertyChanged(this.GetPropertyName(n => n.Position));
                    OnPositionChanged(oldValue, value);
                }
            }
        }

        public IEnumerable<IMediaItem> PreviousItems
        {
            get { return previousItems; }
            set
            {
                if (previousItems != value)
                {
                    previousItems = value;
                    OnPropertyChanged(this.GetPropertyName(n => n.PreviousItems));
                }
            }
        }

        [ScriptableMemberAttribute]
        public List<ScriptCommandItem> ScriptCommands
        {
            get { return scriptCommands; }
            set
            {
                if (scriptCommands != value)
                {
                    scriptCommands = value;
                    OnPropertyChanged(this.GetPropertyName(n => n.ScriptCommands));
                }
            }
        }

        [ScriptableMemberAttribute]
        public string ScriptCommandsUrl
        {
            get { return scriptCommandsUrl; }
            set
            {
                if (scriptCommandsUrl != value)
                {
                    scriptCommandsUrl = value;

                    SynchronizationContext context = SynchronizationContext.Current;
                    //Load and Parse ScripCommands
                    WebClient wc = new WebClient();
                    wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(wc_DownloadStringCompleted);
                    wc.DownloadStringAsync(new Uri(scriptCommandsUrl, UriKind.RelativeOrAbsolute), context);
                    OnPropertyChanged(this.GetPropertyName(n => n.ScriptCommandsUrl));
                }
            }
        }

        public int SelectedIndex
        {
            get { return selectedIndex.GetValueOrDefault(-1); }
            set
            {
                if (Playlist == null || value < 0
                    || value >= Playlist.Count)
                {
                    value = -1;
                }

                if (selectedIndex != value)
                {
                    selectedIndex = value;
                    OnPropertyChanged(this.GetPropertyName(n => n.SelectedIndex));
                }
            }
        }

        [ScriptableMemberAttribute]
        public double Volume
        {
            get { return volume; }
            set
            {
                if (volume != value)
                {
                    volume = value;
                    OnPropertyChanged(this.GetPropertyName(n => n.Volume));
                }
            }
        }

        protected Popup FullscreenPopup
        {
            get { return fullscreenPopup; }
        }

        #endregion Properties

        #region Methods

        public static MediaController GetContentPub(DependencyObject obj)
        {
            return (MediaController)obj.GetValue(ContentPubProperty);
        }

        public static void SetContentPub(DependencyObject obj, MediaController value)
        {
            obj.SetValue(ContentPubProperty, value);
        }

        [ScriptableMember]
        public void AddItem(MediaItem item)
        {
            Playlist.Add(item);
        }

        [ScriptableMemberAttribute]
        public Category CreateCategory()
        {
            return new Category();
        }

        [ScriptableMemberAttribute]
        public MediaItem CreateMediaItem()
        {
            return new MediaItem();
        }

        public virtual void Dispose()
        {
            Playlist = null;
            PlaylistSource = null;
        }

        [ScriptableMember]
        public void InsertItem(int index, MediaItem item)
        {
            Playlist.Insert(index, item);
        }

        [ScriptableMemberAttribute]
        public virtual void Next()
        {
            if (Playlist == null)
                return;

            CurrentItemIndex = Math.Min(Playlist.Count - 1, CurrentItemIndex + 1);
        }

        [ScriptableMember]
        public void PlayIndex(int index)
        {
            if (Playlist == null)
                return;

            if (index >= 0 && index < Playlist.Count)
                CurrentItem = Playlist[index];
        }

        [ScriptableMember]
        public void PlayItem(MediaItem item)
        {
            CurrentItem = item;
        }

        [ScriptableMemberAttribute]
        public virtual void Previous()
        {
            if (Playlist == null)
                return;

            CurrentItemIndex = Math.Max(0, CurrentItemIndex - 1);
        }

        [ScriptableMember]
        public void RevoveItem(MediaItem item)
        {
            Playlist.Remove(item);
        }

        [ScriptableMember]
        public void RevoveItemAt(int index)
        {
            Playlist.RemoveAt(index);
        }

        protected virtual FrameworkElement CreateFullscreenPopupContent()
        {
            if (FullscreenPopupTemplate != null)
            {
                FrameworkElement elem = FullscreenPopupTemplate.LoadContent() as FrameworkElement;
                if (elem != null)
                {
                    elem.DataContext = GetFullscreenDataContext();
                    return elem;
                }
            }

            return null;
        }

        protected virtual object GetFullscreenDataContext()
        {
            return this;
        }

        protected virtual void IsPlayingChanged()
        {
            if (IsPlaying)
                timerTick.Start();
            else
                timerTick.Stop();
        }

        protected virtual void OnCurrentItemChanged()
        {
            IsDownloading = false;
            DownloadProgress = 1;

            RefreshNexPrevious();

            if (CurrentItemChanged != null)
            {
                CurrentItemChanged(this, EventArgs.Empty);
            }
        }

        protected virtual void OnDurationChanged()
        {
        }

        protected virtual void OnFullscreenChanged()
        {
        }

        protected virtual void OnPositionChanged(TimeSpan oldValue, TimeSpan newValue)
        {
            if (PositionChanged != null)
            {
                PositionChanged(this, new RoutedPropertyChangedEventArgs<TimeSpan>(oldValue, newValue));
            }
        }

        protected virtual void OnPositionChanging(TimeSpan oldValue, TimeSpan newValue)
        {
            if (PositionChanging != null)
            {
                PositionChanging(this, new RoutedPropertyChangedEventArgs<TimeSpan>(oldValue, newValue));
            }
        }

        protected virtual void RefreshPosition()
        {
        }

        protected virtual void SwitchPauseOnClick()
        {
            if (PauseOnClick)
            {
                if (IsPlaying)
                {
                    IsPlaying = false;
                }
                else
                {
                    IsPlaying = true;
                }
            }
        }

        protected virtual void TickScriptCommands()
        {
        }

        private static void ContentPubChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MediaController controller = e.NewValue as MediaController;
            controller.ContentPub = d as ContentControl;
        }

        private void BindPlaylistSource()
        {
            INotifyPropertyChanged playlistSource = PlaylistSource as INotifyPropertyChanged;
            if (playlistSource != null)
            {
                playlistSource.PropertyChanged += new PropertyChangedEventHandler(playlistSource_PropertyChanged);
            }
        }

        private void CloseFullscreenPopup()
        {
            if (fullscreenPopup != null)
            {
                fullscreenPopup.IsOpen = false;
                fullscreenPopup = null;
            }

            IsPopupFullscreen = false;
        }

        void Content_FullScreenChanged(object sender, EventArgs e)
        {
            if (!Application.Current.Host.Content.IsFullScreen)
            {
                CloseFullscreenPopup();

                IsFullscreen = false;
            }
            else
            {
                if (fullscreenPopup != null)
                {
                    Grid grid = fullscreenPopup.Child as Grid;
                    grid.Width = Application.Current.Host.Content.ActualWidth;
                    grid.Height = Application.Current.Host.Content.ActualHeight;
                }

                IsFullscreen = true;
            }
        }

        private void RefreshNexPrevious()
        {
            bool nextIsEnabled = false;
            bool prevIsEnabled = false;

            if (Playlist == null)
            {
                nextIsEnabled = false;
                prevIsEnabled = false;
            }
            else
            {
                IMediaItem[] playlist = Playlist.ToArray();
                if (CurrentItem == null)
                {
                    nextIsEnabled = true;
                }
                else
                {
                    int idx = Array.IndexOf(playlist, CurrentItem);
                    if (idx == -1)
                    {
                        nextIsEnabled = true;
                    }
                    else
                    {
                        prevIsEnabled = idx != 0;
                        nextIsEnabled = idx != playlist.Length - 1;
                    }
                }
            }

            IsPreviousEnabled = prevIsEnabled;
            IsNextEnabled = nextIsEnabled;
        }

        private void SwitchFullscreenPopup()
        {
            if (fullscreenPopup == null && IsPopupFullscreen)
            {
                fullscreenPopup = new Popup();
                Grid grid = new Grid();
                grid.Width = Application.Current.Host.Content.ActualWidth;
                grid.Height = Application.Current.Host.Content.ActualHeight;
                grid.Background = new SolidColorBrush(Colors.Black);

                FrameworkElement child = CreateFullscreenPopupContent();
                grid.Children.Add(child);
                child.MouseLeftButtonDown += new MouseButtonEventHandler(fullscreenPopup_MouseLeftButtonDown);

                fullscreenPopup.Child = grid;
                fullscreenPopup.IsOpen = true;
            }

            if (!IsPopupFullscreen)
            {
                CloseFullscreenPopup();
            }
        }

        private void UnbindPlaylistSource()
        {
            INotifyPropertyChanged playlistSource = PlaylistSource as INotifyPropertyChanged;
            if (playlistSource != null)
            {
                playlistSource.PropertyChanged -= new PropertyChangedEventHandler(playlistSource_PropertyChanged);
            }
        }

        void fullscreenPopup_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SwitchPauseOnClick();
        }

        void playlistSource_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == PlaylistSource.GetPropertyName(p => p.Playlist))
            {
                Playlist = new ObservableCollection<IMediaItem>(PlaylistSource.Playlist);
            }
        }

        void timerTick_Tick(object sender, EventArgs e)
        {
            RefreshPosition();
            if ((ScriptCommands != null) && (ContentPub != null))
            {
                TickScriptCommands();
            }
        }

        void wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                XDocument xdoc = XDocument.Parse(e.Result);

                var q = from c in xdoc.Element("ScriptCommands").Elements("ScriptCommand")
                        select ScriptCommandItem.ParseXML(c);

                var synccontext = (SynchronizationContext)e.UserState;
                synccontext.Post(delegate
                {
                    ScriptCommands = q.ToList();
                }, null);
            }
            catch (System.Xml.XmlException xe)
            {
                Debug.WriteLine("XML ScriptCommands Parsing Error:" + xe.ToString());
            }
        }

        #endregion Methods
    }
}