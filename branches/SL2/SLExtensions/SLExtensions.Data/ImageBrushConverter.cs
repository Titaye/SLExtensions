namespace SLExtensions.Data
{
    using System;
    using System.IO;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;

    public class ImageBrushConverter : IValueConverter
    {
        #region Methods

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ImageSource source = new ImageSourceConverter().Convert(value, targetType, parameter, culture) as ImageSource;
            if (source == null)
                return value;

            ImageBrush brush = new ImageBrush();
            brush.ImageFailed += delegate { };
            brush.ImageSource = source;

            if (parameter is Stretch)
                brush.Stretch = (Stretch)parameter;
            else
                brush.Stretch = Stretch.Uniform;
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion Methods
    }
}