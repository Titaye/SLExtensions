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

    public static class IsBusyExtensions
    {
        #region Fields

        // Using a DependencyProperty as the backing store for BusyCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BusyCountProperty = 
            DependencyProperty.RegisterAttached("BusyCount", typeof(int), typeof(IsBusyExtensions), new PropertyMetadata(0, BusyCountChangedCallback));

        // Using a DependencyProperty as the backing store for IsBusy.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsBusyProperty = 
            DependencyProperty.RegisterAttached("IsBusy", typeof(bool), typeof(IsBusyExtensions), new PropertyMetadata(IsBusyChangedCallback));

        #endregion Fields

        #region Methods

        public static int GetBusyCount(DependencyObject obj)
        {
            return (int)obj.GetValue(BusyCountProperty);
        }

        public static bool GetIsBusy(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsBusyProperty);
        }

        public static void SetBusy(this DependencyObject source, bool busy)
        {
            int busyCount = GetBusyCount(source);
            if (busy)
                busyCount++;
            else
                busyCount--;

            SetBusyCount(source, busyCount);
        }

        public static void SetBusyCount(DependencyObject obj, int value)
        {
            obj.SetValue(BusyCountProperty, value);
        }

        private static void BusyCountChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (0.Equals(e.NewValue))
                SetIsBusy(d, false);
            else
                SetIsBusy(d, true);
        }

        private static void IsBusyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        private static void SetIsBusy(DependencyObject obj, bool value)
        {
            obj.SetValue(IsBusyProperty, value);
        }

        #endregion Methods
    }
}