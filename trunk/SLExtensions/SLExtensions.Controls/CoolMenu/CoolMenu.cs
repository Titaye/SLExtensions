namespace SLExtensions.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Represents a dock menu similar to that found in the Mac OS.
    /// </summary>
    public class CoolMenu : ItemsControl
    {
        #region Fields

        /// <summary>
        /// Behavior Dependency Property.
        /// </summary>
        public static readonly DependencyProperty BehaviorProperty = DependencyProperty.RegisterAttached(
            "Behavior",
            typeof(ICoolMenuBehavior),
            typeof(CoolMenu), new PropertyMetadata(new DefaultCoolMenuBehavior(), BehaviorChanged));

        /// <summary>
        /// ItemContainerStyle Dependency Property.
        /// </summary>
        public static readonly DependencyProperty ItemContainerStyleProperty = 
            DependencyProperty.Register(
                "ItemContainerStyle",
                typeof(Style),
                typeof(CoolMenu),
                new PropertyMetadata(null, OnItemContainerStylePropertyChanged));

        private readonly CoolMenuItemContainerGenerator m_generator;

        #endregion Fields

        #region Constructors

        //private Panel m_rootElement;
        /// <summary>
        /// Create a new instance of the CoolMenu control.
        /// </summary>
        public CoolMenu()
        {
            m_generator = new CoolMenuItemContainerGenerator();

            this.DefaultStyleKey = typeof(CoolMenu);
            this.MouseLeave += new MouseEventHandler(CoolMenu_MouseLeave);
        }

        #endregion Constructors

        #region Events

        /// <summary>
        /// Event fired when the mouse moves over a new menu item.
        /// </summary>
        public event MenuIndexChangedHandler MenuIndexChanged;

        /// <summary>
        /// Event fired when a menu item is clicked.
        /// </summary>
        public event MenuIndexChangedHandler MenuItemClicked;

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets or sets the maximum item height of an item when the mouse is moved over the item.
        /// </summary>
        public ICoolMenuBehavior Behavior
        {
            get { return GetValue(BehaviorProperty) as ICoolMenuBehavior; }
            set { SetValue(BehaviorProperty, value); }
        }

        /// <summary>
        /// The style of the item container.
        /// </summary>
        public Style ItemContainerStyle
        {
            get { return GetValue(ItemContainerStyleProperty) as Style; }
            set { SetValue(ItemContainerStyleProperty, value); }
        }

        #endregion Properties

        #region Methods

        internal static CoolMenuItemContainerGenerator GetGenerator(ItemsControl control)
        {
            CoolMenu menu = control as CoolMenu;
            if (menu != null)
            {
                return menu.m_generator;
            }
            return null;
        }

        internal void OnItemMouseDown(int selectedIndex)
        {
            var content = m_generator.ContainerFromIndex(selectedIndex) as CoolMenuItem;
            this.Behavior.ApplyMouseDownBehavior(selectedIndex, content);
        }

        internal void OnItemMouseLeave(int selectedIndex)
        {
            if (selectedIndex == -1)
            {
                return;
            }
            var content = m_generator.ContainerFromIndex(selectedIndex) as CoolMenuItem;
            this.Behavior.ApplyMouseLeaveBehavior(content);
        }

        internal void OnItemMouseUp(int selectedIndex)
        {
            var content = m_generator.ContainerFromIndex(selectedIndex) as CoolMenuItem;
            this.Behavior.ApplyMouseUpBehavior(selectedIndex, content);

            if (MenuItemClicked != null)
            {
                SelectedMenuItemArgs menuArgs = new SelectedMenuItemArgs(content, selectedIndex);
                MenuItemClicked(this, menuArgs);
            }
        }

        internal void OnMouseEnter(int selectedIndex)
        {
            for (int i = 0; i < this.Items.Count; ++i)
            {
                var content = m_generator.ContainerFromIndex(i) as CoolMenuItem;

                // No items selected.
                if (selectedIndex == -1)
                {
                    this.Behavior.ApplyMouseEnterBehavior(-1, content);
                    continue;
                }

                this.Behavior.ApplyMouseEnterBehavior(Math.Abs(i - selectedIndex), content);
            }

            if (MenuIndexChanged != null && selectedIndex != -1)
            {
                var content = m_generator.ContainerFromIndex(selectedIndex) as CoolMenuItem;
                SelectedMenuItemArgs menuArgs = new SelectedMenuItemArgs(content, selectedIndex);
                MenuIndexChanged(this, menuArgs);
            }
        }

        /// <summary>
        /// Removes a CoolMenuItem from the control.
        /// </summary>
        /// <param name="element">The CoolMenuItem element of concern.</param>
        /// <param name="item">The item contained by the CoolMenuItem.</param>
        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            CoolMenuItem menuItem = element as CoolMenuItem;
            if (menuItem != null)
            {
                menuItem.ParentItemsControl = null;
            }
            m_generator.ClearContainerForItemOverride(element, item);
            base.ClearContainerForItemOverride(element, item);
        }

        /// <summary>
        /// Gets a CoolMenuItem instance for wrapping an item.
        /// </summary>
        /// <returns>Returns a new instance of CoolMenuItem.</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new CoolMenuItem();
        }

        /// <summary>
        /// Determine if an item is a CoolMenuItem.
        /// </summary>
        /// <param name="item">The object to test.</param>
        /// <returns>A boolean indicating if the item is a CoolMenuItem.</returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is CoolMenuItem);
        }

        /// <summary>
        /// Associates the element with a parent container and registers the item with the generator.
        /// </summary>
        /// <param name="element">The CoolMenuItem element of concern.</param>
        /// <param name="item">The item contained by the CoolMenuItem.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            CoolMenuItem menuItem = element as CoolMenuItem;
            if (menuItem != null)
            {
                // Associate the parent.
                menuItem.ParentItemsControl = this;
                this.Behavior.Initialize(this, menuItem);
            }

            base.PrepareContainerForItemOverride(element, item);
            m_generator.PrepareContainerForItemOverride(element, item, ItemContainerStyle);
        }

        private static void BehaviorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var b = d as CoolMenu;
            if (b != null)
            {
                foreach (CoolMenuItem item in b.m_generator.GetContainerList())
                {
                    b.Behavior.Initialize(b, item);
                }
            }
        }

        private static void OnItemContainerStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CoolMenu c = d as CoolMenu;
            Style value = e.NewValue as Style;
            var generator = CoolMenu.GetGenerator(c);
            generator.UpdateItemContainerStyle(value);
        }

        void CoolMenu_MouseLeave(object sender, MouseEventArgs e)
        {
            OnMouseEnter(-1);
        }

        #endregion Methods
    }
}