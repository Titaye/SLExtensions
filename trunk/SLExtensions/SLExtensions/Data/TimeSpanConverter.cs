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

    public class TimeSpanConverter : IValueConverter
    {
        #region Methods

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType == typeof(Double))
            {
                if (value is TimeSpan)
                {
                    TimeSpan ts = (TimeSpan)value;
                    return (double)ts.Ticks;
                }

                if (value is Duration)
                {
                    Duration duration = (Duration)value;
                    if(duration.HasTimeSpan)
                        return (double)duration.TimeSpan.Ticks;
                    return 0;
                }
                return 0;
            }

            if (targetType == typeof(string))
            {
                TimeSpan ts;
                if (value is TimeSpan)
                {
                    ts = (TimeSpan)value;
                }
                else if (value is Duration)
                {
                    Duration duration = (Duration)value;
                    if (!duration.HasTimeSpan)
                        return string.Empty;

                    ts = duration.TimeSpan;
                }
                else
                {
                    return string.Empty;
                }

                return ts.ToString().Split('.')[0];
            }

            if (targetType == typeof(TimeSpan))
            {
                long lng = System.Convert.ToInt64(value);
                return TimeSpan.FromTicks(lng);
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType == typeof(Double))
            {
                if (value is TimeSpan)
                {
                    TimeSpan ts = (TimeSpan)value;
                    return (double)ts.Ticks;
                }
                return value;
            }

            if (targetType == typeof(TimeSpan))
            {
                long lng = System.Convert.ToInt64(value);
                return TimeSpan.FromTicks(lng);
            }

            return value;
        }

        #endregion Methods
    }
}