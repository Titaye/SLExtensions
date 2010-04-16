namespace SLMedia.Core
{
    using System;
    using System.Collections.Generic;
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

    using SLExtensions;
    using SLExtensions.Collections.ObjectModel;

    public class MediaItem : NotifyingObject, IMediaItem
    {
        #region Fields

        private ObservableCollection<Category> categories;
        private string description;
        private object extendedProperties;
        private string id;
        private List<IMarkerSource> markerSourcePending = new List<IMarkerSource>();
        private IEnumerable<IMarkerSource> markerSources;
        private string source;
        private string thumbnail;
        private string title;

        #endregion Fields

        #region Constructors

        static MediaItem()
        {
            HtmlPage.RegisterCreateableType("MediaItem", typeof(MediaItem));
        }

        public MediaItem()
        {
            Categories = new ObservableCollection<Category>();
            MarkerSelectors = new ObservableCollection<IMarkerSelector>();
            MarkerSelectors.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(MarkerSelectors_CollectionChanged);
            ActiveMarkerSelectors = new ObservableCollection<IMarkerSelector>();
        }

        #endregion Constructors

        #region Properties

        public ObservableCollection<IMarkerSelector> ActiveMarkerSelectors
        {
            get; private set;
        }

        [ScriptableMember]
        public ObservableCollection<Category> Categories
        {
            get { return categories; }
            set
            {
                if (categories != value)
                {
                    categories = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.Categories));
                }
            }
        }

        [ScriptableMember]
        public string Description
        {
            get { return description; }
            set
            {
                if (description != value)
                {
                    description = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.Description));
                }
            }
        }

        public object ExtendedProperties
        {
            get { return extendedProperties; }
            set
            {
                if (extendedProperties != value)
                {
                    extendedProperties = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.ExtendedProperties));
                }
            }
        }

        [ScriptableMember]
        public string Id
        {
            get { return id; }
            set
            {
                if (id != value)
                {
                    id = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.Id));
                }
            }
        }

        public string ItemType
        {
            get; set;
        }

        public ObservableCollection<IMarkerSelector> MarkerSelectors
        {
            get;
            private set;
        }

        public IEnumerable<IMarkerSource> MarkerSources
        {
            get { return this.markerSources; }
            set
            {
                if (this.markerSources != value)
                {
                    this.markerSources = value;

                    if (value != null)
                    {
                        foreach (var item in value)
                        {
                            markerSourcePending.Add(item);
                        }
                    }

                    this.OnPropertyChanged(this.GetPropertyName(n => n.MarkerSources));
                }
            }
        }

        [ScriptableMember]
        public string Source
        {
            get { return source; }
            set
            {
                if (source != value)
                {
                    source = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.Source));
                    this.OnPropertyChanged(this.GetPropertyName(n => n.SourceUri));
                }
            }
        }

        public Uri SourceUri
        {
            get
            {
                Uri uri;
                if (Uri.TryCreate(Source, UriKind.RelativeOrAbsolute, out uri))
                    return uri;
                return null;
            }
        }

        [ScriptableMember]
        public virtual string Thumbnail
        {
            get { return thumbnail; }
            set
            {
                if (thumbnail != value)
                {
                    thumbnail = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.Thumbnail));
                    this.OnPropertyChanged(this.GetPropertyName(n => n.ThumbnailUri));
                }
            }
        }

        public Uri ThumbnailUri
        {
            get
            {
                Uri uri;
                if (Uri.TryCreate(Thumbnail, UriKind.RelativeOrAbsolute, out uri))
                    return uri;
                return null;
            }
        }

        [ScriptableMember]
        public string Title
        {
            get { return title; }
            set
            {
                if (title != value)
                {
                    title = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.Title));
                }
            }
        }

        IEnumerable<Category> IMediaItem.Categories
        {
            get { return this.Categories; }
        }

        IEnumerable<IMarkerSource> IMediaItem.MarkerSources
        {
            get { return MarkerSources; }
        }

        #endregion Properties

        #region Methods

        [ScriptableMember]
        public void AddCategory(Category category)
        {
            Categories.Add(category);
        }

        [ScriptableMember]
        public void InsertCategory(int index, Category category)
        {
            Categories.Insert(index, category);
        }

        public void LoadMarkers(IDictionary<string, object> autoactivatedMarkers)
        {
            Action setActiveSources = () =>
            {
                if (MarkerSelectors != null && autoactivatedMarkers != null)
                {
                    var tobeActivated = from md in autoactivatedMarkers
                                        from s in MarkerSelectors
                                        where s.Metadata != null
                                        && md.Value != null
                                        && md.Value.Equals(s.Metadata.TryGetValue(md.Key))
                                        select s;

                    foreach (var s in tobeActivated)
                    {
                        s.IsActive = true;
                    }
                }
            };
            setActiveSources();
            if (markerSourcePending.Count > 0)
            {
                foreach (var item in markerSourcePending.ToArray())
                {
                    item.LoadAsync((selectors) =>
                    {
                        markerSourcePending.Remove(item);
                        MarkerSelectors.AddRange(selectors);

                        if(markerSourcePending.Count == 0)
                            setActiveSources();
                    }, item.Metadata);
                }
            }
        }

        [ScriptableMember]
        public void RevoveCategory(Category category)
        {
            Categories.Remove(category);
        }

        [ScriptableMember]
        public void RevoveCategoryAt(int index)
        {
            Categories.RemoveAt(index);
        }

        void MarkerSelectors_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems.OfType<IMarkerSelector>())
                {
                    item.IsActiveChanged += markerSelector_IsActiveChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems.OfType<IMarkerSelector>())
                {
                    item.IsActiveChanged -= markerSelector_IsActiveChanged;
                }
            }
        }

        void markerSelector_IsActiveChanged(object sender, EventArgs e)
        {
            IMarkerSelector selector = (IMarkerSelector)sender;
            if (selector.IsActive)
                ActiveMarkerSelectors.Add(selector);
            else
                ActiveMarkerSelectors.Remove(selector);
        }

        #endregion Methods
    }
}