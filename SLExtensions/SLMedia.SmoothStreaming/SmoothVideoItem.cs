namespace SLMedia.SmoothStreaming
{
    using System;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    using SLMedia.Core;
    using SLMedia.Video;

    public class SmoothVideoItem : VideoItem
    {
        #region Constructors

        public SmoothVideoItem()
        {
            VideoAdapter = SmoothStreamingVideoAdapter.Instance;
        }

        #endregion Constructors

        #region Properties

        public bool JoinLive
        {
            get; set;
        }

        #endregion Properties
    }
}