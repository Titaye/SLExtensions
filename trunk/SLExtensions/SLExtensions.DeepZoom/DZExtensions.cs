namespace SLExtensions.DeepZoom
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    public static class DZExtensions
    {
        #region Fields

        // Using a DependencyProperty as the backing store for ArrangeOnFirstMotionFinished.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ArrangeOnFirstMotionFinishedProperty = 
            DependencyProperty.RegisterAttached("ArrangeOnFirstMotionFinished", typeof(bool), typeof(DZExtensions), new PropertyMetadata(ArrangeOnFirstMotionFinishedChangedCallback));

        // Using a DependencyProperty as the backing store for Context.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContextProperty = 
            DependencyProperty.RegisterAttached("Context", typeof(DZContext), typeof(DZExtensions), null);

        // Using a DependencyProperty as the backing store for IsMousePanEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsMousePanEnabledProperty = 
            DependencyProperty.RegisterAttached("IsMousePanEnabled", typeof(bool), typeof(DZExtensions), new PropertyMetadata(IsMousePanEnabledChangedCallback));

        // Using a DependencyProperty as the backing store for IsMouseWheelEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsMouseWheelEnabledProperty = 
            DependencyProperty.RegisterAttached("IsMouseWheelEnabled", typeof(bool), typeof(DZExtensions), new PropertyMetadata(IsMouseWheelEnabledChangedCallback));

        // Using a DependencyProperty as the backing store for IsZoomOnClickEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsZoomOnClickEnabledProperty = 
            DependencyProperty.RegisterAttached("IsZoomOnClickEnabled", typeof(bool), typeof(DZExtensions), new PropertyMetadata(IsZoomOnClickEnabledChangedCallback));

        // Using a DependencyProperty as the backing store for IsZoomForceEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsZoomForceEnabledProperty =
            DependencyProperty.RegisterAttached("IsZoomForceEnabled", typeof(bool), typeof(DZExtensions), new PropertyMetadata(IsZoomForceEnabledChangedCallback));

        #endregion Fields

        #region Methods

        public static void ArrangeImages(this MultiScaleImage msi, bool withAnim)
        {
            if (msi.ActualHeight == 0)
                return;

            DZContext context = msi.EnsureContext();
            double containerAspectRatio = msi.ActualWidth / msi.ActualHeight;
            double spaceBetweenImages = 0.005;

            List<SubImage> subImages = new List<SubImage>();
            context.ImagesToShow.ForEach(subImage => subImages.Add(new SubImage(subImage)));

            // Capture the total width of all images
            double totalImagesWidth = 0.0;
            subImages.ForEach(subImage => totalImagesWidth += subImage.Width);

            // Calculate the total number of rows required to display all the images
            int numRows = (int)Math.Sqrt((totalImagesWidth / containerAspectRatio) + 1);

            // Assign images to each row
            List<Row> rows = new List<Row>(numRows);
            for (int i = 0; i < numRows; i++)
                rows.Add(new Row(spaceBetweenImages));

            double widthPerRow = totalImagesWidth / numRows;
            double imagesWidth = 0;
            // Separate the images into rows. The total width of all images in a row should not exceed widthPerRow
            for (int i = 0, j = 0; i < numRows; i++, imagesWidth = 0)
            {
                while (imagesWidth < widthPerRow && j < subImages.Count)
                {
                    rows[i].AddImage(subImages[j]);
                    subImages[j].RowNum = i;
                    imagesWidth += subImages[j++].Width;
                }
            }

            // At this point in time the subimage height is 1
            // If we assume that the total height is also 1 we need to scale the subimages to fit within a total height of 1
            // If the total height is 1, the total width is aspectRatio. Hence (aspectRatio)/(total width of all images in a row) is the scaling factor.
            // Added later: take into account spacing between images
            rows.ForEach(Row => Row.Scale(containerAspectRatio));

            // Calculate the total height, with space between images, of the images across all rows
            // Also adjust the colNum for each image
            double totalImagesHeight = (numRows - 1) * spaceBetweenImages;
            rows.ForEach(Row => totalImagesHeight += Row.Height);

            // The totalImagesHeight should not exceed 1.
            // if it does, we need to scale all images by a factor of (1 / totalImagesHeight)
            if (totalImagesHeight > 1)
            {
                subImages.ForEach(subImage => subImage.Scale(1 / (totalImagesHeight + spaceBetweenImages)));
                totalImagesHeight = (numRows - 1) * spaceBetweenImages;
                rows.ForEach(Row => totalImagesHeight += Row.Height);
            }

            // Calculate the top and bottom margin
            double margin = (1 - totalImagesHeight) / 2;

            // First hide all the images that should not be displayed
            context.ImagesToHide.ForEach(subImage => subImage.Opacity = 0.0);

            Storyboard _moveStoryboard = new Storyboard();

            // Then display the displayable images to scale
            for (int i = 0; i < context.ImagesToShow.Count; i++)
            {
                double X = rows[subImages[i].RowNum].CalcX(subImages[i].ColNum);
                double Y = margin;
                for (int j = 0; j < subImages[i].RowNum; j++)
                    Y += spaceBetweenImages + rows[j].Height;

                MultiScaleSubImage subImage = context.ImagesToShow[i];
                subImage.ViewportWidth = containerAspectRatio / subImages[i].Width;

                Point futurePosition = new Point(-(X / subImages[i].Width), -(Y / subImages[i].Width));
                if (withAnim)
                {
                    AnimateImage(subImage, futurePosition, _moveStoryboard);	// for animation, use this statement instead of the next one
                }
                else
                {
                    subImage.ViewportOrigin = futurePosition;
                }
                //				_imagesToShow[i].ViewportOrigin = new Point(-(X / subImages[i].Width), -(Y / subImages[i].Width));
                context.ImagesToShow[i].Opacity = 1.0;
            }
            // Play Storyboard
            if (withAnim)
                _moveStoryboard.Begin();
        }

        public static void CenterLogicalRect(this MultiScaleImage msi, Rect rectLogical)
        {
            msi.CenterLogicalRect(rectLogical, msi.ViewportWidth);
        }

        public static void CenterLogicalRect(this MultiScaleImage msi, Rect rectLogical, double targetWidth)
        {
            double msiAspectRatio = msi.ActualWidth / msi.ActualHeight;

            Point newOrigin = new Point(rectLogical.X - (targetWidth / 2) + (rectLogical.Width / 2),
                                        rectLogical.Y - ((targetWidth / msiAspectRatio) / 2) + (rectLogical.Height / 2));

            msi.ViewportOrigin = newOrigin;
        }

        //public static void DisplayGlobalCentered(this MultiScaleImage msi)
        //{
        //    msi.DisplayGlobalCentered(1, true);
        //}
        //priavte static void DisplayGlobalCentered(this MultiScaleImage msi, int nbVirtualWith, bool goBegin)
        //{
        //    Rect subImageRect = msi.GetGloblaRect(nbVirtualWith);
        //    double num = msi.ActualWidth / msi.ActualHeight;
        //    Point point;
        //    if (goBegin)
        //        point = new Point((subImageRect.X - (msi.ViewportWidth / 2.0)) + (subImageRect.Width / 2.0), (subImageRect.Y - ((msi.ViewportWidth / num) / 2.0)) + (subImageRect.Height / 2.0));
        //    else
        //        point = new Point(msi.ViewportOrigin.X, (subImageRect.Y - ((msi.ViewportWidth / num) / 2.0)) + (subImageRect.Height / 2.0));
        //    double zoomFactor = (msi.ActualWidth / subImageRect.Width) < (msi.ActualHeight / subImageRect.Height) ?
        //                                            msi.ActualWidth / subImageRect.Width : msi.ActualHeight / subImageRect.Height;
        //    msi.ViewportWidth = zoomFactor * msi.ViewportWidth;
        //    msi.ViewportOrigin = point;
        //}
        public static void DisplaySubImageCentered(this MultiScaleImage msi, int indexSubImage)
        {
            if (indexSubImage < 0 || indexSubImage >= msi.SubImages.Count)
                return;

            Rect subImageRect = msi.GetSubImageRect(indexSubImage);
            msi.CenterLogicalRect(subImageRect);
        }

        public static DZContext EnsureContext(this MultiScaleImage msi)
        {
            DZContext context = GetContext(msi);
            if (context == null)
            {
                context = new DZContext(msi);
                SetContext(msi, context);
            }
            return context;
        }

        public static Rect ExpandRect(this MultiScaleImage msi, Rect rect, int expandBy)
        {
            return new Rect(rect.Left - expandBy, rect.Top - expandBy, rect.Width + expandBy * 2, rect.Height + expandBy * 2);
        }

        public static void Filter(this MultiScaleImage msi, IEnumerable<int> zOrderList)
        {
            DZContext context = msi.EnsureContext();

            List<MultiScaleSubImage> _imagesToShow = context.ImagesToShow;
            _imagesToShow.Clear();

            List<MultiScaleSubImage> _imagesToHide = context.ImagesToHide;
            _imagesToShow.Clear();

            for (int i = 0; i < msi.SubImages.Count; i++)
            {
                if ((zOrderList != null) && zOrderList.Contains(i))
                {
                    if (!_imagesToShow.Contains(msi.SubImages[i]))
                    {
                        //_imagesToHide.Remove(msi.SubImages[i]);
                        _imagesToShow.Add(msi.SubImages[i]);
                    }
                }
                else
                {
                    //_imagesToShow.Remove(msi.SubImages[i]);
                    _imagesToHide.Add(msi.SubImages[i]);
                }
            }
        }

        public static bool GetArrangeOnFirstMotionFinished(DependencyObject obj)
        {
            return (bool)obj.GetValue(ArrangeOnFirstMotionFinishedProperty);
        }

        public static DZContext GetContext(DependencyObject obj)
        {
            return (DZContext)obj.GetValue(ContextProperty);
        }

        public static Rect GetGlobalRect(this MultiScaleImage msi, int nbVirtualWidth)
        {
            DZContext context = msi.EnsureContext();

            IEnumerable<MultiScaleSubImage> _imagesToShow = context.ImagesToShow;
            if (_imagesToShow == null || _imagesToShow.Count() == 0)
                return new Rect();

            double xmax = (from i in _imagesToShow select ((Math.Abs(i.ViewportOrigin.X) * (1.0 / i.ViewportWidth)) + (1 / i.ViewportWidth))).Max();
            double xmin = (from i in _imagesToShow select (Math.Abs(i.ViewportOrigin.X) * (1.0 / i.ViewportWidth))).Min();
            double ymax = (from i in _imagesToShow select ((Math.Abs(i.ViewportOrigin.Y) * (1.0 / i.ViewportWidth)) + ((1.0 / i.AspectRatio) * (1.0 / i.ViewportWidth)))).Max();
            double ymin = (from i in _imagesToShow select (Math.Abs(i.ViewportOrigin.Y) * (1.0 / i.ViewportWidth))).Min();

            //double num = 1.0 / (xmax-xmin);
            return new Rect(xmin, ymin, (xmax - xmin) / nbVirtualWidth, (ymax - ymin));
        }

        public static bool GetIsMousePanEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsMousePanEnabledProperty);
        }

        public static bool GetIsMouseWheelEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsMouseWheelEnabledProperty);
        }

        public static bool GetIsZoomOnClickEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsZoomOnClickEnabledProperty);
        }

        public static bool GetIsZoomForceEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsZoomForceEnabledProperty);
        }

        public static Rect GetSubImageRect(this MultiScaleImage msi, int indexSubImage)
        {
            if (indexSubImage < 0 || indexSubImage >= msi.SubImages.Count)
                return Rect.Empty;

            MultiScaleSubImage subImage = msi.SubImages[indexSubImage];

            double scaleBy = 1 / subImage.ViewportWidth;
            return new Rect(-subImage.ViewportOrigin.X * scaleBy, -subImage.ViewportOrigin.Y * scaleBy,
                                            1 * scaleBy, (1 / subImage.AspectRatio) * scaleBy);
        }

        public static void HideAll(this MultiScaleImage msi)
        {
            DZContext context = msi.EnsureContext();
            context.ImagesToHide.AddRange(msi.SubImages);
            context.ImagesToShow.Clear();
        }

        public static Rect LogicalToElementRect(this MultiScaleImage msi, Rect rect)
        {
            return new Rect(msi.LogicalToElementPoint(new Point(rect.Left, rect.Top)),
                            msi.LogicalToElementPoint(new Point(rect.Right, rect.Bottom)));
        }

        public static void RandomizeAndArrange(this MultiScaleImage msi)
        {
            DZContext context = msi.EnsureContext();
            context.ImagesToShow.Clear();
            context.ImagesToShow.AddRange(msi.RandomizedListOfImages());
            msi.ArrangeImages(true);
        }

        public static List<MultiScaleSubImage> RandomizedListOfImages(this MultiScaleImage msi)
        {
            List<MultiScaleSubImage> imageList = new List<MultiScaleSubImage>();
            Random ranNum = new Random();

            DZContext context = msi.EnsureContext();
            // Store List of Images
            context.ImagesToShow.ForEach(subImage => imageList.Add(subImage));

            // Randomize Image List
            int numImages = imageList.Count;
            for (int i = 0; i < numImages; i++)
            {
                MultiScaleSubImage tempImage = imageList[i];
                imageList.RemoveAt(i);

                int ranNumSelect = ranNum.Next(imageList.Count);

                imageList.Insert(ranNumSelect, tempImage);
            }
            return imageList;
        }

        public static void SetArrangeOnFirstMotionFinished(DependencyObject obj, bool value)
        {
            obj.SetValue(ArrangeOnFirstMotionFinishedProperty, value);
        }

        public static void SetContext(DependencyObject obj, DZContext value)
        {
            obj.SetValue(ContextProperty, value);
        }

        public static void SetIsMousePanEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsMousePanEnabledProperty, value);
        }

        public static void SetIsMouseWheelEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsMouseWheelEnabledProperty, value);
        }

        public static void SetIsZoomOnClickEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsZoomOnClickEnabledProperty, value);
        }

        public static void SetIsZoomForceEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsZoomForceEnabledProperty, value);
        }

        public static void ShowAll(this MultiScaleImage msi)
        {
            DZContext context = msi.EnsureContext();
            context.ImagesToShow.AddRange(msi.SubImages);
            context.ImagesToHide.Clear();
        }

        public static int SubImageHitTest(this MultiScaleImage msi, Point p)
        {
            for (int i = 0; i < msi.SubImages.Count; i++)
            {
                Rect subImageRect = msi.GetSubImageRect(i);
                if (subImageRect.Contains(p))
                    return i;
            }

            return -1;
        }

        public static void Zoom(this MultiScaleImage msi, double zoom, Point pointToZoom)
        {
            Zoom(msi, zoom, pointToZoom, false);
        }

        public static void Zoom(this MultiScaleImage msi, double zoom, Point pointToZoom, bool force)
        {
            if (force || ((zoom >= 1.0 && msi.ViewportWidth > 0.05) || (zoom < 1.0 && msi.ViewportWidth < 2)))
            {
                Point logicalPoint = msi.ElementToLogicalPoint(pointToZoom);
                msi.ZoomAboutLogicalPoint(zoom, logicalPoint.X, logicalPoint.Y);
            }           
        }

        public static void ZoomAndCenterImage(this MultiScaleImage msi, int subImageIndex, double zoomFactor)
        {
            Rect subImageRect = msi.GetSubImageRect(subImageIndex);
            msi.DisplaySubImageCentered(subImageIndex);
            msi.ZoomAboutLogicalPoint(zoomFactor, (subImageRect.Left + subImageRect.Right) / 2, (subImageRect.Top + subImageRect.Bottom) / 2);
        }

        public static void ZoomCenter(this MultiScaleImage msi)
        {
            msi.ZoomCenter(1);
        }

        public static void ZoomCenter(this MultiScaleImage msi, int nbVirtualPage)
        {
            Rect subImageRect = msi.GetGlobalRect(1);
            Rect imageRectElement = msi.LogicalToElementRect(subImageRect);

            // Calculate the zoom factor such that the image will fill up the entire screen
            double zoomFactor = (msi.ActualWidth / imageRectElement.Width) < (msi.ActualHeight / imageRectElement.Height) ?
                                                msi.ActualWidth / imageRectElement.Width : msi.ActualHeight / imageRectElement.Height;

            // Not a real zoom factor, don't zoom
            if (!zoomFactor.IsRational())
                return;

            // Center the image
            msi.CenterLogicalRect(subImageRect);

            // Use the mid point of the image to zoom from
            msi.ZoomAboutLogicalPoint(zoomFactor, (subImageRect.Left + subImageRect.Right) / 2, (subImageRect.Top + subImageRect.Bottom) / 2);
        }

        public static void ZoomFullAndCenterImage(this MultiScaleImage msi, int subImageIndex)
        {
            Rect subImageRect = msi.GetSubImageRect(subImageIndex);
            if (subImageRect == Rect.Empty)
            {
                msi.ZoomCenter();
            }
            else
            {
                Rect imageRectElement = msi.LogicalToElementRect(msi.GetSubImageRect(subImageIndex));

                // Calculate the zoom factor such that the image will fill up the entire screen
                double zoomFactor = Math.Min(msi.ActualHeight / imageRectElement.Height, msi.ActualWidth / imageRectElement.Width);

                // Not a real zoom factor, don't zoom
                if (!zoomFactor.IsRational())
                    return;

                // Center the image
                msi.DisplaySubImageCentered(subImageIndex);
                // Use the mid point of the image to zoom from
                msi.ZoomAboutLogicalPoint(zoomFactor, (subImageRect.Left + subImageRect.Right) / 2, (subImageRect.Top + subImageRect.Bottom) / 2);
            }
        }

        public static void ZoomUniformToFillAndCenterImage(this MultiScaleImage msi, int subImageIndex)
        {
            Rect subImageRect = msi.GetSubImageRect(subImageIndex);
            if (subImageRect == Rect.Empty)
            {
                msi.ZoomCenter();
            }
            else
            {
                Rect imageRectElement = msi.LogicalToElementRect(msi.GetSubImageRect(subImageIndex));

                // Calculate the zoom factor such that the image will fill up the entire screen
                double zoomFactor = Math.Max(msi.ActualHeight / imageRectElement.Height, msi.ActualWidth / imageRectElement.Width);

                // Center the image
                msi.DisplaySubImageCentered(subImageIndex);
                // Use the mid point of the image to zoom from
                msi.ZoomAboutLogicalPoint(zoomFactor, (subImageRect.Left + subImageRect.Right) / 2, (subImageRect.Top + subImageRect.Bottom) / 2);
            }
        }

        private static void AnimateImage(MultiScaleSubImage currentImage, Point futurePosition, Storyboard _moveStoryboard)
        {
            // Create Keyframe
            SplinePointKeyFrame endKeyframe = new SplinePointKeyFrame();
            endKeyframe.Value = futurePosition;
            endKeyframe.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1));

            KeySpline ks = new KeySpline();
            ks.ControlPoint1 = new Point(0, 1);
            ks.ControlPoint2 = new Point(1, 1);
            endKeyframe.KeySpline = ks;

            // Create Animation
            PointAnimationUsingKeyFrames moveAnimation = new PointAnimationUsingKeyFrames();
            moveAnimation.KeyFrames.Add(endKeyframe);

            Storyboard.SetTarget(moveAnimation, currentImage);
            Storyboard.SetTargetProperty(moveAnimation, new PropertyPath("ViewportOrigin"));

            _moveStoryboard.Children.Add(moveAnimation);
        }

        private static void ArrangeOnFirstMotionFinishedChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MultiScaleImage msi = d as MultiScaleImage;
            if (msi == null)
                return;

            DZContext context = msi.EnsureContext();
            context.ArrangeOnFirstMotionFinished = (bool)e.NewValue;
        }

        private static void IsMousePanEnabledChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MultiScaleImage msi = d as MultiScaleImage;
            if (msi == null)
                return;

            DZContext context = msi.EnsureContext();
            context.IsMousePanEnabled = (bool)e.NewValue;
        }

        private static void IsMouseWheelEnabledChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MultiScaleImage msi = d as MultiScaleImage;
            if (msi == null)
                return;

            DZContext context = msi.EnsureContext();
            context.IsMouseWheelEnabled = (bool)e.NewValue;
        }

        private static void IsZoomOnClickEnabledChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MultiScaleImage msi = d as MultiScaleImage;
            if (msi == null)
                return;

            DZContext context = msi.EnsureContext();
            context.IsZoomOnClickEnabled = (bool)e.NewValue;
        }

        private static void IsZoomForceEnabledChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MultiScaleImage msi = d as MultiScaleImage;
            if (msi == null)
                return;

            DZContext context = msi.EnsureContext();
            context.IsZoomForceEnabled = (bool)e.NewValue;
        }

        #endregion Methods
    }
}