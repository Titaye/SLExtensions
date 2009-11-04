namespace SLExtensions.Data
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Text;
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

    /// <summary>
    /// Format a parameter as percent.
    /// A string parameter can be given to modify the string output.
    /// If no parameter is given, the culture percent format is used.
    /// Possible paramater values are an int to set the number of decimal, '-%' to remove the percent sign, 'sp' to add a space between value and percent sign.
    /// </summary>
    public class PercentConverter : IValueConverter
    {
        #region Fields

        private static Regex numberRegex = new Regex(@"^\d+$");

        #endregion Fields

        #region Methods

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return value;

            double dbl;
            try
            {
                dbl = System.Convert.ToDouble(value, culture);
            }
            catch (FormatException)
            {
                // Unable to format in current culture, try again with invariant culture
                try
                {
                    dbl = System.Convert.ToDouble(value, CultureInfo.InvariantCulture);
                }
                catch (FormatException)
                {
                    // Not a number
                    dbl = Double.NaN;
                }
            }
            catch (InvalidCastException)
            {
                // object can't be converted to dbl
                dbl = Double.NaN;
            }

            if (Double.IsInfinity(dbl)
                || Double.IsNaN(dbl))
            {
                return null;
            }

            string prms = parameter as string;
            if (string.IsNullOrEmpty(prms))
            {
                return dbl.ToString("p", culture);
            }

            // check the given parameter to modify the output
            string[] parts = prms.ToLower(culture).Split(' ');

            bool showPercent = Array.IndexOf(parts, "-%") == -1;
            bool space = Array.IndexOf(parts, "sp") != -1;

            string number = (from s in parts
                             where numberRegex.IsMatch(s)
                             select s).FirstOrDefault();

            int decDigits = 0;
            if (!string.IsNullOrEmpty(number))
            {
                decDigits = int.Parse(number, culture);
            }

            dbl = Math.Round(dbl * 100, decDigits);

            StringBuilder sb = new StringBuilder();
            sb.Append(dbl);
            if (space)
            {
                sb.Append(" ");
            }

            if (showPercent)
            {
                sb.Append("%");
            }

            return sb.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion Methods
    }
}