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

    public class BitrateConverter : IValueConverter
    {
        #region Constructors

        public BitrateConverter()
        {
            NumberFormat = "N1";
        }

        #endregion Constructors

        #region Properties

        public string NumberFormat
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            decimal val;
            if (value is ulong
                || value is long
                || value is uint
                || value is int
                || value is double)
            {
                val = System.Convert.ToDecimal(value);
                string unit = " bps";
                if (val > 1000)
                {
                    val = val / 1024;
                    unit = " kbps";
                }
                if (val > 1000)
                {
                    val = val / 1024;
                    unit = " mbps";
                }

                return val.ToString(NumberFormat) + unit;
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