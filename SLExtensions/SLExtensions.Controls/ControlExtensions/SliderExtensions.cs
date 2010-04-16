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

namespace SLExtensions.Controls
{
    public class SliderExtensions
    {
        private SliderExtensions(Slider attachedObject)
        {
            this.element = attachedObject as Slider;            
            this.element.AddHandler(FrameworkElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(element_MouseLeftButtonDown), true);
            this.element.MouseMove += new MouseEventHandler(element_MouseMove);
            this.element.MouseLeftButtonUp += new MouseButtonEventHandler(element_MouseLeftButtonUp);
        }

        void element_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isMouseCaptured = false;
            this.element.ReleaseMouseCapture();
        }

        void element_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseCaptured)
            {
                SetValueFromPosition(e);
            }
        }

        private bool isMouseCaptured;
        void element_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var focusedElement = (FocusManager.GetFocusedElement() as FrameworkElement);
            if (GetSetMouseDownPositionValue(element)
                && (e.Handled == false || 
                        focusedElement.IsInVisualTree(element)))
            {
                element.Focus();
                e.Handled = true;
                isMouseCaptured = element.CaptureMouse();
                SetValueFromPosition(e);
            }
        }

        private void SetValueFromPosition(MouseEventArgs e)
        {
            if (!GetSetMouseDownPositionValue(this.element))
                return;

            if (element.Orientation == Orientation.Horizontal)
            {
                var template = (VisualTreeHelper.GetChild(element, 0) as FrameworkElement).FindName("HorizontalTemplate") as FrameworkElement;
                var pos = e.GetPosition(template);
                var value = 0d;
                if (element.IsDirectionReversed)
                    value = (template.ActualWidth - pos.X) / template.ActualWidth * (element.Maximum - element.Minimum) + element.Minimum;
                else
                    value = pos.X / template.ActualWidth * (element.Maximum - element.Minimum) + element.Minimum;

                element.Value = value;
            }
            else
            {
                var template = (VisualTreeHelper.GetChild(element, 0) as FrameworkElement).FindName("VerticalTemplate") as FrameworkElement;
                var pos = e.GetPosition(template);
                var value = 0d;
                if (element.IsDirectionReversed)
                    value = pos.Y / template.ActualHeight * (element.Maximum - element.Minimum) + element.Minimum;
                else
                    value = (template.ActualHeight - pos.Y) / template.ActualHeight * (element.Maximum - element.Minimum) + element.Minimum;
                element.Value = value;
            }
        }

        private Slider element;

        public static bool GetSetMouseDownPositionValue(DependencyObject obj)
        {
            return (bool)obj.GetValue(SetMouseDownPositionValueProperty);
        }

        /// <summary>
        /// Set the value corresponding to the mousedown position to attached slider
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetSetMouseDownPositionValue(DependencyObject obj, bool value)
        {
            obj.SetValue(SetMouseDownPositionValueProperty, value);
        }

        // Using a DependencyProperty as the backing store for SetMouseDownPositionValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SetMouseDownPositionValueProperty =
            DependencyProperty.RegisterAttached("SetMouseDownPositionValue", typeof(bool), typeof(SliderExtensions), new PropertyMetadata(SetMouseDownPositionValueChangedCallback));

        private static void SetMouseDownPositionValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ext = GetSliderExtensions(d);            
        }

        
        internal static readonly DependencyProperty SliderExtensionsProperty =
            DependencyProperty.RegisterAttached("SliderExtensions", typeof(SliderExtensions), typeof(SliderExtensions), null);

        internal static SliderExtensions GetSliderExtensions(DependencyObject obj)
        {
            Slider element = obj as Slider;
            if (element == null)
                return null;

            SliderExtensions fe = obj.GetValue(SliderExtensionsProperty) as SliderExtensions;
            if (fe == null)
            {
                fe = new SliderExtensions(element);
                obj.SetValue(SliderExtensionsProperty, fe);
            }
            return fe;
        }
    }
}
