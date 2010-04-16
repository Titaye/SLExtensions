namespace SLMedia.Deepzoom
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
    using System.Windows.Threading;

    using SLExtensions;
    using SLExtensions.DeepZoom;

    using SLMedia.Core;

    public class DeepZoomController : MediaController
    {
        #region Fields

        // Using a DependencyProperty as the backing store for MultiScaleImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MultiScaleImageProperty = 
            DependencyProperty.RegisterAttached("MultiScaleImage", typeof(DeepZoomController), typeof(DeepZoomController), new PropertyMetadata(MultiScaleImageChangedCallback));

        private int collectionIndex = -1;
        private DateTime lastTick = DateTime.Now;
        private MultiScaleImage multiScaleImage;
        private DispatcherTimer slideShowTimer;

        #endregion Fields

        #region Constructors

        public DeepZoomController()
        {
            slideShowTimer = new DispatcherTimer();
            Duration = new Duration(TimeSpan.FromSeconds(5));
            slideShowTimer.Interval = Duration.TimeSpan;
            slideShowTimer.Tick += new EventHandler(slideShowTimer_Tick);
        }

        #endregion Constructors

        #region Events

        [ScriptableMember]
        public event EventHandler CollectionIndexChanged;

        #endregion Events

        #region Properties

        [ScriptableMember]
        public int CollectionIndex
        {
            get { return collectionIndex; }
            set
            {
                if (collectionIndex != value)
                {
                    collectionIndex = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.CollectionIndex));
                    OnCollectionIndexChanged();
                }
            }
        }

        public MultiScaleImage MultiScaleImage
        {
            get { return multiScaleImage; }
            set
            {
                if (multiScaleImage != value)
                {
                    multiScaleImage = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.MultiScaleImage));
                }
            }
        }

        #endregion Properties

        #region Methods

        public static DeepZoomController GetMultiScaleImage(DependencyObject obj)
        {
            return (DeepZoomController)obj.GetValue(MultiScaleImageProperty);
        }

        public static void SetMultiScaleImage(DependencyObject obj, DeepZoomController value)
        {
            obj.SetValue(MultiScaleImageProperty, value);
        }

        public override void Next()
        {
            if (multiScaleImage == null)
                return;

            DZContext context = multiScaleImage.EnsureContext();
            if (context.ImagesToShow.Count == 0)
                return;

            CollectionIndex = (CollectionIndex + 1) % context.ImagesToShow.Count;
            multiScaleImage.ZoomFullAndCenterImage(collectionIndex);
        }

        public override void Previous()
        {
            if (multiScaleImage == null)
                return;

            DZContext context = multiScaleImage.EnsureContext();
            if (context.ImagesToShow.Count == 0)
                return;

            CollectionIndex = (CollectionIndex + context.ImagesToShow.Count - 1) % context.ImagesToShow.Count;
            multiScaleImage.ZoomFullAndCenterImage(collectionIndex);
        }

        protected override FrameworkElement CreateFullscreenPopupContent()
        {
            return new Rectangle();
        }

        protected override void IsPlayingChanged()
        {
            base.IsPlayingChanged();
            if (IsPlaying)
            {
                if (CollectionIndex == -1)
                    Next();

                lastTick = DateTime.Now;
                slideShowTimer.Start();
            }
            else
                slideShowTimer.Stop();
        }

        protected virtual void OnCollectionIndexChanged()
        {
            if (CollectionIndexChanged != null)
            {
                CollectionIndexChanged(this, EventArgs.Empty);
            }
        }

        protected override void OnDurationChanged()
        {
            base.OnDurationChanged();

            if (Duration.HasTimeSpan)
                slideShowTimer.Interval = Duration.TimeSpan;
        }

        protected override void RefreshPosition()
        {
            if (slideShowTimer.IsEnabled)
                Position = DateTime.Now - lastTick;
            base.RefreshPosition();
        }

        private static void MultiScaleImageChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DeepZoomController controller = e.NewValue as DeepZoomController;
            if (controller != null)
            {
                controller.MultiScaleImage = (MultiScaleImage)d;
            }
        }

        void slideShowTimer_Tick(object sender, EventArgs e)
        {
            lastTick = DateTime.Now;
            Position = new TimeSpan();
            Next();
        }

        #endregion Methods
    }
}