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

    public class BoolConverter : IValueConverter
    {
        #region Methods

        public virtual object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter != null)
            {
                if (value != null)
                {
                    Type valueType = value.GetType();
                    if (parameter.GetType() != valueType)
                    {
                        if (valueType.IsEnum)
                        {
                            bool not = false;
                            string strPrm = System.Convert.ToString(parameter);
                            if (strPrm.StartsWith("!"))
                            {
                                not = true;
                                strPrm = strPrm.Substring(1);
                            }

                            object convertedParam = Enum.Parse(valueType, strPrm, true);
                            bool result = object.Equals(value, convertedParam);
                            return not ? !result : result;
                        }
                        else if(parameter is IConvertible)
                        {
                            try
                            {
                                object convertedParam = System.Convert.ChangeType(parameter, value.GetType(), CultureInfo.CurrentCulture);
                                return object.Equals(value, convertedParam);
                            }
                            catch
                            {
                                // Fallback to default case
                            }
                        }
                    }

                    if (parameter is string)
                    {
                        try
                        {
                            string strPrm = (string)parameter;

                            decimal dec = (decimal)System.Convert.ChangeType(value, typeof(decimal), CultureInfo.CurrentCulture);

                            if (strPrm.StartsWith(">="))
                            {
                                decimal prmval;
                                if (decimal.TryParse(strPrm.Substring(2), NumberStyles.Any, CultureInfo.InvariantCulture, out prmval))
                                {
                                    return dec >= prmval;
                                }
                            }
                            else if (strPrm.StartsWith("<="))
                            {
                                decimal prmval;
                                if (decimal.TryParse(strPrm.Substring(2), NumberStyles.Any, CultureInfo.InvariantCulture, out prmval))
                                {
                                    return dec <= prmval;
                                }
                            }
                            else if (strPrm.StartsWith(">"))
                            {
                                decimal prmval;
                                if (decimal.TryParse(strPrm.Substring(1), NumberStyles.Any, CultureInfo.InvariantCulture, out prmval))
                                {
                                    return dec > prmval;
                                }
                            }
                            else if (strPrm.StartsWith("<"))
                            {
                                decimal prmval;
                                if (decimal.TryParse(strPrm.Substring(1), NumberStyles.Any, CultureInfo.InvariantCulture, out prmval))
                                {
                                    return dec < prmval;
                                }
                            }
                            else if (strPrm.StartsWith("!="))
                            {
                                decimal prmval;
                                if (decimal.TryParse(strPrm.Substring(2), NumberStyles.Any, CultureInfo.InvariantCulture, out prmval))
                                {
                                    return dec != prmval;
                                }
                            }
                            else if (strPrm.StartsWith("!"))
                            {
                                decimal prmval;
                                if (decimal.TryParse(strPrm.Substring(1), NumberStyles.Any, CultureInfo.InvariantCulture, out prmval))
                                {
                                    return dec != prmval;
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                }

                // If a parameter is provided returns true if the value is equals to the parameter
                return object.Equals(value, parameter);
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

            if (value is string)
            {
                return (String.IsNullOrEmpty((string)value)) ? false : true;
            }
            
            // if value != null returns true
            return (value != null) ? true : false;
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion Methods
    }
}