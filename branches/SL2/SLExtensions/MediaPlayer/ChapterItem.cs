// <copyright file="ChapterItem.cs" company="Microsoft">
//     Copyright © Microsoft Corporation. All rights reserved.
// </copyright>
// <summary>Implements the ChapterItem class</summary>
// <author>Microsoft Expression Encoder Team</author>
namespace ExpressionMediaPlayer
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
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
    using System.Xml;
    using System.Xml.Linq;

    /// <summary>
    /// This class describes a Chapter point in the media stream. Chapter points can
    /// contain a title and a thumbnail among other things.
    /// </summary>
    public class ChapterItem : INotifyPropertyChanged
    {
        /// <summary>
        /// The position of the chapter item.
        /// </summary>
        private double m_position;

        /// <summary>
        /// The source of the thumbnail for this chapter item.
        /// </summary>
        private string m_thumbSource;

        /// <summary>
        /// The title of this chapter item.
        /// </summary>
        private string m_title;

        /// <summary>
        /// Initializes a new instance of the ChapterItem class.
        /// </summary>
        public ChapterItem()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ChapterItem class.
        /// </summary>
        /// <param name="chapter">XML describing this chapter item.</param>
        public ChapterItem(XElement chapter)
        {
            XAttribute xa = chapter.Attribute("position");
            if (xa != null)
            {
                Position = double.Parse(xa.Value, CultureInfo.InvariantCulture);
            }
            else
            {
                Position = 0.0;
            }

            xa = chapter.Attribute("title");
            if (xa != null)
            {
                Title = MediaPlayer.UNEscape(xa.Value);
            }
            else
            {
                Title = DefaultTitle();
            }

            xa = chapter.Attribute("thumbnailSource");
            if (xa != null)
            {
                ThumbSource = MediaPlayer.PathFromUri(HtmlPage.Document.DocumentUri, System.Uri.UnescapeDataString(xa.Value)).ToString();
            }
            else
            {
                ThumbSource = DefaultThumbnailSource();
            }
        }

        /// <summary>
        /// Event which fires whenever a property changes on this chapter item.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the title of this chapter item.
        /// </summary>
        public string Title
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
        /// Gets or sets the position in the media stream of this chapter item.
        /// </summary>
        public double Position
        {
            get
            {
                return m_position;
            }

            set
            {
                m_position = value;
                OnPropertyChanged("Position");
            }
        }

        /// <summary>
        /// Gets the position of this chapter item as a string.
        /// </summary>
        public string PositionText
        {
            get
            {
                return TimeSpanStringConverter.ConvertToString(TimeSpan.FromSeconds(m_position), ConverterModes.TenthSecond);
            }
        }

        /// <summary>
        /// Gets or sets the source of the thumbnail for this chapter item.
        /// </summary>
        public string ThumbSource
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
        /// Gets a default title for this chapter item, if one was not specified. 
        /// </summary>
        /// <returns>An empty string.</returns>
        private static String DefaultTitle()
        {
            return string.Empty;
        }

        /// <summary>
        /// Gets a default thumbnail source for this chapter item, if one was
        /// not specified.
        /// </summary>
        /// <returns>An empty string.</returns>
        private static String DefaultThumbnailSource()
        {
            return string.Empty;
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
