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
    using System.Xml.Linq;

    public interface IColumnInfo
    {
        #region Properties

        object DefaultValue
        {
            get;
        }

        string DisplayName
        {
            get;
        }

        bool IsHidden
        {
            get;
        }

        bool IsReadOnly
        {
            get;
        }

        bool IsRequired
        {
            get;
        }

        string Name
        {
            get;
        }

        string StaticName
        {
            get;
        }

        string XmlColumnName
        {
            get;
        }

        #endregion Properties

        #region Methods

        object GetFieldValue(TemplateDataBase templateDataBase, XElement element);

        void ParseColumnDefinition(XElement field);

        void SetFieldValue(XElement field, object value);

        #endregion Methods
    }
}