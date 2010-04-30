// <copyright file="LayerSource.cs" company="Ucaya">
// Distributed under Microsoft Public License (Ms-PL)
// </copyright>
// <author>Thierry Bouquain</author>
namespace SLExtensions.Controls.Layers
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    /// <summary>
    /// Display <see cref="LayerItem"/> on a <see cref="LayerHost"/>
    /// </summary>
    public class LayerSource : ItemsControl
    {
        #region Fields

        /// <summary>
        /// dictionary holding item value and associated LayerItem
        /// </summary>
        private Dictionary<object, LayerItem> layerItems = new Dictionary<object, LayerItem>();

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LayerSource"/> class.
        /// </summary>
        public LayerSource()
        {
            DefaultStyleKey = typeof(LayerSource);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the layer host.
        /// </summary>
        /// <value>The layer host.</value>
        public LayerHost LayerHost
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the layer items.
        /// </summary>
        /// <value>The layer items.</value>
        public IDictionary<object, LayerItem> LayerItems
        {
            get
            {
                return this.layerItems;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Clips the host.
        /// </summary>
        /// <param name="clipRect">The clip rect.</param>
        /// <param name="zoom">The zoom value.</param>
        public virtual void ClipHost(Rect clipRect, double zoom)
        {
        }

        /// <summary>
        /// When overridden in a derived class, undoes the effects of the <see cref="M:System.Windows.Controls.ItemsControl.PrepareContainerForItemOverride(System.Windows.DependencyObject,System.Object)"/> method.
        /// </summary>
        /// <param name="element">The container element.</param>
        /// <param name="item">The Item value.</param>
        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);

            this.LayerItems.Remove(((FrameworkElement)element).DataContext);
        }

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>the dependency object</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            LayerItem item = null;
            if (ItemTemplate != null)
            {
                item = ItemTemplate.LoadContent() as LayerItem;
            }

            if (item == null)
            {
                item = new LayerItem();
            }

            return item;
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
            return item is LayerItem;
        }

        /// <summary>
        /// Prepares the specified element to display the specified item.
        /// </summary>
        /// <param name="element">Element used to display the specified item.</param>
        /// <param name="item">Specified item.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            LayerItem mapItem = element as LayerItem;
            if (mapItem != null)
            {
                mapItem.LayerSource = this;

                Binding bd = new Binding("Zoom");
                bd.Source = LayerHost;
                mapItem.SetBinding(LayerItem.ZoomProperty, bd);
                mapItem.ContentTemplate = ItemTemplate;

                this.LayerItems[item] = mapItem;
            }
        }

        #endregion Methods
    }
}