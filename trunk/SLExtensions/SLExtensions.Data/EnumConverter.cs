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

    public class EnumConverter : IValueConverter
    {
        #region Methods

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return value;

            if (value is Enum)
            {
                if (targetType == typeof(short))
                    return (short)value;

                if (targetType == typeof(ushort))
                    return (ushort)value;

                if (targetType == typeof(int))
                    return (int)value;

                if (targetType == typeof(uint))
                    return (uint)value;

                if (targetType == typeof(long))
                    return (long)value;

                if (targetType == typeof(ulong))
                    return (ulong)value;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return value;

            if (targetType.IsEnum)
            {
                if (value is short
                    || value is ushort
                    || value is int
                    || value is uint
                    || value is long
                    || value is ulong)
                    return Enum.ToObject(targetType, value);
            }

            return value;
        }

        #endregion Methods
    }
}