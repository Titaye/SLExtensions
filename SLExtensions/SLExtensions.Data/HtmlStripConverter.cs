namespace SLExtensions.Data
{
    using System;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    public class HtmlStripConverter : IValueConverter
    {
        #region Methods

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string val = value as string;
            if (string.IsNullOrEmpty(val))
                return string.Empty;

            return Regex.Replace(val, @"\<[^\>]*\>", string.Empty);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion Methods
    }
}