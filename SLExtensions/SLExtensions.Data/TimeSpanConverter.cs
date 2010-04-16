namespace SLExtensions.Data
{
    using System;
    using System.Globalization;
    using System.Linq;
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
        #region Properties

        public bool RemoveEmptyHours
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType == typeof(Double))
            {
                if (value is TimeSpan)
                {
                    TimeSpan ts = (TimeSpan)value;
                    return (double)ts.TotalSeconds;
                }

                if (value is Duration)
                {
                    Duration duration = (Duration)value;
                    if(duration.HasTimeSpan)
                        return (double)duration.TimeSpan.TotalSeconds;
                    return 0;
                }
                return 0;
            }

            if (targetType == typeof(string))
            {
                TimeSpan? ts = value as TimeSpan?;

                if (!ts.HasValue)
                {
                    Duration? duration = value as Duration?;
                    if (duration.HasValue && duration.Value.HasTimeSpan)
                        ts = duration.Value.TimeSpan;

                    double? dbl = value as double?;
                    if (dbl.HasValue)
                        ts = TimeSpan.FromSeconds(dbl.Value);
                }

                if(!ts.HasValue)
                {
                    return string.Empty;
                }

                if (ts.Value.Milliseconds != 0)
                    ts = ts.Value.Add(TimeSpan.FromMilliseconds(-ts.Value.Milliseconds));
                var str = ts.Value.ToString();

                var tparts = str.Split(':');
                if (RemoveEmptyHours && tparts[0] == "00")
                    return string.Join(":", tparts.Skip(1).ToArray());
                return str;
            }

            if (targetType == typeof(TimeSpan))
            {
                double secs;
                try
                {
                    secs = System.Convert.ToDouble(value, culture);
                }
                catch(FormatException)
                {
                    // Failed with current culture, try with invariant one
                    secs = System.Convert.ToDouble(value, CultureInfo.InvariantCulture);
                }
                return TimeSpan.FromSeconds(secs);
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType == typeof(Double))
            {
                TimeSpan? ts = value as TimeSpan?;
                if (ts.HasValue)
                {
                    return (double)ts.Value.TotalSeconds;
                }
                return value;
            }

            if (targetType == typeof(TimeSpan))
            {
                double secs;
                try
                {
                    secs = System.Convert.ToDouble(value, culture);
                }
                catch (FormatException)
                {
                    // Failed with current culture, try with invariant one
                    secs = System.Convert.ToDouble(value, CultureInfo.InvariantCulture);
                }
                return TimeSpan.FromSeconds(secs);
            }

            return value;
        }

        #endregion Methods
    }
}