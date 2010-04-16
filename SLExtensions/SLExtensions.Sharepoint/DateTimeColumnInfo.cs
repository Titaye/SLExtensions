namespace SLExtensions.Sharepoint
{
    using System;
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
    public class DateTimeColumnInfo : TextColumnInfo
    {
        #region Methods

        public override object GetFieldValue(TemplateDataBase templateDataBase, System.Xml.Linq.XElement element)
        {
            var content = base.GetFieldValue(templateDataBase, element);
            return ConvertISO8601ToDateTime(Convert.ToString(content));
        }

        public override void SetFieldValue(System.Xml.Linq.XElement field, object value)
        {
            if(value != null && value is DateTime)
                base.SetFieldValue(field, ConvertDateTimeToISO8601((DateTime)value));
        }

        private static String ConvertDateTimeToISO8601(DateTime dt)
        {
            //return dt.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'");
            string val =  dt.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'");
            return val;
        }

        private static DateTime? ConvertISO8601ToDateTime(String iso)
        {
            DateTime dt;
            if (DateTime.TryParseExact(iso, "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'", System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out dt))
            {
                //return dt.ToLocalTime();
                return dt;
            }
            else if (DateTime.TryParseExact(iso, "yyyy'-'MM'-'dd' 'HH':'mm':'ss", System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.AssumeLocal, out dt))
            {
                //return dt.ToLocalTime();
                return dt;
            }
            return null;
        }

        #endregion Methods
    }
}