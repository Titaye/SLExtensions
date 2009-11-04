namespace SLMedia.Core
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
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;
    using System.Windows.Threading;

    using SLExtensions.Diagnostics;

    public class PictureController : MediaController
    {
        #region Fields

        private double lastCurrentItemDownloadProgress = 1;
        private BitmapImage lastDisplayPictureSource;
        private Rectangle lastPopupRectangleCurrentItem;
        private Rectangle lastPopupRectanglePreviousItem;
        private DateTime lastTick = DateTime.Now;
        private FrameworkElement pictureDisplayElement;
        private Grid popupContent;
        private DispatcherTimer slideShowTimer;

        #endregion Fields

        #region Constructors

        public PictureController()
        {
            slideShowTimer = new DispatcherTimer();
            Duration = new Duration(TimeSpan.FromSeconds(5));
            slideShowTimer.Interval = Duration.TimeSpan;
            slideShowTimer.Tick += new EventHandler(slideShowTimer_Tick);
        }

        #endregion Constructors

        #region Properties

        public FrameworkElement PictureDisplayElement
        {
            get { return pictureDisplayElement; }
            set
            {
                if (pictureDisplayElement != value)
                {
                    pictureDisplayElement = value;
                    OnPropertyChanged("PictureDisplayElement");
                }
            }
        }

        #endregion Properties

        #region Methods

        protected override FrameworkElement CreateFullscreenPopupContent()
        {
            lastPopupRectanglePreviousItem = new Rectangle();
            lastPopupRectangleCurrentItem = new Rectangle();
            popupContent = new Grid();
            popupContent.Children.Add(lastPopupRectanglePreviousItem);
            popupContent.Children.Add(lastPopupRectangleCurrentItem);
            FillPicture();
            return popupContent;
        }

        protected override void IsPlayingChanged()
        {
            base.IsPlayingChanged();
            if (IsPlaying)
            {
                lastTick = DateTime.Now;
                slideShowTimer.Start();
            }
            else
                slideShowTimer.Stop();
        }

        protected override void OnCurrentItemChanged()
        {
            RefreshDisplayPictureSource();

            base.OnCurrentItemChanged();
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

        private void FillPicture()
        {
            if (CurrentItem != null && lastPopupRectangleCurrentItem != null)
            {
                ImageBrush brush = new ImageBrush();
                BitmapImage bmpImage = new BitmapImage(CurrentItem.SourceUri);
                brush.ImageFailed += delegate { };
                brush.ImageSource = bmpImage;
                brush.Stretch = Stretch.Uniform;
                lastPopupRectangleCurrentItem.Fill = brush;
            }

            if (LastSelectedItem != null && lastPopupRectanglePreviousItem != null)
            {
                if (lastCurrentItemDownloadProgress != 1)
                {
                    clearPopupPreviousItem();
                }
                else
                {
                    ImageBrush brush = new ImageBrush();
                    BitmapImage bmpImage = new BitmapImage(LastSelectedItem.SourceUri);
                    brush.ImageFailed += delegate { };
                    brush.ImageSource = bmpImage;
                    brush.Stretch = Stretch.Uniform;
                    lastPopupRectanglePreviousItem.Fill = brush;
                }
            }
        }

        private BitmapImage GetPictureDisplayBitmapImage()
        {
            if (this.PictureDisplayElement == null)
                return null;

            BitmapImage bmpImage = SLExtensions.Controls.ImageBrushDownloader.GetSyncBitmapImage(this.PictureDisplayElement);
            if (bmpImage != null)
                return bmpImage;

            Image img = this.PictureDisplayElement as Image;
            if (img != null)
            {
                ImageSource source = img.Source;
                return source as BitmapImage;
            }


            Shape shape = this.PictureDisplayElement as Shape;
            ImageBrush brush = null;
            if (shape != null)
            {
                brush = shape.Fill as ImageBrush;
                if (brush != null)
                {
                    return brush.ImageSource as BitmapImage;
                }
                return null;
            }

            Panel panel = this.PictureDisplayElement as Panel;
            if (panel != null)
            {
                brush = panel.Background as ImageBrush;
                if (brush != null)
                {
                    return brush.ImageSource as BitmapImage;
                }
                return null;
            }
            return null;
        }

        private void RefreshDisplayPictureSource()
        {
            if (lastDisplayPictureSource != null)
            {
                lastDisplayPictureSource.DownloadProgress -= bmpImage_DownloadProgress;
            }

            lastDisplayPictureSource = GetPictureDisplayBitmapImage();

            if (lastDisplayPictureSource != null)
            {
                lastDisplayPictureSource.DownloadProgress += bmpImage_DownloadProgress;
            }
            refreshPopupImage();
        }

        void bmpImage_DownloadProgress(object sender, DownloadProgressEventArgs e)
        {
            double value = (double)e.Progress / 100;
            lastCurrentItemDownloadProgress = value;
            DownloadProgress = value;
            if (value >= 1)
            {
                IsDownloading = false;
                RefreshControlState();

                clearPopupPreviousItem();

                if (IsPlaying)
                {
                    lastTick = DateTime.Now;
                    slideShowTimer.Start();
                }
            }
            else if (value > 0)
            {
                IsDownloading = true;
                RefreshControlState();
            }
        }

        private void clearPopupPreviousItem()
        {
            if (lastPopupRectanglePreviousItem != null)
                lastPopupRectanglePreviousItem.Fill = new SolidColorBrush(Colors.Black);
        }

        void refreshPopupImage()
        {
            if (FullscreenPopup != null)
            {
                FillPicture();
            }
        }

        void slideShowTimer_Tick(object sender, EventArgs e)
        {
            //Stop slideshowtimer, restart timer is necessary on downloadprogress completion
            slideShowTimer.Stop();
            lastTick = DateTime.Now;
            Position = new TimeSpan();
            Next();
        }

        #endregion Methods
    }
}