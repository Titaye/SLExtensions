namespace SLMedia.Core.Interactivity
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

    using SLExtensions.Interactivity;

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
                typeof(MediaActions),
                typeof(MediaTriggerAction),
                null);

        /// <summary>
        /// MediaController depedency property.
        /// </summary>
        public static readonly DependencyProperty MediaControllerProperty = 
            DependencyProperty.Register(
                "MediaController",
                typeof(MediaController),
                typeof(MediaTriggerAction),
                null);

        #endregion Fields

        #region Properties

        public MediaActions Action
        {
            get
            {
                return (MediaActions)GetValue(ActionProperty);
            }

            set
            {
                SetValue(ActionProperty, value);
            }
        }

        public MediaController MediaController
        {
            get
            {
                return (MediaController)GetValue(MediaControllerProperty);
            }

            set
            {
                SetValue(MediaControllerProperty, value);
            }
        }

        #endregion Properties

        #region Methods

        protected override void Invoke(object parameter)
        {
            if (MediaController == null)
                return;

            switch (Action)
            {
                case MediaActions.Play:
                    MediaController.PlayState = PlayStates.Playing;
                    break;
                case MediaActions.Pause:
                    MediaController.PlayState = PlayStates.Paused;
                    break;
                case MediaActions.Stop:
                    MediaController.PlayState = PlayStates.Stopped;
                    break;
                case MediaActions.Next:
                    MediaController.Next();
                    break;
                case MediaActions.Previous:
                    MediaController.Previous();
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