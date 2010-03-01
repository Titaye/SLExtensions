namespace SLExtensions.DeepZoom
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

    public class SubImage
    {
        #region Fields

        private int _colNum = 0;
        private Size _imageSize = new Size(0.0, 1.0); //normalized height is 1 for all images
        private int _rowNum = 0;

        #endregion Fields

        #region Constructors

        internal SubImage(MultiScaleSubImage image)
        {
            // Capture the normalized size of each image (fixed height of 1)
            // This normalization is required since we want the height of all images to be the same but the widths can differ
            _imageSize.Width = image.AspectRatio;
        }

        #endregion Constructors

        #region Properties

        internal int ColNum
        {
            get { return _colNum; }
            set { _colNum = value; }
        }

        internal double Height
        {
            get { return _imageSize.Height; }
        }

        internal int RowNum
        {
            get { return _rowNum; }
            set { _rowNum = value; }
        }

        internal double Width
        {
            get { return _imageSize.Width; }
        }

        #endregion Properties

        #region Methods

        internal void Scale(double scaleBy)
        {
            _imageSize.Width *= scaleBy;
            _imageSize.Height *= scaleBy;
        }

        #endregion Methods
    }
}