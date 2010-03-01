namespace SLExtensions.Sharepoint
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    public static class SharepointEvents
    {
        #region Events

        public static event EventHandler<CancelSharepointItemEventArgs> IsValid;

        public static event EventHandler<SharepointItemValueEventArgs> ItemValueUpdated;

        public static event EventHandler<SharepointItemEventArgs> PrepareForSave;

        public static event EventHandler<SharepointItemEventArgs> Saved;

        #endregion Events

        #region Methods

        public static void RaiseItemValueUpdated(TemplateDataBase item, params string[] keys)
        {
            if (ItemValueUpdated != null)
            {
                ItemValueUpdated(null, new SharepointItemValueEventArgs(item, keys));
            }
        }

        public static void RaisePrepareForSaveAsync(SharepointList list, TemplateDataBase item, Action callback)
        {
            if (PrepareForSave != null)
            {
                var asyncManager = new asyncManager(PrepareForSave.GetInvocationList().OfType<EventHandler<SharepointItemEventArgs>>(),
                    callback, list, item);
                asyncManager.Start();
            }
            else
            {
                if (callback != null)
                {
                    callback();
                }
            }
        }

        public static void RaiseSaved(SharepointList list, TemplateDataBase item)
        {
            if (Saved != null)
            {
                Saved(null, new SharepointItemEventArgs(item, list, null));
            }
        }

        public static bool Validate(SharepointList list, TemplateDataBase item)
        {
            if (IsValid != null)
            {
                var cancelArgs = new CancelSharepointItemEventArgs(item, list) { Cancel = false };
                IsValid(null, cancelArgs);
                return !cancelArgs.Cancel;
            }
            return true;
        }

        #endregion Methods

        #region Nested Types

        private class asyncManager
        {
            #region Fields

            private Action endCallback;
            TemplateDataBase item;
            private SharepointItemEventArgs lastArg;
            SharepointList list;
            Queue<EventHandler<SharepointItemEventArgs>> queue;

            #endregion Fields

            #region Constructors

            public asyncManager(IEnumerable<EventHandler<SharepointItemEventArgs>> delegates,
                Action endCallback,
                SharepointList list,
                TemplateDataBase item)
            {
                queue = new Queue<EventHandler<SharepointItemEventArgs>>(delegates);
                this.endCallback = endCallback;
                this.list = list;
                this.item = item;
            }

            #endregion Constructors

            #region Methods

            public void Start()
            {
                if (queue.Count > 0)
                {
                    var dlg = queue.Dequeue();
                    SharepointItemEventArgs arg = lastArg = new SharepointItemEventArgs(item, list, () =>
                    {
                        Start();
                    });
                    dlg(null, arg);
                    if (!arg.UseReadyCallback)
                        Start();
                }
                else
                {
                    if (endCallback != null)
                    {
                        endCallback();
                    }
                }
            }

            #endregion Methods
        }

        #endregion Nested Types
    }
}