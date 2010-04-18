namespace SLExtensions.Design
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

    public class DesignTime
    {
        #region Fields

        // Using a DependencyProperty as the backing store for DesignClass.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DesignClassProperty = 
            DependencyProperty.RegisterAttached("DesignClass", typeof(string), typeof(DesignTime), new PropertyMetadata(DesignClassChangedCallback));

        #endregion Fields

        #region Methods

        public static string GetDesignClass(DependencyObject obj)
        {
            return (string)obj.GetValue(DesignClassProperty);
        }

        public static void SetDesignClass(DependencyObject obj, string value)
        {
            obj.SetValue(DesignClassProperty, value);
        }

        private static void DesignClassChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (System.ComponentModel.DesignerProperties.IsInDesignTool)
            {
                var t = Type.GetType((string) e.NewValue);
                if(t != null)
                    Activator.CreateInstance(t, d);
            }
        }

        #endregion Methods
    }
}