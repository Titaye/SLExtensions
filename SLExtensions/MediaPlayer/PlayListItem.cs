// <copyright file="PlaylistItem.cs" company="Microsoft">
//     Copyright © Microsoft Corporation. All rights reserved.
// </copyright>
// <summary>Implements the PlaylistItem class</summary>
// <author>Microsoft Expression Encoder Team</author>
namespace ExpressionMediaPlayer
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Net;
    using System.Windows;
    using System.Windows.Browser;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;
    using System.Xml;
    using System.Xml.Linq;

    /// <summary>
    /// This class represents a media item in a playlist.
    /// </summary>
    [TypeConverter(typeof(UriTypeConverter))]
    public class PlaylistItem : INotifyPropertyChanged
    {
        /// <summary>
        /// The parent collection of this item.
        /// </summary>
        private PlaylistCollection m_collectionParent;

        /// <summary>
        /// The title of this item.
        /// </summary>
        private String m_title = string.Empty;

        /// <summary>
        /// The description of this item.
        /// </summary>
        private String m_description = string.Empty;

        /// <summary>
        /// The thumbnail source for this item.
        /// </summary>
        private String m_thumbSource = string.Empty;

        /// <summary>
        /// The width of the encoded video for this item.
        /// </summary>
        private Double m_width = Double.PositiveInfinity;

        /// <summary>
        /// The height of the encoded video for this item.
        /// </summary>
        private Double m_height = Double.PositiveInfinity;

        /// <summary>
        /// The Url for the media of this item.
        /// </summary>
        private Uri m_mediaUrl;

        /// <summary>
        /// A value indicating whether this item is adaptive streaming or not.
        /// </summary>
        private bool m_isAdaptiveStreaming;

        /// <summary>
        /// The frame rate of this item.
        /// </summary>
        private SmpteFrameRate m_frameRate = SmpteFrameRate.Unknown;

        /// <summary>
        /// The chapters in this item.
        /// </summary>
        private ObservableCollection<ChapterItem> m_chapters = new ObservableCollection<ChapterItem>();

        /// <summary>
        /// Initializes a new instance of the PlaylistItem class.
        /// </summary>
        public PlaylistItem()
        {
        }

        /// <summary>
        /// Initializes a new instance of the PlaylistItem class.
        /// </summary>
        /// <param name="uriDocument">Uri of the source document.</param>
        /// <param name="collectionParent">The collection we are being added to.</param>
        /// <param name="element">Xml element describing this PlaylistItem.</param>
        public PlaylistItem(Uri uriDocument, PlaylistCollection collectionParent, XElement element)
        {
            m_collectionParent = collectionParent;
            XAttribute xa = element.Attribute("title");
            if (xa != null)
            {
                Title = MediaPlayer.UNEscape(xa.Value);
            }

            xa = element.Attribute("thumbSource");
            if (xa != null )
            {
                String tmp = System.Uri.UnescapeDataString(xa.Value);
                if ( tmp.Length > 0 )
                {
                    m_thumbSource = MediaPlayer.PathFromUri(uriDocument, tmp).ToString();
                }
            }

            xa = element.Attribute("width");
            if (xa != null)
            {
                Double width = Double.PositiveInfinity;

                if (Double.TryParse(xa.Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out width))
                {
                    m_width = width;
                }
            }

            xa = element.Attribute("height");
            if (xa != null)
            {
                Double height = Double.PositiveInfinity;

                if (Double.TryParse(xa.Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out height))
                {
                    m_height = height;
                }
            }

            xa = element.Attribute("mediaSource");
            if (xa != null)
            {
                MediaUrl = MediaPlayer.PathFromUri(uriDocument, System.Uri.UnescapeDataString(xa.Value));
            }
            else
            {
                MediaUrl = MediaPlayer.PathFromUri(uriDocument, string.Empty);
            }

            xa = element.Attribute("description");
            if (xa != null)
            {
                Description = MediaPlayer.UNEscape(xa.Value);
            }

            xa = element.Attribute("adaptiveStreaming");
            if (xa != null)
            {
                IsAdaptiveStreaming = bool.Parse(xa.Value);
            }

            xa = element.Attribute("frameRate");
            if (xa != null)
            {
                Double rate = -1;
                
                if (Double.TryParse(xa.Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out rate))
                {
                    FrameRate = TimeCode.ParseFramerate(rate);
                }
                else
                {
                    FrameRate = SmpteFrameRate.Unknown;
                }
            }

            for (IEnumerator<XElement> chaptersEnumerator = element.Descendants("chapter").GetEnumerator(); chaptersEnumerator.MoveNext(); )
            {
                Chapters.Add(new ChapterItem(chaptersEnumerator.Current));
            }
        }

        /// <summary>
        /// Property changed event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Sets the owner collection. Required to enable declaritve collections 
        /// where playlistitems are instantiated in XAML with default constructor.
        /// </summary>
        internal PlaylistCollection OwnerCollection
        {
            set
            {
                m_collectionParent = value;
            }
        }

        /// <summary>
        /// Gets or sets the title of the playlist item.
        /// </summary>
        public String Title
        {
            get
            { 
                return m_title;
            }

            set
            {
                m_title = value;
                OnPropertyChanged("Title");
            }
        }

        /// <summary>
        /// Gets the index of this item in the collection.
        /// </summary>
        public int MyIndex
        {
            get
            {
                if (m_collectionParent != null)
                {
                    return m_collectionParent.IndexOf(this) + 1;
                }
                else
                {
                    return -1;
                }
            }
        }

        /// <summary>
        /// Gets or sets the description of this playlist item.
        /// </summary>
        public String Description
        {
            get
            {
                return m_description;
            }

            set
            {
                m_description = value;
                OnPropertyChanged("Description");
            }
        }

        /// <summary>
        /// Gets or sets the source of the thumbnail for this item. 
        /// Uses a string instead of a URI to facilitate binding and handling cases where 
        /// the thumbnail file is missing without generating a page error.
        /// </summary>
        public String ThumbSource
        {
            get
            {
                return m_thumbSource;
            }

            set
            {
                m_thumbSource = value;
                OnPropertyChanged("ThumbSource");
            }
        }

        /// <summary>
        /// Gets the width of the encoded video for this item.
        /// </summary>
        public Double Width
        {
            get
            {
                return m_width;
            }
        }

        /// <summary>
        /// Gets the width of the encoded video for this item.
        /// </summary>
        public Double Height
        {
            get
            {
                return m_height;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this item uses adaptive streaming.
        /// </summary>
        public bool IsAdaptiveStreaming
        {
            get
            {
                return m_isAdaptiveStreaming;
            }

            set
            {
                m_isAdaptiveStreaming = value;
                OnPropertyChanged("IsAdaptiveStreaming");
            }
        }

        /// <summary>
        /// Gets or sets the Url of the media item.
        /// </summary>
        [TypeConverter(typeof(UriTypeConverter))]
        public Uri MediaUrl
        {
            get
            {
                return m_mediaUrl;
            }

            set
            {
                m_mediaUrl = value;
                OnPropertyChanged("MediaUrl");
            }
        }

        /// <summary>
        /// Gets or sets the frame rate of this item.
        /// </summary>
        public SmpteFrameRate FrameRate
        {
            get
            {
                return m_frameRate;
            }

            set
            {
                m_frameRate = value;
                OnPropertyChanged("FrameRate");
            }
        }

        /// <summary>
        /// Gets the chapters in this item.
        /// </summary>
        public ObservableCollection<ChapterItem> Chapters
        {
            get { return m_chapters; }
        }

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Implements INotifyPropertyChanged.OnPropertyChanged().
        /// </summary>
        /// <param name="memberName">The name of the property that changed.</param>
        protected void OnPropertyChanged(string memberName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(memberName));
            }
        }

        #endregion
    }
}
