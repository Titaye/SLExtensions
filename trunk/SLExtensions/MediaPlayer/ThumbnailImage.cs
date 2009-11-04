// <copyright file="ThumbnailImage.cs" company="Microsoft">
//     Copyright © Microsoft Corporation. All rights reserved.
// </copyright>
// <summary>Implements the ThumbnailImage class</summary>
// <author>Microsoft Expression Encoder Team</author>
namespace ExpressionMediaPlayer
{
    using System;
    using System.Diagnostics;
    using System.Net;
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
    ///  Similar function as the Image control -- but handles UriFormatException cases by leaving image blank instead of thowing the exception higher.
    /// </summary>
    public class ThumbnailImage : ContentControl
    {
        /// <summary>
        /// Using a DependencyProperty as the backing store for SourceUrl.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(string), typeof(ThumbnailImage), new PropertyMetadata(new PropertyChangedCallback(Prop)));

        /// <summary>
        /// Initializes a new instance of the ThumbnailImage class.
        /// </summary>
        public ThumbnailImage()
        {
        }

        /// <summary>
        /// Gets or sets the source of this thumbnail.
        /// </summary>
        public String Source
        {
            get { return (String)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        /// <summary>
        /// Property handler for the source dependency property.
        /// </summary>
        /// <param name="imageObject">Source dependency object.</param>
        /// <param name="eventArgs">Event args.</param>
        protected static void Prop(DependencyObject imageObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            ThumbnailImage i = imageObject as ThumbnailImage;
            if (i != null && eventArgs.NewValue != null && eventArgs.NewValue is string)
            {
                try
                {
                    Image image = new Image();
                    BitmapImage bi = new BitmapImage(new Uri((string)eventArgs.NewValue));
                    image.Source=bi;
                    image.ImageFailed += i.OnImageFailed;
                    i.Content = image;
                }
                catch(UriFormatException)
                {
                    // leave thumbnail blank if image can't be loaded
                    Debug.WriteLine("ThumbnailImage: can't load image:" + eventArgs.NewValue.ToString());
                }
            }
        }

        /// <summary>
        /// Event handler for the Image failed event from the image.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Event args.</param>
        private void OnImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            Debug.WriteLine("OnImageFailed:" + e.ToString());

            string strErrorMessage = e.ErrorException.ToString() + "\r\n";
            if (this.Source != null)
            {
                strErrorMessage += this.Source.ToString() + "\r\n";
            }

            TextBlock textBlock = new TextBlock();
            textBlock.Text = strErrorMessage;
            textBlock.TextWrapping = TextWrapping.Wrap;

            Content = textBlock;            
        }
    }
}
