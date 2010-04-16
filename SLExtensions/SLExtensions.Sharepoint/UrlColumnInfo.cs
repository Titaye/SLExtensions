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
    public class UrlColumnInfo : TextColumnInfo
    {
        #region Methods

        public static string ParserUrl(string url)
        {
            string title;
            return ParserUrl(url, out title);
        }

        public static string ParserUrl(string url, out string title)
        {
            title = null;
            if (url == null)
                return null;

            StringBuilder sb = new StringBuilder(url);
            var idx = sb.IndexOf(", ");
            while (idx > 0 && sb[idx - 1] == ',')
                idx = sb.IndexOf(", ", idx + 1);
            if (idx == 1)
            {
                title = null;
                return url;
            }
            else
            {
                title = sb.ToString(idx + 2, sb.Length - (idx + 2));
                return sb.ToString(0, idx).Replace(",,", ",");
            }
        }

        public override object GetFieldValue(TemplateDataBase templateDataBase, System.Xml.Linq.XElement element)
        {
            var content = element.GetAttribute(XmlColumnName);
            var link = UrlColumnInfo.ParserUrl(content);
            templateDataBase.AddLinkToDownload(link);
            return content;
        }

        #endregion Methods
    }
}