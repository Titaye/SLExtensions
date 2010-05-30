// <copyright file="VirtualizedStackPanel.cs" company="Ucaya">
// Distributed under Microsoft Public License (Ms-PL)
// </copyright>
// <author>Thierry Bouquain</author>
namespace SLExtensions.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Linq;
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
    /// Arrange bound items in a vertical or horizontal stack. Templates are virtualized.
    /// </summary>
    public class VirtualizedStackPanel : Panel, IDisposable
    {

        #region Static Fields
        /// <summary>
        /// ItemsSource dependency property
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                "ItemsSource",
                typeof(IEnumerable),
                typeof(VirtualizedStackPanel),
                new PropertyMetadata((d, e) => ((VirtualizedStackPanel)d).OnItemsSourceChanged((IEnumerable)e.OldValue, (IEnumerable)e.NewValue)));

        /// <summary>
        /// ItemTemplate depdency property
        /// </summary>
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register(
                "ItemTemplate",
                typeof(DataTemplate),
                typeof(VirtualizedStackPanel),
                new PropertyMetadata((d, e) => ((VirtualizedStackPanel)d).OnItemTemplateChanged((DataTemplate)e.OldValue, (DataTemplate)e.NewValue)));

        /// <summary>
        /// CurrentScrollIndex  depdency property
        /// </summary>
        public static readonly DependencyProperty CurrentScrollIndexProperty =
            DependencyProperty.Register(
                "CurrentScrollIndex",
                typeof(double),
                typeof(VirtualizedStackPanel),
                new PropertyMetadata((d, e) => ((VirtualizedStackPanel)d).OnCurrentScrollIndexChanged((double)e.OldValue, (double)e.NewValue)));

        /// <summary>
        /// CanNavigatePrevious  depdency property
        /// </summary>
        public static readonly DependencyProperty CanNavigatePreviousProperty =
            DependencyProperty.Register(
                "CanNavigatePrevious",
                typeof(bool),
                typeof(VirtualizedStackPanel),
                new PropertyMetadata((d, e) => ((VirtualizedStackPanel)d).OnCanNavigatePreviousChanged((bool)e.OldValue, (bool)e.NewValue)));

        /// <summary>
        /// CanNavigateNext depdency property
        /// </summary>
        public static readonly DependencyProperty CanNavigateNextProperty =
            DependencyProperty.Register(
                "CanNavigateNext",
                typeof(bool),
                typeof(VirtualizedStackPanel),
                new PropertyMetadata((d, e) => ((VirtualizedStackPanel)d).OnCanNavigateNextChanged((bool)e.OldValue, (bool)e.NewValue)));

        /// <summary>
        /// Orientation  depdency property
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(
                "Orientation",
                typeof(Orientation),
                typeof(VirtualizedStackPanel),
                new PropertyMetadata((d, e) => ((VirtualizedStackPanel)d).OnOrientationChanged((Orientation)e.OldValue, (Orientation)e.NewValue)));

        /// <summary>
        /// PageCount depedency property
        /// </summary>
        public static readonly DependencyProperty ItemsCountProperty =
            DependencyProperty.Register(
                "ItemsCount",
                typeof(int),
                typeof(VirtualizedStackPanel),
                new PropertyMetadata((d, e) => { ((VirtualizedStackPanel)d).OnItemsCountChanged((int)e.OldValue, (int)e.NewValue); }));

        #endregion Static Fields

        #region Fields

        /// <summary>
        /// Contains visible children by source index
        /// </summary>
        private Dictionary<int, FrameworkElement> childrenByIndex = new Dictionary<int, FrameworkElement>();

        /// <summary>
        /// ItemsSource in IList
        /// </summary>
        private IList source;

        /// <summary>
        /// Last scroll index displayed
        /// </summary>
        private double? lastEnsuredScrollIndex = null;

        /// <summary>
        /// Store index of instanciated template from itemssource
        /// </summary>
        private Dictionary<UIElement, int> indexes = new Dictionary<UIElement, int>();

        /// <summary>
        /// Store absolute position of instanciated template
        /// </summary>
        private Dictionary<UIElement, Rect> positions = new Dictionary<UIElement, Rect>();

        /// <summary>
        /// Clipping geometry
        /// </summary>
        private RectangleGeometry clipGeometry;

        private Size? _templateDesiredSize;

        /// <summary>
        /// template desired size
        /// </summary>
        private Size? templateDesiredSize
        {
            get { return _templateDesiredSize; }
            set
            {
                if (_templateDesiredSize != value)
                {
                    _templateDesiredSize = value;
                    OnTemplateDesiredSizeChanged();
                }
            }
        }

        public Size? TemplateDesiredSize { get { return templateDesiredSize; } }

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualizedStackPanel"/> class.
        /// </summary>
        public VirtualizedStackPanel()
        {
            Orientation = Orientation.Horizontal;
            this.SizeChanged += this.VirtualizedStackPanel_SizeChanged;
        }

        #endregion Constructors

        /// <summary>
        /// Occurs when page count changed.
        /// </summary>
        public event EventHandler ItemsCountChanged;

        #region Public Properties

        /// <summary>
        /// Gets or sets the number of page necessary to display the content
        /// </summary>
        /// <value>The page count.</value>
        public int ItemsCount
        {
            get
            {
                return (int)GetValue(ItemsCountProperty);
            }

            set
            {
                SetValue(ItemsCountProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance can navigate next.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can navigate next; otherwise, <c>false</c>.
        /// </value>
        public bool CanNavigateNext
        {
            get
            {
                return (bool)GetValue(CanNavigateNextProperty);
            }

            private set
            {
                SetValue(CanNavigateNextProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance can navigate previous.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can navigate previous; otherwise, <c>false</c>.
        /// </value>
        public bool CanNavigatePrevious
        {
            get
            {
                return (bool)GetValue(CanNavigatePreviousProperty);
            }

            private set
            {
                SetValue(CanNavigatePreviousProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the crrent scroll index. A step of 1 means one page
        /// </summary>
        /// <value>The index of the current scroll.</value>
        public double CurrentScrollIndex
        {
            get
            {
                return (double)GetValue(CurrentScrollIndexProperty);
            }

            set
            {
                SetValue(CurrentScrollIndexProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the item template.
        /// </summary>
        /// <value>The item template.</value>
        public DataTemplate ItemTemplate
        {
            get
            {
                return (DataTemplate)GetValue(ItemTemplateProperty);
            }

            set
            {
                SetValue(ItemTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the items source.
        /// </summary>
        /// <value>The items source.</value>
        public IEnumerable ItemsSource
        {
            get
            {
                return (IEnumerable)GetValue(ItemsSourceProperty);
            }

            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the orientation.
        /// </summary>
        /// <value>The orientation.</value>
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


        #region ItemContainerGenerator

        public IItemContainerGenerator ItemContainerGenerator
        {
            get
            {
                return (IItemContainerGenerator)GetValue(ItemContainerGeneratorProperty);
            }

            set
            {
                SetValue(ItemContainerGeneratorProperty, value);
            }
        }

        /// <summary>
        /// ItemContainerGenerator depedency property.
        /// </summary>
        public static readonly DependencyProperty ItemContainerGeneratorProperty =
            DependencyProperty.Register(
                "ItemContainerGenerator",
                typeof(IItemContainerGenerator),
                typeof(VirtualizedStackPanel),
                new PropertyMetadata((d, e) => ((VirtualizedStackPanel)d).OnItemContainerGeneratorChanged((IItemContainerGenerator)e.OldValue, (IItemContainerGenerator)e.NewValue)));

        /// <summary>
        /// handles the ItemContainerGeneratorProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnItemContainerGeneratorChanged(IItemContainerGenerator oldValue, IItemContainerGenerator newValue)
        {

        }

        #endregion ItemContainerGenerator


        #endregion Public Properties

        /// <summary>
        /// Called when page count changed.
        /// </summary>
        protected virtual void OnItemsCountChanged()
        {
            if (this.ItemsCountChanged != null)
            {
                this.ItemsCountChanged(this, EventArgs.Empty);
            }
        }

        #region Protected Methods

        /// <summary>
        /// Provides the behavior for the "Arrange" pass of Silverlight layout. Classes can override this method to define their own arrange pass behavior.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (this.ItemTemplate == null
                || this.ItemsSource == null)
            {
                return finalSize;
            }
            

            // Clip content
            if (this.IsAutoClipContent)
            {
                if (this.clipGeometry == null)
                {
                    this.clipGeometry = new RectangleGeometry();
                    this.Clip = this.clipGeometry;
                }

                Rect r = new Rect(0, 0, finalSize.Width, finalSize.Height);

                if (this.MarginAutoClipContent != null)
                {
                    r.X -= this.MarginAutoClipContent.Left;
                    r.Y -= this.MarginAutoClipContent.Top;
                    r.Width += this.MarginAutoClipContent.Left + this.MarginAutoClipContent.Right;
                    r.Height += this.MarginAutoClipContent.Top + this.MarginAutoClipContent.Bottom;
                }

                //this.clipGeometry.Rect = new Rect(0, 0, finalSize.Width, finalSize.Height);
                this.clipGeometry.Rect = r;
            }

            if (this.Orientation == Orientation.Horizontal)
            {
                this.ArrangeHorizontal(finalSize);
            }
            else
            {
                this.ArrangeVertical(finalSize);
            }

            return finalSize;
        }

        #endregion Protected Methods

        #region Private Methods

        /// <summary>
        /// Intersects the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <returns>The computed intersection</returns>
        private static Rect Intersect(Rect source, Rect target)
        {
            source.Intersect(target);
            return source;
        }

        /// <summary>
        /// Handles the SizeChanged event of the VirtualizedStackPanel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.SizeChangedEventArgs"/> instance containing the event data.</param>
        private void VirtualizedStackPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.templateDesiredSize = null;
            this.lastEnsuredScrollIndex = null;

            RecreateVisualTree();
        }

        /// <summary>
        /// Arranges items horizontaly.
        /// </summary>
        /// <param name="finalSize">The final size.</param>
        private void ArrangeHorizontal(Size finalSize)
        {
            //ROMU: bug if this.templateDesiredSize.HasValue = false and this.source.Count == 0
            if (this.source.Count == 0)
                return;

            if (this.source.Count != 0 && !this.templateDesiredSize.HasValue)
            {
                finalSize = ComputeTemplateDesiredSize(finalSize);
            }

            this.ItemsCount = this.source.Count;

            this.EnsureItems();

            double left = this.templateDesiredSize.Value.Width * this.CurrentScrollIndex;

            foreach (var item in this.childrenByIndex)
            {
                Rect pos = this.positions[item.Value];
                if (item.Value.VerticalAlignment == VerticalAlignment.Stretch)
                {
                    pos.Height = finalSize.Height;
                }

                pos.X -= left;
                item.Value.Arrange(pos);
            }

            if (this.childrenByIndex.Count > 0)
            {
                int maxIdx = this.childrenByIndex.Keys.Max();
                this.CanNavigatePrevious = this.CurrentScrollIndex > 0;
                this.CanNavigateNext = maxIdx < this.ItemsCount - 1;
            }
            else
            {
                this.CanNavigatePrevious = false;
                this.CanNavigateNext = false;
            }
        }

        /// <summary>
        /// Arranges items vertically
        /// </summary>
        /// <param name="finalSize">The final size.</param>
        private void ArrangeVertical(Size finalSize)
        {
            if (this.source.Count != 0 && !this.templateDesiredSize.HasValue)
            {
                finalSize = ComputeTemplateDesiredSize(finalSize);
            }

            this.ItemsCount = this.source.Count;

            this.EnsureItems();

            double top = this.templateDesiredSize.Value.Height * this.CurrentScrollIndex;

            foreach (var item in this.childrenByIndex)
            {
                Rect pos = this.positions[item.Value];
                if (item.Value.HorizontalAlignment == HorizontalAlignment.Stretch)
                {
                    pos.Width = finalSize.Width;
                }
                pos.Y -= top;
                item.Value.Arrange(pos);
            }

            if (this.childrenByIndex.Count > 0)
            {
                int maxIdx = this.childrenByIndex.Keys.Max();
                this.CanNavigatePrevious = this.CurrentScrollIndex > 0;
                this.CanNavigateNext = maxIdx < this.ItemsCount - 1;
            }
            else
            {
                this.CanNavigatePrevious = false;
                this.CanNavigateNext = false;
            }
        }

        private Size ComputeTemplateDesiredSize(Size finalSize)
        {
            FrameworkElement elem = this.ItemTemplate.LoadContent() as FrameworkElement;
            elem.DataContext = this.source[0];
            double opacity = elem.Opacity;
            elem.Opacity = 0;
            Children.Add(elem);
            elem.Measure(finalSize);
            this.templateDesiredSize = elem.DesiredSize;
            Children.Remove(elem);
            elem.Opacity = opacity;
            if (IsCacheEnabled)
                elementCache.Enqueue(elem);
            return finalSize;
        }

        /// <summary>
        /// Handles the CollectionChanged event of the collectionChanged control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void CollectionChanged_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            List<object> list = new List<object>();
            if (ItemsSource != null)
            {
                foreach (var item in ItemsSource)
                {
                    list.Add(item);
                }
            }
            this.source = list;

            this.RecreateVisualTree();
        }

        /// <summary>
        /// Reset all visual items
        /// </summary>
        private void RecreateVisualTree()
        {
            foreach (var item in this.Children)
            {
                if (IsCacheEnabled)
                    this.elementCache.Enqueue((FrameworkElement)item);
            }

            this.Children.Clear();
            this.indexes.Clear();
            this.positions.Clear();
            this.childrenByIndex.Clear();
            this.CurrentScrollIndex = 0;
            this.templateDesiredSize = null;
            this.lastEnsuredScrollIndex = null;

            foreach (var item in this.elementCache)
            {
                item.DisposeRecursive();
            }

            this.elementCache.Clear();

            if (this.ItemTemplate == null
                || this.ItemsSource == null)
            {
                return;
            }

            this.EnsureItems();
            InvalidateArrange();
        }

        /// <summary>
        /// Called when CanNavigateNext changed.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnCanNavigateNextChanged(bool oldValue, bool newValue)
        {
        }

        /// <summary>
        /// Called when CanNavigatePrevious changed.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnCanNavigatePreviousChanged(bool oldValue, bool newValue)
        {
        }

        /// <summary>
        /// Arranges items vertically
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnCurrentScrollIndexChanged(double oldValue, double newValue)
        {
            this.EnsureItems();
            InvalidateArrange();
        }

        /// <summary>
        /// Ensures the child items.
        /// </summary>
        private void EnsureItems()
        {
            if (this.templateDesiredSize == null
                || (this.lastEnsuredScrollIndex.HasValue && this.lastEnsuredScrollIndex.Value == this.CurrentScrollIndex))
            {
                return;
            }

            this.lastEnsuredScrollIndex = this.CurrentScrollIndex;

            double firstChild = this.GetFirstVisibleChild();
            double lastChild = this.GetLastVisibleChild();
            int firstChildIdx = (int)Math.Floor(firstChild);
            int lastChildIdx = Math.Min((int)Math.Ceiling(lastChild), ItemsCount);
            if (firstChildIdx == lastChildIdx)
                lastChildIdx++;

            IList<int> idxToRemove = (from k in childrenByIndex.Keys
                                      where k < firstChildIdx || k >= lastChildIdx
                                      select k).ToList();

            foreach (int idx in idxToRemove)
            {
                this.RemoveChildren(idx);
            }

            List<int> idxToCreate = new List<int>();
            for (int i = firstChildIdx; i < lastChildIdx; i++)
            {
                if (!this.childrenByIndex.ContainsKey(i))
                {
                    idxToCreate.Add(i);
                }
            }

            foreach (var idx in idxToCreate)
            {
                this.CreateChildren(idx);
            }
        }



        private static bool GetIsItemContainer(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsItemContainerProperty);
        }

        private static void SetIsItemContainer(DependencyObject obj, bool value)
        {
            obj.SetValue(IsItemContainerProperty, value);
        }

        public static readonly DependencyProperty IsItemContainerProperty =
            DependencyProperty.RegisterAttached("IsItemContainer", typeof(bool), typeof(VirtualizedStackPanel), null);


        private Queue<FrameworkElement> elementCache = new Queue<FrameworkElement>();

        /// <summary>
        /// Creates the children.
        /// </summary>
        /// <param name="idx">The index in ItemsSource.</param>
        private void CreateChildren(int idx)
        {
            if (idx < 0 || idx >= this.source.Count)
            {
                return;
            }
            object datacontext = this.source[idx];

            FrameworkElement element;
            if (elementCache.Count > 0)
            {
                element = elementCache.Dequeue();

                element.DataContext = datacontext;
            }
            else
            {
                element = this.ItemTemplate.LoadContent() as FrameworkElement;

                if (ItemContainerGenerator != null)
                {
                    if (!ItemContainerGenerator.IsItemItsOwnContainerOverride(element))
                    {
                        element = (FrameworkElement)ItemContainerGenerator.GetContainerForItemOverride();
                        element.DataContext = datacontext;
                        SetIsItemContainer(element, true);
                        ItemContainerGenerator.PrepareContainerForItemOverride(element, datacontext);
                    }
                }

                if (element.DataContext == null)
                {
                    element.DataContext = datacontext;
                }
            }

            this.indexes[element] = idx;
            this.childrenByIndex[idx] = element;

            // Add the control before measuring
            this.Children.Insert(0, element);
            //this.Children.Add(element);

            Size size = new Size(ActualWidth, ActualHeight);
            element.Measure(size);

            Size elementSize = element.DesiredSize;

            if (Orientation == Orientation.Horizontal
                && element.VerticalAlignment == VerticalAlignment.Stretch)
            {
                elementSize.Height = size.Height;
            }

            if (Orientation == Orientation.Vertical
                && element.HorizontalAlignment == HorizontalAlignment.Stretch)
            {
                elementSize.Width = size.Width;
            }

            Rect elementRect;

            if (Orientation == Orientation.Horizontal)
            {
                elementRect = new Rect(idx * this.templateDesiredSize.Value.Width, 0, elementSize.Width, elementSize.Height);
            }
            else
            {
                elementRect = new Rect(0, idx * this.templateDesiredSize.Value.Height, elementSize.Width, elementSize.Height);
            }

            this.positions[element] = elementRect;
        }

        /// <summary>
        /// Reset all visual items
        /// </summary>
        /// <param name="idx">The source index.</param>
        private void RemoveChildren(int idx)
        {
            FrameworkElement elem;

            if (!this.childrenByIndex.TryGetValue(idx, out elem))
            {
                return;
            }

            if (ItemContainerGenerator != null && GetIsItemContainer(elem))
            {
                ItemContainerGenerator.ClearContainerForItemOverride(elem, source[idx]);
            }

            this.childrenByIndex.Remove(idx);
            this.indexes.Remove(elem);
            this.positions.Remove(elem);
            this.Children.Remove(elem);

            FrameworkElement fe = elem as FrameworkElement;
            if (fe != null)
            {
                // Release datacontext to prevent memory leaks
                fe.DataContext = null;
                if (IsCacheEnabled)
                    elementCache.Enqueue(fe);
            }
        }

        /// <summary>
        /// Called when CanNavigateNext changed.
        /// </summary>
        /// <returns>returns the itemssource index</returns>
        private double GetFirstVisibleChild()
        {
            if (!this.lastEnsuredScrollIndex.HasValue
                || !this.templateDesiredSize.HasValue)
            {
                return -1;
            }

            return this.lastEnsuredScrollIndex.Value;
        }

        /// <summary>
        /// Called when CanNavigatePrevious changed.
        /// </summary>
        /// <returns>returns the itemssource index</returns>
        private double GetLastVisibleChild()
        {
            if (!this.lastEnsuredScrollIndex.HasValue
                || !this.templateDesiredSize.HasValue)
            {
                return -1;
            }

            if (this.templateDesiredSize.Value.Height == 0)
            {
                return 0;
            }

            if (this.Orientation == Orientation.Vertical)
                return this.GetFirstVisibleChild() + this.ActualHeight / this.templateDesiredSize.Value.Height;
            else
                return this.GetFirstVisibleChild() + this.ActualWidth / this.templateDesiredSize.Value.Width;
        }

        /// <summary>
        /// Called when ItemTempalte changed.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnItemTemplateChanged(DataTemplate oldValue, DataTemplate newValue)
        {
            this.templateDesiredSize = null;
            this.RecreateVisualTree();
        }

        /// <summary>
        /// Called when ItemsSource changed.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            INotifyCollectionChanged collectionChanged = oldValue as INotifyCollectionChanged;
            if (collectionChanged != null)
            {
                collectionChanged.CollectionChanged -= this.CollectionChanged_CollectionChanged;
            }

            List<object> list = new List<object>();
            if (newValue != null)
            {
                foreach (var item in newValue)
                {
                    list.Add(item);
                }
            }

            this.source = list;

            this.RecreateVisualTree();
            collectionChanged = newValue as INotifyCollectionChanged;
            if (collectionChanged != null)
            {
                collectionChanged.CollectionChanged += this.CollectionChanged_CollectionChanged;
            }
        }

        /// <summary>
        /// Called when PageCount changed
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnItemsCountChanged(int oldValue, int newValue)
        {
            this.OnItemsCountChanged();
        }

        /// <summary>
        /// Called when Orientation changed.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnOrientationChanged(Orientation oldValue, Orientation newValue)
        {
            InvalidateArrange();
        }

        #endregion Private Methods

        #region IDisposable Members

        public void Dispose()
        {

            foreach (var item in this.Children)
            {
                item.DisposeRecursive();
            }

            foreach (var item in elementCache)
            {
                item.DisposeRecursive();
            }
        }

        #endregion

        #region IsCacheEnabled

        public bool IsCacheEnabled
        {
            get
            {
                return (bool)GetValue(IsCacheEnabledProperty);
            }

            set
            {
                SetValue(IsCacheEnabledProperty, value);
            }
        }

        /// <summary>
        /// IsCacheEnabled depedency property.
        /// </summary>
        public static readonly DependencyProperty IsCacheEnabledProperty =
            DependencyProperty.Register(
                "IsCacheEnabled",
                typeof(bool),
                typeof(VirtualizedStackPanel),
                new PropertyMetadata((d, e) => ((VirtualizedStackPanel)d).OnIsCacheEnabledChanged((bool)e.OldValue, (bool)e.NewValue)));

        /// <summary>
        /// handles the IsCacheEnabledProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnIsCacheEnabledChanged(bool oldValue, bool newValue)
        {
            RecreateVisualTree();
        }

        #endregion IsCacheEnabled

        #region IsAutoClipContent

        public bool IsAutoClipContent
        {
            get
            {
                return (bool)GetValue(IsAutoClipContentProperty);
            }

            set
            {
                SetValue(IsAutoClipContentProperty, value);
            }
        }

        /// <summary>
        /// IsAutoClipContent depedency property.
        /// </summary>
        public static readonly DependencyProperty IsAutoClipContentProperty =
            DependencyProperty.Register(
                "IsAutoClipContent",
                typeof(bool),
                typeof(VirtualizedStackPanel),
                new PropertyMetadata(true, (d, e) => ((VirtualizedStackPanel)d).OnIsAutoClipContentChanged((bool)e.OldValue, (bool)e.NewValue)));

        /// <summary>
        /// handles the IsAutoClipContentProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnIsAutoClipContentChanged(bool oldValue, bool newValue)
        {
            this.clipGeometry = null;
            InvalidateArrange();
        }

        #endregion IsAutoClipContent


        #region MarginAutoClipContent

        public Thickness MarginAutoClipContent
        {
            get
            {
                return (Thickness)GetValue(MarginAutoClipContentProperty);
            }

            set
            {
                SetValue(MarginAutoClipContentProperty, value);
            }
        }

        /// <summary>
        /// MarginAutoClipContent depedency property.
        /// </summary>
        public static readonly DependencyProperty MarginAutoClipContentProperty =
            DependencyProperty.Register(
                "MarginAutoClipContent",
                typeof(Thickness),
                typeof(VirtualizedStackPanel),
                new PropertyMetadata((d, e) => ((VirtualizedStackPanel)d).OnMarginAutoClipContentChanged((Thickness)e.OldValue, (Thickness)e.NewValue)));

        /// <summary>
        /// handles the MarginAutoClipContentProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnMarginAutoClipContentChanged(Thickness oldValue, Thickness newValue)
        {
            InvalidateArrange();
        }

        #endregion MarginAutoClipContent


        public event EventHandler TemplateDesiredSizeChanged;

        protected virtual void OnTemplateDesiredSizeChanged()
        {
            if (TemplateDesiredSizeChanged != null)
            {
                TemplateDesiredSizeChanged(this, EventArgs.Empty);
            }
        }


    }
}
