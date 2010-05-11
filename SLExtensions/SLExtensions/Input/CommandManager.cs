using System;
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
using System.Collections.Generic;

namespace SLExtensions.Input
{
    public class CommandManager
    {
        private static List<WeakReference> eventHandlers = new List<WeakReference>();

        public static event EventHandler RequerySuggested
        {
            add
            {
                eventHandlers.Add(new WeakReference(value));
            }
            remove
            {
                eventHandlers.RemoveAll(_ =>
                {
                    var evtHandler = _.Target as EventHandler;
                    return evtHandler == null
                        || evtHandler == value;
                });
                eventHandlers.Remove(new WeakReference(value));
            }
        }


        public static void InvalidateRequerySuggested()
        {
            eventHandlers.RemoveAll(_ =>
            {
                var evtHandler = _.Target as EventHandler;
                return evtHandler == null;
            });

            EventHandler[] handlers = eventHandlers
                .Select(_ => _.Target as EventHandler)
                .ToArray();

            Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    foreach (var item in handlers)
                    {
                        if (item != null)
                            item(null, EventArgs.Empty);
                    }
                });
        }





    }
}
