namespace SLExtensions.Sharepoint
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.ServiceModel;
    using System.Text;
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

    using SLExtensions;
    using SLExtensions.Sharepoint;
    using SLExtensions.Sharepoint.Services.Lists;

    public static class SharepointSite
    {
        #region Fields

        public static string Xmlns = "http://schemas.microsoft.com/sharepoint/soap/";
        public static string Xmlnsrs = "urn:schemas-microsoft-com:rowset";
        public static string Xmlnsz = "#RowsetSchema";

        private static BasicHttpBinding basicbinding = new BasicHttpBinding() { MaxReceivedMessageSize = int.MaxValue, MaxBufferSize = int.MaxValue };

        #endregion Fields

        #region Methods

        public static XElement AddBatchMethod(XElement batch, string cmd, params object[] batchContent)
        {
            XElement method = new XElement("Method",
                            new XAttribute("ID", batch.DescendantNodes().Count() + 1),
                            new XAttribute("Cmd", cmd));
            batch.Add(method);

            if (batchContent != null)
            {
                foreach (var item in batchContent)
                {
                    method.Add(item);
                }
            }

            return method;
        }

        public static XElement CreateBatch(bool onErrorContinue)
        {
            var camlStatement = new XElement("Batch");
            if (onErrorContinue)
                camlStatement.Add(new XAttribute("OnError", "Continue"));
            return camlStatement;
        }

        public static void CreateNewItem(SharepointList list,
            Dictionary<string, object> item,
            Action<bool, ICollection<TemplateDataBase>, Exception> callback)
        {
            TemplateDataBase tditem = new TemplateDataBase { ListName = list.Name, Data = item };

            SharepointEvents.RaisePrepareForSaveAsync(list, tditem, () =>
            {
                var wslist = CreateListSoapClient(list);

                Action doCreateItem = () =>
                {
                    SharepointListWrapper wrapper = new SharepointListWrapper();
                    wrapper.List = list;
                    wrapper.Callback = (success, spList, data) =>
                        {
                            if (callback != null)
                            {
                                callback(success, data, wrapper.Exception);
                            }
                        };

                    wslist.UpdateListItemsCompleted += new EventHandler<UpdateListItemsCompletedEventArgs>(wslist_UpdateListItemsCompleted);

                    var batch = CreateBatch(false);
                    AddBatchMethod(batch, "New", GetFieldValue(list, item, false));
                    wrapper.Batch = batch;
                    wslist.UpdateListItemsAsync(list.Name, batch, wrapper);
                };

                EnsureSharepointListColumnsCreated(list, (ex) =>
                {
                    if (callback != null)
                        callback(false, null, ex);
                }, wslist, doCreateItem);
            });
        }

        public static void DownloadListChanges(SharepointList list, Action<bool, SharepointList, ICollection<TemplateDataBase>> callback)
        {
            DownloadListChanges(list, callback, true);
        }

        /// <summary>
        /// Download list changes
        /// </summary>
        /// <param name="list">The SharepointList to be downloaded</param>
        /// <param name="callback">The callback fucntion to be called. bool : true when succes, SharepointList : the list synchronized </param>
        public static void DownloadListChanges(SharepointList list, Action<bool, SharepointList, ICollection<TemplateDataBase>> callback,
            bool checkOffline)
        {
            DownloadListChanges(new SharepointListWrapper { List = list, Callback = callback, CheckOffline = checkOffline });
        }

        public static XElement[] GetFieldValue(SharepointList list, Dictionary<string, object> newValue, bool update)
        {
            List<XElement> values = new List<XElement>();
            foreach (var column in list.Columns)
            {
                object value;
                if (newValue.TryGetValue(column.StaticName, out value))
                {
                    if (column.IsReadOnly
                        && !(update && column.StaticName == TemplateDataBase.ColumnId))
                        continue;

                    var field = new XElement("Field", new XAttribute("Name", column.StaticName));
                    column.SetFieldValue(field, value);
                    values.Add(field);
                }
            }
            return values.ToArray();
        }

        public static void GetList(SharepointList list, Action<bool, Exception> callback)
        {
            var wslist = CreateListSoapClient(list);
            EnsureSharepointListColumnsCreated(list, (ex) =>
            {
                if (callback != null)
                {
                    callback(false, ex);
                }
            }, wslist, () =>
            {
                if (callback != null)
                {
                    callback(true, null);
                }
            });
        }

        public static void GetListItem(SharepointList list, string id, Action<bool, Exception, TemplateDataBase> callback)
        {
            GetListItems(list, new XElement("Query",
                        new XElement("Where",
                            new XElement("Eq",
                                new XElement("FieldRef", new XAttribute("Name", "ID")),
                                new XElement("Value", new XAttribute("Type", "Number"), id)
                                )
                        )
                    ), (success, ex, data) =>
                        {
                            if (callback != null)
                                callback(success, ex, data != null ? data.FirstOrDefault() : null);
                        });
        }

        public static void GetListItems(SharepointList list, XElement query, Action<bool, Exception, IEnumerable<TemplateDataBase>> callback)
        {
            GetListItems(list, query, callback, null, null);
        }

        public static void GetListItems(SharepointList list, XElement query, Action<bool, Exception, IEnumerable<TemplateDataBase>> callback
            , string rowLimit, XElement queryOptions)
        {
            var wslist = CreateListSoapClient(list);

            Action action = () =>
            {
                wslist.GetListItemsCompleted += (o, e) =>
                {
                    if (e.Error != null)
                    {
                        if (callback != null)
                        {
                            callback(false, e.Error, null);
                        }
                    }
                    else
                    {

                        var data = (from r in e.Result.Elements(XName.Get("data", Xmlnsrs))
                                    from d in r.Elements(XName.Get("row", Xmlnsz))
                                    let item = list.ParseData(d)
                                    where item != null
                                    select item);
                        if (callback != null)
                        {
                            callback(true, null, data);
                        }
                    }
                };
                var view = new XElement("ViewFields");
                foreach (var item in list.Columns)
                {
                    view.Add(new XElement("FieldRef", new XAttribute("Name", item.StaticName)));
                }

                wslist.GetListItemsAsync(list.Name, null,
                    query, view, rowLimit, queryOptions, null);
            };

            EnsureSharepointListColumnsCreated(list, (ex) =>
            {
                if (callback != null)
                    callback(false, ex, null);
            }, wslist, action);
        }

        public static void GetLists(Uri serverUri, Action<KeyValuePair<string, string>[]> callback)
        {
            var wsList = CreateListSoapClient(serverUri);
            wsList.GetListCollectionCompleted += (o, e) =>
            {
                if (e.Error == null)
                {
                    var lists = from l in e.Result.Elements(XName.Get("List", Xmlns))
                                select new KeyValuePair<string, string>(l.GetAttribute("ID"), l.GetAttribute("Title"));
                    if (callback != null)
                    {
                        callback(lists.ToArray());
                    }
                }
                else if (callback != null)
                {
                    callback(null);
                }
            };
            wsList.GetListCollectionAsync();
        }

        //        $l = $wslists.GetListItems("WorkflowAmbassadeursReponses", $null, $null, $null, $null, [xml](xe QueryOptions {
        //   xe ExpandUserField { "TRUE" }
        //}), $null)
        public static XElement GetQueryOptions(bool expandUserField)
        {
            return new XElement("QueryOptions",
                    new XElement("ExpandUserField", expandUserField ? "TRUE" : "FALSE"));
        }

        public static void UpdateItem(SharepointList list,
            Dictionary<string, object> item,
            Action<bool, ICollection<TemplateDataBase>, Exception> callback)
        {
            TemplateDataBase tditem = new TemplateDataBase { ListName = list.Name, Id = Convert.ToString(item.TryGetValue("ID")), Data = item };
            SharepointEvents.RaisePrepareForSaveAsync(list, tditem, () =>
            {
                var wslist = CreateListSoapClient(list);

                Action doCreateItem = () =>
                {
                    SharepointListWrapper wrapper = new SharepointListWrapper();
                    wrapper.List = list;
                    wrapper.Callback = (success, spList, data) =>
                    {
                        if (callback != null)
                        {
                            callback(success, data, wrapper.Exception);
                        }
                    };

                    wslist.UpdateListItemsCompleted += new EventHandler<UpdateListItemsCompletedEventArgs>(wslist_UpdateListItemsCompleted);

                    var batch = CreateBatch(false);
                    AddBatchMethod(batch, "Update", GetFieldValue(list, item, true));
                    wrapper.Batch = batch;
                    wslist.UpdateListItemsAsync(list.Name, batch, wrapper);
                };

                EnsureSharepointListColumnsCreated(list, (ex) =>
                {
                    if (callback != null)
                        callback(false, null, ex);
                }, wslist, doCreateItem);
            });
        }

        private static ListsSoapClient CreateListSoapClient(SharepointList list)
        {
            if (list.ServerUri == null)
                throw new NullReferenceException("ServerUri for list " + list.Name + " is null");

            return CreateListSoapClient(list.ServerUri);
        }

        private static ListsSoapClient CreateListSoapClient(Uri serverUri)
        {
            var wslist = new ListsSoapClient(basicbinding, new EndpointAddress(serverUri));

            return wslist;
        }

        private static void DownloadListChanges(SharepointListWrapper wrapper)
        {
            var wslist = CreateListSoapClient(wrapper.List);

            XElement viewFields = null;
            if (wrapper.List.Columns == null)
            {
                wslist.GetListCompleted += new EventHandler<GetListCompletedEventArgs>(wslist_GetListCompleted);
                wslist.GetListAsync(wrapper.List.Name, wrapper);
            }
            else
            {
                wslist.GetListItemChangesSinceTokenCompleted += new EventHandler<GetListItemChangesSinceTokenCompletedEventArgs>(wslist_GetListItemChangesSinceTokenCompleted);

                viewFields = new XElement("ViewFields", (from c in wrapper.List.Columns
                                                         select new XElement("FieldRef", new XAttribute("Name", c.StaticName))).ToArray());

                wrapper.LastSyncRows = null;
                wslist.GetListItemChangesSinceTokenAsync(wrapper.List.Name, null, null, viewFields, null, null, wrapper.List.SyncToken, null, wrapper);
            }
        }

        private static void EndSynchronize(SharepointListWrapper wrapper)
        {
            if (wrapper.NeedMoreSync)
            {
                DownloadListChanges(wrapper);
                return;
            }

            wrapper.List.LastSyncTime = DateTime.Now;
            wrapper.RaiseCallback(true);
        }

        private static void EnsureSharepointListColumnsCreated(SharepointList list, Action<Exception> exceptionCallback, ListsSoapClient wslist, Action doAction)
        {
            if (list.Columns == null)
            {
                wslist.GetListCompleted += (o, args) =>
                {
                    if (args.Error != null)
                    {
                        if (exceptionCallback != null)
                            exceptionCallback(args.Error);
                    }
                    else
                    {
                        list.ParseColumns(args.Result);
                        doAction();
                    }
                };
                wslist.GetListAsync(list.Name);
            }
            else
            {
                doAction();
            }
        }

        private static void ResetList(SharepointListWrapper wrapper, XElement listElement)
        {
            wrapper.Data = new Dictionary<string, TemplateDataBase>();
            if (listElement != null)
            {
                wrapper.List.ParseColumns(listElement);
            }

            wrapper.List.SyncToken = null;
            wrapper.NeedMoreSync = true;

            //skip row parsing, we are reseting all the list
            EndSynchronize(wrapper);
        }

        static void attachmentDownloader_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            SharepointListWrapper wrapper = (SharepointListWrapper)e.UserState;
            TemplateDataBase data = wrapper.LastSyncRows[0];
            if (e.Error == null)
            {
                string uri = data.ToBeDownloaded[0];
                byte[] buffer = new byte[e.Result.Length];
                e.Result.Read(buffer, 0, buffer.Length);
                data.ReadDownloadedDataFromStore();
                data.DownloadedData[uri] = buffer;
            }
            data.ToBeDownloaded.RemoveAt(0);
            downloadAttachments(wrapper);
        }

        private static void downloadAttachments(SharepointListWrapper wrapper)
        {
            while (wrapper.LastSyncRows != null
                && wrapper.LastSyncRows.Count > 0)
            {
                var item = wrapper.LastSyncRows[0];
                if (item.ToBeDownloaded == null
                    || item.ToBeDownloaded.Count == 0)
                {
                    wrapper.LastSyncRows.RemoveAt(0);
                    continue;
                }
                break;
            }

            TemplateDataBase row = null;
            if(wrapper.LastSyncRows != null)
                row = wrapper.LastSyncRows.FirstOrDefault();

            if (row == null
                || row.ToBeDownloaded == null
                || row.ToBeDownloaded.Count == 0)
            {
                EndSynchronize(wrapper);
            }
            else
            {

                Uri uri = new Uri(row.ToBeDownloaded[0], UriKind.RelativeOrAbsolute);
                WebClient attachmentDownloader = new WebClient();
                attachmentDownloader.OpenReadCompleted += new OpenReadCompletedEventHandler(attachmentDownloader_OpenReadCompleted);
                if (!uri.IsAbsoluteUri)
                    uri = new Uri(Application.Current.Host.Source, uri);
                attachmentDownloader.OpenReadAsync(uri, wrapper);
            }
        }

        static void wslist_GetListCompleted(object sender, GetListCompletedEventArgs e)
        {
            SharepointListWrapper wrapper = (SharepointListWrapper)e.UserState;
            if (e.Error != null)
            {
                wrapper.Exception = e.Error;
                wrapper.RaiseCallback(false);
            }
            else
            {
                var listElement = e.Result;
                ResetList(wrapper, listElement);
            }
        }

        static void wslist_GetListItemChangesSinceTokenCompleted(object sender, GetListItemChangesSinceTokenCompletedEventArgs e)
        {
            SharepointListWrapper wrapper = (SharepointListWrapper)e.UserState;
            wrapper.NeedMoreSync = false;
            if (e.Error != null)
            {
                wrapper.Exception = e.Error;
                wrapper.RaiseCallback(false);
            }
            else
            {
                var changesElement = e.Result.Element(XName.Get("Changes", Xmlns));
                var wasFirstSync = wrapper.List.SyncToken == null;
                wrapper.List.SyncToken = changesElement.GetAttribute("LastChangeToken");

                var listElement = changesElement.Element(XName.Get("List", Xmlns));

                if (listElement != null)
                {
                    wrapper.List.ParseColumns(listElement);
                }

                if (wrapper.Data == null)
                {
                    var storeData = Store.ReadListData(wrapper.List);
                    if (storeData == null && !wasFirstSync)
                    {
                        wrapper.List.SyncToken = null;
                        wrapper.NeedMoreSync = true;
                    }
                    else if (storeData == null && wasFirstSync)
                    {
                        wrapper.Data = new Dictionary<string, TemplateDataBase>();
                    }
                    else
                    {
                        wrapper.Data = storeData.ToDictionary(item => item.Id);
                    }
                }

                if (StringComparer.InvariantCultureIgnoreCase.Compare(changesElement.GetAttribute("MoreChanges"), "true") == 0)
                {
                    wrapper.NeedMoreSync = true;
                }
                else
                {
                    var needFullSync = (from elem in changesElement.Elements(XName.Get("Id", Xmlns))
                                        where StringComparer.InvariantCultureIgnoreCase.Compare(elem.GetAttribute("ChangeType"), "InvalidToken") == 0
                                        || StringComparer.InvariantCultureIgnoreCase.Compare(elem.GetAttribute("ChangeType"), "Restore") == 0
                                        select 1).Any();
                    if (needFullSync)
                    {
                        ResetList(wrapper, null);
                        return;
                    }
                }

                var deleteIds = (from elem in changesElement.Elements(XName.Get("Id", Xmlns))
                                 where StringComparer.InvariantCultureIgnoreCase.Compare(elem.GetAttribute("ChangeType"), "MoveAway") == 0
                                     || StringComparer.InvariantCultureIgnoreCase.Compare(elem.GetAttribute("ChangeType"), "Delete") == 0
                                 select elem.Value).ToArray();

                foreach (var id in deleteIds)
                {
                    wrapper.Data.Remove(id);
                }

                var rows = (from data in e.Result.Elements(XName.Get("data", Xmlnsrs))
                            from d in data.Elements(XName.Get("row", Xmlnsz))
                            let item = wrapper.List.ParseData(d)
                            where item != null
                            select item).ToArray();

                if (wrapper.Data != null)
                {
                    foreach (var row in rows)
                    {
                        wrapper.Data[row.Id] = row;
                    }

                    wrapper.LastSyncRows = rows.ToList();
                }

                if (rows.Length == 0 && !wrapper.NeedMoreSync)
                {
                    wrapper.List.LastSyncTime = DateTime.Now;
                    wrapper.RaiseCallback(true);
                }
                else
                {
                    downloadAttachments(wrapper);
                }

            }
        }

        static void wslist_UpdateListItemsCompleted(object sender, UpdateListItemsCompletedEventArgs e)
        {
            SharepointListWrapper wrapper = (SharepointListWrapper)e.UserState;
            if (e.Error != null)
            {
                wrapper.Exception = e.Error;
                wrapper.RaiseCallback(false);
            }
            else
            {
                var exceptions = from r in e.Result.Elements(XName.Get("Result", Xmlns))
                                 from errorcode in r.Elements(XName.Get("ErrorCode", Xmlns))
                                 where errorcode.Value != "0x00000000"
                                 select errorcode.Parent.Element(XName.Get("ErrorText", Xmlns)).Value;
                if (exceptions.Count() > 0)
                {
                    wrapper.Exception = new Exception(string.Join("\n", exceptions.ToArray()));
                    wrapper.RaiseCallback(false);
                }
                else
                {
                    var data = (from r in e.Result.Elements(XName.Get("Result", Xmlns))
                                from d in r.Elements(XName.Get("row", Xmlnsz))
                                let item = wrapper.List.ParseData(d)
                                where item != null
                                select item);
                    wrapper.Data = data.ToDictionary(i => i.Id);

                    wrapper.RaiseCallback(true);
                }
            }
        }

        #endregion Methods

        #region Nested Types

        private class SharepointListWrapper
        {
            #region Constructors

            public SharepointListWrapper()
            {
                //Data = new Dictionary<string, TemplateDataBase>();
            }

            #endregion Constructors

            #region Properties

            public XElement Batch
            {
                get;
                set;
            }

            public Action<bool, SharepointList, ICollection<TemplateDataBase>> Callback
            {
                get;
                set;
            }

            public bool CheckOffline
            {
                get;
                set;
            }

            public Dictionary<string, TemplateDataBase> Data
            {
                get;
                set;
            }

            public Exception Exception
            {
                get;
                set;
            }

            public List<TemplateDataBase> LastSyncRows
            {
                get;
                set;
            }

            public SharepointList List
            {
                get;
                set;
            }

            public bool NeedMoreSync
            {
                get;
                set;
            }

            #endregion Properties

            #region Methods

            public void RaiseCallback(bool success)
            {
                if (Callback != null)
                {
                    if (!success)
                        Callback(success, List, null);
                    else
                        Callback(success, List, Data.Values);
                }
            }

            #endregion Methods
        }

        #endregion Nested Types
    }
}