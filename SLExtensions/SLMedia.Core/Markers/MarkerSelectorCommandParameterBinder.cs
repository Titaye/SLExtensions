namespace SLMedia.Core
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

    using SLExtensions.Input;

    public class MarkerSelectorCommandParameterBinder
    {
        #region Fields

        // Using a DependencyProperty as the backing store for AllowMultipleActive.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AllowMultipleActiveProperty = 
            DependencyProperty.RegisterAttached("AllowMultipleActive", typeof(bool), typeof(MarkerSelectorCommandParameterBinder), new PropertyMetadata(AllowMultipleActiveChangedCallback));

        // Using a DependencyProperty as the backing store for Key.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty KeyProperty = 
            DependencyProperty.RegisterAttached("Key", typeof(string), typeof(MarkerSelectorCommandParameterBinder), new PropertyMetadata(KeyChangedCallback));

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty = 
            DependencyProperty.RegisterAttached("Value", typeof(object), typeof(MarkerSelectorCommandParameterBinder), new PropertyMetadata(ValueChangedCallback));

        #endregion Fields

        #region Methods

        public static bool GetAllowMultipleActive(DependencyObject obj)
        {
            return (bool)obj.GetValue(AllowMultipleActiveProperty);
        }

        public static string GetKey(DependencyObject obj)
        {
            return (string)obj.GetValue(KeyProperty);
        }

        public static object GetValue(DependencyObject obj)
        {
            return (object)obj.GetValue(ValueProperty);
        }

        public static void SetAllowMultipleActive(DependencyObject obj, bool value)
        {
            obj.SetValue(AllowMultipleActiveProperty, value);
        }

        public static void SetKey(DependencyObject obj, string value)
        {
            obj.SetValue(KeyProperty, value);
        }

        public static void SetValue(DependencyObject obj, object value)
        {
            obj.SetValue(ValueProperty, value);
        }

        private static void AllowMultipleActiveChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MarkerSelectorCommandParameter prm = GetParameter(d);
            prm.AllowMultipleActive = (bool)e.NewValue;
        }

        private static MarkerSelectorCommandParameter GetParameter(DependencyObject d)
        {
            MarkerSelectorCommandParameter prm = CommandService.GetCommandParameter(d) as MarkerSelectorCommandParameter;
            if (prm == null)
            {
                prm = new MarkerSelectorCommandParameter();
                CommandService.SetCommandParameter(d, prm);
            }
            return prm;
        }

        private static void KeyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MarkerSelectorCommandParameter prm = GetParameter(d);
            prm.Key = e.NewValue as string;
        }

        private static void ValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MarkerSelectorCommandParameter prm = GetParameter(d);
            prm.Value = e.NewValue;
        }

        #endregion Methods
    }
}