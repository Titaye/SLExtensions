// <copyright file="StretchBox.cs" company="Microsoft">
//     Copyright © Microsoft Corporation. All rights reserved.
// </copyright>
// <summary>Implements the StretchBox class</summary>
// <author>Microsoft Expression Encoder Team</author>
namespace ExpressionMediaPlayer
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    /// <summary>
    /// Stretch box control.
    /// </summary>
    [ContentProperty("Child")]
    [TemplatePart(Name = StretchBox.ChildBorder, Type = typeof(Border))]
    public class StretchBox : Control
    {
        /// <summary>
        /// ChildBorder property string.
        /// </summary>
        private const string ChildBorder = "ChildBorder";

        /// <summary>
        /// Child border.
        /// </summary>
        private Border childBorder;

        /// <summary>
        /// Original size.
        /// </summary>
        private Size sizeOriginal;

        /// <summary>
        /// Scaling factor.
        /// </summary>
        private double scalingFactor = 1.0;

        /// <summary>
        /// The size we actually need.
        /// </summary>
        private Size sizeActuallyNeeded;

        /// <summary>
        /// Initializes a new instance of the StretchBox class.
        /// </summary>
        public StretchBox()
        {
            DefaultStyleKey = typeof(StretchBox);
        }

        /// <summary>
        /// Child dependency property.
        /// </summary>
        public static readonly DependencyProperty ChildProperty = DependencyProperty.Register
            ("Child", typeof(UIElement), typeof(StretchBox), new PropertyMetadata(new PropertyChangedCallback(StretchBox.OnChildPropertyChanged)));

        /// <summary>
        /// Gets or sets the child UI element.
        /// </summary>
        public UIElement Child
        {
            get { return GetValue(ChildProperty) as UIElement; }
            set { SetValue(ChildProperty, value); }
        }

        /// <summary>
        /// Overrides OnApplyTemplate().
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            childBorder = GetTemplateChild(ChildBorder) as Border;
            sizeOriginal = new Size(0, 0);
            OnChildChanged();
        }

        /// <summary>
        /// Dependency property handler for the child property.
        /// </summary>
        /// <param name="dobj">Source dependency object.</param>
        /// <param name="args">Event args.</param>
        private static void OnChildPropertyChanged(DependencyObject dobj, DependencyPropertyChangedEventArgs args)
        {
            (dobj as StretchBox).OnChildChanged();
        }

        /// <summary>
        /// Event handler for the child property changed event.
        /// </summary>
        private void OnChildChanged()
        {
            if (childBorder != null)
            {
                childBorder.Child = Child;
            }
        }

        /// <summary>
        /// Overrides MeasureOverride().
        /// </summary>
        /// <param name="availableSize">The available size.</param>
        /// <returns>Returns the measured size.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            Debug.WriteLine("MeasureOverride(availableSize=" + availableSize.ToString() + " )");
            if (childBorder != null)
            {
                if (sizeOriginal.Width == 0) // Make this measurement only once per instance
                {
                    childBorder.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    sizeOriginal = childBorder.DesiredSize;
                }

                Size desiredSizeMax = sizeOriginal;

                childBorder.Measure(availableSize);
                Size desiredSizeFit = childBorder.DesiredSize;                

                Double scalingFactorX = 1.0;
                if (availableSize.Width < desiredSizeFit.Width)
                {
                    scalingFactorX = availableSize.Width / desiredSizeFit.Width;
                }
                else if (availableSize.Width < desiredSizeMax.Width)
                {
                    scalingFactorX = availableSize.Width / desiredSizeMax.Width;
                }

                Double scalingFactorY = 1.0;
                if (availableSize.Height < desiredSizeFit.Height)
                {
                    scalingFactorY = availableSize.Height / desiredSizeFit.Height;
                }
                else if (availableSize.Height < desiredSizeMax.Height)
                {
                    scalingFactorY = availableSize.Height / desiredSizeMax.Height;
                }

                scalingFactor = Math.Min(scalingFactorX, scalingFactorY);

                Debug.WriteLine("scalingFactorX:" + scalingFactorX.ToString(CultureInfo.CurrentCulture));
                Debug.WriteLine("scalingFactorY:" + scalingFactorY.ToString(CultureInfo.CurrentCulture));
                Debug.WriteLine("scalingFactor:" + scalingFactor.ToString(CultureInfo.CurrentCulture));

                 if (Double.IsPositiveInfinity(availableSize.Width) || Double.IsPositiveInfinity(availableSize.Height))
                 {
                    sizeActuallyNeeded = new Size(desiredSizeFit.Width * scalingFactor, desiredSizeFit.Height * scalingFactor);
                 }
                 else
                 {
                    sizeActuallyNeeded = new Size(availableSize.Width / scalingFactor, availableSize.Height / scalingFactor);
                 }
            }

            Debug.WriteLine("MeasureOverride(desiredSize=" + sizeActuallyNeeded.ToString() + " )");
            return sizeActuallyNeeded;
        }

        /// <summary>
        /// Overrides ArrangeOverride().
        /// </summary>
        /// <param name="finalSize">The final size.</param>
        /// <returns>The returned final size.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            Debug.WriteLine("ArrangeOverride(finalSize=" + finalSize.ToString() + " )");

            if (childBorder != null)
            {
                ScaleTransform st = new ScaleTransform();
                st.ScaleX = scalingFactor;
                st.ScaleY = scalingFactor;
                st.CenterX = 0;
                st.CenterY = 0;

                childBorder.RenderTransform = st;

                childBorder.Arrange(new Rect(0, 0, sizeActuallyNeeded.Width, sizeActuallyNeeded.Height));

                Debug.WriteLine("ArrangeOverride(scalingFactor=" + scalingFactor.ToString(CultureInfo.CurrentUICulture) + " )");
            }

            return finalSize;
        }
    }
}
