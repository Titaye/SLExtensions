namespace SLMedia.SmoothStreaming
{
    using System;
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

    using SLMedia.Video;

    public class SmoothStreamingVideoSourceAdapter : VideoSourceAdapter
    {
        #region Fields

        private const string adaptativeStreamingExtension = ".ism/Manifest";
        private const string adaptativeStreamingLiveExtension = ".isml/Manifest";

        #endregion Fields

        #region Methods

        public static void Initialize()
        {
            VideoSourceAdapter.RegisteredAdapters.Add(new SmoothStreamingVideoSourceAdapter());
        }

        public override VideoAdapter CreateVideoAdapter(MediaElement mediaElement, Uri newSource)
        {
            return new SmoothStreamingVideoAdapter(mediaElement, newSource);
        }

        public override bool HandleSource(Uri newSource)
        {
            return newSource != null &&
                (
                newSource.ToString().EndsWith(adaptativeStreamingExtension, StringComparison.OrdinalIgnoreCase)
                || newSource.ToString().EndsWith(adaptativeStreamingLiveExtension, StringComparison.OrdinalIgnoreCase));
        }

        #endregion Methods
    }
}