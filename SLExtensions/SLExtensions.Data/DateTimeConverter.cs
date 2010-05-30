namespace SLExtensions.Data
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
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

    public class DateTimeValueConverter : IValueConverter
    {
        #region Methods

        /// <summary>
        /// Converts a value. The data binding engine calls this method when it propagates a value from the binding source to the binding target.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value.If the method returns null, the valid null value is used.A return value of <see cref="T:System.Windows.DependencyProperty"></see>.<see cref="F:System.Windows.DependencyProperty.UnsetValue"></see> indicates that the converter produced no value and that the binding uses the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"></see>, if available, or the default value instead.A return value of <see cref="T:System.Windows.Data.Binding"></see>.<see cref="F:System.Windows.Data.Binding.DoNothing"></see> indicates that the binding does not transfer the value or use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"></see> or default value.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DateTime dateTime;
            if (value is DateTime)
            {
                dateTime = (DateTime)value;

                // if the parameter isn't used or isn't a string,
                // just convert to a short datetime
                IFormatProvider format = CultureInfo.CurrentCulture.GetFormat(targetType) as IFormatProvider;
                if (parameter == null || !(parameter is String))
                {
                    return dateTime.ToString("d", format);
                }
                else
                {
                    // Attempt to get the format provider for the culture specified
                    return dateTime.ToString((string)parameter, format);
                }
            }

            // Can't do anything with it so jsut return the value
            Debug.WriteLine("Failed to convert DateTime to string, value is not a DateTime value.");
            return value;
        }

        /// <summary>
        /// Converts a value. The data binding engine calls this method when it propagates a value from the binding target to the binding source.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value.If the method returns null, the valid null value is used.A return value of <see cref="T:System.Windows.DependencyProperty"></see>.<see cref="F:System.Windows.DependencyProperty.UnsetValue"></see> indicates that the converter produced no value and that to the binding uses the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"></see>, if available, or the default value instead.A return value of <see cref="T:System.Windows.Data.Binding"></see>.<see cref="F:System.Windows.Data.Binding.DoNothing"></see> indicates that the binding does not transfer the value or use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"></see> or default value.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // if its not a date time we can't convert it.
            if (targetType != typeof(DateTime) && targetType != typeof(DateTime?))
            {
                Debug.WriteLine("Failed to convert back to DateTime, invalid target type specified");
                return value;
            }

            // Attempt to parse the DateTime
            DateTime date;
            if (!DateTime.TryParse(value as string, out date))
            {
                Debug.WriteLine("Failed to convert back to DateTime, failed to parse string.");
                return value;
            }

            // We converted it so return it
            return date;
        }

        #endregion Methods
    }
}