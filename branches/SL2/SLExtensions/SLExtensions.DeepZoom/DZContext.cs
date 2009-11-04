namespace SLExtensions.DeepZoom
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    using SLExtensions.WeakEvents;

    public class DZContext
    {
        #region Fields

        private bool arrangeOnFirstMotionFinished;
        private int clickedImageIndex;
        private List<MultiScaleSubImage> imagesToShow;
        private bool needArrangeOnFistMotion = false;

        #endregion Fields

        #region Constructors

        static DZContext()
        {
            MouseWheelMultiscaleImage.Initialize();
        }

        public DZContext(MultiScaleImage msi)
        {
            Owner = msi;
            Owner.UseSprings = false;
            Owner.ViewportChanged += new RoutedEventHandler(Owner_ViewportChanged);
            ImagesToHide = new List<MultiScaleSubImage>();
            ImagesToShow = new List<MultiScaleSubImage>();

            /* ---------------------------------------------------------------------------------------------------
            -- Start Mouse and Keyboard handler for the MultiScaleImage --
            This block of code is the mouse and keyboard handler for the DeepZoom control.
            Parts of this code is borrowed from Scott Hanselman's and Soul Solutions blogs
            // Based on prior work done by Lutz Gerhard, Peter Blois, and Scott Hanselman
            ---------------------------------------------------------------------------------------------------- */
            bool mouseButtonPressed = false;
            bool dragInProgress = false;
            Point dragOffset = new Point();
            Point currentPosition = new Point();

            msi.ImageOpenSucceeded += new RoutedEventHandler(msi_ImageOpenSucceeded);
            msi.MotionFinished += new RoutedEventHandler(ArrangeOnFirstMotionFinished_MotionFinished);

            /*			KeyDown += delegate(object sender, KeyEventArgs e)
                        {
                            Point p = msi.ElementToLogicalPoint(new Point((msi.Width / 2), ((msi.Width / msi.AspectRatio) / 2)));
                            switch (e.Key)
                            {
                                case Key.I:
                                case Key.C:
                                case Key.Add:
                                    msi.ZoomAboutLogicalPoint(1.1, p.X, p.Y);
                                    break;
                                case Key.O:
                                case Key.Space:
                                case Key.Subtract:
                                    msi.ZoomAboutLogicalPoint(0.9, p.X, p.Y);
                                    break;
                                case Key.Left:
                                case Key.A:
                                    msi.ViewportOrigin = new Point(msi.ViewportOrigin.X - 0.1, msi.ViewportOrigin.Y);
                                    break;
                                case Key.Right:
                                case Key.D:
                                    msi.ViewportOrigin = new Point(msi.ViewportOrigin.X + 0.1, msi.ViewportOrigin.Y);
                                    break;
                                case Key.Up:
                                case Key.W:
                                    msi.ViewportOrigin = new Point(msi.ViewportOrigin.X, msi.ViewportOrigin.Y - 0.1);
                                    break;
                                case Key.Down:
                                case Key.S:
                                    msi.ViewportOrigin = new Point(msi.ViewportOrigin.X, msi.ViewportOrigin.Y + 0.1);
                                    break;
                                case Key.R:
                                    RandomizeAndArrange();
                                    break;
                            }
                        };
            */
            msi.MouseLeave += delegate(object sender, MouseEventArgs e)
            {
                if (mouseButtonPressed)
                {
                    mouseButtonPressed = false;
                    dragInProgress = false;
                }
            };

            msi.MouseLeftButtonDown += delegate(object sender, MouseButtonEventArgs e)
            {
                mouseButtonPressed = false;
                if (IsMousePanEnabled)
                {
                    mouseButtonPressed = true;
                }

                dragInProgress = false;
                dragOffset = e.GetPosition(msi);
                currentPosition = msi.ViewportOrigin;
                ClickedImageIndex = -1;
            };

            msi.MouseMove += delegate(object sender, MouseEventArgs e)
            {
                if (mouseButtonPressed)
                {
                    dragInProgress = true;
                    Point newOrigin = new Point();
                    newOrigin.X = currentPosition.X - (((e.GetPosition(msi).X - dragOffset.X) / msi.ActualWidth) * msi.ViewportWidth);
                    newOrigin.Y = currentPosition.Y - (((e.GetPosition(msi).Y - dragOffset.Y) / msi.ActualHeight) * msi.ViewportWidth);
                    msi.ViewportOrigin = newOrigin;
                }
                LastMousePosition = e.GetPosition(msi);
            };

            msi.MouseLeftButtonUp += delegate(object sender, MouseButtonEventArgs e)
            {
                if (mouseButtonPressed && !dragInProgress)
                {
                    Point p = msi.ElementToLogicalPoint(e.GetPosition(msi));
                    int subImageIndex = msi.SubImageHitTest(p);
                    if (subImageIndex >= 0)
                    {
                        msi.DisplaySubImageCentered(subImageIndex);
                        ClickedImageIndex = subImageIndex;
                    }

                    if (IsZoomOnClickEnabled)
                    {
                        bool shiftDown = (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
                        if (shiftDown)
                            msi.Zoom(0.5, e.GetPosition(msi));
                        else
                            msi.Zoom(2.0, e.GetPosition(msi));
                    }
                }
                mouseButtonPressed = false;
                dragInProgress = false;
            };
        }

        #endregion Constructors

        #region Events

        public event EventHandler ClickedImageIndexChanged;

        public event EventHandler ImageArranged;

        #endregion Events

        #region Properties

        public bool ArrangeOnFirstMotionFinished
        {
            get
            {
                return this.arrangeOnFirstMotionFinished;
            }

            set
            {
                if (this.arrangeOnFirstMotionFinished != value)
                {
                    this.arrangeOnFirstMotionFinished = value;
                }
            }
        }

        public int ClickedImageIndex
        {
            get
            {
                return this.clickedImageIndex;
            }

            set
            {
                if (this.clickedImageIndex != value)
                {
                    this.clickedImageIndex = value;
                    OnClickedImageIndexChanged();
                }
            }
        }

        public List<MultiScaleSubImage> ImagesToHide
        {
            get;
            private set;
        }

        public List<MultiScaleSubImage> ImagesToShow
        {
            get
            {
                return this.imagesToShow;
            }

            private set
            {
                this.imagesToShow = value;
            }
        }

        public bool IsMousePanEnabled
        {
            get;
            set;
        }

        public bool IsMouseWheelEnabled
        {
            get;
            set;
        }

        public bool IsZoomOnClickEnabled
        {
            get;
            set;
        }

        public Point LastMousePosition
        {
            get;
            set;
        }

        public MultiScaleImage Owner
        {
            get;
            private set;
        }

        #endregion Properties

        #region Methods

        protected virtual void OnClickedImageIndexChanged()
        {
            if (ClickedImageIndexChanged != null)
            {
                ClickedImageIndexChanged(this, EventArgs.Empty);
            }
        }

        protected virtual void OnImageArranged()
        {
            if (ImageArranged != null)
            {
                ImageArranged(this, EventArgs.Empty);
            }
        }

        void ArrangeOnFirstMotionFinished_MotionFinished(object sender, RoutedEventArgs e)
        {
            if (needArrangeOnFistMotion)
            {
                needArrangeOnFistMotion = false;
                Owner.UseSprings = true;
                Owner.ShowAll();
                Owner.ArrangeImages(false);
                Owner.ZoomCenter();
                OnImageArranged();
            }
        }

        void Owner_ViewportChanged(object sender, RoutedEventArgs e)
        {
        }

        void msi_ImageOpenSucceeded(object sender, RoutedEventArgs e)
        {
            ImagesToShow.Clear();
            ImagesToHide.Clear();

            if (ArrangeOnFirstMotionFinished)
            {
                needArrangeOnFistMotion = true;
                Owner.UseSprings = false;
            }
        }

        #endregion Methods
    }
}