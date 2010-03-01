//---------------------------------------------------------------------
// <copyright file="TouchHelper.cs" company="Microsoft">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace SLExtensions.Manipulation
{
    /// <summary>
    /// Passes data for TouchReported event.
    /// </summary>
    public class TouchReportedEventArgs : EventArgs
    {
        internal TouchReportedEventArgs(IEnumerable<TouchPoint> touchPoints)
        {
            TouchPoints = touchPoints;
        }

        /// <summary>
        /// Returns reported touch points. 
        /// </summary>
        public IEnumerable<TouchPoint> TouchPoints { get; private set; }
    }


    /// <summary>
    /// Passes data for touch events.
    /// </summary>
    public class TouchEventArgs : EventArgs
    {
        internal TouchEventArgs(TouchPoint touchPoint)
        {
            TouchPoint = touchPoint;
        }

        /// <summary>
        /// Returns the associated touch point.
        /// </summary>
        public TouchPoint TouchPoint { get; private set; }
    }


    /// <summary>
    /// A group of touch event handlers.
    /// </summary>
    public class TouchHandlers
    {
        public EventHandler<TouchEventArgs> TouchDown { get; set; }
        public EventHandler<TouchEventArgs> CapturedTouchUp { get; set; }
        public EventHandler<TouchReportedEventArgs> CapturedTouchReported { get; set; }
        public EventHandler<TouchEventArgs> LostTouchCapture { get; set; }
    }

    /// <summary>
    /// A helper class to process, deliver and capture touch related events.
    /// Note: the class is not thread safe.
    /// </summary>
    public static class TouchHelper
    {
        // indicates if touch input is enabled or not
        private static bool isEnabled;

        // current event handlers
        private static readonly Dictionary<UIElement, TouchHandlers> currentHandlers =
            new Dictionary<UIElement, TouchHandlers>();

        private static readonly Dictionary<UIElement, List<EventHandler<TouchReportedEventArgs>>> previewHandlers =
            new Dictionary<UIElement, List<EventHandler<TouchReportedEventArgs>>>();

        // current captured touch devices (touchDevice.Id -> capturing UIElement)
        private static readonly Dictionary<int, UIElement> currentCaptures = new Dictionary<int, UIElement>();

        // current touch points (for captured touch devices only)
        private static readonly Dictionary<int, TouchPoint> currentTouchPoints = new Dictionary<int, TouchPoint>();

        // an empty array of TouchPoints
        private static readonly TouchPoint[] emptyTouchPoints = new TouchPoint[0];

        /// <summary>
        /// Returns true if there is at least one touch over the root. Otheriwse - false.
        /// </summary>
        public static bool AreAnyTouches
        {
            get
            {
                return currentTouchPoints.Count != 0;
            }
        }

        public static void ClearAllCaptures()
        {
            currentCaptures.Clear();
            currentTouchPoints.Clear();
        }

        /// <summary>
        /// Captured the given touchDevice. To release capture, pass element=null.
        /// </summary>
        /// <param name="touchDevice"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public static bool Capture(this TouchDevice touchDevice, UIElement element)
        {
            if (touchDevice == null)
            {
                throw new ArgumentNullException("element");
            }

            // raise LostCapture if the capturing element is different than the existing one
            UIElement existingCapture;
            if (currentCaptures.TryGetValue(touchDevice.Id, out existingCapture) &&
                !object.ReferenceEquals(existingCapture, element))
            {
                // check if a handler exists
                TouchHandlers handlers;
                if (currentHandlers.TryGetValue(existingCapture, out handlers))
                {
                    EventHandler<TouchEventArgs> handler = handlers.LostTouchCapture;
                    if (handler != null)
                    {
                        // raise LostCapture with the last known touchPoint
                        TouchPoint touchPoint;
                        if (currentTouchPoints.TryGetValue(touchDevice.Id, out touchPoint))
                        {
                            handler(existingCapture, new TouchEventArgs(touchPoint));
                        }
                    }
                }
            }

            // update currentCaptures dictionary
            if (element != null)
            {
                // capture
                currentCaptures[touchDevice.Id] = element;
            }
            else
            {
                // release
                currentCaptures.Remove(touchDevice.Id);
            }

            return true;
        }

        /// <summary>
        /// Adds event handlers for the given UIElement. Note: the method overrides all touch handler for the given element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="handlers"></param>
        public static void AddHandlers(UIElement element, TouchHandlers handlers)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            if (handlers == null)
            {
                throw new ArgumentNullException("handlers");
            }

            currentHandlers[element] = handlers;
        }

        /// <summary>
        /// Removes event handlers from the given element.
        /// </summary>
        /// <param name="element"></param>
        public static void RemoveHandlers(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            currentHandlers.Remove(element);
        }

        public static void AddPreviewCapturedTouchReported(UIElement element, EventHandler<TouchReportedEventArgs> handler)
        {
            List<EventHandler<TouchReportedEventArgs>> handlers;
            if (!previewHandlers.TryGetValue(element, out handlers))
            {
                handlers = new List<EventHandler<TouchReportedEventArgs>>();
                previewHandlers.Add(element, handlers);
            }
            handlers.Add(handler);
        }

        public static void RemovePreviewCapturedTouchReported(UIElement element, EventHandler<TouchReportedEventArgs> handler)
        {
            List<EventHandler<TouchReportedEventArgs>> handlers;
            if (!previewHandlers.TryGetValue(element, out handlers))
            {
                return;
            }
            handlers.Remove(handler);
        }


        /// <summary>
        /// Enables or disables touch input.
        /// </summary>
        /// <param name="enable"></param>
        public static void EnableInput(bool enable)
        {
            if (enable)
            {
                if (!isEnabled)
                {
                    EnableInput();
                    isEnabled = true;
                }
            }
            else
            {
                if (isEnabled)
                {
                    DisableInput();
                    isEnabled = false;
                }
            }
        }

        /// <summary>
        /// Enables touch input.
        /// </summary>
        private static void EnableInput()
        {
            Touch.FrameReported += TouchFrameReported;
        }

        /// <summary>
        /// Disables touch input and clear all dictionaries.
        /// </summary>
        private static void DisableInput()
        {
            Touch.FrameReported -= TouchFrameReported;
            currentCaptures.Clear();
            currentHandlers.Clear();
            currentTouchPoints.Clear();
        }


        /// <summary>
        /// Handles TouchFrameReported event and raise TouchDown/Up/Move events.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void TouchFrameReported(object sender, TouchFrameEventArgs e)
        {
            // get te root
            UIElement root = Application.Current.RootVisual;
            if (root == null)
            {
                return;
            }

            foreach (TouchPoint touchPoint in e.GetTouchPoints(null))
            {
                int id = touchPoint.TouchDevice.Id;

                // check if the touchDevice is captured or not.
                UIElement captured;
                currentCaptures.TryGetValue(id, out captured);

                switch (touchPoint.Action)
                {
                    // TouchDown
                    case TouchAction.Down:
                        HitTestAndRaiseDownEvent(root, touchPoint);
                        currentTouchPoints[id] = touchPoint;
                        break;

                    // TouchUp
                    case TouchAction.Up:
                        // handle only captured touches
                        if (captured != null)
                        {
                            RaiseUpEvent(captured, touchPoint);

                            // release capture
                            Capture(touchPoint.TouchDevice, null);
                            captured = null;
                        }
                        currentTouchPoints.Remove(id);
                        break;

                    // TouchMove
                    case TouchAction.Move:
                        // just remember the new touchPoint, the event will be raised in bulk later
                        currentTouchPoints[id] = touchPoint;
                        break;
                }
            }

            // raise CapturedReportEvents
            RaiseCapturedReportEvent();
        }

        /// <summary>
        /// Iterates through all event handlers, combines all touches captured by the corresponding UIElement
        /// and raise apturedReportEvent.
        /// </summary>
        private static void RaiseCapturedReportEvent()
        {
            // walk through all handlers
            foreach (KeyValuePair<UIElement, TouchHandlers> pairHandler in currentHandlers)
            {
                EventHandler<TouchReportedEventArgs> handler = pairHandler.Value.CapturedTouchReported;
                if (handler == null)
                {
                    continue;
                }

                List<TouchPoint> capturedTouchPoints = null;
                List<EventHandler<TouchReportedEventArgs>> previewList;
                if (previewHandlers.TryGetValue(pairHandler.Key, out previewList) && previewList.Count > 0)
                {
                    // PreviewPass
                    // walk through all touch devices captured by the current UIElement
                    foreach (KeyValuePair<int, UIElement> pairCapture in currentCaptures)
                    {
                        if (!object.ReferenceEquals(pairCapture.Value, pairHandler.Key))
                        {
                            continue;
                        }

                        // add the captured touchPoint
                        TouchPoint capturedTouchPoint;
                        if (currentTouchPoints.TryGetValue(pairCapture.Key, out capturedTouchPoint))
                        {
                            if (capturedTouchPoints == null)
                            {
                                capturedTouchPoints = new List<TouchPoint>();
                            }
                            capturedTouchPoints.Add(capturedTouchPoint);
                        }
                    }

                    var args = new TouchReportedEventArgs(capturedTouchPoints ?? (IEnumerable<TouchPoint>)emptyTouchPoints);

                    foreach (var item in previewList)
                    {
                        item(pairHandler.Key, args);
                    }
                }

                // Event pass, Recheck for lost capture
                capturedTouchPoints = null;
                foreach (KeyValuePair<int, UIElement> pairCapture in currentCaptures)
                {
                    if (!object.ReferenceEquals(pairCapture.Value, pairHandler.Key))
                    {
                        continue;
                    }

                    // add the captured touchPoint
                    TouchPoint capturedTouchPoint;
                    if (currentTouchPoints.TryGetValue(pairCapture.Key, out capturedTouchPoint))
                    {
                        if (capturedTouchPoints == null)
                        {
                            capturedTouchPoints = new List<TouchPoint>();
                        }
                        capturedTouchPoints.Add(capturedTouchPoint);
                    }
                }

                // raise event
                handler(pairHandler.Key, new TouchReportedEventArgs(capturedTouchPoints ?? (IEnumerable<TouchPoint>)emptyTouchPoints));
            }
        }

        /// <summary>
        /// Performs hit testing, find the first element in the parent chain that has TouchDown event handler and
        /// raises TouchTouch event.
        /// </summary>
        /// <param name="root"></param>
        /// <param name="touchPoint"></param>
        private static void HitTestAndRaiseDownEvent(UIElement root, TouchPoint touchPoint)
        {
            foreach (UIElement element in InputHitTest(root, touchPoint.Position))
            {
                TouchHandlers handlers;
                if (currentHandlers.TryGetValue(element, out handlers))
                {
                    EventHandler<TouchEventArgs> handler = handlers.TouchDown;
                    if (handler != null)
                    {
                        // call the first found handler and break
                        handler(element, new TouchEventArgs(touchPoint));
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Performs hit testing and returns a collection of UIElement the given point is within.
        /// </summary>
        /// <param name="root"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        private static IEnumerable<UIElement> InputHitTest(UIElement root, Point position)
        {
            foreach (UIElement element in VisualTreeHelper.FindElementsInHostCoordinates(position, root))
            {
                yield return element;

                for (UIElement parent = VisualTreeHelper.GetParent(element) as UIElement;
                     parent != null;
                     parent = VisualTreeHelper.GetParent(parent) as UIElement)
                {
                    yield return parent;
                }
            }
        }

        /// <summary>
        /// Raises TouchUp event.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="touchPoint"></param>
        private static void RaiseUpEvent(UIElement element, TouchPoint touchPoint)
        {
            TouchHandlers handlers;
            if (currentHandlers.TryGetValue(element, out handlers))
            {
                EventHandler<TouchEventArgs> handler = handlers.CapturedTouchUp;
                if (handler != null)
                {
                    handler(element, new TouchEventArgs(touchPoint));
                }
            }
        }
    }
}
