// <copyright file="MouseWheelMultiscaleImage.cs" company="Ucaya">
// Distributed under Microsoft Public License (Ms-PL)
// </copyright>
// <author>Thierry Bouquain</author>
namespace SLExtensions.DeepZoom
{
    using System.Windows.Controls;

    using SLExtensions.Controls;

    internal static class MouseWheelMultiscaleImage
    {
        #region Fields

        /// <summary>
        /// Is the ScrollableScrollViewer initialized
        /// </summary>
        private static bool initialized = false;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Initialize the MouseWheelMultiscaleImage
        /// </summary>
        public static void Initialize()
        {
            if (!MouseWheelMultiscaleImage.initialized)
            {
                MouseWheelGenerator.MouseWheelEvent.RegisterClassHandler(typeof(MultiScaleImage), MouseWheelMultiscaleImage.HandleMouseWheel, false);
                MouseWheelMultiscaleImage.initialized = true;
            }
        }

        /// <summary>
        /// Handles the mouse wheel.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SLExtensions.Controls.MouseWheelEventArgs"/> instance containing the event data.</param>
        private static void HandleMouseWheel(object sender, MouseWheelEventArgs e)
        {
            MultiScaleImage msi = (MultiScaleImage)sender;
            DZContext context = msi.EnsureContext();

            if (!context.IsMouseWheelEnabled)
                return;

            e.Handled = true;
            if (e.Delta > 0)
                msi.Zoom(1.2, context.LastMousePosition);
            else
                msi.Zoom(0.8, context.LastMousePosition);

            context.ClickedImageIndex = -1;
        }

        #endregion Methods
    }
}