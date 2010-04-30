namespace SLExtensions.Sharepoint
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
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

    using SLExtensions;

    [DataContract]
    public class SharepointList : INotifyPropertyChanged
    {
        #region Fields

        private object saveListSync;

        #endregion Fields

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Properties

        [DataMember]
        public List<IColumnInfo> Columns
        {
            get; set;
        }

        [DataMember]
        public DateTime? LastSyncTime
        {
            get; set;
        }

        [DataMember]
        public string Name
        {
            get; set;
        }

        public Uri ServerUri
        {
            get; set;
        }

        [DataMember]
        public string SyncToken
        {
            get; set;
        }

        internal object SaveListSync
        {
            get
            {
                if (saveListSync == null)
                {
                    lock (this)
                    {
                        if (saveListSync == null)
                            saveListSync = new object();
                    }
                }
                return saveListSync;
            }
        }

        #endregion Properties

        #region Methods

        public virtual IColumnInfo ParseColumn(XElement field)
        {
            var type = field.GetAttribute("Type");
            Type columnType = ColumnTypes.RegisteredColumnTypes.TryGetValue(type) ?? ColumnTypes.FallbackColumnType;
            IColumnInfo colInfo = (IColumnInfo) Activator.CreateInstance(columnType);
            colInfo.ParseColumnDefinition(field);
            return colInfo;
        }

        public virtual void ParseColumns(XElement listElement)
        {
            var columns = new List<IColumnInfo>();
            var r1 = from fields in listElement.Elements(XName.Get("Fields", SharepointSite.Xmlns))
                     from field in fields.Elements(XName.Get("Field", SharepointSite.Xmlns))
                     select field;

            foreach (var item in r1)
            {
                var col = ParseColumn(item);
                columns.Add(col);
            }
            Columns = columns;
        }

        public virtual TemplateDataBase ParseData(XElement element)
        {
            var data = CreateTemplateData();
            data.ListName = Name;
            foreach (var item in Columns)
            {
                data.ParseColumn(item, element);
            }
            return data;
        }

        public void RaisePropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }

        public virtual void SaveListData(ICollection<TemplateDataBase> data)
        {
            Store.SaveListData(this, data);
        }

        //[DataMember]
        //public ContentType[] ContentTypes { get; set; }
        //public void LoadData()
        //{
        //    if (data == null)
        //    {
        //        SyncService.Instance.SyncData(this, SetData);
        //    }
        //}
        //public void LoadData(Action<IEnumerable<TemplateDataBase>> callback)
        //{
        //    if (data == null)
        //    {
        //        SyncService.Instance.SyncData(this, r =>
        //            {
        //                SetData(r);
        //                if (callback != null)
        //                    callback(r);
        //            });
        //    }
        //    else if (callback != null)
        //    {
        //        callback(data);
        //    }
        //}
        //private IEnumerable<TemplateDataBase> data;
        //public IEnumerable<TemplateDataBase> Data
        //{
        //    get
        //    {
        //        LoadData();
        //        return this.data;
        //    }
        //    set
        //    {
        //        if (this.data != value)
        //        {
        //            this.data = value;
        //            this.OnPropertyChanged(this.GetPropertyName(n => n.Data));
        //        }
        //    }
        //}
        //private void SetData(IEnumerable<TemplateDataBase> data)
        //{
        //    this.Data = data;
        //}
        protected virtual TemplateDataBase CreateTemplateData()
        {
            return new TemplateDataBase();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Methods
    }
}