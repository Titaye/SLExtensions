#region Header

// <copyright file="BindingComparer.cs" company="Microsoft">
// Distributed under Microsoft Public License (Ms-PL)
// </copyright>
// <author>Pierre Lagarde</author>

#endregion Header

namespace SLExtensions.Data
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    #region Enumerations

    public enum TypeCompare
    {
        Equal = 0,
        Diff = 1,
        Supp = 2,
        Inf = 3,
        Custom = 4
    }

    #endregion Enumerations

    public class BindingComparer
    {
        #region Fields

        // Using a DependencyProperty as the backing store for Compare.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CompareProperty = 
            DependencyProperty.RegisterAttached("Compare", typeof(TypeCompare), typeof(BindingComparer), new PropertyMetadata(OnValueChanged));

        // Using a DependencyProperty as the backing store for CustomComparer.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CustomComparerProperty = 
            DependencyProperty.RegisterAttached("CustomComparer", typeof(IComparer), typeof(BindingComparer), new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for Template.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TemplateProperty = 
            DependencyProperty.RegisterAttached("Template", typeof(ControlTemplate), typeof(BindingComparer), new PropertyMetadata(OnValueChanged));

        // Using a DependencyProperty as the backing store for ValueA.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueAProperty = 
            DependencyProperty.RegisterAttached("ValueA", typeof(object), typeof(BindingComparer), new PropertyMetadata(OnValueChanged));

        // Using a DependencyProperty as the backing store for ValueB.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueBProperty = 
            DependencyProperty.RegisterAttached("ValueB", typeof(object), typeof(BindingComparer), new PropertyMetadata(OnValueChanged));

        #endregion Fields

        #region Methods

        public static TypeCompare GetCompare(DependencyObject obj)
        {
            return (TypeCompare)obj.GetValue(CompareProperty);
        }

        public static IComparer GetCustomComparer(DependencyObject obj)
        {
            return (IComparer)obj.GetValue(CustomComparerProperty);
        }

        public static ControlTemplate GetTemplate(DependencyObject obj)
        {
            return (ControlTemplate)obj.GetValue(TemplateProperty);
        }

        public static object GetValueA(DependencyObject obj)
        {
            return (object)obj.GetValue(ValueAProperty);
        }

        public static object GetValueB(DependencyObject obj)
        {
            return (object)obj.GetValue(ValueBProperty);
        }

        public static void SetCompare(DependencyObject obj, TypeCompare value)
        {
            obj.SetValue(CompareProperty, value);
        }

        public static void SetCustomComparer(DependencyObject obj, IComparer value)
        {
            obj.SetValue(CustomComparerProperty, value);
        }

        public static void SetTemplate(DependencyObject obj, ControlTemplate value)
        {
            obj.SetValue(TemplateProperty, value);
        }

        public static void SetValueA(DependencyObject obj, object value)
        {
            obj.SetValue(ValueAProperty, value);
        }

        public static void SetValueB(DependencyObject obj, object value)
        {
            obj.SetValue(ValueBProperty, value);
        }

        private static bool CheckParam(DependencyObject o)
        {
            if (!(o is FrameworkElement))
                return false;

            Object A = o.GetValue(BindingComparer.ValueAProperty);
            Object B = o.GetValue(BindingComparer.ValueBProperty);
            Object Comp = o.GetValue(BindingComparer.CompareProperty);
            Object TemplateComp = o.GetValue(BindingComparer.TemplateProperty);
            if ((A == null) || (B == null) || (Comp== null))
                return false;
            else
                return true;
        }

        private static void OnValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (!CheckParam(o))
                return;

            var A = o.GetValue(BindingComparer.ValueAProperty);
            var B = o.GetValue(BindingComparer.ValueBProperty);
            TypeCompare Comp = (TypeCompare)o.GetValue(BindingComparer.CompareProperty);
            ControlTemplate TemplateComp = (ControlTemplate)o.GetValue(BindingComparer.TemplateProperty);
            IComparer CustomComparer = (IComparer)o.GetValue(BindingComparer.CustomComparerProperty);

            Type type = A.GetType();

            //Comparer<int>.Default.Compare
            var compType = (typeof(Comparer<>).MakeGenericType(type));
            var comp = compType.GetProperty("Default").GetValue(null, null) as IComparer;

            bool ret = true;
            switch (Comp)
            {
                case TypeCompare.Equal:
                    ret = comp.Compare(A, B) == 0;
                    break;
                case TypeCompare.Diff:
                    ret = comp.Compare(A, B) != 0;
                    break;
                case TypeCompare.Supp:
                    ret = comp.Compare(A, B) > 0;
                    break;
                case TypeCompare.Inf:
                    ret = comp.Compare(A, B) < 0;
                    break;
                case TypeCompare.Custom:
                    ret = CustomComparer.Compare(A, B) != 0;
                    break;
                default:
                    break;
            }

            FrameworkElement f = (FrameworkElement)o;
            if (!f.Resources.Contains("__BindingComparer.Save"))
            {
                f.Resources.Add("__BindingComparer.Save", ((Control)f).Template);
            }
            if (ret)
            {
                ((Control)f).Template = TemplateComp;
            }
            else
            {
                ((Control)f).Template = (ControlTemplate)f.Resources["__BindingComparer.Save"];
            }
            ((Control)f).Focus();
        }

        #endregion Methods
    }
}