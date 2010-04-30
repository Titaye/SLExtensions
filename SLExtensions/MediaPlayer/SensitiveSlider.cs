// <copyright file="SensitiveSlider.cs" company="Microsoft">
//     Copyright © Microsoft Corporation. All rights reserved.
// </copyright>
// <summary>Implements the SensitiveSlider class</summary>
// <author>Microsoft Expression Encoder Team</author>
namespace ExpressionMediaPlayer
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    /// <summary>
    /// A timeline slider control class.
    /// </summary>
    [TemplatePart(Name = SensitiveSlider.HorizontalThumb, Type = typeof(Thumb))]
    [TemplatePart(Name = SensitiveSlider.VerticalThumb, Type = typeof(Thumb))]
    [TemplatePart(Name = SensitiveSlider.LeftTrack, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = SensitiveSlider.RightTrack, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = SensitiveSlider.UpTrack, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = SensitiveSlider.DownTrack, Type = typeof(FrameworkElement))]
    public class SensitiveSlider : Slider
    {
        /// <summary>
        /// Property string for HorizontalThumb.
        /// </summary>
        private const string HorizontalThumb = "HorizontalThumb";

        /// <summary>
        /// Property string for VerticalThumb.
        /// </summary>
        private const string VerticalThumb = "VerticalThumb";

        /// <summary>
        /// Property string for LeftTrack.
        /// </summary>
        private const string LeftTrack = "LeftTrack";

        /// <summary>
        /// Property string for RightTrack.
        /// </summary>
        private const string RightTrack = "RightTrack";

        /// <summary>
        /// Property string for UpTrack.
        /// </summary>
        private const string UpTrack = "UpTrack";

        /// <summary>
        /// Property string for DownTrack.
        /// </summary>
        private const string DownTrack = "DownTrack";

        /// <summary>
        /// Internal element for Slider horizontal thumb "HorizontalThumb".
        /// </summary>
        private Thumb m_horizontalThumb;

        /// <summary>
        /// Internal element for Slider verical thumb "VerticalThumb".
        /// </summary>        
        private Thumb m_verticalThumb;

        /// <summary>
        /// Internal element for SensitiveSlider left track "LeftTrack".
        /// </summary>
        private FrameworkElement m_leftTrack;

        /// <summary>
        /// Internal element for SensitiveSlider right track "RightTrack".
        /// </summary>
        private FrameworkElement m_rightTrack;

        /// <summary>
        /// Internal element for SensitiveSlider up track "UpTrack".
        /// </summary>
        private FrameworkElement m_upTrack;

        /// <summary>
        /// Internal element for SensitiveSlider down track "DownTrack".
        /// </summary>
        private FrameworkElement m_downTrack;

        /// <summary>
        /// The slider position at the start of a drag.
        /// </summary>
        private double m_dragStartValue;
      
        /// <summary>
        /// Initializes a new instance of the SensitiveSlider class. You can click anywhere on slider to step to a particular time.
        /// </summary>
        public SensitiveSlider() 
        {
            this.ValueChanged += new RoutedPropertyChangedEventHandler<double>(SensitiveSliderValueChanged);
        }

        /// <summary>
        /// Value changed event (with optional filtering of SliderDrag value changes.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<double> FilteredValueChanged;

        /// <summary>
        /// Thumb drag started event. 
        /// </summary>
        public event DragStartedEventHandler DragStarted;

        /// <summary>
        /// Thumb drag completed event.
        /// </summary>
        public event DragCompletedEventHandler DragCompleted;

        /// <summary>
        /// Gets a value indicating whether the user is currently dragging the thumb.
        /// </summary>
        [System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsDragging
        {
            get
            {
                if (m_horizontalThumb != null && m_horizontalThumb.IsDragging)
                {
                    Debug.WriteLine("Horizontal drag!");
                    return true;
                }

                if (m_verticalThumb != null && m_verticalThumb.IsDragging)
                {
                    Debug.WriteLine("Vertical drag!");
                    return true;
                }

                return false;
            }
        }
        
        /// <summary>
        /// Overridden OnApplyTemplate, sets internal elements and events.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            
            m_horizontalThumb = GetTemplateChild(HorizontalThumb) as Thumb;
            m_verticalThumb = GetTemplateChild(VerticalThumb) as Thumb;
            m_leftTrack = GetTemplateChild(LeftTrack) as FrameworkElement;
            m_rightTrack = GetTemplateChild(RightTrack) as FrameworkElement;
            m_upTrack = GetTemplateChild(UpTrack) as FrameworkElement;
            m_downTrack = GetTemplateChild(DownTrack) as FrameworkElement;

            if (m_horizontalThumb != null)
            {
                m_horizontalThumb.DragStarted += new DragStartedEventHandler(OnDragStarted);
                m_horizontalThumb.DragCompleted += new DragCompletedEventHandler(OnDragCompleted);
            }

            if (m_verticalThumb != null)
            {
                m_verticalThumb.DragStarted += new DragStartedEventHandler(OnDragStarted);
                m_verticalThumb.DragCompleted += new DragCompletedEventHandler(OnDragCompleted);
            }

            if (m_leftTrack != null)
            {
                m_leftTrack.MouseLeftButtonDown += new MouseButtonEventHandler(OnMouseClick);
            }

            if (m_rightTrack != null)
            {
                m_rightTrack.MouseLeftButtonDown += new MouseButtonEventHandler(OnMouseClick);
            }

            if (m_upTrack != null)
            {
                m_upTrack.MouseLeftButtonDown += new MouseButtonEventHandler(OnMouseClick);
            }

            if (m_downTrack != null)
            {
                m_downTrack.MouseLeftButtonDown += new MouseButtonEventHandler(OnMouseClick);
            }
        }

        /// <summary>
        /// Filtered ValueChanged event handler.
        /// </summary>
        /// <param name="sender">Source object, Slider.</param>
        /// <param name="e">Property changed event args.</param>
        void SensitiveSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Debug.WriteLine("SensitiveSliderValueChanged -- sending change new value:" + e.NewValue.ToString(CultureInfo.CurrentCulture));
            RoutedPropertyChangedEventHandler<double> handler = FilteredValueChanged;
            if (handler != null && !IsDragging)
            {
                handler(this, new RoutedPropertyChangedEventArgs<double>(e.OldValue, e.NewValue));
            }            
        }

        /// <summary>
        /// Computes the new slider value based on mouse position -- takes into account the orientation of the slider.
        /// </summary>
        /// <returns>The new slider value.</returns>
        /// <param name="args">Mouse button args.</param>
        private Double CalcValue(MouseButtonEventArgs args)
        {
            Point pt = args.GetPosition(this);

            Double valueNew;

            if (this.Orientation == Orientation.Horizontal)
            {
                valueNew =
                (
                    (
                        (
                            pt.X
                            -
                            (m_horizontalThumb.ActualWidth / 2)
                        )
                        /
                        (ActualWidth - m_horizontalThumb.ActualWidth) * (Maximum - Minimum)
                    )
                    +
                    Minimum
                );
            }
            else 
            {
                valueNew =
                (
                    (
                        (ActualHeight - pt.Y)
                        -
                        (m_verticalThumb.ActualHeight / 2)
                    )
                    /
                    (ActualHeight - m_verticalThumb.ActualHeight)
                    *
                    (
                        (Maximum - Minimum)
                        +
                        Minimum
                    )
                );
            }

            return valueNew;
        }

        /// <summary>
        /// Handle DragStart event.
        /// </summary>
        /// <param name="sender">Source object, Thumb.</param>
        /// <param name="e">Drag start args.</param>
        private void OnDragStarted(object sender, DragStartedEventArgs e)
        {
            Debug.WriteLine("OnDragStarted e=" + e.ToString());

            m_dragStartValue = this.Value;
            if (DragStarted != null)
            {
                DragStarted(sender, e);
            }
        }

        /// <summary>
        /// Handle DragCompleted event.
        /// </summary>
        /// <param name="sender">Source object, Thumb.</param>
        /// <param name="e">Drag completed args.</param>
        private void OnDragCompleted(object sender, DragCompletedEventArgs e)
        {
            Debug.WriteLine("OnDragCompleted e=" + e.ToString());

            RoutedPropertyChangedEventHandler<double> handler = FilteredValueChanged;
            if (handler != null)
            {
                handler(this, new RoutedPropertyChangedEventArgs<double>(m_dragStartValue, this.Value));
            }

            if (DragCompleted != null)
            {
                DragCompleted(sender, e);
            }
        }
 
        /// <summary>
        /// Send send change event when the mouse clicks down the track bar.
        /// </summary>
        /// <param name="sender">Source object, track.</param>
        /// <param name="args">Mouse button args.</param>
        private void OnMouseClick(object sender, MouseButtonEventArgs args) 
        {
            Debug.WriteLine("OnMouseDownHorizontal sender=" + sender.ToString() + " args=" + args.ToString());
            Value = CalcValue(args);
        }
    }
}
