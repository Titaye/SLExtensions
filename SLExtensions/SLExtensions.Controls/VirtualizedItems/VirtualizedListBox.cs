namespace SLExtensions.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    [TemplatePart(Name = VirtualizedStackPanelName, Type = typeof(VirtualizedStackPanel))]
    [TemplatePart(Name = ScrollBarName, Type = typeof(ScrollBar))]
    public class VirtualizedListBox : Control
    {
        #region Fields

        /// <summary>
        /// IsCacheEnabled depedency property.
        /// </summary>
        public static readonly DependencyProperty IsCacheEnabledProperty = 
            DependencyProperty.Register(
                "IsCacheEnabled",
                typeof(bool),
                typeof(VirtualizedListBox),
                new PropertyMetadata((d, e) => ((VirtualizedListBox)d).OnIsCacheEnabledChanged((bool)e.OldValue, (bool)e.NewValue)));

        /// <summary>
        /// ItemContainerStyle depedency property.
        /// </summary>
        public static readonly DependencyProperty ItemContainerStyleProperty = 
            DependencyProperty.Register(
                "ItemContainerStyle",
                typeof(Style),
                typeof(VirtualizedListBox),
                new PropertyMetadata((d, e) => ((VirtualizedListBox)d).OnItemContainerStyleChanged((Style)e.OldValue, (Style)e.NewValue)));

        /// <summary>
        /// ItemsSource depedency property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty = 
            DependencyProperty.Register(
                "ItemsSource",
                typeof(IEnumerable),
                typeof(VirtualizedListBox),
                new PropertyMetadata((d, e) => ((VirtualizedListBox)d).OnItemsSourceChanged((IEnumerable)e.OldValue, (IEnumerable)e.NewValue)));

        /// <summary>
        /// ItemTemplate depedency property.
        /// </summary>
        public static readonly DependencyProperty ItemTemplateProperty = 
            DependencyProperty.Register(
                "ItemTemplate",
                typeof(DataTemplate),
                typeof(VirtualizedListBox),
                new PropertyMetadata((d, e) => ((VirtualizedListBox)d).OnItemTemplateChanged((DataTemplate)e.OldValue, (DataTemplate)e.NewValue)));

        /// <summary>
        /// ItemContainerGenerator depedency property.
        /// </summary>
        internal static readonly DependencyProperty ItemContainerGeneratorProperty = 
            DependencyProperty.Register(
                "ItemContainerGenerator",
                typeof(IItemContainerGenerator),
                typeof(VirtualizedListBox),
                new PropertyMetadata((d, e) => ((VirtualizedListBox)d).OnItemContainerGeneratorChanged((IItemContainerGenerator)e.OldValue, (IItemContainerGenerator)e.NewValue)));

        private const string ScrollBarName = "scrollBar";
        private const string VirtualizedStackPanelName = "virtualizedStackPanel";

        private Dictionary<object, VirtualizedListBoxItem> ObjectToVirtualizedListBoxItem = new Dictionary<object, VirtualizedListBoxItem>();
        private ScrollBar scrollbar;
        private VirtualizedStackPanel stackPanel;

        #endregion Fields

        #region Constructors

        public VirtualizedListBox()
        {
            this.DefaultStyleKey = typeof(VirtualizedListBox);
            this.ItemContainerGenerator = new VirtualizedListBox.ItemGenerator(this);
        }

        #endregion Constructors

        #region Properties

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

        public IItemContainerGenerator ItemContainerGenerator
        {
            get
            {
                return (IItemContainerGenerator)GetValue(ItemContainerGeneratorProperty);
            }

            private set
            {
                SetValue(ItemContainerGeneratorProperty, value);
            }
        }

        public Style ItemContainerStyle
        {
            get
            {
                return (Style)GetValue(ItemContainerStyleProperty);
            }

            set
            {
                SetValue(ItemContainerStyleProperty, value);
            }
        }

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

        public double ScrollBarValue
        {
            get { return scrollbar.Value; } set { scrollbar.Value = value; }
        }

        #endregion Properties

        #region Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            stackPanel = GetTemplateChild(VirtualizedStackPanelName) as VirtualizedStackPanel;
            scrollbar = GetTemplateChild(ScrollBarName) as ScrollBar;

            if (scrollbar != null)
            {
                scrollbar.ValueChanged += new RoutedPropertyChangedEventHandler<double>(scrollbar_ValueChanged);
            }

            if (stackPanel != null)
            {
                stackPanel.ItemsCountChanged += new EventHandler(stackPanel_ItemsCountChanged);
            }
        }

        /// <summary>
        /// handles the IsCacheEnabledProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnIsCacheEnabledChanged(bool oldValue, bool newValue)
        {
            if (stackPanel != null)
                stackPanel.IsCacheEnabled = newValue;
        }

        /// <summary>
        /// handles the ItemContainerGeneratorProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnItemContainerGeneratorChanged(IItemContainerGenerator oldValue, IItemContainerGenerator newValue)
        {
        }

        /// <summary>
        /// handles the ItemContainerStyleProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnItemContainerStyleChanged(Style oldValue, Style newValue)
        {
        }

        /// <summary>
        /// handles the ItemsSourceProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
        }

        /// <summary>
        /// handles the ItemTemplateProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnItemTemplateChanged(DataTemplate oldValue, DataTemplate newValue)
        {
        }

        void scrollbar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (stackPanel != null)
                stackPanel.CurrentScrollIndex = e.NewValue;
        }

        void stackPanel_ItemsCountChanged(object sender, EventArgs e)
        {
            scrollbar.Maximum = stackPanel.ItemsCount;
        }

        void VirtualizedListBox_Click(object sender, RoutedEventArgs e)
        {
            stackPanel.CurrentScrollIndex += 4;
        }

        #endregion Methods

        #region Nested Types

        private class ItemGenerator : IItemContainerGenerator
        {
            #region Fields

            private VirtualizedListBox listbox;

            #endregion Fields

            #region Constructors

            public ItemGenerator(VirtualizedListBox listbox)
            {
                this.listbox = listbox;
            }

            #endregion Constructors

            #region Methods

            public void ClearContainerForItemOverride(DependencyObject element, object item)
            {
                VirtualizedListBoxItem item2 = element as VirtualizedListBoxItem;
                if (item == null)
                {
                    item = item2.DataContext ?? item2;
                }
                //if ((item2.Item ?? item2) == this.SelectedItem)
                //{
                //    this.SelectedItem = null;
                //}
                item2.IsSelected = false;
                //item2.ParentListBox = null;
                if (item2 != item)
                {
                    listbox.ObjectToVirtualizedListBoxItem.Remove(item);
                }
            }

            public DependencyObject GetContainerForItemOverride()
            {
                VirtualizedListBoxItem item = new VirtualizedListBoxItem();
                if (listbox.ItemContainerStyle != null)
                {
                    item.Style = listbox.ItemContainerStyle;
                }
                return item;
            }

            public bool IsItemItsOwnContainerOverride(object item)
            {
                return item is VirtualizedListBoxItem;
            }

            public void PrepareContainerForItemOverride(DependencyObject element, object item)
            {
                VirtualizedListBoxItem item2 = element as VirtualizedListBoxItem;
                bool flag = true;
                if (item2 != item)
                {
                    if (listbox.ItemTemplate != null)
                    {
                        item2.ContentTemplate = listbox.ItemTemplate;
                    }

                    if (flag)
                    {
                        item2.Content = item;
                    }
                    listbox.ObjectToVirtualizedListBoxItem[item] = item2;
                }
                if ((listbox.ItemContainerStyle != null) && (item2.Style == null))
                {
                    item2.Style = listbox.ItemContainerStyle;
                }
                //if (item2.IsSelected)
                //{
                //    listbox.SelectedItem = item2.Item ?? item2;
                //}
                //else if ((-1 != this.SelectedIndex) && (base.Items.IndexOf(this.SelectedItem) != this.SelectedIndex))
                //{
                //    try
                //    {
                //        this._processingSelectionPropertyChange = true;
                //        this.SelectedIndex = base.Items.IndexOf(this.SelectedItem);
                //    }
                //    finally
                //    {
                //        this._processingSelectionPropertyChange = false;
                //    }
                //}
            }

            #endregion Methods
        }

        #endregion Nested Types
    }
}