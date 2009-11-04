// <copyright file="AnimatingTilePanel.cs" company="SLExtensions">
// Converted from Kevin's Bag o trick http://j832.com/bagotricks/
// </copyright>
namespace SLExtensions.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Threading;

    /// <summary>
    /// Flow layout panel with animations
    /// </summary>
    public class AnimatingTilePanel : Panel
    {
        #region Fields

        public static readonly DependencyProperty AnimatesNewItemProperty = 
            DependencyProperty.Register("AnimatesNewItem", typeof(bool),
                typeof(AnimatingTilePanel), null);
        public static readonly DependencyProperty AttractionProperty = 
            DependencyProperty.Register(
                "Attraction",
                typeof(double),
                typeof(AnimatingTilePanel),
                null);
        public static readonly DependencyProperty DampeningProperty = 
            DependencyProperty.Register(
                "Dampening",
                typeof(double),
                typeof(AnimatingTilePanel),
                null);
        public static readonly DependencyProperty ItemHeightProperty = 
              DependencyProperty.RegisterAttached(
                  "ItemHeight",
                  typeof(double),
                  typeof(AnimatingTilePanel),
                  null);
        public static readonly DependencyProperty ItemWidthProperty = 
            DependencyProperty.RegisterAttached(
                "ItemWidth",
                typeof(double),
                typeof(AnimatingTilePanel),
                null);

        // Using a DependencyProperty as the backing store for SameSizeItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SameSizeItemsProperty = 
            DependencyProperty.RegisterAttached("SameSizeItems", typeof(bool), typeof(AnimatingTilePanel), null);

        //public static readonly DependencyProperty VariationProperty =
        //    CreateDoubleDP("Variation", 1, FrameworkPropertyMetadataOptions.None, 0, true, 1, true, false);
        public static readonly DependencyProperty VariationProperty = 
            DependencyProperty.Register(
                "Variation",
                typeof(double),
                typeof(AnimatingTilePanel),
                null);

        private static readonly DependencyProperty DataProperty = 
            DependencyProperty.RegisterAttached("Data", typeof(AnimatingTilePanelData), typeof(AnimatingTilePanel), null);

        private const double c_diff = 0.1;
        private const double c_terminalVelocity = 10000;

        //private bool m_appliedTemplate;
        private bool m_hasArranged; // Used to make sure items are not arranged on the initial load
        private DispatcherTimer timer = new DispatcherTimer();

        #endregion Fields

        #region Constructors

        public AnimatingTilePanel()
        {
            ItemWidth = 50;
            ItemHeight = 50;
            Dampening = 0.2;
            Attraction = 2;
            SameSizeItems = true;
            this.Loaded += new RoutedEventHandler(AnimatingTilePanel_Loaded);
        }

        #endregion Constructors

        #region Properties

        //public static readonly DependencyProperty AttractionProperty =
        //    CreateDoubleDP("Attraction", 2, FrameworkPropertyMetadataOptions.None, 0, double.PositiveInfinity, false);
        public bool AnimatesNewItem
        {
            get { return (bool)GetValue(AnimatesNewItemProperty); }
            set { SetValue(AnimatesNewItemProperty, value); }
        }

        //public static readonly DependencyProperty DampeningProperty =
        //    CreateDoubleDP("Dampening", 0.2, FrameworkPropertyMetadataOptions.None, 0, 1, false);
        public double Attraction
        {
            get { return (double)GetValue(AttractionProperty); }
            set { SetValue(AttractionProperty, value); }
        }

        public double Dampening
        {
            get { return (double)GetValue(DampeningProperty); }
            set { SetValue(DampeningProperty, value); }
        }

        public double ItemHeight
        {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        public double ItemWidth
        {
            get { return (double)GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }

        public bool SameSizeItems
        {
            get { return (bool)GetValue(SameSizeItemsProperty); }
            set { SetValue(SameSizeItemsProperty, value); }
        }

        public double Variation
        {
            get { return (double)GetValue(VariationProperty); }
            set { SetValue(VariationProperty, value); }
        }

        #endregion Properties

        #region Methods

        public static bool Animate(
            Point currentValue, Vector currentVelocity, Point targetValue,
            double attractionFator, double dampening,
            double terminalVelocity, double minValueDelta, double minVelocityDelta,
            out Point newValue, out Vector newVelocity)
        {
            Vector diff = new Vector(targetValue.X - currentValue.X, targetValue.Y - currentValue.Y);

            if (diff.Length > minValueDelta || currentVelocity.Length > minVelocityDelta)
            {
                newVelocity = currentVelocity * (1 - dampening);
                newVelocity += diff * attractionFator;
                newVelocity *= (currentVelocity.Length > terminalVelocity) ? terminalVelocity / currentVelocity.Length : 1;

                newValue = currentValue;
                newValue.X += newVelocity.X;
                newValue.Y += newVelocity.Y;

                return true;
            }
            else
            {
                newValue = targetValue;
                newVelocity = new Vector();
                return false;
            }
        }

        public static double GetItemHeight(DependencyObject element)
        {
            return (double)element.GetValue(ItemHeightProperty);
        }

        public static double GetItemWidth(DependencyObject element)
        {
            return (double)element.GetValue(ItemWidthProperty);
        }

        public static bool GetSameSizeItems(DependencyObject obj)
        {
            return (bool)obj.GetValue(SameSizeItemsProperty);
        }

        public static List<T> MakeList<T>(T itemOftype)
        {
            List<T> newList = new List<T>(); return newList;
        }

        public static void SetItemHeight(DependencyObject element, double itemHeight)
        {
            element.SetValue(ItemHeightProperty, itemHeight);
        }

        public static void SetItemWidth(DependencyObject element, double itemWidth)
        {
            element.SetValue(ItemWidthProperty, itemWidth);
        }

        public static void SetSameSizeItems(DependencyObject obj, bool value)
        {
            obj.SetValue(SameSizeItemsProperty, value);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (SameSizeItems)
            {
                if (!this.ItemWidth.IsRational() || this.ItemWidth == 0 || !finalSize.Width.IsRational() || !finalSize.Height.IsRational())
                    return finalSize;

                // Calculate how many children fit on each row
                int childrenPerRow = Math.Max(1, (int)Math.Floor(finalSize.Width / this.ItemWidth));
                if (childrenPerRow == 0)
                    return finalSize;

                bool animateNewItem = AnimatesNewItem && m_hasArranged;
                m_hasArranged = true;

                Size theChildSize = new Size(this.ItemWidth, this.ItemHeight);
                for (int i = 0; i < this.Children.Count; i++)
                {
                    UIElement child = this.Children[i];

                    // Figure out where the child goes
                    Point newOffset = calculateChildOffset(i, childrenPerRow,
                        this.ItemWidth, this.ItemHeight,
                        finalSize.Width, this.Children.Count);

                    AnimatingTilePanelData data = (AnimatingTilePanelData)child.GetValue(DataProperty);
                    if (data == null)
                    {
                        data = new AnimatingTilePanelData();
                        child.SetValue(DataProperty, data);
                    }

                    //set the location attached DP
                    data.ChildTarget = newOffset;

                    if (data.IsNew) // first time I've seen this...
                    {
                        data.IsNew = false;

                        if (animateNewItem)
                        {
                            //newOffset.X -= theChildSize.Width;
                            newOffset.Y = finalSize.Height;
                            newOffset.X = finalSize.Width / 2;
                        }

                        data.ChildLocation = newOffset;
                        child.Arrange(new Rect(newOffset.X, newOffset.Y, theChildSize.Width, theChildSize.Height));

                    }
                    else
                    {
                        Point currentOffset = data.ChildLocation;
                        // Position the child and set its size
                        child.Arrange(new Rect(currentOffset.X, currentOffset.Y, theChildSize.Width, theChildSize.Height));
                    }
                }
                return finalSize;
            }
            else
            {

                bool animateNewItem = AnimatesNewItem && m_hasArranged;
                m_hasArranged = true;

                List<UIElement> rowItems = new List<UIElement>();
                double rowHeight = 0;
                double left = 0;
                double top = 0;

                if (finalSize.Width == 0)
                    return finalSize;

                for (int i = 0; i < this.Children.Count; i++)
                {
                    UIElement child = this.Children[i];
                    Size theChildSize = child.DesiredSize;

                    // Figure out where the child goes
                    if (left > finalSize.Width)
                    {
                        arrangeRow(ref rowItems, ref rowHeight, ref left, animateNewItem, finalSize, top);
                        top += rowHeight;
                    }

                    rowHeight = Math.Max(theChildSize.Height, rowHeight);
                    left += theChildSize.Width;
                    rowItems.Add(child);
                }
                arrangeRow(ref rowItems, ref rowHeight, ref left, animateNewItem, finalSize, top);
                return finalSize;
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            onPreApplyTemplate();

            Size theChildSize;
            if (SameSizeItems)
                theChildSize = new Size(this.ItemWidth, this.ItemHeight);
            else
                theChildSize = new Size(double.PositiveInfinity, double.PositiveInfinity);

            //object foo = DependencyPropertyHelper.GetValueSource(this, ItemHeightProperty);

            foreach (UIElement child in Children)
            {
                child.Measure(theChildSize);
            }

            int childrenPerRow;

            // Figure out how many children fit on each row
            if (availableSize.Width == Double.PositiveInfinity)
            {
                childrenPerRow = this.Children.Count;
            }
            else
            {
                childrenPerRow = Math.Max(1, (int)Math.Floor(availableSize.Width / this.ItemWidth));
            }

            if (SameSizeItems)
            {
                // Calculate the width and height this results in
                double width = childrenPerRow * this.ItemWidth;
                double height = this.ItemHeight * (Math.Floor((double)this.Children.Count / childrenPerRow) + 1);
                height = (height.IsRational()) ? height : 0;
                return new Size(width, height);
            }
            return availableSize;
        }

        private static ItemsControl LookForItemsControl(FrameworkElement element)
        {
            if (element == null)
            {
                return null;
            }
            else if (element is ItemsControl)
            {
                return (ItemsControl)element;
            }
            else
            {
                FrameworkElement parent = VisualTreeHelper.GetParent(element) as FrameworkElement;
                return LookForItemsControl(parent);
            }
        }

        private static void arrangeRow(ref List<UIElement> rowItems, ref double rowHeight, ref double left, bool animateNewItem, Size finalSize, double top)
        {
            double tmpLeft = 0;
            foreach (var rowitem in rowItems)
            {
                Rect r = new Rect(tmpLeft, top, rowitem.DesiredSize.Width, rowHeight);
                FrameworkElement fe = rowitem as FrameworkElement;
                if (fe != null)
                {
                    switch (fe.VerticalAlignment)
                    {
                        case VerticalAlignment.Bottom:
                            r.Y = top + rowHeight - rowitem.DesiredSize.Height;
                            break;
                        case VerticalAlignment.Center:
                            r.Y = top + (rowHeight - rowitem.DesiredSize.Height) / 2;
                            break;
                    }
                }

                AnimatingTilePanelData data = (AnimatingTilePanelData)rowitem.GetValue(DataProperty);
                if (data == null)
                {
                    data = new AnimatingTilePanelData();
                    rowitem.SetValue(DataProperty, data);
                }

                Point newOffset = r.Location();
                //set the location attached DP
                data.ChildTarget = newOffset;

                if (data.IsNew) // first time I've seen this...
                {
                    data.IsNew = false;

                    if (animateNewItem)
                    {
                        //newOffset.X -= theChildSize.Width;
                        newOffset.Y = finalSize.Height;
                        newOffset.X = finalSize.Width / 2;
                    }

                    data.ChildLocation = newOffset;
                    rowitem.Arrange(r);

                }
                else
                {
                    Point currentOffset = data.ChildLocation;
                    // Position the child and set its size
                    rowitem.Arrange(new Rect(currentOffset.X, currentOffset.Y, r.Width, r.Height));
                }
                tmpLeft += rowitem.DesiredSize.Width;
            }

            rowItems = new List<UIElement>();
            rowHeight = 0;
            left = 0;
        }

        // Given a child index, child size and children per row, figure out where the child goes
        private static Point calculateChildOffset(
            int index,
            int childrenPerRow,
            double itemWidth,
            double itemHeight,
            double panelWidth,
            int totalChildren)
        {
            double fudge = 0;
            if (totalChildren > childrenPerRow)
            {
                fudge = (panelWidth - childrenPerRow * itemWidth) / childrenPerRow;
                Debug.Assert(fudge >= 0);
            }

            int row = index / childrenPerRow;
            int column = index % childrenPerRow;
            return new Point(.5 * fudge + column * (itemWidth + fudge), row * itemHeight);
        }

        private static bool updateElement(AnimatingTilePanelData data, double dampening, double attractionFactor, double variation)
        {
            Debug.Assert(dampening > 0 && dampening < 1);
            Debug.Assert(attractionFactor > 0 && !double.IsInfinity(attractionFactor));

            if (data == null)
                return false;

            attractionFactor *= 1 + (variation * data.Random - .5);

            Point newLocation;
            Vector newVelocity;

            bool anythingChanged =
                Animate(data.ChildLocation, data.Velocity, data.ChildTarget,
                    attractionFactor, dampening, c_terminalVelocity, c_diff, c_diff,
                    out newLocation, out newVelocity);

            data.ChildLocation = newLocation;
            data.Velocity = newVelocity;

            return anythingChanged;
        }

        void AnimatingTilePanel_Loaded(object sender, RoutedEventArgs e)
        {
            timer.Interval = TimeSpan.FromSeconds(1d / Application.Current.Host.Settings.MaxFrameRate);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        private void bindToParentItemsControl(DependencyProperty property, DependencyObject source, string path)
        {
            if (source == null)
                return;

            object val = source.GetValue(property);
            if (val != null)
            {
                Binding binding = new Binding(path);
                binding.Source = source;
                //binding.Path = new PropertyPath(property);
                base.SetBinding(property, binding);
            }
        }

        private void onPreApplyTemplate()
        {
            //if (!m_appliedTemplate)
            //{
            //    m_appliedTemplate = true;

            //    DependencyObject source = base.Parent;
            //    if (source is ItemsPresenter)
            //    {
            //        source = LookForItemsControl((ItemsPresenter)source);
            //    }

            //    if (source != null)
            //    {
            //        bindToParentItemsControl(ItemHeightProperty, source, "(AnimatingTilePanel.ItemHeight)");
            //        bindToParentItemsControl(ItemWidthProperty, source, "(AnimatingTilePanel.ItemWidth)");
            //    }
            //}
        }

        void timer_Tick(object sender, EventArgs e)
        {
            double dampening = this.Dampening;
            double attractionFactor = this.Attraction * .01;
            double variation = this.Variation;

            bool shouldChange = false;
            for (int i = 0; i < Children.Count; i++)
            {
                shouldChange = updateElement(
                    (AnimatingTilePanelData)Children[i].GetValue(DataProperty),
                    dampening,
                    attractionFactor,
                    variation) || shouldChange;
            }

            if (shouldChange)
            {
                InvalidateArrange();
            }
            else
            {
                //m_listener.StopListening();
            }
        }

        #endregion Methods

        #region Nested Types

        private class AnimatingTilePanelData
        {
            #region Fields

            public readonly double Random = s_random.NextDouble();

            public Point ChildLocation;
            public Point ChildTarget;
            public bool IsNew = true;
            public Vector Velocity = new Vector(0, 0);

            private static readonly Random s_random = new Random();

            #endregion Fields
        }

        #endregion Nested Types
    }
}