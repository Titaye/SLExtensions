namespace SLMedia.Core
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

    using SLExtensions.DeepZoom;

    public class DeepZoomController : MediaController
    {
        #region Fields

        public const string CollectionIndexPropertyName = "CollectionIndex";
        public const string MultiScaleImagePropertyName = "MultiScaleImage";

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
                    OnPropertyChanged(CollectionIndexPropertyName);
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
                    OnPropertyChanged(MultiScaleImagePropertyName);
                }
            }
        }

        #endregion Properties

        #region Methods

        public override void Next()
        {
            DZContext context = multiScaleImage.EnsureContext();
            CollectionIndex = (CollectionIndex + 1) % context.ImagesToShow.Count;
            multiScaleImage.ZoomFullAndCenterImage(collectionIndex);
        }

        public override void Previous()
        {
            DZContext context = multiScaleImage.EnsureContext();
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

        void slideShowTimer_Tick(object sender, EventArgs e)
        {
            //Stop slideshowtimer, restart timer is necessary on downloadprogress completion
            //slideShowTimer.Stop();
            lastTick = DateTime.Now;
            Position = new TimeSpan();
            Next();
        }

        #endregion Methods
    }
}