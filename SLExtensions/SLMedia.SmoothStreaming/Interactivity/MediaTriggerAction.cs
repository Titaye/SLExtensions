namespace SLMedia.SmoothStreaming.Interactivity
{
    using System;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Interactivity;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    using SLMedia.Video;

    [DefaultTrigger(typeof(ButtonBase), typeof(System.Windows.Interactivity.EventTrigger), new object[] { "Click" })]
    [DefaultTrigger(typeof(UIElement), typeof(System.Windows.Interactivity.EventTrigger), new object[] { "MouseLeftButtonDown" })]
    [DefaultTrigger(typeof(TextBox), typeof(System.Windows.Interactivity.EventTrigger), new object[] { "KeyDown" })]
    public class MediaTriggerAction : TriggerAction<FrameworkElement>
    {
        #region Fields

        /// <summary>
        /// Action depedency property.
        /// </summary>
        public static readonly DependencyProperty ActionProperty = 
            DependencyProperty.Register(
                "Action",
                typeof(SmoothStreamingActions),
                typeof(MediaTriggerAction),
                null);

        /// <summary>
        /// MediaController depedency property.
        /// </summary>
        public static readonly DependencyProperty VideoControllerProperty = 
            DependencyProperty.Register(
                "VideoController",
                typeof(VideoController),
                typeof(MediaTriggerAction),
                null);

        #endregion Fields

        #region Properties

        public SmoothStreamingActions Action
        {
            get
            {
                return (SmoothStreamingActions)GetValue(ActionProperty);
            }

            set
            {
                SetValue(ActionProperty, value);
            }
        }

        public VideoController VideoController
        {
            get
            {
                return (VideoController)GetValue(VideoControllerProperty);
            }

            set
            {
                SetValue(VideoControllerProperty, value);
            }
        }

        #endregion Properties

        #region Methods

        protected override void Invoke(object parameter)
        {
            if (VideoController == null)
                return;

            var videoAdapter = VideoController.VideoAdapter as SmoothStreamingVideoAdapter;
            if (videoAdapter == null)
                return;

            switch (Action)
            {
                case SmoothStreamingActions.GoToLive:
                    videoAdapter.GoToLive();
                    break;
                case SmoothStreamingActions.Review:
                    videoAdapter.Review();
                    break;
            }
        }

        #endregion Methods

        #region Other

        //new PropertyMetadata(MediaActions.Play, (d, e) => ((MediaController)d).OnActionChanged((MediaActions)e.OldValue, (MediaActions)e.NewValue))
        ///// <summary>
        ///// handles the ActionProperty changes.
        ///// </summary>
        ///// <param name="oldValue">The old value.</param>
        ///// <param name="newValue">The new value.</param>
        //private void OnActionChanged(MediaActions oldValue, MediaActions newValue)
        //{
        //}

        #endregion Other
    }
}