// <copyright file="VisibilityConverter.cs" company="Ucaya">
// Distributed under Microsoft Public License (Ms-PL)
// </copyright>
// <author>Thierry Bouquain</author>
namespace SLExtensions.Data
{
    using System;
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

    /// <summary>
    /// Converts a given value to a <see cref="System.Windows.Visibility"/> enum value.
    /// If a VisibilityConverterParameter is provided, handle the convertion from its properties values
    /// If a parameter is provided returns visible if the value is equals to the parameter
    /// if no parameter and value is bool, returns visible when value equals true
    /// if value != null returns visible
    /// </summary>
    public class VisibilityConverter : BoolConverter
    {
        #region Methods

        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The <see cref="T:System.Type"/> of data expected by the target dependency property.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>
        /// The value to be passed to the target dependency property.
        /// </returns>
        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            VisibilityConverterParameter prm = parameter as VisibilityConverterParameter;
            if (prm != null)
            {
                bool prmResult = (bool)base.Convert(value, targetType, prm.Value, culture);

                // If a VisibilityConverterParameter is provided, handle the convertion from its properties values
                Visibility trueValue = prm.Condition == VisibilityCondition.IfValueVisible ? Visibility.Visible : Visibility.Collapsed;
                Visibility falseValue = prm.Condition == VisibilityCondition.IfValueVisible ? Visibility.Collapsed : Visibility.Visible;
                return object.Equals(value, prmResult) ? trueValue : falseValue;
            }

            return (bool)base.Convert(value, targetType, parameter, culture) ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion Methods
    }
}