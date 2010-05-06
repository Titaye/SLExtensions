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

namespace SLExtensions.Interactivity
{
    public class MouseActivityExclusion : DependencyObject
    {
        #region Element

        public FrameworkElement Element
        {
            get
            {
                return (FrameworkElement)GetValue(ElementProperty);
            }

            set
            {
                SetValue(ElementProperty, value);
            }
        }

        /// <summary>
        /// Element depedency property.
        /// </summary>
        public static readonly DependencyProperty ElementProperty =
            DependencyProperty.Register(
                "Element",
                typeof(FrameworkElement),
                typeof(MouseActivityExclusion),
                null);

        #endregion Element        

    }
}
