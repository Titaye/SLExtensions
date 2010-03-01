// <copyright file="TreeView.cs" company="Ucaya">
// Distributed under Microsoft Public License (Ms-PL)
// </copyright>
// <author>Thierry Bouquain</author>
namespace SLExtensions.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    using SLExtensions.Data;

    /// <summary>
    /// Treeview control
    /// </summary>
    [TemplatePart(Name = TreeView.ElementScrollViewerName, Type = typeof(ScrollViewer))]
    public class TreeView : ItemsControl
    {
        #region Fields

        /// <summary>
        /// Dependency property for selecting on expand
        /// </summary>
        public static readonly DependencyProperty SelectOnExpandChangeProperty = 
            DependencyProperty.Register(
                "SelectOnExpandChange",
                typeof(bool),
                typeof(TreeView),
                new PropertyMetadata((d, e) => ((TreeView)d).OnSelectOnExpandChangeChanged((bool)e.OldValue, (bool)e.NewValue)));

        /// <summary>
        /// Dependency property for the selected item
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty = 
            DependencyProperty.Register(
                "SelectedItem",
                typeof(object),
                typeof(TreeView),
                new PropertyMetadata((d, e) => ((TreeView)d).OnSelectedItemChanged((object)e.OldValue, (object)e.NewValue)));

        /// <summary>
        /// Dependency property for the selected node
        /// </summary>
        public static readonly DependencyProperty SelectedNodeProperty = 
            DependencyProperty.Register(
                "SelectedNode",
                typeof(TreeViewItem),
                typeof(TreeView),
                new PropertyMetadata((d, e) => ((TreeView)d).OnSelectedNodeChanged((TreeViewItem)e.OldValue, (TreeViewItem)e.NewValue)));

        /// <summary>
        /// Dependency property for the template selector
        /// </summary>
        public static readonly DependencyProperty TemplateSelectorProperty = 
            DependencyProperty.Register(
                "TemplateSelector",
                typeof(DataTemplateSelector),
                typeof(TreeView),
                null);

        /// <summary>
        /// Scroll viewer default template name
        /// </summary>
        private const string ElementScrollViewerName = "ScrollViewer";

        /// <summary>
        /// template scroll viewer
        /// </summary>
        private ScrollViewer elementScrollViewer;

        /// <summary>
        /// tells if we are in selection change processing 
        /// </summary>
        private bool isselectionChangeActive;

        /// <summary>
        /// Store temp container created in IsItemItsOwnContainerOverride for reuse in CreateContainerForItemOverride
        /// </summary>
        private TreeViewItem lastContainerCheck;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeView"/> class.
        /// </summary>
        public TreeView()
        {
            this.TabNavigation = KeyboardNavigationMode.Once;
            this.DefaultStyleKey = typeof(TreeView);
            this.Nodes = new List<TreeViewItem>();
            this.IsTabStop = false;
            this.SelectOnExpandChange = false;
        }

        #endregion Constructors

        #region Events

        public event EventHandler SelectionChanged;

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets the child node list.
        /// </summary>
        /// <value>The child nodes.</value>
        public IList<TreeViewItem> Nodes
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether we are selecting the nodes when the node is expanding or collapsing
        /// </summary>
        /// <value>
        /// 	<c>true</c> if select on expanding or collapsing otherwise, <c>false</c>.
        /// </value>
        public bool SelectOnExpandChange
        {
            get
            {
                return (bool)GetValue(SelectOnExpandChangeProperty);
            }

            set
            {
                SetValue(SelectOnExpandChangeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        /// <value>The selected item.</value>
        public object SelectedItem
        {
            get
            {
                return (object)GetValue(SelectedItemProperty);
            }

            set
            {
                SetValue(SelectedItemProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the selected node.
        /// </summary>
        /// <value>The selected node.</value>
        public TreeViewItem SelectedNode
        {
            get
            {
                return (TreeViewItem)GetValue(SelectedNodeProperty);
            }

            set
            {
                SetValue(SelectedNodeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a template selector for programmaticaly chose the template to apply.
        /// </summary>
        /// <value>The template selector.</value>
        public DataTemplateSelector TemplateSelector
        {
            get
            {
                return (DataTemplateSelector)GetValue(TemplateSelectorProperty);
            }

            set
            {
                SetValue(TemplateSelectorProperty, value);
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.elementScrollViewer = this.GetTemplateChild(TreeView.ElementScrollViewerName) as ScrollViewer;
        }

        /// <summary>
        /// Scrolls the treeview to display the given item
        /// </summary>
        /// <param name="item">The item to display.</param>
        public void ScrollIntoView(TreeViewItem item)
        {
            if ((null != this.elementScrollViewer))
            {
                Rect itemsHostRect;
                Rect listBoxItemRect;
                if (!this.IsOnCurrentPage(item, out itemsHostRect, out listBoxItemRect))
                {
                    // Scroll into view vertically (first make the right bound visible, then the left)
                    double verticalOffset = this.elementScrollViewer.VerticalOffset;
                    double verticalDelta = 0;
                    if (itemsHostRect.Bottom < listBoxItemRect.Bottom)
                    {
                        verticalDelta = listBoxItemRect.Bottom - itemsHostRect.Bottom;
                        verticalOffset += verticalDelta;
                    }

                    if (listBoxItemRect.Top - verticalDelta < itemsHostRect.Top)
                    {
                        verticalOffset -= itemsHostRect.Top - (listBoxItemRect.Top - verticalDelta);
                    }

                    this.elementScrollViewer.ScrollToVerticalOffset(verticalOffset);
                }
            }
        }

        /// <summary>
        /// Change the current TreeView SelectedItem and SelectedNode
        /// </summary>
        /// <param name="container">the treeview </param>
        /// <param name="selected">new selection value for the given container</param>
        internal void ChangeSelection(TreeViewItem container, bool selected)
        {
            if (!this.isselectionChangeActive)
            {
                this.isselectionChangeActive = true;

                try
                {
                    if (!selected)
                    {
                        if (container == this.SelectedNode)
                        {
                            this.SelectedNode = null;
                            this.SelectedItem = null;
                        }
                    }

                    if (container.IsSelected != selected)
                    {
                        container.IsSelected = selected;
                    }

                    if (selected)
                    {
                        if (this.SelectedNode != null && this.SelectedNode != container)
                        {
                            this.SelectedNode.IsSelected = false;
                        }

                        this.SelectedNode = container;
                        this.SelectedItem = container.DataContext ?? container;
                    }
                }
                finally
                {
                    this.isselectionChangeActive = false;
                }
            }
        }

        /// <summary>
        /// Ensures the node selection.
        /// </summary>
        internal void EnsureNodeSelection()
        {
            object selectedItem = this.SelectedItem;

            if (selectedItem == null)
            {
                if (this.SelectedNode != null)
                {
                    this.SelectedNode.IsSelected = false;
                }
            }
            else if (this.SelectedNode == null || this.SelectedNode.DataContext != selectedItem)
            {
                TreeViewItem node = this.FindChildNode(selectedItem);
                if (node != null)
                {
                    node.IsSelected = true;
                    TreeViewItem parentNode = node.ParentNode;
                    while (parentNode != null)
                    {
                        parentNode.IsExpanded = true;
                        parentNode = parentNode.ParentNode;
                    }

                    this.ScrollIntoView(node);
                }
            }
        }

        /// <summary>
        /// When overridden in a derived class, undoes the effects of the <see cref="M:System.Windows.Controls.ItemsControl.PrepareContainerForItemOverride(System.Windows.DependencyObject,System.Object)"/> method.
        /// </summary>
        /// <param name="element">The container element.</param>
        /// <param name="item">The Item to check.</param>
        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            this.Nodes.Remove(item as TreeViewItem);
            base.ClearContainerForItemOverride(element, item);
        }

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>the item container</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            if (this.lastContainerCheck != null)
            {
                return this.lastContainerCheck;
            }

            return new TreeViewItem();
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own container.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>
        /// true if the item is (or is eligible to be) its own container; otherwise, false.
        /// </returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            this.lastContainerCheck = null;
            if (item is TreeViewItem)
            {
                return true;
            }

            DataTemplate template = this.ItemTemplate;
            if (this.TemplateSelector != null)
            {
                template = this.TemplateSelector.SelectTemplate(item, new TreeViewItem());
            }

            if (template != null)
            {
                DependencyObject container = template.LoadContent();
                if (container is TreeViewItem)
                {
                    this.lastContainerCheck = (TreeViewItem)container;
                }
            }

            return false;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (this.SelectedItem == null && this.Items.Count > 0)
            {
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            TreeViewItem item = GetTreeViewItem(e.OriginalSource);
            if (item != null)
            {
                item.IsSelected = true;
            }
        }

        protected virtual void OnSelectionChanged()
        {
            if (SelectionChanged != null)
            {
                SelectionChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Prepares the specified element to display the specified item.
        /// </summary>
        /// <param name="element">Element used to display the specified item.</param>
        /// <param name="item">Specified item.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            TreeViewItem tvitem = element as TreeViewItem;
            tvitem.ParentTreeview = this;
            this.Nodes.Add(tvitem);

            if (tvitem.TemplateSelector == null)
            {
                tvitem.TemplateSelector = this.TemplateSelector;
            }

            if (tvitem.ItemTemplate == null)
            {
                tvitem.ItemTemplate = this.ItemTemplate;
            }

            if (this.lastContainerCheck != null)
            {
                return;
            }

            DataTemplate template = this.ItemTemplate;
            if (this.TemplateSelector != null)
            {
                template = this.TemplateSelector.SelectTemplate(item, element);
            }

            bool setContent = true;
            if (tvitem != item)
            {
                // If not a treeviewitem, propagate the TreeView's ItemTemplate
                if (null != template)
                {
                    // ItemsControl owns recreating containers if ItemTemplate ever changes
                    tvitem.HeaderTemplate = template;
                }
                else if (!string.IsNullOrEmpty(this.DisplayMemberPath))
                {
                    // Create a binding for displaying the DisplayMemberPath (which always renders as a string)
                    Binding binding = new Binding(this.DisplayMemberPath);
                    binding.Converter = new DisplayMemberValueConverter();
                    tvitem.SetBinding(ContentControl.ContentProperty, binding);
                    setContent = false;
                }

                // Push the item into the ListBoxItem container
                if (setContent)
                {
                    tvitem.Header = item;
                }
            }
        }

        /// <summary>
        /// Gets the treeviewitem holding the visual element
        /// </summary>
        /// <param name="elem">the visual element hold in a TreeViewItem</param>
        /// <returns>The TreeViewItem if found, null otherwise</returns>
        private static TreeViewItem GetTreeViewItem(object elem)
        {
            FrameworkElement fe = elem as FrameworkElement;
            while (fe != null)
            {
                if (fe is TreeViewItem)
                {
                    return fe as TreeViewItem;
                }

                fe = fe.Parent as FrameworkElement;
            }

            return null;
        }

        /// <summary>
        /// Finds a child node from it's data item.
        /// </summary>
        /// <param name="dataItem">The dataitem to find the node for.</param>
        /// <returns>The node if found, null otherwise</returns>
        private TreeViewItem FindChildNode(object dataItem)
        {
            var item = (from n in Nodes
                        where n.DataContext == dataItem
                        select n).FirstOrDefault();

            if (item == null)
            {
                foreach (var childNode in this.Nodes)
                {
                    item = childNode.FindChildNode(dataItem);
                    if (item != null)
                    {
                        return item;
                    }
                }
            }

            return item;
        }

        /// <summary>
        /// Determines whether the treeviewitem is on current page.
        /// </summary>
        /// <param name="treeViewItem">The tree view item.</param>
        /// <param name="itemsHostRect">The items host rect.</param>
        /// <param name="treeViewItemRect">The tree view item rect.</param>
        /// <returns>
        /// 	<c>true</c> if the treeview is on current page otherwise, <c>false</c>.
        /// </returns>
        private bool IsOnCurrentPage(TreeViewItem treeViewItem, out Rect itemsHostRect, out Rect treeViewItemRect)
        {
            // Get Rect for item host element
            FrameworkElement itemsHost = this.elementScrollViewer;

            itemsHostRect = new Rect(new Point(), new Point(itemsHost.RenderSize.Width, itemsHost.RenderSize.Height));

            // Adjust Rect to account for padding
            Control itemsHostControl = itemsHost as Control;
            if (null != itemsHostControl)
            {
                Thickness padding = itemsHostControl.Padding;
                itemsHostRect = new Rect(
                    itemsHostRect.Left + padding.Left,
                    itemsHostRect.Top + padding.Top,
                    Math.Max(0, itemsHostRect.Width - padding.Left - padding.Right),
                    Math.Max(0, itemsHostRect.Height - padding.Top - padding.Bottom));
            }

            // Get relative Rect for ListBoxItem
            GeneralTransform generalTransform = treeViewItem.TransformToVisual(itemsHost);
            treeViewItemRect = new Rect(generalTransform.Transform(new Point()), generalTransform.Transform(new Point(treeViewItem.RenderSize.Width, treeViewItem.RenderSize.Height)));

            // Return result
            return ((itemsHostRect.Top <= treeViewItemRect.Top) && (treeViewItemRect.Bottom <= itemsHostRect.Bottom));
        }

        /// <summary>
        /// handles the SelectOnExpandChange property changes.
        /// </summary>
        /// <param name="oldValue">if set to <c>true</c> [old value].</param>
        /// <param name="newValue">if set to <c>true</c> [new value].</param>
        private void OnSelectOnExpandChangeChanged(bool oldValue, bool newValue)
        {
        }

        /// <summary>
        /// handles the SelectedItemChanged property changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnSelectedItemChanged(object oldValue, object newValue)
        {
            this.EnsureNodeSelection();
            OnSelectionChanged();
        }

        /// <summary>
        /// handles the SelectedNodeChanged property changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnSelectedNodeChanged(TreeViewItem oldValue, TreeViewItem newValue)
        {
            if (oldValue != null)
                oldValue.IsSelected = false;
            if (newValue != null)
                newValue.IsSelected = true;
        }

        #endregion Methods
    }
}