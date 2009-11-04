namespace SLMedia.Core
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Windows;
    using System.Windows.Browser;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;
    using System.Windows.Threading;

    public class MediaBinderSubscription
    {
        #region Fields

        private MediaController mediaController;
        private MediaElement mediaElement;
        private MultiScaleImage multiScaleImage;
        private FrameworkElement nextElement;
        private FrameworkElement pictureDisplayElement;
        private FrameworkElement previousElement;
        private ContentControl contentPub;
        private Control stateControl;

        #endregion Fields

        #region Constructors

        public MediaBinderSubscription(string name)
        {
            this.Name = name;
        }

        #endregion Constructors

        #region Properties

        public MediaController MediaController
        {
            get
            {
                return this.mediaController;
            }

            set
            {
                if (this.mediaController != value)
                {
                    this.mediaController = value;
                    if (this.MediaController != null)
                    {
                        this.mediaController.StateControl = StateControl;
                        this.mediaController.NextElement = NextElement;
                        this.mediaController.PreviousElement = PreviousElement;
                        this.mediaController.ContentPub = ContentPub;
                    }

                    VideoController videoController = this.MediaController as VideoController;
                    if (videoController != null)
                        videoController.MediaElement = mediaElement;

                    PictureController controller = this.MediaController as PictureController;
                    if (controller != null)
                        controller.PictureDisplayElement = this.PictureDisplayElement;

                    DeepZoomController dzcontroller = this.MediaController as DeepZoomController;
                    if (dzcontroller != null)
                        dzcontroller.MultiScaleImage = this.MultiScaleImage;
                }
            }
        }

        public MediaElement MediaElement
        {
            get
            {
                return this.mediaElement;
            }

            set
            {
                this.mediaElement = value;
                VideoController videoController = this.MediaController as VideoController;
                if (videoController != null)
                    videoController.MediaElement = mediaElement;
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
                    DeepZoomController controller = this.MediaController as DeepZoomController;
                    if (controller != null)
                        controller.MultiScaleImage = this.MultiScaleImage;
                }
            }
        }

        public ContentControl ContentPub {
            get { return contentPub; }
            set {

                if (contentPub != value)
                {
                    contentPub = value;
                    if (this.MediaController != null)
                    {
                        this.mediaController.ContentPub = this.contentPub;
                    }
                }
            } 
        
        }

        public string Name
        {
            get;
            private set;
        }

        public FrameworkElement NextElement
        {
            get
            {
                return this.nextElement;
            }

            set
            {
                this.nextElement = value;
                if (this.MediaController != null)
                {
                    this.mediaController.NextElement = this.nextElement;
                }
            }
        }

        public FrameworkElement PictureDisplayElement
        {
            get
            {
                return this.pictureDisplayElement;
            }

            set
            {
                this.pictureDisplayElement = value;
                PictureController controller = this.MediaController as PictureController;
                if (controller != null)
                    controller.PictureDisplayElement = this.PictureDisplayElement;
            }
        }

        public FrameworkElement PreviousElement
        {
            get
            {
                return this.previousElement;
            }

            set
            {
                this.previousElement = value;
                if (this.MediaController != null)
                {
                    this.mediaController.PreviousElement = this.previousElement;
                }
            }
        }

        public Control StateControl
        {
            get { return stateControl; }
            set
            {
                if (stateControl != value)
                {
                    stateControl = value;
                    if (this.MediaController != null)
                        this.MediaController.StateControl = stateControl;
                }
            }
        }


        private FrameworkElement playElement;
                public FrameworkElement PlayElement
        {
            get
            {
                return this.playElement;
            }

            set
            {
                this.playElement = value;
                if (this.MediaController != null)
                {
                    this.mediaController.PlayElement = this.playElement;
                }
            }
        }
                private FrameworkElement pauseElement;
                public FrameworkElement PauseElement
                {
                    get
                    {
                        return this.pauseElement;
                    }

                    set
                    {
                        this.pauseElement = value;
                        if (this.MediaController != null)
                        {
                            this.mediaController.PauseElement = this.pauseElement;
                        }
                    }
                }

                private FrameworkElement stopElement;
                public FrameworkElement StopElement
                {
                    get
                    {
                        return this.stopElement;
                    }

                    set
                    {
                        this.stopElement = value;
                        if (this.MediaController != null)
                        {
                            this.mediaController.StopElement = this.stopElement;
                        }
                    }
                }
        #endregion Properties

        
    }
}