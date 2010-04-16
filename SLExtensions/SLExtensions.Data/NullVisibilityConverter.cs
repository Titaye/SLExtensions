namespace SLExtensions.Data
{
    using System;
    using System.Windows;
    using System.Windows.Data;

    public class NullVisibilityConverter : IValueConverter
    {
        #region Methods

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType == typeof(Visibility))
              {
            if (parameter is string && ((String)parameter).ToLower() == "false")
            {
              return value == null ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
              return value != null ? Visibility.Visible : Visibility.Collapsed;
            }
              }

              // No Conversion
              return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // No support for converting back
              return value;
        }

        #endregion Methods
    }
}