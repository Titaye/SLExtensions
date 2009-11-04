namespace SLExtensions.Controls.Animation
{
    using System;
    using System.ComponentModel;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using sysPropertyPath = System.Windows.PropertyPath;
    using System.Windows.Shapes;

    public class PropertyPath
    {
        #region Fields

        // Using a DependencyProperty as the backing store for PropertyPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PropertyPathProperty = 
            DependencyProperty.RegisterAttached("PropertyPath", typeof(PropertyPath), typeof(PropertyPath), new PropertyMetadata(PropertyPathChangedCallback));

        #endregion Fields

        #region Constructors

        public PropertyPath()
        {
        }

        #endregion Constructors

        #region Properties

        public string AttachedProperty
        {
            get; set;
        }

        public string Type
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public static PropertyPath GetPropertyPath(DependencyObject obj)
        {
            return (PropertyPath)obj.GetValue(PropertyPathProperty);
        }

        public static void SetPropertyPath(DependencyObject obj, PropertyPath value)
        {
            obj.SetValue(PropertyPathProperty, value);
        }

        public sysPropertyPath GetSytemPropertyPath()
        {
            Type t = System.Type.GetType(Type);
            DependencyProperty dp = (DependencyProperty) t.GetField(AttachedProperty + "Property", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).GetValue(null);
            sysPropertyPath pp = new sysPropertyPath(dp);
            return pp;
        }

        private static void PropertyPathChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            sysPropertyPath pp = null;
            if(e.NewValue != null)
            {
                pp = ((PropertyPath) e.NewValue).GetSytemPropertyPath();
            }
            Storyboard.SetTargetProperty((Timeline)d, pp);
        }

        #endregion Methods
    }
}