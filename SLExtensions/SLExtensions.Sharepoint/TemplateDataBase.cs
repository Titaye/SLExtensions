namespace SLExtensions.Sharepoint
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Runtime.Serialization;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.Xml.Linq;

    using SLExtensions;

    [DataContract]
    public class TemplateDataBase
    {
        #region Fields

        public const string ColumnAuthor = "Author";
        public const string ColumnContentType = "ContentType";
        public const string ColumnCreated = "Created";
        public const string ColumnId = "ID";
        public const string ColumnTitle = "Title";

        protected const string extensions = @"(\.png|\.jpg|\.zip|\.pdf|\.doc(x)?|\.xls(x)?|\.ppt(x)?|\.7z|\.rar)";

        protected static Regex ExternalContentRegex = new Regex(@"(?<key>(href|src))=('(?<value>[^']*" + extensions
            + @")'|""(?<value>[^""]*" + extensions
            + @")""|(?<value>[\w_$]+" + extensions + "))", RegexOptions.IgnoreCase);
        protected static Regex ExternalLinkRegex = new Regex(@"\w+" + extensions, RegexOptions.IgnoreCase);

        #endregion Fields

        #region Constructors

        public TemplateDataBase()
        {
            ToBeDownloaded = new List<string>();
            Data = new Dictionary<string, object>();
        }

        #endregion Constructors

        #region Properties

        [DataMember]
        public string ContentType
        {
            get; set;
        }

        [DataMember]
        public Dictionary<string, object> Data
        {
            get; set;
        }

        public Dictionary<string, byte[]> DownloadedData
        {
            get; set;
        }

        [DataMember]
        public string Id
        {
            get; set;
        }

        [DataMember]
        public string ListName
        {
            get; set;
        }

        [DataMember]
        public string Title
        {
            get; set;
        }

        public List<string> ToBeDownloaded
        {
            get; private set;
        }

        #endregion Properties

        #region Methods

        public static T Clone<T>(T source)
            where T : TemplateDataBase, new()
        {
            var clonee = new T();
            clonee.CloneFromSource(source);
            return clonee;
        }

        public void AddContentToDownload(string content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                var r = from m in ExternalContentRegex.Matches(content).OfType<Match>()
                        where m.Success
                        select m.Groups["value"].Value;
                ToBeDownloaded.AddRange(r);
            }
        }

        public void AddLinkToDownload(string content)
        {
            if (!string.IsNullOrEmpty(content) && ExternalLinkRegex.IsMatch(content))
            {
                ToBeDownloaded.Add(content);
            }
        }

        public virtual object ParseColumn(IColumnInfo item, XElement element)
        {
            object content = null;
            switch (item.StaticName)
            {
                case ColumnId:
                    Id = element.GetAttribute(item.XmlColumnName);
                    content = Id;
                    break;
                case ColumnTitle:
                    Title = element.GetAttribute(item.XmlColumnName);
                    content = Title;
                    break;
                case ColumnContentType:
                    ContentType = element.GetAttribute(item.XmlColumnName);
                    content = ContentType;
                    break;
            }

            content = item.GetFieldValue(this, element);
            Data[item.StaticName] = content;
            return content;
        }

        public void ReadDownloadedDataFromStore()
        {
            if (DownloadedData == null)
            {
                DownloadedData = Store.ReadDownloadedData(ListName, Id) ?? new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);
            }
        }

        protected virtual void CloneFromSource(TemplateDataBase source)
        {
            Id = source.Id;
            Title = source.Title;
            ListName = source.ListName;
            ContentType = source.ContentType;
            if (source.Data != null)
                Data = new Dictionary<string, object>(source.Data);
            if (source.DownloadedData != null)
                DownloadedData = new Dictionary<string, byte[]>(source.DownloadedData);
        }

        #endregion Methods
    }
}