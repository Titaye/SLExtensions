namespace SLExtensions
{
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

    public class Locator
    {
        #region Fields

        // Using a DependencyProperty as the backing store for LocatorKey.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LocatorKeyProperty = 
            DependencyProperty.RegisterAttached("LocatorKey", typeof(LocatorKey), typeof(Locator), new PropertyMetadata(LocatorKeyChangedCallback));

        #endregion Fields

        #region Constructors

        public Locator()
        {
            Items = new NotifyingDictionary<object>();
        }

        #endregion Constructors

        #region Properties

        public NotifyingDictionary<object> Items
        {
            get; private set;
        }

        #endregion Properties

        #region Methods

        public static LocatorKey GetLocatorKey(DependencyObject obj)
        {
            return (LocatorKey)obj.GetValue(LocatorKeyProperty);
        }

        public static void SetLocatorKey(DependencyObject obj, LocatorKey value)
        {
            obj.SetValue(LocatorKeyProperty, value);
        }

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

        #endregion Methods
    }
}