// <copyright file="MouseWheelGenerator.cs" company="Ucaya">
// Distributed under Microsoft Public License (Ms-PL)
// </copyright>
// <author>Thierry Bouquain</author>
namespace SLExtensions.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Browser;
    using System.Windows.Media;

    /// <summary>
    /// Hook mouse wheel dom events
    /// </summary>
    public class MouseWheelGenerator
    {
        #region Fields

        // Using a DependencyProperty as the backing store for IgnoreMouseWheel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IgnoreMouseWheelProperty = 
            DependencyProperty.RegisterAttached("IgnoreMouseWheel", typeof(bool), typeof(MouseWheelGenerator), null);

        /// <summary>
        /// Static mouse wheel event
        /// </summary>
        public static readonly BubblingEvent<MouseWheelEventArgs> MouseWheelEvent = new BubblingEvent<MouseWheelEventArgs>("MouseWheel", RoutingStrategy.Bubble);

        /// <summary>
        /// force event hooking
        /// </summary>
        private static MouseWheelGenerator helper = new MouseWheelGenerator();

        private System.Collections.Generic.List<System.Windows.Controls.Primitives.Popup> popups = new System.Collections.Generic.List<System.Windows.Controls.Primitives.Popup>();

        #endregion Fields

        #region Constructors

        /// <summary>
        /// hide public default constructor
        /// </summary>
        private MouseWheelGenerator()
        {
            if (HtmlPage.IsEnabled)
            {
                HtmlPage.Window.AttachEvent("DOMMouseScroll", this.HandleMouseWheel);
                HtmlPage.Window.AttachEvent("onmousewheel", this.HandleMouseWheel);
                HtmlPage.Document.AttachEvent("onmousewheel", this.HandleMouseWheel);
            }
        }

        #endregion Constructors

        #region Methods

        public static bool GetIgnoreMouseWheel(DependencyObject obj)
        {
            return (bool)obj.GetValue(IgnoreMouseWheelProperty);
        }

        public static void RegisterPopup(System.Windows.Controls.Primitives.Popup popup)
        {
            if (!helper.popups.Contains(popup))
            {
                helper.popups.Add(popup);
            }
        }

        public static void SetIgnoreMouseWheel(DependencyObject obj, bool value)
        {
            obj.SetValue(IgnoreMouseWheelProperty, value);
        }

        public static void UnRegisterPopup(System.Windows.Controls.Primitives.Popup popup)
        {
            if (helper.popups.Contains(popup))
            {
                helper.popups.Remove(popup);
            }
        }

        /// <summary>
        /// Handles mouse wheel events.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.Browser.HtmlEventArgs"/> instance containing the event data.</param>
        private void HandleMouseWheel(object sender, HtmlEventArgs args)
        {
            double delta = 0;

            ScriptObject eventObj = args.EventObject;

            if (eventObj.GetProperty("wheelDelta") != null)
            {
                delta = ((double)eventObj.GetProperty("wheelDelta")) / 120;

                if (HtmlPage.Window.GetProperty("opera") != null)
                {
                    delta = -delta;
                }
            }
            else if (eventObj.GetProperty("detail") != null)
            {
                delta = -((double)eventObj.GetProperty("detail")) / 3;

                if (HtmlPage.BrowserInformation.UserAgent.IndexOf("Macintosh") != -1)
                {
                    delta = delta * 3;
                }
            }

            if (delta != 0)
            {
                if (this.OnMouseWheel(delta, args))
                {
                    args.PreventDefault();
                }
            }
        }

        /// <summary>
        /// Raise routed MouseWheel event
        /// </summary>
        /// <param name="delta">The delta.</param>
        /// <param name="e">The <see cref="System.Windows.Browser.HtmlEventArgs"/> instance containing the event data.</param>
        /// <returns>returns <c>true</c> if the mouse wheel event is handled, <c>false</c> otherwise.</returns>
        private bool OnMouseWheel(double delta, HtmlEventArgs e)
        {
            Point mousePosition = new Point(e.OffsetX, e.OffsetY);

            if (HtmlPage.BrowserInformation.Name.Contains("Netscape") && HtmlPage.BrowserInformation.UserAgent.Contains("Firefox/2"))
            {
                mousePosition = new Point(e.ScreenX, e.ScreenY);
            }

            UIElement rootVisual = (UIElement)Application.Current.RootVisual;

            UIElement firstElement = null;

            if (popups.Count == 0)
            {
                foreach (UIElement element in VisualTreeHelper.FindElementsInHostCoordinates(mousePosition, rootVisual))
                {
                    firstElement = element;
                    break;
                }
            }
            else
            {
                if (firstElement == null)
                {
                    foreach (var popup in popups)
                    {
                        foreach (UIElement element in VisualTreeHelper.FindElementsInHostCoordinates(mousePosition, popup.Child))
                        {
                            firstElement = element;
                            break;
                        }

                        if (firstElement != null)
                            break;
                    }
                }
            }

            bool handled = false;

            if (firstElement != null)
            {
                FrameworkElement source = (FrameworkElement)firstElement;

                MouseWheelEventArgs wheelArgs = new MouseWheelEventArgs(source, delta);
                MouseWheelGenerator.MouseWheelEvent.RaiseEvent(wheelArgs, source, (dependencyObject => !GetIgnoreMouseWheel(dependencyObject)));

                handled = wheelArgs.Handled;
                return true;
            }

            return false;
        }

        #endregion Methods
    }
}