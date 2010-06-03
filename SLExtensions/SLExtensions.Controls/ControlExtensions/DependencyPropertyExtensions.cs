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
using System.Windows.Data;
using System.Collections.Generic;

namespace SLExtensions.Controls
{
    public static class DependencyObjectExtensions
    {
        /// <summary>
        /// Inner class bound to the Property we want to monitor
        /// </summary>
        private class BindingSubscription : DependencyObject, IDisposable
        {
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="source">The DependencyObject instance on which we want to monitor the property</param>
            /// <param name="name">The PropertyPath to the property we want to monitor</param>
            /// <param name="handler">The handler to invoke when the property changed</param>
            public BindingSubscription(DependencyObject source, string name, DependencyPropertyChangedEventHandler handler)
            {
                this.Handler = handler;
                this.source = source;
                binding = new Binding(name) { Source = source };
                this.PropertyPath = name;
                BindingOperations.SetBinding(this, ValueProperty, binding);
            }

            private Binding binding;
            private DependencyObject source;
            /// <summary>
            /// The PropertyPath to the property we want to monitor
            /// </summary>
            public string PropertyPath { get; set; }

            /// <summary>
            /// The handler to invoke when the property changed
            /// </summary>
            public DependencyPropertyChangedEventHandler Handler { get; set; }

            /// <summary>
            /// Value depedency property.
            /// </summary>
            public static readonly DependencyProperty ValueProperty =
                DependencyProperty.Register(
                    "Value",
                    typeof(object),
                    typeof(BindingSubscription),
                    new PropertyMetadata((d, e) =>
                    {
                        var subscription = ((BindingSubscription)d);
                        var handler = subscription.Handler;
                        if (handler != null)
                            handler(subscription.source, e);
                    }));

            /// <summary>
            /// Release the binding from the source
            /// </summary>
            public void Dispose()
            {
                this.ClearValue(ValueProperty);
            }
        }

        /// <summary>
        /// Get the list of BindingSubscription stored on one object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static List<BindingSubscription> GetListeners(DependencyObject obj)
        {
            var list = (List<BindingSubscription>)obj.GetValue(ListenersProperty);
            if (list == null)
            {
                list = new List<BindingSubscription>();
                SetListeners(obj, list);
            }
            return list;
        }

        /// <summary>
        /// Set the list of BindingSubscription on an object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        private static void SetListeners(DependencyObject obj, List<BindingSubscription> value)
        {
            obj.SetValue(ListenersProperty, value);
        }


        /// <summary>
        /// Listeners dependency property
        /// </summary>
        private static readonly DependencyProperty ListenersProperty =
            DependencyProperty.RegisterAttached("Listeners", typeof(List<BindingSubscription>), typeof(DependencyObjectExtensions), null);


        /// <summary>
        /// Add a handler to a DependencyProperty for change notification
        /// </summary>
        /// <param name="source">The DependencyObject instance on which we want to monitor the property</param>
        /// <param name="name">The PropertyPath to the property we want to monitor</param>
        /// <param name="handler">The handler to invoke when the property changed</param>
        public static void AddChangedHandler(this DependencyObject source, string name, DependencyPropertyChangedEventHandler handler)
        {
            // Create the binding
            BindingSubscription subscriber = new BindingSubscription(source, name, handler);
            // Store the binding into de DependencyObject to prevent garbage collection of the subscriber
            GetListeners(source).Add(subscriber);
        }

        /// <summary>
        /// Remove a handler from a DependencyProperty
        /// </summary>
        /// <param name="source">The DependencyObject instance on which we want to monitor the property</param>
        /// <param name="name">The PropertyPath to the property we want to monitor</param>
        /// <param name="handler">The handler to invoke when the property changed</param>
        public static void RemoveChangedHandler(this DependencyObject source, string name, DependencyPropertyChangedEventHandler handler)
        {
            var subscribers = GetListeners(source);
            var subscriber = subscribers.FirstOrDefault(s => s.Handler == handler && s.PropertyPath == name);
            if (subscriber != null)
            {
                subscriber.Dispose();
                subscribers.Remove(subscriber);
            }

            if (subscribers.Count == 0)
                source.ClearValue(ListenersProperty);
        }
    }
}
