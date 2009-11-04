using System;
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
using System.Globalization;
using System.Diagnostics;

namespace SLExtensions.Data
{
  /// <summary>
  /// Converts a Decimal value to a culture sensitive money value.
  /// </summary>
  public class MoneyConverter : IValueConverter
  {
    #region IValueConverter Members

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
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value == null) return value;

      if (value.GetType() == typeof(Decimal) || value.GetType() == typeof(Decimal?))
      {
        return ((Decimal)value).ToString("C", culture.NumberFormat);
      }
      if (value.GetType() == typeof(double) || value.GetType() == typeof(double?))
      {
        return ((double)value).ToString("C", culture.NumberFormat);
      }

      // If can't convert just fail.
      Debug.WriteLine("Failed to Convert money value. Not a supported value type (decimal or double).");
      return value;
    }

    /// <summary>
    /// Modifies the target data before passing it to the source object.  This method is called only in <see cref="F:System.Windows.Data.BindingMode.TwoWay"/> bindings.
    /// </summary>
    /// <param name="value">The target data being passed to the source.</param>
    /// <param name="targetType">The <see cref="T:System.Type"/> of data expected by the source object.</param>
    /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
    /// <param name="culture">The culture of the conversion.</param>
    /// <returns>
    /// The value to be passed to the source object.
    /// </returns>
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value is string)
      {
        if (targetType == typeof(decimal) || targetType == typeof(decimal?))
        {
          Decimal returnValue;

          if (decimal.TryParse(((string)value), NumberStyles.Currency, culture.NumberFormat, out returnValue))
          {
            return returnValue;
          }
          else
          {
            Debug.WriteLine("Failed to ConvertBack money value. Failed to parse value.");
          }
        }
        else if (targetType == typeof(double) || targetType == typeof(double?))
        {
          double returnValue;

          if (double.TryParse(((string)value), NumberStyles.Currency, culture.NumberFormat, out returnValue))
          {
            return returnValue;
          }
          else
          {
            Debug.WriteLine("Failed to ConvertBack money value. Failed to parse value.");
          }
        }
      }
      else
      {
        Debug.WriteLine("Failed to ConvertBack money value. Value is not a string.");
      }

      // If it doesn't match our scenarios, just return the unconverted type
      return value;
    }

    #endregion
  }
}
