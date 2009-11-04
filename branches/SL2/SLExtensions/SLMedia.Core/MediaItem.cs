namespace SLMedia.Core
{
    using System;
    using System.Collections.Generic;
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
        }

        #endregion Constructors

        #region Properties

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
        public string Thumbnail
        {
            get { return thumbnail; }
            set
            {
                if (thumbnail != value)
                {
                    thumbnail = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.Thumbnail));
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

        #endregion Methods
    }
}