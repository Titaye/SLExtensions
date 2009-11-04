// <copyright file="ScrollableScrollViewer.cs" company="Ucaya">
// Distributed under Microsoft Public License (Ms-PL)
// </copyright>
// <author>Thierry Bouquain</author>
namespace SLExtensions.Controls
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Simple class to make all scrollviewers in an application scrollable.
    /// </summary>
    public static class ScrollableScrollViewer
    {
        #region Fields

        // Using a DependencyProperty as the backing store for MouseWheelScrollOrientation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MouseWheelScrollOrientationProperty = 
            DependencyProperty.RegisterAttached("MouseWheelScrollOrientation", typeof(Orientation), typeof(ScrollableScrollViewer), null);

        /// <summary>
        /// Is the ScrollableScrollViewer initialized
        /// </summary>
        private static bool initialized = false;

        #endregion Fields

        #region Methods

        public static Orientation GetMouseWheelScrollOrientation(DependencyObject obj)
        {
            return (Orientation)obj.GetValue(MouseWheelScrollOrientationProperty);
        }

        /// <summary>
        /// Initialize the ScrollableScrollViewer
        /// </summary>
        public static void Initialize()
        {
            if (!ScrollableScrollViewer.initialized)
            {
                MouseWheelGenerator.MouseWheelEvent.RegisterClassHandler(typeof(ScrollViewer), ScrollableScrollViewer.HandleMouseWheel, false);
                ScrollableScrollViewer.initialized = true;
            }
        }

        public static void SetMouseWheelScrollOrientation(DependencyObject obj, Orientation value)
        {
            obj.SetValue(MouseWheelScrollOrientationProperty, value);
        }

        /// <summary>
        /// Handles the mouse wheel.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SLExtensions.Controls.MouseWheelEventArgs"/> instance containing the event data.</param>
        private static void HandleMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer sv = (ScrollViewer)sender;

            Orientation orientation = GetMouseWheelScrollOrientation(sv);
            if (orientation == Orientation.Vertical)
            {
                double verticalOffset = sv.VerticalOffset;
                if (e.Delta > 0 && verticalOffset > 0)
                {
                    sv.ScrollToVerticalOffset(verticalOffset - e.Delta * 50);
                    e.Handled = true;
                }
                else if (e.Delta < 0 && verticalOffset < sv.ScrollableHeight)
                {
                    sv.ScrollToVerticalOffset(verticalOffset - e.Delta * 50);
                    e.Handled = true;
                }
            }
            else
            {
                double horizontalOffset = sv.HorizontalOffset;
                if (e.Delta > 0 && horizontalOffset > 0)
                {
                    sv.ScrollToHorizontalOffset(horizontalOffset - e.Delta * 50);
                    e.Handled = true;
                }
                else if (e.Delta < 0 && horizontalOffset < sv.ScrollableWidth)
                {
                    sv.ScrollToHorizontalOffset(horizontalOffset - e.Delta * 50);
                    e.Handled = true;
                }
            }
        }

        #endregion Methods
    }
}