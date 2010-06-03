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
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;
    using System.Windows.Threading;
    using System.Xml.Linq;

    using SLExtensions;
    using SLExtensions.Collections.ObjectModel;
    using SLExtensions.Input;
    using SLExtensions.ComponentModel;

    [ContentProperty("Playlist")]
    public abstract class MediaController : NotifyingObject, IDisposable
    {
        #region Fields

        // Using a DependencyProperty as the backing store for ContentPup.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentPubProperty = 
            DependencyProperty.RegisterAttached("ContentPub", typeof(MediaController), typeof(MediaController), new PropertyMetadata(ContentPubChangedCallback));

        private Dictionary<string, object> autoActivedMarkers = new Dictionary<string,object>();
        private bool autoPlay;
        private ContentControl contentPub;
        private IMediaItem currentItem;
        private int currentItemIndex = -1;
        private Dictionary<IMarkerSelector, int> currentMarkerIndexes = new Dictionary<IMarkerSelector, int>();
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
        private bool lockCurrentItem;
        private string mediaName;
        private IEnumerable<IMediaItem> nextItems;
        private ObservableCollection<IMediaItem> playlist;
        private IPlaylistSource playlistSource;
        private PlayStates playState;
        private TimeSpan position;
        private IEnumerable<IMediaItem> previousItems;
        private List<ScriptCommandItem> scriptCommands;
        private string scriptCommandsUrl;

        //private int? selectedIndex;
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

            SetMarkerSelectorActive = new Command();
            SetMarkerSelectorActive.Executed += new EventHandler<ExecutedEventArgs>(SetMarkerSelectorActive_Executed);
            SetMarkerSelectorUnactive = new Command();
            SetMarkerSelectorUnactive.Executed += new EventHandler<ExecutedEventArgs>(SetMarkerSelectorUnactive_Executed);
            GoToPosition = new Command();
            GoToPosition.Executed += new EventHandler<ExecutedEventArgs>(GoToPosition_Executed);
            PreviousItem = new Command(Previous, () => Playlist != null && CurrentItemIndex > 0);
            NextItem = new Command(Next, () => Playlist != null && CurrentItemIndex < Playlist.Count - 1);

            AutoActivatedMarkers.Add(MarkerMetadata.Chapter, "");
        }

        #endregion Constructors

        #region [ Commands ]


        public Command GoToPosition
        {
            get;
            private set;
        }

        public Command SetMarkerSelectorActive
        {
            get;
            private set;
        }

        public Command SetMarkerSelectorUnactive
        {
            get;
            private set;
        }

        #endregion [ Commands ]

        #region Events

        [ScriptableMember]
        public event EventHandler<PropertyValueChangedEventArgs<IMediaItem>> CurrentItemChanged;

        
        [ScriptableMember]
        public event EventHandler<PropertyValueChangedEventArgs<IMediaItem>> CurrentItemChanging;

        public event RoutedPropertyChangedEventHandler<TimeSpan> PositionChanged;

        public event RoutedPropertyChangedEventHandler<TimeSpan> PositionChanging;

        #endregion Events

        #region Properties

        public Dictionary<string, object> AutoActivatedMarkers
        {
            get { return this.autoActivedMarkers; }
            set
            {
                if (this.autoActivedMarkers != value)
                {
                    this.autoActivedMarkers = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.AutoActivatedMarkers));
                }
            }
        }

        [ScriptableMember]
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

        [ScriptableMember]
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

        [ScriptableMember]
        public IMediaItem CurrentItem
        {
            get { return currentItem; }
            set
            {
                if (lockCurrentItem)
                    return;

                lockCurrentItem = true;
                try
                {
                    if (currentItem != value)
                    {
                        var lastItem = currentItem;
                        OnCurrentItemChanging(lastItem, value);
                        LastSelectedItem = currentItem;
                        currentItem = value;
                        OnCurrentItemChanged(lastItem, value);

                        if (Playlist != null && CurrentItem != null)
                        {
                            CurrentItemIndex = Playlist.IndexOf(CurrentItem);
                        }
                        else
                        {
                            CurrentItemIndex = -1;
                        }
                        OnPropertyChanged(this.GetPropertyName(n => n.CurrentItem));
                    }
                }
                finally
                {
                    lockCurrentItem = false;
                }
            }
        }

        [ScriptableMember]
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
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        [ScriptableMember]
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

        [ScriptableMember]
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

        [ScriptableMember]
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

        [ScriptableMember]
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

        [ScriptableMember]
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

        [ScriptableMember]
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

        [ScriptableMember]
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

        [ScriptableMember]
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

        [ScriptableMember]
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

        [ScriptableMember]
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

        [ScriptableMember]
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

        public ICommand NextItem
        {
            get;
            private set;
        }

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

        [ScriptableMember]
        public ObservableCollection<IMediaItem> Playlist
        {
            get { return playlist; }
            set
            {
                if (playlist != value)
                {
                    if (playlist != null)
                    {
                        playlist.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(playlist_CollectionChanged);
                    }

                    playlist = value;
                    OnPropertyChanged(this.GetPropertyName(n => n.Playlist));

                    if (playlist != null)
                    {
                        CurrentItemIndex = -1;
                        Next();
                        playlist.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(playlist_CollectionChanged);
                    }

                    RefreshNextPrevious();
                }
            }
        }

        [ScriptableMember]
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

        [ScriptableMember]
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

        [ScriptableMember]
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

        public ICommand PreviousItem
        {
            get;
            private set;
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

        [ScriptableMember]
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

        [ScriptableMember]
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

        [ScriptableMember]
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

        [ScriptableMember]
        public Category CreateCategory()
        {
            return new Category();
        }

        [ScriptableMember]
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

        [ScriptableMember]
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
                PlayItem(Playlist[index]);
        }

        [ScriptableMember]
        public void PlayItem(MediaItem item)
        {
            PlayItem((IMediaItem)item);
        }

        public virtual void PlayItem(IMediaItem item)
        {
            CurrentItem = item;
        }

        [ScriptableMember]
        public virtual void Previous()
        {
            if (Playlist == null)
                return;

            CurrentItemIndex = Math.Max(0, CurrentItemIndex - 1);
        }

        [ScriptableMember]
        public void RemoveItem(IMediaItem item)
        {
            Playlist.Remove(item);
        }

        [ScriptableMember]
        public void RemoveItemAt(int index)
        {
            Playlist.RemoveAt(index);
        }

        /// <summary>
        /// Returns a -1 if there is marker following the one at currentIdx with a position lower than the given position
        /// Returns 0 if the marker position at currentIdx is lower than the given position and next marker is null or greater
        /// Return 1 if the marker position at currentIdx is greater than the given position
        /// </summary>
        /// <param name="markers"></param>
        /// <param name="currentIdx"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        internal static int CompareMarkerPosition(IList<IMarker> markers, int currentIdx, TimeSpan position)
        {
            if (markers == null)
                throw new ArgumentNullException("markers");

            IMarker marker = null;
            IMarker nextMarker = null;
            var markersLength = markers.Count();

            if (currentIdx < 0)
                throw new ArgumentOutOfRangeException("currentIdx");

            if (currentIdx > markersLength)
                throw new ArgumentOutOfRangeException("currentIdx");

            marker = markers[currentIdx];

            var nextMarkerIdx = currentIdx + 1;
            if (nextMarkerIdx >= 0 && nextMarkerIdx < markersLength)
                nextMarker = markers[nextMarkerIdx];

            if (marker != null && marker.Position <= position
                && (nextMarker == null
                   || nextMarker.Position > position))
            {
                return 0;
            }

            if (nextMarker != null && nextMarker.Position < position)
                return -1;

            if (marker.Position > position)
                return 1;

            return 0;
        }

        internal static void SetActiveMarkerForSelector(List<IMarkerSelector> notProcessedMarkerSelector, IMarkerSelector markerSelector, TimeSpan position
            , Dictionary<IMarkerSelector, int> cacheCurrentMarkerIndexes)
        {
            var currentIdx = -1;
            if (notProcessedMarkerSelector.Remove(markerSelector))
            {
                currentIdx = cacheCurrentMarkerIndexes[markerSelector];
            }

            int markersLength;
            IMarker markerActive = null;
            if (markerSelector.Markers != null && (markersLength = markerSelector.Markers.Count()) > 0)
            {
                int compareResult;

                if (currentIdx < 0)
                    currentIdx = 0;
                if (currentIdx >= markersLength)
                    currentIdx = markersLength - 1;

                while ((compareResult = CompareMarkerPosition(markerSelector.Markers, currentIdx, position)) != 0)
                {
                    if (compareResult > 1)
                    {
                        currentIdx--;
                        if (currentIdx < 0)
                        {
                            currentIdx = -1;
                            break;
                        }
                    }
                    else
                    {
                        currentIdx++;
                        if (currentIdx >= markersLength)
                        {
                            currentIdx = -1;
                            break;
                        }
                    }
                }

                if (currentIdx >= 0 && currentIdx < markersLength)
                {
                    markerActive = markerSelector.Markers[currentIdx];
                    if (markerActive.Duration.HasTimeSpan && !markerActive.IsMarkerActiveAtPosition(position).GetValueOrDefault())
                        markerActive = null;
                }
            }
            if (markerSelector.ActiveMarker != markerActive)
            {
                if (markerSelector.ActiveMarker != null)
                    markerSelector.ActiveMarker.IsActive = false;
                if (markerActive != null)
                    markerActive.IsActive = true;

                markerSelector.ActiveMarker = markerActive;
            }
            cacheCurrentMarkerIndexes[markerSelector] = currentIdx;
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

        protected virtual void GoToPositionExecuted(TimeSpan position)
        {
            this.Position = position;
        }

        protected virtual void IsPlayingChanged()
        {
            if (IsPlaying)
                timerTick.Start();
            else
                timerTick.Stop();
        }

        protected virtual void OnCurrentItemChanging(IMediaItem oldValue, IMediaItem newValue)
        {
            if (CurrentItemChanging != null)
            {
                CurrentItemChanging(this, PropertyValueChangedEventArgs.Create(oldValue, newValue));
            }
        }

        protected virtual void OnCurrentItemChanged(IMediaItem oldValue, IMediaItem newValue)
        {
            IsDownloading = false;
            DownloadProgress = 1;

            RefreshNextPrevious();

            if (CurrentItem != null)
            {
                CurrentItem.LoadMarkers(AutoActivatedMarkers);
            }

            if (CurrentItemChanged != null)
            {
                CurrentItemChanged(this, PropertyValueChangedEventArgs.Create(oldValue, newValue));
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
            SetMarkers();
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
            if (PlaylistSource != null)
            {
                PlaylistSource.PlaylistChanged += new EventHandler(PlaylistSource_PlaylistChanged);
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

        void GoToPosition_Executed(object sender, ExecutedEventArgs e)
        {
            TimeSpan? ts = e.Parameter as TimeSpan?;
            GoToPositionExecuted(ts.GetValueOrDefault(TimeSpan.Zero));
        }

        void PlaylistSource_PlaylistChanged(object sender, EventArgs e)
        {
            Playlist = new ObservableCollection<IMediaItem>(((PlaylistSource)sender).Playlist);
        }

        void playlist_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RefreshSelection();
        }

        private void RefreshNextPrevious()
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

        private void RefreshSelection()
        {
            if (Playlist != null && Playlist.Count > 0 && CurrentItemIndex == -1)
                CurrentItemIndex = 0;
        }

        private void SetMarkers()
        {
            var item = CurrentItem;
            if (item == null)
            {
                currentMarkerIndexes.Clear();
                return;
            }
            // Store cached sources in a list and remove them if still in CurrentItem MarkerSources
            // Remaining items will be removed from cache
            var notProcessedMarkerSources = currentMarkerIndexes.Keys.ToList();

            var markerSelectors = item.MarkerSelectors;
            if (item != null && markerSelectors != null)
            {
                foreach (var markerSelector in markerSelectors.Where(s => s.IsActive))
                {
                    SetActiveMarkerForSelector(notProcessedMarkerSources, markerSelector, position, currentMarkerIndexes);
                }
            }

            foreach (var markerSource in notProcessedMarkerSources)
            {
                currentMarkerIndexes.Remove(markerSource);
            }
        }

        void SetMarkerSelectorActive_Executed(object sender, ExecutedEventArgs e)
        {
            var prm = e.Parameter as MarkerSelectorCommandParameter;
            if (prm != null && CurrentItem != null && CurrentItem.MarkerSelectors != null)
            {
                var markerSelectorsForType = (CurrentItem.MarkerSelectors.Where(s => s.Metadata != null && s.Metadata.ContainsKey(prm.Key))).ToList();

                IMarkerSelector toBeSelected = null;
                if (prm.Value != null)
                    toBeSelected = markerSelectorsForType.FirstOrDefault(s => s.Metadata.ContainsKey(prm.Key) && prm.Value.Equals(s.Metadata[prm.Key]));

                if (toBeSelected != null)
                    markerSelectorsForType.Remove(toBeSelected);

                if (!prm.AllowMultipleActive)
                {
                    foreach (var item in markerSelectorsForType)
                    {
                        item.IsActive = false;
                    }
                }

                if (toBeSelected != null)
                    toBeSelected.IsActive = true;
            }
        }

        void SetMarkerSelectorUnactive_Executed(object sender, ExecutedEventArgs e)
        {
            var prm = e.Parameter as MarkerSelectorCommandParameter;
            if (prm != null && CurrentItem != null && CurrentItem.MarkerSelectors != null
                && prm.Value != null)
            {
                var toBeUnselected = CurrentItem.MarkerSelectors.FirstOrDefault(s => s.Metadata != null && s.Metadata.ContainsKey(prm.Key) && prm.Value.Equals(s.Metadata[prm.Key]));

                if (toBeUnselected != null)
                    toBeUnselected.IsActive = false;
            }
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

                fullscreenPopup.Child = grid;
                fullscreenPopup.IsOpen = true;
            }

            if (!IsPopupFullscreen)
            {
                CloseFullscreenPopup();
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

        private void UnbindPlaylistSource()
        {
            if (PlaylistSource != null)
            {
                PlaylistSource.PlaylistChanged -= new EventHandler(PlaylistSource_PlaylistChanged);
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