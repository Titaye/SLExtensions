namespace SLExtensions.Data
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// Used to decode Html Entities during databinding.
    /// </summary>
    public class HtmlDecodeConverter : IValueConverter
    {
        #region Methods

        /// <summary>
        /// Called during data binding.  Decodes any Html entities in the provided string.
        /// </summary>
        /// <param name="value">The string to decode.</param>
        /// <param name="targetType">The destination type, should be string.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">Not used.</param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Windows.Browser.HttpUtility.HtmlDecode(value as string);
        }

        /// <summary>
        /// Not implemented. 
        /// </summary>
        /// <param name="value">Not implemented.</param>
        /// <param name="targetType">Not implemented.</param>
        /// <param name="parameter">Not implemented.</param>
        /// <param name="culture">Not implemented.</param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion Methods
    }
}