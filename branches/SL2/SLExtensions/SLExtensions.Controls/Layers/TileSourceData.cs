// <copyright file="TileSourceData.cs" company="Ucaya">
// Distributed under Microsoft Public License (Ms-PL)
// </copyright>
// <author>Thierry Bouquain</author>
namespace SLExtensions.Controls.Layers
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;

    /// <summary>
    /// Store data for a Tile on TileSource
    /// </summary>
    public class TileSourceData : INotifyPropertyChanged
    {
        #region Fields

        /// <summary>
        /// the height value.
        /// </summary>
        private double height;

        /// <summary>
        /// the image source.
        /// </summary>
        private ImageSource imageSource;

        /// <summary>
        /// index location
        /// </summary>
        private Point indexLocation;

        /// <summary>
        /// the left value
        /// </summary>
        private double left;

        /// <summary>
        /// the level of detail
        /// </summary>
        private uint levelOfDetail;

        /// <summary>
        /// the tag value
        /// </summary>
        private object tag;

        /// <summary>
        /// the top value
        /// </summary>
        private double top;

        /// <summary>
        /// the image uri.
        /// </summary>
        private Uri uri;

        /// <summary>
        /// the width value.
        /// </summary>
        private double width;

        #endregion Fields

        #region Events

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        public double Height
        {
            get
            {
                return this.height;
            }

            set
            {
                if (this.height != value)
                {
                    this.height = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.Height));
                }
            }
        }

        /// <summary>
        /// Gets or sets the image source.
        /// </summary>
        /// <value>The image source.</value>
        public ImageSource ImageSource
        {
            get
            {
                return this.imageSource;
            }

            set
            {
                if (this.imageSource != value)
                {
                    this.imageSource = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.ImageSource));
                }
            }
        }

        /// <summary>
        /// Gets or sets the index location.
        /// </summary>
        /// <value>The index location.</value>
        public Point IndexLocation
        {
            get
            {
                return this.indexLocation;
            }

            set
            {
                if (this.indexLocation != value)
                {
                    this.indexLocation = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.IndexLocation));
                }
            }
        }

        /// <summary>
        /// Gets or sets the left.
        /// </summary>
        /// <value>The left value.</value>
        public double Left
        {
            get
            {
                return this.left;
            }

            set
            {
                if (this.left != value)
                {
                    this.left = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.Left));
                }
            }
        }

        /// <summary>
        /// Gets or sets the level of detail.
        /// </summary>
        /// <value>The level of detail.</value>
        public uint LevelOfDetail
        {
            get
            {
                return this.levelOfDetail;
            }

            set
            {
                if (this.levelOfDetail != value)
                {
                    this.levelOfDetail = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.LevelOfDetail));
                }
            }
        }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>The tag value.</value>
        public object Tag
        {
            get
            {
                return this.tag;
            }

            set
            {
                if (this.tag != value)
                {
                    this.tag = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.Tag));
                }
            }
        }

        /// <summary>
        /// Gets or sets the top.
        /// </summary>
        /// <value>The top value.</value>
        public double Top
        {
            get
            {
                return this.top;
            }

            set
            {
                if (this.top != value)
                {
                    this.top = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.Top));
                }
            }
        }

        /// <summary>
        /// Gets or sets the URI.
        /// </summary>
        /// <value>The image URI.</value>
        public Uri Uri
        {
            get
            {
                return this.uri;
            }

            set
            {
                if (this.uri != value)
                {
                    this.uri = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.Uri));
                    this.ImageSource = new BitmapImage(this.uri);
                }
            }
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public double Width
        {
            get
            {
                return this.width;
            }

            set
            {
                if (this.width != value)
                {
                    this.width = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.Width));
                }
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Called when property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion Methods
    }
}