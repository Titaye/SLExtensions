namespace SLExtensions.Sharepoint
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
    using System.Xml.Linq;
    using System.Windows.Browser;

    [DataContract]
    public class ChoiceColumnInfo : TextColumnInfo
    {
        #region Fields

        public const string ChoiceSeparator = ";#";

        #endregion Fields

        #region Properties

        [DataMember]
        public string[] Choices
        {
            get; set;
        }

        [DataMember]
        public bool MultiChoice
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public static string Escape(string data)
        {
            return null;
        }

        public static string[] Parse(XElement field)
        {
            return (from choices in field.Elements(XName.Get("CHOICES", SharepointSite.Xmlns))
                    from choice in choices.Elements(XName.Get("CHOICE", SharepointSite.Xmlns))
                    select choice.Value).ToArray();
        }

        public static string[] ParseMulti(string data)
        {
            if (string.IsNullOrEmpty(data))
                return null;

            return data.Split(new string[]{ ChoiceSeparator}, StringSplitOptions.RemoveEmptyEntries);
        }

        public override void ParseColumnDefinition(XElement field)
        {
            base.ParseColumnDefinition(field);
            MultiChoice = field.GetAttribute("Type") == "MultiChoice";
            Choices = ChoiceColumnInfo.Parse(field);
        }

        #endregion Methods

        public override object GetFieldValue(TemplateDataBase templateDataBase, XElement element)
        {
            if (!MultiChoice)
            {
                string val = (string)base.GetFieldValue(templateDataBase, element);
                if(val != null)
                    return HttpUtility.HtmlDecode(val);
            }
            return base.GetFieldValue(templateDataBase, element);
        }
    }
}