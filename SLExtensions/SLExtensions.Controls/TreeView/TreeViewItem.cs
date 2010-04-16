// <copyright file="TreeViewItem.cs" company="Ucaya">
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
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    using SLExtensions.Data;

    /// <summary>
    /// Create a TreeViewItem
    /// </summary>
    [TemplatePart(Name = TreeViewItem.ElementRootName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = TreeViewItem.ItemsPresenterName, Type = typeof(ItemsPresenter))]
    [TemplatePart(Name = TreeViewItem.ExpanderName, Type = typeof(ToggleButton))]
    [TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Selected", GroupName = "SelectionStates")]
    [TemplateVisualState(Name = "Unselected", GroupName = "SelectionStates")]
    [TemplateVisualState(Name = "Collapsed", GroupName = "ItemStates")]
    [TemplateVisualState(Name = "Expanded", GroupName = "ItemStates")]
    [TemplateVisualState(Name = "HasItems", GroupName = "SourceStates")]
    [TemplateVisualState(Name = "NoItems", GroupName = "SourceStates")]
    public class TreeViewItem : ItemsControl
    {
        #region Fields

        /// <summary>
        /// HasItems dependency property
        /// </summary>
        public static readonly DependencyProperty HasItemsProperty = 
            DependencyProperty.Register("HasItems", typeof(bool), typeof(TreeViewItem), new PropertyMetadata(HasItemsChangedCallback));

        /// <summary>
        /// Header dependency proeprty
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = 
            DependencyProperty.Register("Header", typeof(object), typeof(TreeViewItem), null);

        /// <summary>
        /// HeaderTemplate dependency property
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty = 
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(TreeViewItem), null);

        /// <summary>
        /// IsExpander dependency property
        /// </summary>
        public static readonly DependencyProperty IsExpandedProperty = 
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(TreeViewItem), new PropertyMetadata(OnIsExpandedChanged));

        /// <summary>
        /// Is Mouse over dependency property
        /// </summary>
        public static readonly DependencyProperty IsMouseOverProperty = 
            DependencyProperty.Register(
                "IsMouseOver",
                typeof(bool),
                typeof(TreeViewItem),
                new PropertyMetadata((d, e) => ((TreeViewItem)d).OnIsMouseOverChanged((bool)e.OldValue, (bool)e.NewValue)));

        /// <summary>
        /// IsSelected dependency property
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty = 
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(TreeViewItem), new PropertyMetadata(IsSelectedChangedCallback));

        /// <summary>
        /// SelectOnExpand dependency property
        /// </summary>
        public static readonly DependencyProperty SelectOnExpandChangeProperty = 
            DependencyProperty.Register(
                "SelectOnExpandChange",
                typeof(bool?),
                typeof(TreeViewItem),
                new PropertyMetadata((d, e) => ((TreeViewItem)d).OnSelectOnExpandChangeChanged((bool?)e.OldValue, (bool?)e.NewValue)));

        /// <summary>
        /// TemplateSelector dependency property
        /// </summary>
        public static readonly DependencyProperty TemplateSelectorProperty = 
            DependencyProperty.Register(
                "TemplateSelector",
                typeof(DataTemplateSelector),
                typeof(TreeViewItem),
                null);

        /// <summary>
        /// Template element root name
        /// </summary>
        private const string ElementRootName = "LayoutRoot";

        /// <summary>
        /// Template expander element name
        /// </summary>
        private const string ExpanderName = "Expander";

        /// <summary>
        /// Template ItemsPresenter name
        /// </summary>
        private const string ItemsPresenterName = "ItemsHost";

        /// <summary>
        /// Template element root instance
        /// </summary>
        private FrameworkElement elementRoot;

        /// <summary>
        /// Template expander instance
        /// </summary>
        private ToggleButton expander;

        /// <summary>
        /// Template itemshost element 
        /// </summary>
        private ItemsPresenter itemsHost;

        /// <summary>
        /// Store temp container created in IsItemItsOwnContainerOverride for reuse in CreateContainerForItemOverride
        /// </summary>
        private TreeViewItem lastContainerCheck;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeViewItem"/> class.
        /// </summary>
        public TreeViewItem()
        {
            DefaultStyleKey = typeof(TreeViewItem);
            this.Nodes = new List<TreeViewItem>();
            this.HasItems = false;
            TabNavigation = KeyboardNavigationMode.Local;
            IsTabStop = true;

            Loaded += delegate
            {
                this.UpdateVisualState(true);
            };

            MouseEnter += this.OnMouseEnter;
            MouseLeave += this.OnMouseLeave;
            MouseLeftButtonDown += this.TreeViewItem_MouseLeftButtonDown;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance has child items.
        /// </summary>
        /// <value><c>true</c> if this instance has child items; otherwise, <c>false</c>.</value>
        public bool HasItems
        {
            get
            {
                return (bool)GetValue(HasItemsProperty);
            }

            set
            {
                SetValue(HasItemsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the header content.
        /// </summary>
        /// <value>The header.</value>
        public object Header
        {
            get
            {
                return GetValue(HeaderProperty);
            }

            set
            {
                SetValue(HeaderProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the header template.
        /// </summary>
        /// <value>The header template.</value>
        public DataTemplate HeaderTemplate
        {
            get
            {
                return GetValue(HeaderTemplateProperty) as DataTemplate;
            }

            set
            {
                SetValue(HeaderTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is expanded.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is expanded; otherwise, <c>false</c>.
        /// </value>
        public bool IsExpanded
        {
            get
            {
                return (bool)GetValue(IsExpandedProperty);
            }

            set
            {
                SetValue(IsExpandedProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this mouse is over current control
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is mouse over; otherwise, <c>false</c>.
        /// </value>
        public bool IsMouseOver
        {
            get
            {
                return (bool)GetValue(IsMouseOverProperty);
            }

            internal set
            {
                SetValue(IsMouseOverProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsSelected
        {
            get
            {
                return (bool)GetValue(IsSelectedProperty);
            }

            set
            {
                SetValue(IsSelectedProperty, value);
            }
        }

        /// <summary>
        /// Gets the child nodes lists.
        /// </summary>
        /// <value>The child nodes.</value>
        public IList<TreeViewItem> Nodes
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the parent node.
        /// </summary>
        /// <value>The parent node.</value>
        public TreeViewItem ParentNode
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the parent treeview.
        /// </summary>
        /// <value>The parent treeview.</value>
        public TreeView ParentTreeview
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets if we select this node expanding or collapsing. If value is null, it takes the parent node value
        /// </summary>
        /// <value><c>true</c> if we select the node on expanding and collapsing, false otherwise</value>
        public bool? SelectOnExpandChange
        {
            get
            {
                return (bool?)GetValue(SelectOnExpandChangeProperty);
            }

            set
            {
                SetValue(SelectOnExpandChangeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the template selector.
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
        /// Finds a child node from it's data item.
        /// </summary>
        /// <param name="dataItem">The dataitem to find the node for.</param>
        /// <returns>The node if found, null otherwise</returns>
        public TreeViewItem FindChildNode(object dataItem)
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
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.elementRoot = GetTemplateChild(ElementRootName) as FrameworkElement;
            this.itemsHost = GetTemplateChild(ItemsPresenterName) as ItemsPresenter;
            this.expander = GetTemplateChild(ExpanderName) as ToggleButton;

            if (this.expander != null)
            {
                this.expander.IsEnabled = this.HasItems;
                this.expander.IsChecked = false;
                this.expander.Checked += this.ToggleButton_CheckChange;
                this.expander.Unchecked += this.ToggleButton_CheckChange;
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

            DataTemplate template = ItemTemplate;
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

        /// <summary>
        /// Invoked when the <see cref="P:System.Windows.Controls.ItemsControl.Items"/> property changes.
        /// </summary>
        /// <param name="e">Information about the change.</param>
        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            if (e.NewItems == null || e.NewItems.Count == 0)
            {
                if (this.ItemsSource != null)
                {
                    IEnumerator iterator = ItemsSource.GetEnumerator();
                    this.HasItems = iterator.MoveNext();
                }
                else
                {
                    this.HasItems = false;
                }
            }
            else
            {
                this.HasItems = e.NewItems.Count > 0;
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (!e.Handled)
            {
                this.Select(true);
                e.Handled = true;
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
            tvitem.ParentTreeview = this.ParentTreeview;
            this.Nodes.Add(tvitem);

            tvitem.ParentNode = this;

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

            DataTemplate template = ItemTemplate;
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
                else if (!string.IsNullOrEmpty(DisplayMemberPath))
                {
                    // Create a binding for displaying the DisplayMemberPath (which always renders as a string)
                    Binding binding = new Binding(DisplayMemberPath);
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
        /// Handles the HasItemsProperty chnges
        /// </summary>
        /// <param name="d">the treeviewitem</param>
        /// <param name="e">event args</param>
        private static void HasItemsChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItem tvi = (TreeViewItem)d;
            if (!(bool)e.NewValue && tvi.expander != null)
            {
                tvi.expander.IsEnabled = false;
                tvi.expander.IsChecked = false;
            }
            else if (tvi.expander != null)
            {
                tvi.expander.IsEnabled = true;
            }

            ((TreeViewItem)d).UpdateVisualState(true);
        }

        /// <summary>
        /// Handles the IsSelectedProperty chnges
        /// </summary>
        /// <param name="d">the treeviewitem</param>
        /// <param name="e">event args</param>
        private static void IsSelectedChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItem element = (TreeViewItem)d;
            bool newValue = (bool)e.NewValue;
            element.Select(newValue);

            element.UpdateVisualState(true);
        }

        /// <summary>
        /// Handles the IsExpandedProperty chnges
        /// </summary>
        /// <param name="d">the treeviewitem</param>
        /// <param name="e">event args</param>
        private static void OnIsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItem source = d as TreeViewItem;
            Debug.Assert(source != null, "The source is not an instance of TreeViewItem!");

            // Notify derived classes of the change
            source.UpdateVisualState(true);
        }

        /// <summary>
        /// Goes to the specified visual states. Stop on first state name found
        /// </summary>
        /// <param name="useTransitions">if set to <c>true</c> uses transitions.</param>
        /// <param name="stateNames">The state names.</param>
        private void GoToState(bool useTransitions, params string[] stateNames)
        {
            if (stateNames != null)
            {
                foreach (string str in stateNames)
                {
                    if (VisualStateManager.GoToState(this, str, useTransitions))
                    {
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Handles the MouseOver property changes
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnIsMouseOverChanged(bool oldValue, bool newValue)
        {
            this.UpdateVisualState(true);
        }

        /// <summary>
        /// Handles the MouseEnter event
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            this.IsMouseOver = true;
            this.UpdateVisualState(true);
        }

        /// <summary>
        /// Handles the MouseLeave event
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            this.IsMouseOver = false;
            this.UpdateVisualState(true);
        }

        /// <summary>
        /// Handles the SelectOnExpand property changes
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnSelectOnExpandChangeChanged(bool? oldValue, bool? newValue)
        {
        }

        /// <summary>
        /// Selects or unselect this node
        /// </summary>
        /// <param name="selected">if set to <c>true</c> [selected].</param>
        private void Select(bool selected)
        {
            TreeView parentTreeView = this.ParentTreeview;
            if (parentTreeView != null)
            {
                parentTreeView.ChangeSelection(this, selected);
            }
        }

        /// <summary>
        /// Handles the CheckChange event of the ToggleButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ToggleButton_CheckChange(object sender, EventArgs e)
        {
            this.IsExpanded = this.expander.IsChecked.GetValueOrDefault();

            bool? selectOnExpandChange = null;
            TreeViewItem nodeParent = this;

            while (nodeParent != null
                && selectOnExpandChange == null)
            {
                selectOnExpandChange = nodeParent.SelectOnExpandChange;
                nodeParent = nodeParent.ParentNode;
            }

            if (selectOnExpandChange == null)
            {
                selectOnExpandChange = this.ParentTreeview.SelectOnExpandChange;
            }

            if (selectOnExpandChange.Value)
            {
                this.IsSelected = true;
            }
        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the TreeViewItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void TreeViewItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.ParentTreeview.Focus() && !e.Handled)
            {
                this.IsSelected = true;
                e.Handled = true;
            }
        }

        /// <summary>
        /// Updates the visual state of the treeview.
        /// </summary>
        /// <param name="useTransitions">if set to <c>true</c> uses transitions.</param>
        private void UpdateVisualState(bool useTransitions)
        {
            if (this.IsSelected)
            {
                this.GoToState(useTransitions, new string[] { "Selected" });
            }
            else
            {
                this.GoToState(useTransitions, new string[] { "Unselected" });
            }

            if (this.IsMouseOver)
            {
                this.GoToState(useTransitions, new string[] { "MouseOver", "Normal" });
            }
            else
            {
                this.GoToState(useTransitions, new string[] { "Normal" });
            }

            if (this.IsExpanded == true)
            {
                this.GoToState(useTransitions, new string[] { "Expanded" });
            }
            else
            {
                this.GoToState(useTransitions, new string[] { "Collapsed" });
            }

            if (this.HasItems)
            {
                this.GoToState(useTransitions, new string[] { "HasItems", "NoItems" });
            }
            else
            {
                this.GoToState(useTransitions, new string[] { "NoItems" });
            }
        }

        #endregion Methods

        #region Other

        ////private static bool IsControlKeyDown
        ////{
        ////    get
        ////    {
        ////        return ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control);
        ////    }
        ////}
        ////private static bool IsShiftKeyDown
        ////{
        ////    get
        ////    {
        ////        return ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift);
        ////    }
        ////}
        ////private bool LogicalLeft(Key key)
        ////{
        ////    //bool flag = this.FlowDirection == FlowDirection.RightToLeft;
        ////    //return ((!flag && (key == Key.Left)) || (flag && (key == Key.Right)));
        ////    return (key == Key.Left);
        ////}
        ////private bool CanExpand
        ////{
        ////    get
        ////    {
        ////        //return this.HasItems;
        ////        return true;
        ////    }
        ////}
        ////private TreeViewItem GetPreviousItem(bool checkChild)
        ////{
        ////    TreeView parentTreeView = this.ParentTreeview;
        ////    TreeViewItem parentTreeViewItem = this.ParentTreeViewItem;
        ////    TreeViewItem previousTreeViewItem = null;
        ////    int currentIndex = -1;
        ////    int previousIndex = -1;
        ////    if (parentTreeViewItem != null)
        ////    {
        ////        currentIndex = parentTreeViewItem.Items.IndexOf(this);
        ////        previousIndex = currentIndex - 1;
        ////        if (previousIndex > -1)
        ////        {
        ////            previousTreeViewItem = parentTreeViewItem.Items[previousIndex] as TreeViewItem;
        ////            if (checkChild && previousTreeViewItem.IsExpanded && previousTreeViewItem.Items.Count > 0)
        ////            {
        ////                previousTreeViewItem = previousTreeViewItem.Items[previousTreeViewItem.Items.Count - 1] as TreeViewItem;
        ////            }
        ////            return previousTreeViewItem;
        ////        }
        ////        else
        ////        {
        ////            return parentTreeViewItem;
        ////        }
        ////    }
        ////    else
        ////    {
        ////        currentIndex = parentTreeView.Items.IndexOf(this);
        ////        previousIndex = currentIndex - 1;
        ////        if (previousIndex > -1)
        ////        {
        ////            previousTreeViewItem = parentTreeView.Items[previousIndex] as TreeViewItem;
        ////            if (checkChild && previousTreeViewItem.IsExpanded && previousTreeViewItem.Items.Count > 0)
        ////            {
        ////                previousTreeViewItem = previousTreeViewItem.Items[previousTreeViewItem.Items.Count - 1] as TreeViewItem;
        ////            }
        ////            return previousTreeViewItem;
        ////        }
        ////        else
        ////        {
        ////            return null;
        ////        }
        ////    }
        ////}
        ////private TreeViewItem GetNextItem(bool checkChild)
        ////{
        ////    TreeView parentTreeView = this.ParentTreeview;
        ////    TreeViewItem parentTreeViewItem = this.ParentTreeViewItem;
        ////    TreeViewItem nextTreeViewItem = null;
        ////    int currentIndex = -1;
        ////    int nextIndex = -1;
        ////    if (parentTreeViewItem != null)
        ////    {
        ////        if (checkChild && this.IsExpanded && this.Items.Count > 0)
        ////        {
        ////            nextTreeViewItem = this.Items[0] as TreeViewItem;
        ////            return nextTreeViewItem;
        ////        }
        ////        else
        ////        {
        ////            currentIndex = parentTreeViewItem.Items.IndexOf(this);
        ////            nextIndex = currentIndex + 1;
        ////            if (nextIndex < parentTreeViewItem.Items.Count)
        ////            {
        ////                nextTreeViewItem = parentTreeViewItem.Items[nextIndex] as TreeViewItem;
        ////                return nextTreeViewItem;
        ////            }
        ////            else
        ////            {
        ////                return parentTreeViewItem.GetNextItem(false);
        ////            }
        ////        }
        ////    }
        ////    else
        ////    {
        ////        if (checkChild && this.IsExpanded && this.Items.Count > 0)
        ////        {
        ////            nextTreeViewItem = this.Items[0] as TreeViewItem;
        ////            return nextTreeViewItem;
        ////        }
        ////        else
        ////        {
        ////            currentIndex = parentTreeView.Items.IndexOf(this);
        ////            nextIndex = currentIndex + 1;
        ////            if (nextIndex < parentTreeView.Items.Count)
        ////            {
        ////                nextTreeViewItem = parentTreeView.Items[nextIndex] as TreeViewItem;
        ////                return nextTreeViewItem;
        ////            }
        ////            else
        ////            {
        ////                return null;
        ////            }
        ////        }
        ////    }
        ////}
        ////public static FrameworkElement GetParent(FrameworkElement e)
        ////{
        ////    if (!(e is UIElement))
        ////    {
        ////        return (e.Parent as FrameworkElement);
        ////    }
        ////    Panel parent = e.Parent as Panel;
        ////    if (parent == null)
        ////    {
        ////        return null;
        ////    }
        ////    return (parent.Parent as FrameworkElement);
        ////}
        ////public static FrameworkElement GetParentOfType(FrameworkElement e, Type elementType)
        ////{
        ////    e = GetParent(e);
        ////    while (e != null)
        ////    {
        ////        if (elementType.IsAssignableFrom(e.GetType()))
        ////        {
        ////            return e;
        ////        }
        ////        e = GetParent(e);
        ////    }
        ////    return null;
        ////}
        ////internal TreeViewItem ParentTreeViewItem
        ////{
        ////    get
        ////    {
        ////        //return (this.Parent as UcaTreeViewItem);
        ////        return (TreeViewItem)GetParentOfType(this, typeof(TreeViewItem));
        ////    }
        ////}
        ////internal bool HandleUpKey()
        ////{
        ////    TreeViewItem previous = this.GetPreviousItem(true);
        ////    if (previous != null)
        ////    {
        ////        return FocusIntoItem(previous);
        ////    }
        ////    return false;
        ////}
        ////internal bool HandleDownKey()
        ////{
        ////    TreeViewItem next = this.GetNextItem(true);
        ////    if (next != null)
        ////    {
        ////        return FocusIntoItem(next);
        ////    }
        ////    return false;
        ////}
        ////internal static bool FocusIntoItem(TreeViewItem item)
        ////{
        ////    return ((item != null) && item.PerformSelect());
        ////}
        ////private void OnKeyDown(KeyEventArgs e)
        ////{
        ////    if (!e.Handled)
        ////    {
        ////        switch (e.Key)
        ////        {
        ////            case Key.Left:
        ////            case Key.Right:
        ////                if (!this.LogicalLeft(e.Key))
        ////                {
        ////                    //TODO: CanExpand...
        ////                    if (!IsControlKeyDown && this.CanExpand)
        ////                    {
        ////                        if (!this.IsExpanded)
        ////                        {
        ////                            this.IsExpanded = true;
        ////                            e.Handled = true;
        ////                            return;
        ////                        }
        ////                        if (this.HandleDownKey())
        ////                        {
        ////                            e.Handled = true;
        ////                            return;
        ////                        }
        ////                    }
        ////                    return;
        ////                }
        ////                if ((IsControlKeyDown || !this.CanExpand) || !this.IsExpanded)
        ////                {
        ////                    return;
        ////                }
        ////                //if (!this.IsFocused)
        ////                if (!this.IsSelected)
        ////                {
        ////                    FocusIntoItem(this);
        ////                    break;
        ////                }
        ////                this.IsExpanded = false;
        ////                break;
        ////            case Key.Up:
        ////                if (!IsControlKeyDown && this.HandleUpKey())
        ////                {
        ////                    e.Handled = true;
        ////                }
        ////                return;
        ////            case Key.Down:
        ////                if (!IsControlKeyDown && this.HandleDownKey())
        ////                {
        ////                    e.Handled = true;
        ////                }
        ////                return;
        ////            case Key.Add:
        ////                if (this.CanExpand && !this.IsExpanded)
        ////                {
        ////                    this.IsExpanded = true;
        ////                    e.Handled = true;
        ////                }
        ////                return;
        ////            //case Key.Separator:
        ////            //    return;
        ////            case Key.Subtract:
        ////                if (this.CanExpand && this.IsExpanded)
        ////                {
        ////                    this.IsExpanded = false;
        ////                    e.Handled = true;
        ////                }
        ////                return;
        ////            default:
        ////                return;
        ////        }
        ////        e.Handled = true;
        ////    }
        ////}

        #endregion Other
    }
}