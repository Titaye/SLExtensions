#region Header

// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

#endregion Header

namespace SLExtensions.Controls
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Automation.Peers;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Animation;

    using SLExtensions.Controls.Primitives;

    /// <summary>
    /// Slider control lets the user select from a range of values by moving a slider.
    /// Slider is used to enable to user to gradually modify a value (range selection). 
    /// Slider is an easy and natural interface for users, because it provides good visual feedback.
    /// </summary>
    [TemplatePart(Name = Slider2D.ElementThumbName, Type = typeof(Thumb))]
    [TemplatePart(Name = Slider2D.ElementTemplateRootName, Type = typeof(FrameworkElement))]
    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateMouseOver, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateUnfocused, GroupName = VisualStates.GroupFocus)]
    [TemplateVisualState(Name = VisualStates.StateFocused, GroupName = VisualStates.GroupFocus)]
    public class Slider2D : Range2DBase
    {
        #region Fields

        /// <summary> 
        /// Identifies the IsDirectionReversed dependency property. 
        /// </summary>
        public static readonly DependencyProperty IsDirectionXReversedProperty = 
            DependencyProperty.Register(
                "IsDirectionXReversed",
                typeof(bool),
                typeof(Slider2D),
                new PropertyMetadata(OnIsDirectionXReversedChanged));

        /// <summary> 
        /// Identifies the IsDirectionReversed dependency property. 
        /// </summary>
        public static readonly DependencyProperty IsDirectionYReversedProperty = 
            DependencyProperty.Register(
                "IsDirectionYReversed",
                typeof(bool),
                typeof(Slider2D),
                new PropertyMetadata(OnIsDirectionYReversedChanged));

        /// <summary>
        /// Identifies the IsFocused dependency property.
        /// </summary> 
        public static readonly DependencyProperty IsFocusedProperty = 
            DependencyProperty.Register(
                "IsFocused",
                typeof(bool),
                typeof(Slider2D),
                new PropertyMetadata(OnIsFocusedPropertyChanged));

        /// <summary>
        /// SetMouseDownPositionValue depedency property.
        /// </summary>
        public static readonly DependencyProperty SetMouseDownPositionValueProperty = 
            DependencyProperty.Register(
                "SetMouseDownPositionValue",
                typeof(bool),
                typeof(Slider2D),
                null);

        internal const string ElementTemplateRootName = "TemplateRoot";
        internal const string ElementThumbName = "Thumb";

        private bool isMouseCaptured;

        /// <summary> 
        /// Accumulates drag offsets in case the mouse drags off the end of the track.
        /// </summary>
        private double _dragValueX;
        private double _dragValueY;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Constructor to setup the Slider class 
        /// </summary> 
        public Slider2D()
        {
            SizeChanged += delegate { UpdateTrackLayout(); };

            DefaultStyleKey = typeof(Slider2D);
            IsEnabledChanged += OnIsEnabledChanged;
        }

        #endregion Constructors

        #region Properties

        /// <summary> 
        /// Gets a value that determines whether the direction is reversed. 
        /// </summary>
        public bool IsDirectionXReversed
        {
            get { return (bool)GetValue(IsDirectionXReversedProperty); }
            set { SetValue(IsDirectionXReversedProperty, value); }
        }

        /// <summary> 
        /// Gets a value that determines whether the direction is reversed. 
        /// </summary>
        public bool IsDirectionYReversed
        {
            get { return (bool)GetValue(IsDirectionYReversedProperty); }
            set { SetValue(IsDirectionYReversedProperty, value); }
        }

        /// <summary>
        /// Gets a value that determines whether this element has logical focus.
        /// </summary> 
        public bool IsFocused
        {
            get { return (bool)GetValue(IsFocusedProperty); }
            internal set { SetValue(IsFocusedProperty, value); }
        }

        public bool SetMouseDownPositionValue
        {
            get
            {
                return (bool)GetValue(SetMouseDownPositionValueProperty);
            }

            set
            {
                SetValue(SetMouseDownPositionValueProperty, value);
            }
        }

        /// <summary> 
        /// template root
        /// </summary>
        internal virtual FrameworkElement ElementTemplateRoot
        {
            get; set;
        }

        /// <summary> 
        /// Thumb for dragging track
        /// </summary>
        internal virtual Thumb ElementThumb
        {
            get; set;
        }

        /// <summary> 
        /// Whether the mouse is currently over the control
        /// </summary> 
        internal bool IsMouseOver
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        /// <summary> 
        /// Apply a template to the slider.
        /// </summary> 
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Get the parts
            ElementThumb = GetTemplateChild(ElementThumbName) as Thumb;
            ElementTemplateRoot = GetTemplateChild(ElementTemplateRootName) as FrameworkElement;

            if (ElementThumb != null)
            {
                ElementThumb.DragStarted += delegate(object sender, DragStartedEventArgs e) { this.Focus(); OnThumbDragStarted(); };
                ElementThumb.DragDelta += delegate(object sender, DragDeltaEventArgs e) { OnThumbDragDelta(e); };
            }
            // Updating states for parts where properties might have been updated through
            // XAML before the template was loaded.
            ChangeVisualState(false);
        }

        /// <summary>
        /// Change to the correct visual state for the Slider.
        /// </summary> 
        /// <param name="useTransitions"> 
        /// true to use transitions when updating the visual state, false to
        /// snap directly to the new visual state. 
        /// </param>
        internal void ChangeVisualState(bool useTransitions)
        {
            if (!IsEnabled)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateDisabled, VisualStates.StateNormal);
            }
            else if (IsMouseOver)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateMouseOver, VisualStates.StateNormal);
            }
            else
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateNormal);
            }

            if (IsFocused && IsEnabled)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateFocused, VisualStates.StateUnfocused);
            }
            else
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateUnfocused);
            }
        }

        /// <summary> 
        /// Called when the IsFocused property changes.
        /// </summary>
        /// <param name="e"> 
        /// The data for DependencyPropertyChangedEventArgs.
        /// </param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "e", Justification = "Compat with WPF.")]
        internal virtual void OnIsFocusChanged(DependencyPropertyChangedEventArgs e)
        {
            UpdateVisualState();
        }

        /// <summary>
        /// This method will take the current min, max, and value to
        /// calculate and layout the current control measurements. 
        /// </summary>
        internal virtual void UpdateTrackLayout()
        {
            double maximumX = MaximumX;
            double minimumX = MinimumX;
            double valueX = ValueX;
            double multiplierX = 1 - (maximumX - valueX) / (maximumX - minimumX);

            double maximumY = MaximumY;
            double minimumY = MinimumY;
            double valueY = ValueY;
            double multiplierY = 1 - (maximumY - valueY) / (maximumY - minimumY);

            Grid templateGrid = ElementTemplateRoot as Grid; ;
            if (templateGrid != null)
            {
                if (templateGrid.ColumnDefinitions != null && templateGrid.ColumnDefinitions.Count == 3)
                {
                    var thumbWidth = ElementThumb != null ? ElementThumb.ActualWidth : 0;
                    var colWidth = multiplierX * (ActualWidth - thumbWidth);
                    if (IsDirectionXReversed)
                    {
                        templateGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
                        templateGrid.ColumnDefinitions[2].Width = new GridLength(colWidth);
                    }
                    else
                    {
                        templateGrid.ColumnDefinitions[0].Width = new GridLength(colWidth);
                        templateGrid.ColumnDefinitions[2].Width = new GridLength(1, GridUnitType.Star);
                    }
                }

                if (templateGrid.RowDefinitions != null && templateGrid.RowDefinitions.Count == 3)
                {
                    var thumbHeight = ElementThumb != null ? ElementThumb.ActualHeight : 0;
                    var rowHeight = multiplierY * (ActualHeight - thumbHeight);

                    if (IsDirectionYReversed)
                    {
                        templateGrid.RowDefinitions[0].Height = new GridLength(rowHeight);
                        templateGrid.RowDefinitions[2].Height = new GridLength(1, GridUnitType.Star);
                    }
                    else
                    {
                        templateGrid.RowDefinitions[0].Height = new GridLength(1, GridUnitType.Star);
                        templateGrid.RowDefinitions[2].Height = new GridLength(rowHeight);
                    }
                }
            }
        }

        /// <summary>
        /// Update the current visual state of the slider. 
        /// </summary> 
        internal void UpdateVisualState()
        {
            ChangeVisualState(true);
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            IsFocused = true;
        }

        /// <summary> 
        /// Responds to the KeyPressed event.
        /// </summary>
        /// <param name="e">The event data for the KeyPressed event.</param> 
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Handled)
            {
                return;
            }

            if (!IsEnabled)
            {
                return;
            }

            if (e.Key == Key.Left)
            {
                if (IsDirectionXReversed)
                {
                    ValueX += SmallChangeX;
                }
                else
                {
                    ValueX -= SmallChangeX;
                }
            }
            else if (e.Key == Key.Down)
            {
                if (IsDirectionYReversed)
                {
                    ValueY += SmallChangeY;
                }
                else
                {
                    ValueY -= SmallChangeY;
                }
            }
            else if (e.Key == Key.Right)
            {
                if (IsDirectionXReversed)
                {
                    ValueX -= SmallChangeX;
                }
                else
                {
                    ValueX += SmallChangeX;
                }
            }
            else if (e.Key == Key.Up)
            {
                if (IsDirectionYReversed)
                {
                    ValueY -= SmallChangeY;
                }
                else
                {
                    ValueY += SmallChangeY;
                }
            }
            else if (e.Key == Key.Home)
            {
                ValueX = MinimumX;
                ValueY = MinimumY;
            }
            else if (e.Key == Key.End)
            {
                ValueX = MaximumX;
                ValueY = MaximumY;
            }
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            IsFocused = false;
        }

        /// <summary>
        /// Called when the Maximum property changes. 
        /// </summary>
        /// <param name="oldMaximum">Old value of the Maximum property.</param>
        /// <param name="newMaximum">New value of the Maximum property.</param> 
        protected override void OnMaximumXChanged(double oldMaximum, double newMaximum)
        {
            base.OnMaximumXChanged(oldMaximum, newMaximum);
            UpdateTrackLayout();
        }

        /// <summary>
        /// Called when the Maximum property changes. 
        /// </summary>
        /// <param name="oldMaximum">Old value of the Maximum property.</param>
        /// <param name="newMaximum">New value of the Maximum property.</param> 
        protected override void OnMaximumYChanged(double oldMaximum, double newMaximum)
        {
            base.OnMaximumYChanged(oldMaximum, newMaximum);
            UpdateTrackLayout();
        }

        /// <summary> 
        /// Called when the Minimum property changes.
        /// </summary>
        /// <param name="oldMinimum">Old value of the Minimum property.</param> 
        /// <param name="newMinimum">New value of the Minimum property.</param>
        protected override void OnMinimumXChanged(double oldMinimum, double newMinimum)
        {
            base.OnMinimumXChanged(oldMinimum, newMinimum);
            UpdateTrackLayout();
        }

        /// <summary> 
        /// Called when the Minimum property changes.
        /// </summary>
        /// <param name="oldMinimum">Old value of the Minimum property.</param> 
        /// <param name="newMinimum">New value of the Minimum property.</param>
        protected override void OnMinimumYChanged(double oldMinimum, double newMinimum)
        {
            base.OnMinimumYChanged(oldMinimum, newMinimum);
            UpdateTrackLayout();
        }

        /// <summary> 
        /// Responds to the MouseEnter event.
        /// </summary> 
        /// <param name="e">The event data for the MouseEnter event.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "e", Justification = "Compat with WPF.")]
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            IsMouseOver = true;
            if (ElementThumb != null && !ElementThumb.IsDragging)
            {
                UpdateVisualState();
            }
        }

        /// <summary>
        /// Responds to the MouseLeave event. 
        /// </summary> 
        /// <param name="e">The event data for the MouseLeave event.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "e", Justification = "Compat with WPF.")]
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            IsMouseOver = false;
            if (ElementThumb != null && !ElementThumb.IsDragging)
            {
                UpdateVisualState();
            }
        }

        /// <summary> 
        /// Responds to the MouseLeftButtonDown event.
        /// </summary>
        /// <param name="e">The event data for the MouseLeftButtonDown event.</param> 
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "e", Justification = "Compat with WPF.")]
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (e.Handled)
            {
                return;
            }
            e.Handled = true;
            Focus();
            isMouseCaptured = CaptureMouse();

            SetValueFromMousePosition(e);
        }

        /// <summary>
        /// Responds to the MouseLeftButtonUp event. 
        /// </summary>
        /// <param name="e">The event data for the MouseLeftButtonUp event.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "e", Justification = "Compat with WPF.")]
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            if (e.Handled)
            {
                return;
            }
            e.Handled = true;
            isMouseCaptured = false;
            ReleaseMouseCapture();
            UpdateVisualState();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (isMouseCaptured)
            {
                SetValueFromMousePosition(e);
            }
        }

        /// <summary> 
        /// Called when the Value property changes.
        /// </summary> 
        /// <param name="oldValue">Old value of the Value property.</param>
        /// <param name="newValue">New value of the Value property.</param>
        protected override void OnValueXChanged(double oldValue, double newValue)
        {
            base.OnValueXChanged(oldValue, newValue);
            UpdateTrackLayout();
        }

        /// <summary> 
        /// Called when the Value property changes.
        /// </summary> 
        /// <param name="oldValue">Old value of the Value property.</param>
        /// <param name="newValue">New value of the Value property.</param>
        protected override void OnValueYChanged(double oldValue, double newValue)
        {
            base.OnValueYChanged(oldValue, newValue);
            UpdateTrackLayout();
        }

        /// <summary> 
        /// IsDirectionReversedProperty property changed handler.
        /// </summary> 
        /// <param name="d">Slider that changed IsDirectionReversed.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnIsDirectionXReversedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Slider2D s = d as Slider2D;
            Debug.Assert(s != null);

            s.UpdateTrackLayout();
        }

        /// <summary> 
        /// IsDirectionReversedProperty property changed handler.
        /// </summary> 
        /// <param name="d">Slider that changed IsDirectionReversed.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnIsDirectionYReversedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Slider2D s = d as Slider2D;
            Debug.Assert(s != null);

            s.UpdateTrackLayout();
        }

        /// <summary>
        /// IsFocusedProperty property changed handler. 
        /// </summary> 
        /// <param name="d">Slider that changed IsFocused.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param> 
        private static void OnIsFocusedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Slider2D s = d as Slider2D;
            Debug.Assert(s != null);

            s.OnIsFocusChanged(e);
        }

        /// <summary> 
        /// Called when the IsEnabled property changes.
        /// </summary> 
        /// <param name="e">Property changed args</param>
        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!IsEnabled)
            {
                IsMouseOver = false;
            }

            UpdateVisualState();
        }

        /// <summary>
        /// Whenever the thumb gets dragged, we handle the event through 
        /// this function to update the current value depending upon the
        /// thumb drag delta.
        /// </summary> 
        /// <param name="e">DragEventArgs</param> 
        private void OnThumbDragDelta(DragDeltaEventArgs e)
        {
            double offsetX = 0;
            double offsetY = 0;

            offsetX = e.HorizontalChange / (ActualWidth - ElementThumb.ActualWidth) * (MaximumX - MinimumX);
            offsetY = -e.VerticalChange / (ActualHeight - ElementThumb.ActualHeight) * (MaximumY - MinimumY);

            if (!double.IsNaN(offsetX) && !double.IsInfinity(offsetX))
            {
                _dragValueX += IsDirectionXReversed ? -offsetX : offsetX;

                double newValue = Math.Min(MaximumX, Math.Max(MinimumX, _dragValueX));

                if (newValue != ValueX)
                {
                    ValueX = newValue;
                }
            }

            if (!double.IsNaN(offsetY) && !double.IsInfinity(offsetY))
            {
                _dragValueY += IsDirectionYReversed ? -offsetY : offsetY;

                double newValue = Math.Min(MaximumY, Math.Max(MinimumY, _dragValueY));

                if (newValue != ValueY)
                {
                    ValueY = newValue;
                }
            }
        }

        /// <summary> 
        /// Called whenever the Thumb drag operation is started
        /// </summary>
        private void OnThumbDragStarted()
        {
            this._dragValueX = this.ValueX;
            this._dragValueY = this.ValueY;
        }

        private void SetValueFromMousePosition(MouseEventArgs e)
        {
            if (SetMouseDownPositionValue && ElementTemplateRoot != null)
            {
                var pos = e.GetPosition(ElementTemplateRoot);

                var valueX = 0d;
                if (IsDirectionXReversed)
                    valueX = (ElementTemplateRoot.ActualWidth - pos.X) / ElementTemplateRoot.ActualWidth * (MaximumX - MinimumX) + MinimumX;
                else
                    valueX = pos.X / ElementTemplateRoot.ActualWidth * (MaximumX - MinimumX) + MinimumX;

                var valueY = 0d;
                if (IsDirectionYReversed)
                    valueY = pos.Y / ElementTemplateRoot.ActualHeight * (MaximumX - MinimumX) + MinimumX;
                else
                    valueY = (ElementTemplateRoot.ActualHeight - pos.Y) / ElementTemplateRoot.ActualHeight * (MaximumX - MinimumX) + MinimumX;

                ValueX = valueX;
                ValueY = valueY;
            }
        }

        #endregion Methods
    }
}