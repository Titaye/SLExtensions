// <copyright file="DockPanel.cs" company="Ucaya">
// Distributed under Microsoft Public License (Ms-PL)
// </copyright>
// <author>Thierry Bouquain</author>
namespace SLExtensions.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// Dock panel. Dock child elements on this panel based on their DockPanel.Dock attached property
    /// </summary>
    public class DockPanel : Panel
    {
        #region Fields

        /// <summary>
        ///  Using a DependencyProperty as the backing store for Dock.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DockProperty = 
            DependencyProperty.RegisterAttached("Dock", typeof(Dock), typeof(DockPanel), new PropertyMetadata(dockChanged));

        /// <summary>
        /// LastChildFill depedency property.
        /// </summary>
        public static readonly DependencyProperty LastChildFillProperty = 
            DependencyProperty.Register(
                "LastChildFill",
                typeof(bool),
                typeof(DockPanel),
                new PropertyMetadata((d, e) => ((DockPanel)d).OnLastChildFillChanged((bool)e.OldValue, (bool)e.NewValue)));

        #endregion Fields

        #region Properties

        public bool LastChildFill
        {
            get
            {
                return (bool)GetValue(LastChildFillProperty);
            }

            set
            {
                SetValue(LastChildFillProperty, value);
            }
        }

        #endregion Properties

        #region Methods

        public static Dock GetDock(DependencyObject obj)
        {
            return (Dock)obj.GetValue(DockProperty);
        }

        public static void SetDock(DependencyObject obj, Dock value)
        {
            obj.SetValue(DockProperty, value);
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            UIElementCollection internalChildren = Children;
            int count = internalChildren.Count;
            int num2 = count - (this.LastChildFill ? 1 : 0);
            double x = 0.0;
            double y = 0.0;
            double num5 = 0.0;
            double num6 = 0.0;
            for (int i = 0; i < count; i++)
            {
                UIElement element = internalChildren[i];
                if (element != null)
                {
                    Size desiredSize = element.DesiredSize;
                    Rect finalRect = new Rect(x, y, Math.Max((double)0.0, (double)(arrangeSize.Width - (x + num5))), Math.Max((double)0.0, (double)(arrangeSize.Height - (y + num6))));
                    if (i < num2)
                    {
                        switch (GetDock(element))
                        {
                            case Dock.Left:
                                x += desiredSize.Width;
                                finalRect.Width = desiredSize.Width;
                                break;

                            case Dock.Top:
                                y += desiredSize.Height;
                                finalRect.Height = desiredSize.Height;
                                break;

                            case Dock.Right:
                                num5 += desiredSize.Width;
                                finalRect.X = Math.Max((double)0.0, (double)(arrangeSize.Width - num5));
                                finalRect.Width = desiredSize.Width;
                                break;

                            case Dock.Bottom:
                                num6 += desiredSize.Height;
                                finalRect.Y = Math.Max((double)0.0, (double)(arrangeSize.Height - num6));
                                finalRect.Height = desiredSize.Height;
                                break;
                        }
                    }
                    element.Arrange(finalRect);
                }
            }
            return arrangeSize;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            double totalDesiredWidth = 0;
            double totalDesiredHeight = 0;

            double maxLeftRightHeight = 0;
            double maxTopBottomWidth = 0;

            double totalHeight = 0;
            double totalWidth = 0;

            foreach (UIElement child in Children)
            {
                if (child.Visibility != Visibility.Collapsed)
                {
                    child.Measure(new Size(availableSize.Width - totalWidth, availableSize.Height - totalHeight));

                    switch ((Dock)child.GetValue(DockProperty))
                    {
                        default:
                        case Dock.Left:
                        case Dock.Right:
                            totalWidth += child.DesiredSize.Width;
                            if (child.DesiredSize.Height > maxLeftRightHeight)
                                maxLeftRightHeight = child.DesiredSize.Height;
                            break;
                        case Dock.Top:
                        case Dock.Bottom:
                            totalHeight += child.DesiredSize.Height;
                            if (child.DesiredSize.Width > maxTopBottomWidth)
                                maxTopBottomWidth = child.DesiredSize.Width;
                            break;
                    }
                }
            }

            totalDesiredWidth = Math.Max(totalWidth, maxTopBottomWidth);
            totalDesiredHeight = Math.Max(totalHeight, maxLeftRightHeight);

            Size result = new Size(Math.Min(availableSize.Width, totalDesiredWidth), Math.Min(availableSize.Height, totalDesiredHeight));

            return result;
        }

        private static void dockChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement reference = d as UIElement;
            if (reference != null)
            {
                DockPanel parent = VisualTreeHelper.GetParent(reference) as DockPanel;
                if (parent != null)
                {
                    parent.InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// handles the LastChildFillProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnLastChildFillChanged(bool oldValue, bool newValue)
        {
            InvalidateMeasure();
        }

        #endregion Methods
    }
}