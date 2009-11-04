using System;
using System.Linq;
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
using System.Text.RegularExpressions;
using System.Text;

namespace SLExtensions.Data
{
    public class PercentConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return value;

            try
            {
                double dbl = System.Convert.ToDouble(value);
                string prms = parameter as string;
                if (string.IsNullOrEmpty(prms))
                {
                    return dbl.ToString("p");
                }

                string[] parts = prms.ToLower().Split(' ');

                bool showPercent = Array.IndexOf(parts, "-%") == -1;
                bool space = Array.IndexOf(parts, "sp") != -1;

                string number = (from s in parts
                                where numberRegex.IsMatch(s)
                                select s).FirstOrDefault();

                int decDigits = 0;
                if (!string.IsNullOrEmpty(number))
                    decDigits = int.Parse(number);

                dbl = Math.Round(dbl * 100, decDigits);

                StringBuilder sb = new StringBuilder();
                sb.Append(dbl);
                if(space)
                    sb.Append(" ");
                if(showPercent)
                    sb.Append("%");

                return sb.ToString();
            }
            catch
            {
                return null;
            }
        }

        private static Regex numberRegex = new Regex(@"^\d+$");

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
