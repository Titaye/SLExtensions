//using J832.Common;
//using J832.Wpf;
namespace SLExtensions.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Threading;

    public class AnimatingFillPanel : Panel
    {
        #region Fields

        public static readonly DependencyProperty AnimatesNewItemProperty = 
            DependencyProperty.Register("AnimatesNewItem", typeof(bool),
                typeof(AnimatingFillPanel), null);
        public static readonly DependencyProperty AttractionProperty = 
        DependencyProperty.Register(
        "Attraction",
        typeof(double),
        typeof(AnimatingFillPanel),
        null);
        public static readonly DependencyProperty DampeningProperty = 
            DependencyProperty.Register(
                "Dampening",
                typeof(double),
                typeof(AnimatingFillPanel),
                null);

        //public static readonly DependencyProperty VariationProperty =
        //    CreateDoubleDP("Variation", 1, FrameworkPropertyMetadataOptions.None, 0, true, 1, true, false);
        public static readonly DependencyProperty VariationProperty = 
            DependencyProperty.Register(
                "Variation",
                typeof(double),
                typeof(AnimatingFillPanel),
                null);

        private static readonly DependencyProperty DataProperty = 
            DependencyProperty.RegisterAttached("Data", typeof(AnimatingFillPanelData), typeof(AnimatingFillPanel), null);

        // Using a DependencyProperty as the backing store for FillData.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty FillDataProperty = 
            DependencyProperty.RegisterAttached("FillData", typeof(Rect), typeof(AnimatingFillPanel), null);

        private const double c_diff = 0.1;
        private const double c_terminalVelocity = 10000;

        //private bool m_appliedTemplate;
        private bool m_hasArranged; // Used to make sure items are not arranged on the initial load
        private DispatcherTimer timer = new DispatcherTimer();

        #endregion Fields

        #region Constructors

        //private class DataRect
        //{
        //    public Rect? Rect1 { get; set; }
        //    public Rect? Rect2 { get; set; }
        //    public bool CheckCanContain(Size size)
        //    {
        //        if (Rect1 != null && Rect1.Value.Width >= size.Width && Rect1.Value.Height >= size.Height)
        //            return true;
        //        if (Rect2 != null && Rect2.Value.Width >= size.Width && Rect2.Value.Height >= size.Height)
        //            return true;
        //        return false;
        //    }
        //    public Rect? GetRect(Size size)
        //    {
        //        if (Rect1 != null && Rect1.Value.Width >= size.Width && Rect1.Value.Height >= size.Height)
        //            return Rect1.Value;
        //        if (Rect2 != null && Rect2.Value.Width >= size.Width && Rect2.Value.Height >= size.Height)
        //            return Rect2.Value;
        //        return null;
        //    }
        //    public void RemoveRect(Rect rect)
        //    {
        //        if (Rect1.HasValue && Rect1.Value == rect)
        //            Rect1 = null;
        //        else if (Rect2.HasValue && Rect2.Value == rect)
        //            Rect2 = null;
        //    }
        //}
        public AnimatingFillPanel()
        {
            Dampening = 0.2;
            Attraction = 2;
            this.Loaded += new RoutedEventHandler(AnimatingFillPanel_Loaded);
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

        public object StartElement
        {
            get; set;
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

        protected override Size ArrangeOverride(Size finalSize)
        {
            bool animateNewItem = AnimatesNewItem && m_hasArranged;
            m_hasArranged = true;

            //Size theChildSize = new Size(this.ItemWidth, this.ItemHeight);
            for (int i = 0; i < this.Children.Count; i++)
            {
                UIElement child = this.Children[i];

                // Figure out where the child goes
                Rect rect = GetFillData(child);
                Point newOffset = rect.Location();

                AnimatingFillPanelData data = (AnimatingFillPanelData)child.GetValue(DataProperty);
                if (data == null)
                {
                    data = new AnimatingFillPanelData();
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
                    child.Arrange(new Rect(newOffset.X, newOffset.Y, child.DesiredSize.Width, child.DesiredSize.Height));

                }
                else
                {
                    Point currentOffset = data.ChildLocation;
                    // Position the child and set its size
                    child.Arrange(new Rect(currentOffset.X, currentOffset.Y, child.DesiredSize.Width, child.DesiredSize.Height));
                }
            }
            return finalSize;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            //onPreApplyTemplate();

            Size theChildSize = new Size(availableSize.Width, double.PositiveInfinity);

            List<Rect> spaces = new List<Rect>();
            spaces.Add(new Rect(0, 0, availableSize.Width, double.PositiveInfinity));

            IEnumerable<UIElement> children = from c in Children
                                              orderby c == StartElement ? 0 : 1
                                              select c;
            foreach (var item in children)
            {
                item.Measure(theChildSize);
                Rect rect = (from r in spaces
                            where r.Width >= item.DesiredSize.Width
                                && r.Height >= item.DesiredSize.Height
                            orderby r.Top, r.Left
                             select r).FirstOrDefault();

                spaces.Remove(rect);

                SetFillData(item, rect);
                Rect newRect;

                newRect = new Rect(rect.Left, rect.Top + item.DesiredSize.Height, rect.Width, Math.Max(0, rect.Height - item.DesiredSize.Height));
                if (!newRect.IsEmpty)
                    spaces.Add(newRect);

                newRect = new Rect(rect.Left + item.DesiredSize.Width, rect.Top, Math.Max(0, rect.Width - item.DesiredSize.Width), item.DesiredSize.Height);
                if (!newRect.IsEmpty)
                    spaces.Add(newRect);

            }

            return availableSize;
        }

        private static Rect GetFillData(DependencyObject obj)
        {
            return (Rect)obj.GetValue(FillDataProperty);
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

        private static void SetFillData(DependencyObject obj, Rect value)
        {
            obj.SetValue(FillDataProperty, value);
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

        private static bool updateElement(AnimatingFillPanelData data, double dampening, double attractionFactor, double variation)
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

        void AnimatingFillPanel_Loaded(object sender, RoutedEventArgs e)
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
            //        bindToParentItemsControl(ItemHeightProperty, source, "(AnimatingFillPanel.ItemHeight)");
            //        bindToParentItemsControl(ItemWidthProperty, source, "(AnimatingFillPanel.ItemWidth)");
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
                    (AnimatingFillPanelData)Children[i].GetValue(DataProperty),
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

        private class AnimatingFillPanelData
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