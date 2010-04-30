// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="DraggableExtensions.cs" company="RAW Engineering">
// Distributed under Microsoft Public License (Ms-PL)
// </copyright>
// <author>Kunal Shetye</author>
// <example>
// 1) Making a FrameworkElement draggable:
//      someXAMLElement.Draggable(LayoutRoot);
//      someXAMLElement.Draggable(LayoutRoot,lblStatus); use this if you want to report mouse positions on a label
//
// 2) Stopping a FrameworkElement from being draggable
//      someXAMLElement.StopDraggable();
//
// 3) Checking if a FrameworkElement is already draggable
//      bool flag = someXAMLElement.IsAlreadyDraggable();
// </example>
// ---------------------------------------------------------------------------------------------------------------------
namespace SLExtensions
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// Adds dragging capability to FrameworkElements
    /// </summary>
    public static class DraggableExtensions
    {
        #region Methods

        /// <summary>
        /// This method makes a FrameworkElement draggable and takes 2 parameters.
        /// </summary>
        /// <param name="element">The FrameworkElement.</param>
        /// <param name="elementParent">The immediate parent element containing this FrameworkElement.</param>
        /// <param name="status">The TextBlock to report mouse co-ordinates.</param>
        /// Wrapper Method over MakeDraggable
        public static void Draggable(
            this FrameworkElement element,
            Panel elementParent,
            TextBlock status)
        {
            MakeDraggable(
                element,
                elementParent,
                status);
        }

        /// <summary>
        /// This method makes a FrameworkElement draggable and takes single parameters.
        /// </summary>
        /// <param name="element">The FrameworkElement.</param>
        /// <param name="elementParent">The immediate parent element containing this FrameworkElement.</param>
        /// Wrapper Method over MakeDraggable
        public static void Draggable(
            this FrameworkElement element,
            Panel elementParent)
        {
            MakeDraggable(
                element,
                elementParent,
                null);
        }

        /// <summary>
        /// Determines if the FrameworkElement can be dragged or not
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>true if FrameWorkElement can be dragged otherwise, false</returns>
        public static bool IsAlreadyDraggable(this FrameworkElement element)
        {
            try
            {
                // Try fetching the instance of DragInfo class.
                var di = (((element as FrameworkElement).Parent as Canvas).Tag as DragInfo);

                // If we succeed in getting DragInfo instance then this element is Draggable type.
                if (di != null)
                {
                    return di.IsDraggable;
                }
                else
                {
                    // Else this element is not Draggable type.
                    return false; // Not Draggable
                }
            }
            catch (Exception)
            {
                return false; // Not Draggable
            }
        }

        /// <summary>
        /// Determines if the FrameworkElement is already converted into a draggable type or not.
        /// </summary>
        /// <param name="element">The element to check for.</param>
        /// <returns>true if FrameWorkElement is Draggable type otherwise, false</returns>
        public static bool IsDraggableType(this FrameworkElement element)
        {
            try
            {
                // Try to cast the Tag as DragInfo to check if this element is Draggable.
                var di = (((element as FrameworkElement).Parent as Canvas).Tag as DragInfo);
                return true; // If we succeed we know that we are dealing with a Draggable type
            }
            catch (Exception)
            {
                return false; // No Draggable type here
            }
        }

        /// <summary>
        /// Stop the FrameworkElement from Dragging.
        /// </summary>
        /// <param name="element">The element.</param>
        public static void StopDraggable(this FrameworkElement element)
        {
            if (IsDraggableType(element))
            {
                var di = (((element as FrameworkElement).Parent as Canvas).Tag as DragInfo);

                // ReSharper disable PossibleNullReferenceException
                di.IsDraggable = false;
            }
        }

        /// <summary>
        /// The actual method to make the FrameworkElement Draggable
        /// </summary>
        /// <param name="element">The element who.</param>
        /// <param name="elementParent">The element parent.</param>
        /// <param name="status">The status.</param>
        private static void MakeDraggable(
            FrameworkElement element,
            Panel elementParent,
            TextBlock status)
        {
            if (IsDraggableType(element))
            {
                if (IsDraggableType(element))
                {
                    // If this element already has a DragInfo associated with it then
                    // enable the IsDraggable and return.
                    var di = (((element as FrameworkElement).Parent as Canvas).Tag as DragInfo);

                    di.IsDraggable = true;

                    return;
                }
            }

            var canvasWrapper = new Canvas
                                    {
                                        Background = new SolidColorBrush(Colors.Transparent),
                                    };

            elementParent.Children.Remove(element);

            canvasWrapper.Tag = new DragInfo(); // Keep a DragInfo instance in FrameworkElement.Tag for further reference

            canvasWrapper.Children.Add(element);

            elementParent.Children.Add(canvasWrapper);

            // Attach to the MouseLeftButtonDown event.
            element.MouseLeftButtonDown += (o,
                                            e) =>
                                               {
                                                   var dragInfo = canvasWrapper.Tag as DragInfo;
                                                   if (dragInfo != null)
                                                   {
                                                       // Check if we can drag
                                                       if (dragInfo.IsDraggable)
                                                       {
                                                           // Mark that we're doing a drag
                                                           dragInfo.IsDragging = true;

                                                           // Ensure that the mouse can't leave element being dragged
                                                           element.CaptureMouse();

                                                           // Determine where the mouse 'grabbed'
                                                           // to use during MouseMove
                                                           dragInfo.Offset = e.GetPosition(element);
                                                       }
                                                   }
                                               };

            // Attach to the MouseLeftButtonUp event.
            element.MouseLeftButtonUp += (o,
                                          e) =>
                                             {
                                                 var dragInfo = canvasWrapper.Tag as DragInfo;
                                                 if (dragInfo != null)
                                                 {
                                                     // Check if we can drag
                                                     if (dragInfo.IsDraggable)
                                                     {
                                                         if (dragInfo.IsDragging)
                                                         {
                                                             // Turn off Dragging
                                                             dragInfo.IsDragging = false;

                                                             // Free the Mouse
                                                             element.ReleaseMouseCapture();
                                                         }
                                                     }
                                                 }
                                             };

            // Attach to the MouseMove event.
            element.MouseMove += (o,
                                  e) =>
                                     {
                                         var dragInfo = canvasWrapper.Tag as DragInfo;
                                         if (dragInfo != null)
                                         {
                                             // Check if we can drag
                                             if (dragInfo.IsDraggable)
                                             {
                                                 if (dragInfo.IsDragging)
                                                 {
                                                     // Where is the mouse now?
                                                     Point newPosition = e.GetPosition(canvasWrapper);

                                                     if (status != null)
                                                     {
                                                         status.Text = string.Format(
                                                             "Position: {0},{1} and {2},{3}",
                                                             newPosition.X,
                                                             newPosition.Y,
                                                             dragInfo.Offset.X,
                                                             dragInfo.Offset.Y);
                                                     }

                                                     // Move the element via the new position less the Offset
                                                     element.SetValue(
                                                         Canvas.LeftProperty,
                                                         newPosition.X - dragInfo.Offset.X);

                                                     element.SetValue(
                                                         Canvas.TopProperty,
                                                         newPosition.Y - dragInfo.Offset.Y);
                                                 }
                                             }
                                         }
                                     };
        }

        #endregion Methods
    }

    /// <summary>
    /// Stores information about Draggable type.
    /// </summary>
    internal class DragInfo
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DragInfo"/> class. 
        /// </summary>
        public DragInfo()
        {
            this.IsDragging = false;
            this.IsDraggable = true;
            this.Offset = new Point(
                0,
                0); // Relative Offset to move the FrameworkElement.
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether the user is holding down the MouseLeftButton
        /// and performing a drag.
        /// </summary>
        public bool IsDraggable
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether Dragging is enabled or disabled.
        /// </summary>
        public bool IsDragging
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets mouse Offset.
        /// </summary>
        public Point Offset
        {
            get; set;
        }

        #endregion Properties
    }
}