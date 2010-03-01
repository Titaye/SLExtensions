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

    public class InvertConverter : IValueConverter
    {
        #region Methods

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is int)
                return (int)value * -1;
            if (value is short)
                return (short)value * -1;
            if (value is long)
                return (long)value * -1;
            else if(value is float)
                return (float)value * -1;
            else if (value is double)
                return (double)value * -1;
            else if (value is decimal)
                return (decimal)value * -1;
            throw new NotSupportedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is int)
                return (int)value * -1;
            if (value is short)
                return (short)value * -1;
            if (value is long)
                return (long)value * -1;
            else if (value is float)
                return (float)value * -1;
            else if (value is double)
                return (double)value * -1;
            else if (value is decimal)
                return (decimal)value * -1;

            throw new NotSupportedException();
        }

        #endregion Methods
    }
}