namespace SLExtensions.Sharepoint
{
    using System;
    using System.Net;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Browser;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.Xml.Linq;

    [DataContract]
    public class TextColumnInfo : IColumnInfo
    {
        #region Fields

        private const string RichTextModeFullHtml = "FullHtml";
        private const string attributeRichTextMode = "RichTextMode";

        #endregion Fields

        #region Properties

        [DataMember]
        public object DefaultValue { get; set; }

        [DataMember]
        public string DisplayName
        {
            get; set;
        }

        [DataMember]
        public bool IsFullTextHtml
        {
            get; set;
        }

        [DataMember]
        public bool IsHidden
        {
            get; set;
        }

        [DataMember]
        public bool IsReadOnly
        {
            get; set;
        }

        [DataMember]
        public bool IsRequired
        {
            get; set;
        }

        public string Name
        {
            get
            {
                return StaticName.Replace("_x0020_", " ");
            }
        }

        [DataMember]
        public string StaticName
        {
            get; set;
        }

        public string XmlColumnName
        {
            get { return "ows_" + StaticName; }
        }

        #endregion Properties

        #region Methods

        public virtual object GetFieldValue(TemplateDataBase templateDataBase, System.Xml.Linq.XElement element)
        {
            var content = element.GetAttribute(XmlColumnName);

            if (IsFullTextHtml)
            {
                templateDataBase.AddContentToDownload(content);
            }
            return content;
        }

        public virtual void ParseColumnDefinition(System.Xml.Linq.XElement field)
        {
            StaticName = field.GetAttribute("StaticName");
            DisplayName = field.GetAttribute("DisplayName");
            IsReadOnly = field.GetAttribute("ReadOnly") == "TRUE";
            IsHidden = field.GetAttribute("Hidden") == "TRUE";
            IsRequired = field.GetAttribute("Required") == "TRUE";
            var defaultValue = field.GetElementValue(XName.Get("Default", SharepointSite.Xmlns));
            if (!string.IsNullOrEmpty(defaultValue))
                DefaultValue = GetFieldValue(new TemplateDataBase(), new XElement("temp", new XAttribute(XmlColumnName, defaultValue)));

            if (StringComparer.OrdinalIgnoreCase.Compare(field.GetAttribute(attributeRichTextMode), RichTextModeFullHtml) == 0)
                IsFullTextHtml = true;
        }

        public virtual void SetFieldValue(System.Xml.Linq.XElement field, object value)
        {
            if (!IsFullTextHtml)
                field.Add(HttpUtility.HtmlEncode(Convert.ToString(value)));
            else
                field.Add(value);
        }

        #endregion Methods
    }
}