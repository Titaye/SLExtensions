// <copyright file="StackPanel.cs" company="Ucaya">
// Distributed under Microsoft Public License (Ms-PL)
// </copyright>
// <author>Thierry Bouquain</author>
namespace SLExtensions.Controls
{
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

    /// <summary>
    /// StackPanel with Interval property
    /// </summary>
    public class StackPanel : Panel
    {
        #region Fields

        /// <summary>
        /// Interval depedency property.
        /// </summary>
        public static readonly DependencyProperty IntervalProperty = 
            DependencyProperty.Register(
                "Interval",
                typeof(int),
                typeof(StackPanel),
                new PropertyMetadata((d, e) => ((StackPanel)d).OnIntervalChanged((int)e.OldValue, (int)e.NewValue)));

        /// <summary>
        /// Orientation depedency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty = 
            DependencyProperty.Register(
                "Orientation",
                typeof(Orientation),
                typeof(StackPanel),
                new PropertyMetadata((d, e) => ((StackPanel)d).OnOrientationChanged((Orientation)e.OldValue, (Orientation)e.NewValue)));

        #endregion Fields

        #region Properties

        public int Interval
        {
            get
            {
                return (int)GetValue(IntervalProperty);
            }

            set
            {
                SetValue(IntervalProperty, value);
            }
        }

        public Orientation Orientation
        {
            get
            {
                return (Orientation)GetValue(OrientationProperty);
            }

            set
            {
                SetValue(OrientationProperty, value);
            }
        }

        #endregion Properties

        #region Methods

        protected override Size ArrangeOverride(Size finalSize)
        {
            Rect finalRect = new Rect(0,0,finalSize.Width, finalSize.Height);

            bool firstElement = true;
            double lastSize = 0;
            foreach (var element in Children)
            {
                if (firstElement)
                {
                    firstElement = false;
                }
                else
                {
                    if (this.Orientation == Orientation.Horizontal)
                    {
                        finalRect.X += Interval;
                    }
                    else
                    {
                        finalRect.Y += Interval;
                    }
                }

                if (this.Orientation == Orientation.Horizontal)
                {
                    finalRect.X += lastSize;
                    lastSize = element.DesiredSize.Width;
                    finalRect.Width = lastSize;
                    finalRect.Height = Math.Max(finalSize.Height, element.DesiredSize.Height);
                }
                else
                {
                    finalRect.Y += lastSize;
                    lastSize = element.DesiredSize.Height;
                    finalRect.Height = lastSize;
                    finalRect.Width = Math.Max(finalSize.Width, element.DesiredSize.Width);
                }

                element.Arrange(finalRect);
            }

            return finalSize;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Size size = new Size();

            Size constraint = availableSize;

            if (this.Orientation == Orientation.Horizontal)
            {
                availableSize.Width = double.PositiveInfinity;
            }
            else
            {
                availableSize.Height = double.PositiveInfinity;
            }

            bool firstElement = true;

            foreach (var element in Children)
            {
                if (firstElement)
                {
                    firstElement = false;
                }
                else
                {
                    if (this.Orientation == Orientation.Horizontal)
                    {
                        size.Width += Interval;
                    }
                    else
                    {
                        size.Height += Interval;
                    }
                }

                element.Measure(availableSize);
                if (this.Orientation == Orientation.Horizontal)
                {
                    size.Width += element.DesiredSize.Width;
                    size.Height = Math.Max(size.Height, element.DesiredSize.Height);
                }
                else
                {
                    size.Height += element.DesiredSize.Height;
                    size.Width = Math.Max(size.Width, element.DesiredSize.Width);
                }
            }

            return constraint;
        }

        /// <summary>
        /// handles the IntervalProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnIntervalChanged(int oldValue, int newValue)
        {
            InvalidateMeasure();
        }

        /// <summary>
        /// handles the OrientationProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnOrientationChanged(Orientation oldValue, Orientation newValue)
        {
            InvalidateMeasure();
        }

        #endregion Methods
    }
}