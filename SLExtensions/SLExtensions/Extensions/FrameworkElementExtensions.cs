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

    public static class FrameworkElementExtensions
    {
        #region Methods

        public static T FirstVisualAncestorOfType<T>(this FrameworkElement element)
            where T : FrameworkElement
        {
            return FirstVisualAncestorOfType<T>(element, int.MaxValue);
        }

        public static T FirstVisualAncestorOfType<T>(this FrameworkElement element, int maxDistance)
            where T : FrameworkElement
        {
            var cnt = 0;
            var parent = VisualTreeHelper.GetParent(element) as FrameworkElement;
            cnt++;
            while (parent != null && cnt < maxDistance)
            {
                if (parent is T)
                    return (T)parent;
                parent = VisualTreeHelper.GetParent(parent) as FrameworkElement;
                cnt++;
            }
            return parent as T;
        }

        public static T FirstVisualAncestorOfType<T>(this FrameworkElement element, FrameworkElement rootAncestor)
            where T : FrameworkElement
        {
            var parent = VisualTreeHelper.GetParent(element) as FrameworkElement;
            while (parent != null && parent != rootAncestor)
            {
                if (parent is T)
                    return (T)parent;
                parent = VisualTreeHelper.GetParent(parent) as FrameworkElement;
            }
            return parent as T;
        }

        /// <summary>
        /// Determines whether an element is in the visual tree of the current application.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        /// 	<c>true</c> if element paramter is in visual tree otherwise, <c>false</c>.
        /// </returns>
        public static bool IsInVisualTree(this FrameworkElement element)
        {
            return IsInVisualTree(element, Application.Current.RootVisual as FrameworkElement);
        }

        /// <summary>
        /// Determines whether an element is in the visual tree of a given ancestor.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="ancestor">The ancestor.</param>
        /// <returns>
        /// 	<c>true</c> if element paramter is in visual tree otherwise, <c>false</c>.
        /// </returns>
        public static bool IsInVisualTree(this FrameworkElement element, FrameworkElement ancestor)
        {
            FrameworkElement fe = element;
            while (fe != null)
            {
                if (fe == ancestor)
                {
                    return true;
                }

                fe = VisualTreeHelper.GetParent(fe) as FrameworkElement;
            }

            return false;
        }

        #endregion Methods
    }
}