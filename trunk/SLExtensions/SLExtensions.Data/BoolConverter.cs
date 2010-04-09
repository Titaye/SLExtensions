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
    using System.Collections;

    public class BoolConverter : IValueConverter
    {
        #region Methods

        public virtual object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter != null)
            {
                return ConvertAndHandleParameter(value, parameter, culture);
            }

            if (value is bool)
            {
                // if no parameter and value is bool, returns true when value equals true
                return (bool)value;
            }

            if (value is bool?)
            {
                // if no parameter and value is bool, returns true when value equals true
                return ((bool?)value).GetValueOrDefault();
            }

            if (value is int)
            {
                return ((int)value == 0) ? false : true;
            }

            if (value is IEnumerable)
            {
                var iter = ((IEnumerable)value).GetEnumerator();
                return iter.MoveNext();
            }

            string strValue = value as string;
            if (strValue != null)
            {
                return string.IsNullOrEmpty(strValue) ? false : true;
            }

            // if value != null returns true
            return (value != null) ? true : false;
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
            {
                bool b = (bool)value;
                if ("!".Equals(parameter) || "!?".Equals(parameter))
                {
                    return !b;
                }
                return b;
            }
            else
                throw new NotImplementedException();
        }

        private static bool? ComputeDecimalResult(string op, decimal dec, string strPrm, Func<decimal, decimal, bool> func)
        {
            if (strPrm.StartsWith(op, StringComparison.CurrentCulture))
            {
                decimal prmval;
                if (decimal.TryParse(strPrm.Substring(op.Length), NumberStyles.Any, CultureInfo.InvariantCulture, out prmval))
                {
                    return func(dec, prmval);
                }
            }
            return null;
        }

        private static object ConvertAndHandleParameter(object value, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                if (value is bool)
                {
                    bool b = (bool)value;
                    if ("!".Equals(parameter) || "!?".Equals(parameter))
                    {
                        return !b;
                    }
                    return b;
                }

                if (value is IEnumerable)
                {
                    var iter = ((IEnumerable)value).GetEnumerator();
                    if ("!".Equals(parameter) || "!?".Equals(parameter))
                    {
                        return !iter.MoveNext();
                    }
                    return iter.MoveNext();                    
                }

                bool convertResult;
                if (ConvertEnum(value, parameter, culture, out convertResult))
                    return convertResult;

                if (ConvertDecimal(value, parameter, culture, out convertResult))
                    return convertResult;

                if (ConvertIConvertible(value, parameter, culture, out convertResult))
                    return convertResult;

                if (ConvertUri(value, parameter, culture, out convertResult))
                    return convertResult;
            }

            if (parameter is string && "!?".Equals(parameter) && value == null)
            {
                return value;
            }

            if (parameter is string && "!".Equals(parameter))
            {
                return value == null;
            }

            // If a parameter is provided returns true if the value is equals to the parameter

            return object.Equals(value, parameter);
        }

        private static bool ConvertDecimal(object value, object parameter, System.Globalization.CultureInfo culture, out bool result)
        {
            result = false;

            if (value == null)
                return false;

            Type valueType = value.GetType();
            if (!typeof(IConvertible).IsAssignableFrom(valueType))
                return false;

            string strPrm = parameter as string;
            if (!string.IsNullOrEmpty(strPrm))
            {
                try
                {

                    decimal dec = (decimal)System.Convert.ChangeType(value, typeof(decimal), culture);

                    bool? decResult = ComputeDecimalResult(">=", dec, strPrm, (d1, d2) => d1 >= d2)
                                        ?? ComputeDecimalResult("<=", dec, strPrm, (d1, d2) => d1 <= d2)
                                        ?? ComputeDecimalResult(">", dec, strPrm, (d1, d2) => d1 > d2)
                                        ?? ComputeDecimalResult("<", dec, strPrm, (d1, d2) => d1 < d2)
                                        ?? ComputeDecimalResult("!=", dec, strPrm, (d1, d2) => d1 != d2)
                                        ?? ComputeDecimalResult("!", dec, strPrm, (d1, d2) => d1 != d2);
                    if (decResult.HasValue)
                    {
                        result = decResult.Value;
                        return true;
                    }
                }
                catch (FormatException)
                {
                    // Can't convert
                    // Fallback to default case
                }
                catch (InvalidCastException)
                {
                    // Can't convert
                    // Fallback to default case
                }
            }

            return false;
        }

        private static bool ConvertEnum(object value, object parameter, System.Globalization.CultureInfo culture, out bool result)
        {
            result = false;

            if (value == null)
                return false;

            Type valueType = value.GetType();
            if (valueType.IsEnum)
            {
                bool not = false;
                string strPrm = System.Convert.ToString(parameter, culture);
                if (string.IsNullOrEmpty(strPrm))
                    return false;

                if (strPrm.StartsWith("!", StringComparison.CurrentCulture))
                {
                    not = true;
                    strPrm = strPrm.Substring(1);
                }

                object convertedParam = Enum.Parse(valueType, strPrm, true);
                result = object.Equals(value, convertedParam);
                result = not ? !result : result;
                return true;
            }

            return false;
        }

        private static bool ConvertIConvertible(object value, object parameter, System.Globalization.CultureInfo culture, out bool result)
        {
            result = false;

            if (value == null)
                return false;

            Type valueType = value.GetType();
            if (typeof(IConvertible).IsAssignableFrom(valueType))
            {
                try
                {
                    object convertedParam = null;
                    try
                    {
                        convertedParam = System.Convert.ChangeType(parameter, valueType, culture);
                    }
                    catch (FormatException)
                    {
                        // Bad format for given culture, trying with invariant culture
                        convertedParam = System.Convert.ChangeType(parameter, valueType, CultureInfo.InvariantCulture);
                    }

                    result = object.Equals(value, convertedParam);
                    return true;

                }
                catch (FormatException)
                {
                    // Can't convert
                    // Fallback to default case
                }
                catch (InvalidCastException)
                {
                    // Can't convert
                    // Fallback to default case
                }

            }
            return false;
        }

        private static bool ConvertUri(object value, object parameter, CultureInfo culture, out bool result)
        {
            result = false;

            if (value == null)
                return false;

            Uri uri = value as Uri;
            if (uri == null)
            {
                return false;
            }

            Uri uriParamater;
            string strParameter = parameter as string;
            if (string.IsNullOrEmpty(strParameter))
                return false;

            if ("!".Equals(strParameter))
            {
                result = false;
                return true;
            }

            bool not = strParameter.StartsWith("!");
            strParameter = strParameter.TrimStart('!');

            if (!Uri.TryCreate(strParameter, UriKind.RelativeOrAbsolute, out uriParamater))
                return false;

            result = object.Equals(uriParamater, uri);
            if (not)
                result = !result;
            return true;
        }

        #endregion Methods
    }
}