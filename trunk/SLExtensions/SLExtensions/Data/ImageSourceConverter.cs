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
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.IO;

namespace SLExtensions.Data
{
    public class ImageSourceConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            byte[] content = null;
            Uri uri = null;
            if (value is string)
            {
                uri = new Uri(value as string, UriKind.RelativeOrAbsolute);
            }
            else if (value is Uri)
            {
                uri = new UriConverter().Convert(value, targetType, parameter, culture) as Uri;
                uri = value as Uri;
            }
            else if (value is byte[])
            {
                content = value as byte[];
            }
            else
                return value;

            if (content == null && uri == null)
                return value;

            if (uri != null)
            {
                return new BitmapImage(uri);
            }
            else if (content != null)
            {
                BitmapImage source = new BitmapImage();
                source.SetSource(new MemoryStream(content));
                return source;
            }
            else
                return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
