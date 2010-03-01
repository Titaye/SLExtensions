namespace SLExtensions.Sharepoint
{
    using System;
    using System.Globalization;
    using System.Net;
    using System.Runtime.Serialization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    [DataContract]
    public class NumberColumnInfo : TextColumnInfo
    {
        #region Methods

        public override object GetFieldValue(TemplateDataBase templateDataBase, System.Xml.Linq.XElement element)
        {
            object value = base.GetFieldValue(templateDataBase, element);

            double dbl;
            if (double.TryParse(value as string, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out dbl))
                return dbl;

            return value;
        }

        public override void SetFieldValue(System.Xml.Linq.XElement field, object value)
        {
            if (value != null && value is double)
            {
                NumberFormatInfo nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
                nfi.NumberGroupSeparator = string.Empty;
                value = ((double)value).ToString(nfi);
            }
            else if (value is string)
            {
                value = ((string)value).Trim();
            }
            base.SetFieldValue(field, value);
        }

        #endregion Methods
        
    }
}