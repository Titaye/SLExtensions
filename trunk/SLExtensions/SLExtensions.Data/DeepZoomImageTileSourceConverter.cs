namespace SLExtensions.Data
{
    using System;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    public class DeepZoomImageTileSourceConverter : IValueConverter
    {
        #region Methods

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string strValue = value as string;
            Uri uri;
            if (!string.IsNullOrEmpty(strValue))
            {
                uri = new Uri(strValue, UriKind.RelativeOrAbsolute);
            }
            else if ((uri = value as Uri) != null)
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

        #endregion Methods
    }
}