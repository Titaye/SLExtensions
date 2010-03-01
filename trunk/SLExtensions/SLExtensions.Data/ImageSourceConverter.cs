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

    public class ImageSourceConverter : IValueConverter
    {
        #region Methods

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            byte[] content = value as byte[];
            if (content != null)
            {
                BitmapImage source = new BitmapImage();
                source.SetSource(new MemoryStream(content));
                return source;
            }

            Uri uri = new UriConverter().Convert(value, targetType, parameter, culture) as Uri;
            if (uri != null)
            {
                return new BitmapImage(uri);
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion Methods
    }
}