namespace SLExtensions.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    public class FlowLayoutPanel : Panel
    {
        #region Fields

        /// <summary>
        /// Orientation depedency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty = 
            DependencyProperty.Register(
                "Orientation",
                typeof(Orientation),
                typeof(FlowLayoutPanel),
                new PropertyMetadata((d, e) => ((FlowLayoutPanel)d).OnOrientationChanged((Orientation)e.OldValue, (Orientation)e.NewValue)));

        #endregion Fields

        #region Constructors

        public FlowLayoutPanel()
        {
            Orientation = Orientation.Horizontal;
        }

        #endregion Constructors

        #region Properties

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
            if (Orientation == Orientation.Horizontal)
            {
                double rowHeight = 0;
                double totalcolSize = 0;
                double maxWidth = 0;
                int firstIdx = 0;
                Rect arrangeRect = new Rect();

                for (int i = 0; i < Children.Count; i++)
                {
                    UIElement child = Children[i];
                    if (totalcolSize + child.DesiredSize.Width <= finalSize.Width)
                    {
                        rowHeight = Math.Max(child.DesiredSize.Height, rowHeight);
                        totalcolSize += child.DesiredSize.Width;
                        maxWidth = Math.Max(totalcolSize, maxWidth);
                    }
                    else
                    {
                        arrangeRect.Height = rowHeight;
                        for (int j = firstIdx; j < i; j++)
                        {
                            UIElement tobeArrangedChild = Children[j];
                            arrangeRect.Width = tobeArrangedChild.DesiredSize.Width;
                            tobeArrangedChild.Arrange(arrangeRect);

                            arrangeRect.X += tobeArrangedChild.DesiredSize.Width;
                        }
                        firstIdx = i;
                        arrangeRect.Y += rowHeight;
                        arrangeRect.X = 0;

                        rowHeight = child.DesiredSize.Height;
                        totalcolSize = child.DesiredSize.Width;
                        maxWidth = Math.Max(totalcolSize, maxWidth);
                    }
                }

                arrangeRect.Height = rowHeight;
                for (int j = firstIdx; j < Children.Count; j++)
                {
                    UIElement tobeArrangedChild = Children[j];
                    arrangeRect.Width = tobeArrangedChild.DesiredSize.Width;
                    tobeArrangedChild.Arrange(arrangeRect);

                    arrangeRect.X += tobeArrangedChild.DesiredSize.Width;
                }

                return new Size(arrangeRect.Y != 0 ? finalSize.Width : maxWidth, arrangeRect.Bottom);
            }
            else
            {
                double colWidth = 0;
                double totalrowSize = 0;
                double maxHeight = 0;
                int firstIdx = 0;
                Rect arrangeRect = new Rect();
                for (int i = 0; i < Children.Count; i++)
                {
                    UIElement child = Children[i];
                    if (totalrowSize + child.DesiredSize.Height <= finalSize.Height)
                    {
                        colWidth = Math.Max(child.DesiredSize.Width, colWidth);
                        totalrowSize += child.DesiredSize.Height;
                        maxHeight = Math.Max(totalrowSize, maxHeight);
                    }
                    else
                    {
                        arrangeRect.Width = colWidth;
                        for (int j = firstIdx; j < i; j++)
                        {
                            UIElement tobeArrangedChild = Children[j];
                            arrangeRect.Height = tobeArrangedChild.DesiredSize.Height;
                            tobeArrangedChild.Arrange(arrangeRect);

                            arrangeRect.Y += tobeArrangedChild.DesiredSize.Height;
                        }
                        firstIdx = i;
                        arrangeRect.X += colWidth;
                        arrangeRect.Y = 0;

                        colWidth = child.DesiredSize.Width;
                        totalrowSize = child.DesiredSize.Height;
                        maxHeight = Math.Max(totalrowSize, maxHeight);
                    }
                }

                arrangeRect.Width = colWidth;
                for (int j = firstIdx; j < Children.Count; j++)
                {
                    UIElement tobeArrangedChild = Children[j];
                    arrangeRect.Height = tobeArrangedChild.DesiredSize.Height;
                    tobeArrangedChild.Arrange(arrangeRect);

                    arrangeRect.Y += tobeArrangedChild.DesiredSize.Height;
                }

                return new Size(arrangeRect.Right, arrangeRect.Y != 0 ? finalSize.Height : maxHeight);
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (Orientation == Orientation.Horizontal)
            {
                Size tempSize = availableSize;

                // Current computed Row Height
                double rowHeight = 0;

                // Total needed height of the control
                double totalHeight = 0;

                // Store current column size
                double totalcolSize = 0;

                foreach (var item in Children)
                {
                    // Measure current child
                    item.Measure(availableSize);

                    if (totalcolSize + item.DesiredSize.Width <= availableSize.Width)
                    {
                        // Current item fit into the remaining width
                        tempSize.Width -= Math.Min(tempSize.Width, item.DesiredSize.Width);

                        // Row height is the Max of all the children displayed in the row
                        rowHeight = Math.Min(Math.Max(rowHeight, item.DesiredSize.Height), availableSize.Height);

                        totalcolSize += item.DesiredSize.Width;
                    }
                    else
                    {
                        // Current item don't fit
                        // Put it on a new Row
                        totalHeight += rowHeight;
                        totalcolSize = item.DesiredSize.Width;
                        tempSize = availableSize;
                        tempSize.Height -= Math.Min(rowHeight, tempSize.Height);
                        rowHeight = 0;
                    }

                }

                totalHeight += rowHeight;
                return new Size(availableSize.Width, totalHeight);
            }
            else
            {
                Size tempSize = availableSize;
                double rowWidth = 0;
                double totalWidth = 0;
                double totalrowSize = 0;

                foreach (var item in Children)
                {
                    item.Measure(availableSize);

                    if (totalrowSize + item.DesiredSize.Height <= availableSize.Height)
                    {
                        tempSize.Height -= Math.Min(tempSize.Height, item.DesiredSize.Height);
                        rowWidth = Math.Min(Math.Max(rowWidth, item.DesiredSize.Width), availableSize.Width);
                        totalrowSize += item.DesiredSize.Height;
                    }
                    else
                    {
                        totalWidth += rowWidth;
                        totalrowSize = item.DesiredSize.Height;
                        tempSize = availableSize;
                        tempSize.Width -= Math.Min(rowWidth, tempSize.Width);
                        rowWidth = 0;
                    }
                }

                totalWidth += rowWidth;
                return new Size(totalWidth, availableSize.Height);

            }
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