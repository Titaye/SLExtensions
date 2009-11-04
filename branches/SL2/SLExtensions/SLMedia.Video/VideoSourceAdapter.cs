namespace SLMedia.Video
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

    public abstract class VideoSourceAdapter
    {
        #region Fields

        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceProperty = 
            DependencyProperty.RegisterAttached("Source", typeof(object), typeof(VideoSourceAdapter), new PropertyMetadata(SourceChangedCallback));

        // Using a DependencyProperty as the backing store for VideoAdapter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VideoAdapterProperty = 
            DependencyProperty.RegisterAttached("VideoAdapter", typeof(VideoAdapter), typeof(VideoSourceAdapter), new PropertyMetadata(VideoAdapterChangedCallback));

        private static List<VideoSourceAdapter> registeredAdapters = new List<VideoSourceAdapter>();

        #endregion Fields

        #region Properties

        public static List<VideoSourceAdapter> RegisteredAdapters
        {
            get { return registeredAdapters; }
        }

        public string Extension
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public static object GetSource(DependencyObject obj)
        {
            return (object)obj.GetValue(SourceProperty);
        }

        public static VideoAdapter GetVideoAdapter(DependencyObject obj)
        {
            return (VideoAdapter)obj.GetValue(VideoAdapterProperty);
        }

        public static void SetSource(DependencyObject obj, object value)
        {
            obj.SetValue(SourceProperty, value);
        }

        public static void SetVideoAdapter(DependencyObject obj, VideoAdapter value)
        {
            obj.SetValue(VideoAdapterProperty, value);
        }

        public abstract VideoAdapter CreateVideoAdapter(MediaElement mediaElement, Uri newSource);

        public abstract bool HandleSource(Uri newSource);

        private static void MediaStreamSourceChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MediaElement mediaElement = d as MediaElement;
            if(mediaElement == null)
                return;

            MediaStreamSource source = e.NewValue as MediaStreamSource;
            mediaElement.Source = null;
            mediaElement.SetSource(source);
        }

        private static void SourceChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MediaElement mediaElement = d as MediaElement;
            if(mediaElement == null)
                return;

            Uri sourceUri = e.NewValue as Uri;
            string url = e.NewValue as string;
            if(url != null)
            {
                Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out sourceUri);
            }

            if (sourceUri != null)
            {
                var stringSource = sourceUri.ToString();
                var source = (from videosource in RegisteredAdapters
                              where videosource.HandleSource(sourceUri)
                              select videosource).FirstOrDefault();

                if (source != null)
                {
                    VideoAdapter videoAdapter = source.CreateVideoAdapter(mediaElement, sourceUri);
                    VideoSourceAdapter.SetVideoAdapter(mediaElement, videoAdapter);
                    if (videoAdapter != null)
                    {
                        mediaElement.SetSource(videoAdapter.Source);
                    }
                }
                else
                {
                    if (mediaElement.Source != sourceUri)
                    {
                        VideoSourceAdapter.SetVideoAdapter(mediaElement, null);
                        mediaElement.Source = sourceUri;
                    }
                }
            }
        }

        private static void VideoAdapterChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        #endregion Methods
    }
}