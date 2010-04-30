// <copyright file="StringFormatConverter.cs" company="Ucaya">
// Distributed under Microsoft Public License (Ms-PL)
// </copyright>
// <author>Thierry Bouquain</author>
namespace SLExtensions.Data
{
    using System;
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
    /// Convert data to string objects as it passes through the binding engine.
    /// </summary>
    public class StringFormatConverter : IValueConverter
    {
        #region Fields

        /// <summary>
        /// Default string format syntax
        /// </summary>
        private const string DefaultStringFormat = "{0}";

        #endregion Fields

        #region Methods

        /// <summary>
        /// Convert source data to string before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The <see cref="T:System.Type"/> of data expected by the target dependency property.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>
        /// The value to be passed to the target dependency property.
        /// </returns>
        public virtual object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(string))
            {
                throw new ArgumentException(Resource.StringFormatConverterExceptionInvalidTargetType);
            }

            if (value == null)
            {
                return string.Empty;
            }

            if (parameter == null && value is Duration)
            {
                Duration duration = (Duration)value;
                if (!duration.HasTimeSpan)
                    return string.Empty;

                return duration.TimeSpan.ToString().Split('.')[0];
            }

            if (parameter == null && value is TimeSpan)
            {
                return ((TimeSpan) value).ToString().Split('.')[0];
            }

            string format = (parameter as string) ?? DefaultStringFormat;
            return string.Format(culture, format, value);
        }

        /// <summary>
        /// This convertion is not supported
        /// </summary>
        /// <param name="value">The target data being passed to the source.</param>
        /// <param name="targetType">The <see cref="T:System.Type"/> of data expected by the source object.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>
        /// The value to be passed to the source object.
        /// </returns>
        public virtual object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion Methods
    }
}