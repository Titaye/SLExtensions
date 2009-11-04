// <copyright file="ScrollableVirtualizedListBox.cs" company="no company">
// Distributed under Microsoft Public License (Ms-PL)
// </copyright>
namespace SLExtensions.Controls
{
    using System.Windows.Controls;

    /// <summary>
    /// Simple class to make all VirtualizedListBox in an application scrollable.
    /// </summary>
    public static class ScrollableVirtualizedListBox
    {
        #region Fields

        /// <summary>
        /// Is the ScrollableVirtualizedListBox initialized
        /// </summary>
        private static bool initialized = false;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Initialize the ScrollableVirtualizedListBox
        /// </summary>
        public static void Initialize()
        {
            if (!ScrollableVirtualizedListBox.initialized)
            {
                MouseWheelGenerator.MouseWheelEvent.RegisterClassHandler(typeof(VirtualizedListBox), ScrollableVirtualizedListBox.HandleMouseWheel, false);
                ScrollableVirtualizedListBox.initialized = true;
            }
        }

        /// <summary>
        /// Handles the mouse wheel.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SLExtensions.Controls.MouseWheelEventArgs"/> instance containing the event data.</param>
        private static void HandleMouseWheel(object sender, MouseWheelEventArgs e)
        {
            VirtualizedListBox vlb = (VirtualizedListBox)sender;

            if (e.Delta > 0)
            {
                vlb.ScrollBarValue--;
                e.Handled = true;
            }
            else if (e.Delta < 0)
            {
                vlb.ScrollBarValue++;
                e.Handled = true;
            }
        }

        #endregion Methods
    }
}