// <copyright file="ScrollableVirtualizedStackPanel.cs" company="no company">
// Distributed under Microsoft Public License (Ms-PL)
// </copyright>
namespace SLExtensions.Controls
{
    using System.Windows.Controls;

    /// <summary>
    /// Simple class to make all VirtualizedStackPanel in an application scrollable.
    /// </summary>
    public static class ScrollableVirtualizedStackPanel
    {
        #region Fields

        /// <summary>
        /// Is the ScrollableVirtualizedStackPanel initialized
        /// </summary>
        private static bool initialized = false;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Initialize the ScrollableVirtualizedStackPanel
        /// </summary>
        public static void Initialize()
        {
            if (!ScrollableVirtualizedStackPanel.initialized)
            {
                MouseWheelGenerator.MouseWheelEvent.RegisterClassHandler(typeof(VirtualizedStackPanel), ScrollableVirtualizedStackPanel.HandleMouseWheel, false);
                ScrollableVirtualizedStackPanel.initialized = true;
            }
        }

        /// <summary>
        /// Handles the mouse wheel.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SLExtensions.Controls.MouseWheelEventArgs"/> instance containing the event data.</param>
        private static void HandleMouseWheel(object sender, MouseWheelEventArgs e)
        {
            VirtualizedStackPanel vsp = (VirtualizedStackPanel)sender;

            if (e.Delta > 0)
            {
                vsp.CurrentScrollIndex--;
                e.Handled = true;
            }
            else if (e.Delta < 0)
            {
                vsp.CurrentScrollIndex++;
                e.Handled = true;
            }
        }

        #endregion Methods
    }
}