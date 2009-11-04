namespace SLMedia.Video
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

    using SLExtensions;

    public abstract class VideoAdapter : NotifyingObject, IDisposable
    {
        #region Properties

        public virtual VideoController Controller
        {
            get; set;
        }

        public abstract MediaStreamSource Source
        {
            get;
        }

        #endregion Properties

        #region Methods

        public virtual void Dispose()
        {
        }

        #endregion Methods
    }
}