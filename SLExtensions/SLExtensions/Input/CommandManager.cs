namespace SLExtensions.Input
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

    public class CommandManager
    {
        #region Fields

        // Using a DependencyProperty as the backing store for Client.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty ClientProperty = 
            DependencyProperty.RegisterAttached("Client", typeof(CommandManagerClient), typeof(CommandManager), null);

        private static List<WeakReference> clientObjects = new List<WeakReference>();

        #endregion Fields

        #region Events

        public static event EventHandler RequerySuggested
        {
            add
            {
                lock (clientObjects)
                {
                    var wr = clientObjects.FirstOrDefault(_ => value.Equals(_.Target));

                    var obj = value.Target as DependencyObject;
                    if (obj != null)
                    {
                        CommandManagerClient client = GetClient(obj);
                        client.RequerySuggested += value;
                        if (wr == null)
                            clientObjects.Add(new WeakReference(obj));
                    }
                    else
                    {
                        requerySuggested += value;
                    }
                }
            }
            remove
            {
                lock (clientObjects)
                {
                    cleanWeakReferences();

                    var toBeRemoved = new List<WeakReference>();
                    foreach (var wr in clientObjects.Where(_ => value.Equals(_.Target)))
                    {
                        var obj = wr.Target as DependencyObject;
                        CommandManagerClient client = GetClient(obj);
                        client.RequerySuggested -= value;
                        if (!client.HasEvents)
                        {
                            obj.ClearValue(ClientProperty);
                            toBeRemoved.Add(wr);
                        }
                    }

                    foreach (var wr in toBeRemoved)
                    {
                        clientObjects.Remove(wr);
                    }

                    requerySuggested -= value;
                }
            }
        }

        private static event EventHandler requerySuggested;

        #endregion Events

        #region Methods

        public static void InvalidateRequerySuggested()
        {
            cleanWeakReferences();

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                foreach (var wr in clientObjects)
                {
                    var obj = wr.Target as DependencyObject;
                    if (obj != null)
                        GetClient(obj).RaiseRequerySuggested();
                }

                if (requerySuggested != null)
                    requerySuggested(null, EventArgs.Empty);
            });
        }

        private static void cleanWeakReferences()
        {
            lock (clientObjects)
            {
                clientObjects.RemoveAll(_ =>
                {
                    var evtHandler = _.Target as DependencyObject;
                    return evtHandler == null;
                });
            }
        }

        private static CommandManagerClient GetClient(DependencyObject obj)
        {
            var client = (CommandManagerClient)obj.GetValue(ClientProperty);
            if (client == null)
            {
                client = new CommandManagerClient();
                SetClient(obj, client);
            }
            return client;
        }

        private static void SetClient(DependencyObject obj, CommandManagerClient value)
        {
            obj.SetValue(ClientProperty, value);
        }

        #endregion Methods

        #region Nested Types

        private class CommandManagerClient
        {
            #region Events

            public event EventHandler RequerySuggested;

            #endregion Events

            #region Properties

            public bool HasEvents
            {
                get { return this.RequerySuggested != null && this.RequerySuggested.GetInvocationList().Length > 0; }
            }

            #endregion Properties

            #region Methods

            public void RaiseRequerySuggested()
            {
                if (RequerySuggested != null)
                {
                    RequerySuggested(null, EventArgs.Empty);
                }
            }

            #endregion Methods
        }

        #endregion Nested Types
    }
}