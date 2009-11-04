namespace SLExtensions.Sharepoint.Sync
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO.IsolatedStorage;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.Windows.Threading;

    using SLExtensions;
    using SLExtensions.Sharepoint;

    public class Synchronizer : NotifyingObject
    {
        #region Fields

        private static readonly TimeSpan SyncRefresh = TimeSpan.FromDays(1);

        private bool inListSynchroAsync;
        private bool isSynchronizing;
        private Queue<SharepointList> pendingSynchro;
        private string synchronizingListName;

        #endregion Fields

        #region Constructors

        public Synchronizer()
        {
            pendingSynchro = new Queue<SharepointList>();
        }

        #endregion Constructors

        #region Properties

        public bool IsSynchronizing
        {
            get { return this.isSynchronizing; }
            set
            {
                if (this.isSynchronizing != value)
                {
                    this.isSynchronizing = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.IsSynchronizing));
                }
            }
        }

        public string SynchronizingListName
        {
            get { return this.synchronizingListName; }
            set
            {
                if (this.synchronizingListName != value)
                {
                    this.synchronizingListName = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.SynchronizingListName));
                }
            }
        }

        #endregion Properties

        #region Methods

        public void Synchronize(params SharepointList[] lists)
        {
            Synchronize((IEnumerable<SharepointList>)lists);
        }

        public void Synchronize(IEnumerable<SharepointList> lists)
        {
            pendingSynchro.Clear();

            foreach (var item in lists)
            {
                appendForSynchro(item);
            }

            Synchronize();
        }

        private void EndSynchronize()
        {
            SynchronizingListName = null;
            IsSynchronizing = false;
        }

        //public void SyncData(SharepointList sharepointList,
        //    Action<IEnumerable<TemplateDataBase>> callback)
        //{
        //    if (pendingSynchro.Contains(sharepointList))
        //    {
        //        SharepointSite.DownloadListChanges(sharepointList, (success, list, data) =>
        //        {
        //            HandleSyncResult(callback, success, list, data);
        //        });
        //    }
        //    else
        //    {
        //        if (callback != null)
        //        {
        //            callback(Store.ReadListData(sharepointList));
        //        }
        //    }
        //}
        private void HandleSyncResult(Action<IEnumerable<TemplateDataBase>> callback, bool success, 
            SharepointList list, ICollection<TemplateDataBase> data)
        {
            if (!success)
            {
                if (callback != null)
                    callback(Store.ReadListData(list));
            }
            else
            {
                list.SaveListData(data);
                if (callback != null)
                {
                    callback(data);
                }
            }
        }

        private void Synchronize()
        {
            if (pendingSynchro.Count > 0)
            {
                IsSynchronizing = true;
            }

            if (pendingSynchro.Count == 0)
            {
                EndSynchronize();
                //LogController.Log(LogController.Debug, "No sharepoint list to synchronize ", null);
                return;
            }
            //else if(!IsEnoughFreeSpace)
            //{
            //    EndSynchronize();
            //    LogController.Log(LogController.Debug, "Not enough space to synchronize ", null);
            //    return;
            //}
            else if(inListSynchroAsync)
                return;

            SharepointList spl = pendingSynchro.Dequeue();
            if (spl == null)
            {
                Synchronize();
            }
            else
            {
                SynchronizingListName = spl.Name;
                inListSynchroAsync = true;
                //LogController.Log(LogController.Debug, string.Format("Synchronizing {0}", spl.Name), null);

                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += delegate
                {                    
                    SharepointSite.DownloadListChanges(spl, (success, list, data) =>
                    {
                        inListSynchroAsync = false;
                        //if (success)
                        //    LogController.Log(LogController.Debug, string.Format("End sync {0}", spl.Name), null);
                        //else
                        //    LogController.Log(LogController.Warn, string.Format("End sync {0} failed", spl.Name), null);

                        if (!success)
                        {
                            EndSynchronize();
                        }
                        else
                        {
                            list.SaveListData(data);
                            Synchronize();
                        }
                    });
                };
                worker.RunWorkerAsync();
            }
        }

        private void appendForSynchro(SharepointList spl)
        {
            if (spl != null
                //&& (spl.LastSyncTime == null || (DateTime.Now - spl.LastSyncTime) > SyncRefresh)
                && spl.Name != SynchronizingListName)
            {
                pendingSynchro.Enqueue(spl);
            }
        }

        #endregion Methods
    }
}