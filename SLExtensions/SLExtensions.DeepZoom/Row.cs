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

    public class Row
    {
        #region Fields

        double _spaceBetweenImages = 0;
        List<SubImage> _subImages = new List<SubImage>();

        #endregion Fields

        #region Constructors

        internal Row(double spaceBetweenImage)
        {
            _spaceBetweenImages = spaceBetweenImage;
        }

        #endregion Constructors

        #region Properties

        internal double Height
        {
            get { return (ImageCount <= 0) ? 0 : _subImages[0].Height; }
        }

        internal int ImageCount
        {
            get { return _subImages.Count; }
        }

        internal double TotalSpaceBetweenImages
        {
            get { return (ImageCount <= 0) ? 0 : (ImageCount - 1) * _spaceBetweenImages; }
        }

        internal double TotalWidth
        {
            get
            {
                double totalWidth = 0;
                _subImages.ForEach(image => totalWidth += image.Width);
                return totalWidth;
            }
        }

        #endregion Properties

        #region Methods

        internal void AddImage(SubImage subImage)
        {
            _subImages.Add(subImage);
            subImage.ColNum = _subImages.Count - 1;
        }

        internal double CalcX(int colNum)
        {
            double X = 0;
            for (int i = 0; i < colNum; i++)
            {
                X += _subImages[i].Width + _spaceBetweenImages;
            }
            return X;
        }

        internal void Scale(double canvasWidth)
        {
            double scaleBy = (canvasWidth - TotalSpaceBetweenImages) / TotalWidth;
            foreach (SubImage subImage in _subImages)
                subImage.Scale(scaleBy);
        }

        #endregion Methods
    }
}