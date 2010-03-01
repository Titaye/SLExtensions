namespace SLExtensions.Sharepoint
{
    using System;
    using System.Net;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    using SLExtensions;

    [DataContract]
    public class BoolColumnInfo : TextColumnInfo
    {
        #region Methods

        public override object GetFieldValue(TemplateDataBase templateDataBase, System.Xml.Linq.XElement element)
        {
            var value = base.GetFieldValue(templateDataBase, element);
            if (StringComparer.OrdinalIgnoreCase.Compare(value, "true") == 0
                || "1".Equals(value))
            {
                return true;
            }
            else if (StringComparer.OrdinalIgnoreCase.Compare(value, "false") == 0
                || "0".Equals(value))
            {
                return false;
            }

            return value;
        }

        public override void SetFieldValue(System.Xml.Linq.XElement field, object value)
        {
            if(true.Equals( value ))
                base.SetFieldValue(field, "TRUE");
            else if(false.Equals(value))
                base.SetFieldValue(field, "FALSE");
            else
                base.SetFieldValue(field, value);
        }

        #endregion Methods
    }
}