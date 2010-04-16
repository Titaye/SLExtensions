namespace SLExtensions.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// Representation of the ItemContainerGenerator.  Contains utilities for mapping items
    /// of a ItemsControl to a generated container.
    /// </summary>
    public class CoolMenuItemContainerGenerator
    {
        #region Fields

        private readonly IDictionary<DependencyObject, object> m_itemContainer;

        private Panel m_itemsHost;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes an instance of CoolMenuItemContainerGenerator.
        /// </summary>
        public CoolMenuItemContainerGenerator()
        {
            m_itemContainer = new Dictionary<DependencyObject, object>();
        }

        #endregion Constructors

        #region Properties

        internal Panel ItemsHost
        {
            get
            {
                if (m_itemsHost == null)
                {
                    if (m_itemContainer.Count <= 0)
                        return null;

                    DependencyObject container = m_itemContainer.First().Key;
                    m_itemsHost = VisualTreeHelper.GetParent(container) as Panel;
                }

                return m_itemsHost;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Gets a item container from a specified index.
        /// </summary>
        /// <param name="index">The index for which to obtain the container.</param>
        /// <returns>A container if one can be found.  Returns null otherwise.</returns>
        public DependencyObject ContainerFromIndex(int index)
        {
            Panel host = ItemsHost;
            if (host == null || host.Children == null || index < 0 || index >= host.Children.Count)
                return null;

            return host.Children[index];
        }

        /// <summary>
        /// Gets a list of containers.
        /// </summary>
        /// <returns>Returns a ReadOnlyCollection of containers.</returns>
        public ReadOnlyCollection<DependencyObject> GetContainerList()
        {
            List<DependencyObject> containers = new List<DependencyObject>(m_itemContainer.Keys);
            return new ReadOnlyCollection<DependencyObject>(containers);
        }

        /// <summary>
        /// Gets an index from a specified container.
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        public int IndexFromContainer(DependencyObject container)
        {
            if (container == null)
                throw new ArgumentException("container");

            UIElement element = container as UIElement;
            if (element == null)
                return -1;

            Panel host = ItemsHost;
            if (host == null || host.Children == null)
                return -1;

            return host.Children.IndexOf(element);
        }

        internal void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            m_itemContainer.Remove(element);
        }

        internal void PrepareContainerForItemOverride(DependencyObject element, object item, Style parentItemContainerStyle)
        {
            m_itemContainer[element] = item;

            Control control = element as Control;
            if (parentItemContainerStyle != null && control != null && control.Style == null)
            {
                control.SetValue(Control.StyleProperty, parentItemContainerStyle);
            }
        }

        internal void UpdateItemContainerStyle(Style itemContainerStyle)
        {
            if (itemContainerStyle == null)
                return;

            Panel host = ItemsHost;
            if (host == null || host.Children == null)
                return;

            foreach (var element in host.Children)
            {
                FrameworkElement obj = element as FrameworkElement;
                if (obj.Style == null)
                {
                    obj.Style = itemContainerStyle;
                }
            }
        }

        #endregion Methods
    }
}