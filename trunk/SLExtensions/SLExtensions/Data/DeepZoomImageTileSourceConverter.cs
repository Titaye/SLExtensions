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

namespace SLExtensions.Data
{
    public class DeepZoomImageTileSourceConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Uri uri;
            if (value is string)
            {
                uri = new Uri(value as string, UriKind.RelativeOrAbsolute);
            }
            else if (value is Uri)
            {
                uri = new UriConverter().Convert(value, targetType, parameter, culture) as Uri;
            }
            else
                return value;

            return new DeepZoomImageTileSource(uri);            
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
