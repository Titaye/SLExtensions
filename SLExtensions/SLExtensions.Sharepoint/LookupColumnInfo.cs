namespace SLExtensions.Sharepoint
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
    using System.Xml.Linq;

    [DataContract]
    public class LookupColumnInfo : TextColumnInfo
    {
        #region Fields

        public const string LookupSeparator = ";#";

        #endregion Fields

        #region Properties

        [DataMember]
        public bool IsMulti
        {
            get; set;
        }

        [DataMember]
        public string List
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public static string GetLookupString(string key, string data)
        {
            return string.Concat(key, LookupSeparator, data);
        }

        public static string GetLookupString(params KeyValuePair<string, string>[] data)
        {
            return GetLookupString((IEnumerable<KeyValuePair<string, string>>)data);
        }

        public static string GetLookupString(IEnumerable<KeyValuePair<string, string>> data)
        {
            if (data == null)
                return null;
            StringBuilder sb = new StringBuilder();
            var array = data.ToArray();
            for (int i = 0; i < array.Length; i++)
            {
                var item = array[i];
                sb.Append(item.Key);
                sb.Append(LookupSeparator);
                sb.Append(item.Value);
                if (i != array.Length - 1)
                    sb.Append(LookupSeparator);
            }
            return sb.ToString();
        }

        public static KeyValuePair<string, string> ParseLookup(string data)
        {
            string txt;
            string key = ParseLookup(data, out txt);
            return new KeyValuePair<string, string>(key, txt);
        }

        public static string ParseLookup(string data, out string display)
        {
            display = null;
            if (string.IsNullOrEmpty(data))
                return null;

            var str = data.Split(new string[] { LookupSeparator }, StringSplitOptions.None);
            display = str[1];
            return str[0];
        }

        public static KeyValuePair<string, string>[] ParseMultiLookup(string data)
        {
            if (string.IsNullOrEmpty(data))
                return null;

            var str = data.Split(new string[] { LookupSeparator }, StringSplitOptions.None);
            KeyValuePair<string, string>[] values = new KeyValuePair<string, string>[str.Length / 2];
            for (int i = 0; i < values.Length; i++)
            {
                var idx = i * 2;
                values[i] = new KeyValuePair<string, string>(str[idx], str[idx + 1]);
            }
            return values;
        }

        public override object GetFieldValue(TemplateDataBase templateDataBase, System.Xml.Linq.XElement element)
        {
            var content = element.GetAttribute(XmlColumnName);
            return content;
        }

        public override void ParseColumnDefinition(System.Xml.Linq.XElement field)
        {
            base.ParseColumnDefinition(field);
            List = field.GetAttribute("List");
            IsMulti = field.GetAttribute("Type") == "LookupMulti";
        }

        public override void SetFieldValue(System.Xml.Linq.XElement field, object value)
        {
            field.Add(new XAttribute("Type", "Lookup"));

            var keys = ParseMultiLookup(value as string);

            if (keys == null)
                base.SetFieldValue(field, null);
            else
                base.SetFieldValue(field, string.Join(LookupSeparator, keys.Select(k => string.Concat(k.Key, LookupSeparator)).ToArray()));
        }

        #endregion Methods
    }
}