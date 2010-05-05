using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SLExtensions
{
    public class Locator
    {
        public Locator()
        {
            Items = new NotifyingDictionary<object>();
        }

        public NotifyingDictionary<object> Items { get; private set; }



        public static LocatorKey GetLocatorKey(DependencyObject obj)
        {
            return (LocatorKey)obj.GetValue(LocatorKeyProperty);
        }

        public static void SetLocatorKey(DependencyObject obj, LocatorKey value)
        {
            obj.SetValue(LocatorKeyProperty, value);
        }

        // Using a DependencyProperty as the backing store for LocatorKey.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LocatorKeyProperty =
            DependencyProperty.RegisterAttached("LocatorKey", typeof(LocatorKey), typeof(Locator), new PropertyMetadata(LocatorKeyChangedCallback));

        private static void LocatorKeyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                var locatorKey = e.OldValue as LocatorKey;
                if (locatorKey != null && locatorKey.Locator != null)
                    locatorKey.Locator.Items.Remove(locatorKey.Key);
            }

            if (e.NewValue != null)
            {
                var locatorKey = e.NewValue as LocatorKey;
                if (locatorKey != null && locatorKey.Locator != null)
                    locatorKey.Locator.Items.Add(locatorKey.Key, d);
            }
        }

    }
}
