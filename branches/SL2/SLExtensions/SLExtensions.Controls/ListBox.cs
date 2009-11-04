// <copyright file="ListBox.cs" company="Ucaya">
// Distributed under Microsoft Public License (Ms-PL)
// </copyright>
// <author>Thierry Bouquain</author>
namespace SLExtensions.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    public class ListBox : System.Windows.Controls.ListBox, IDisposable
    {
        #region Fields

        private Dictionary<object, ListBoxItem> items = new Dictionary<object,ListBoxItem>();

        #endregion Fields

        #region Events

        public event EventHandler ItemsChanged;

        #endregion Events

        #region Methods

        public void Dispose()
        {
        }

        public ListBoxItem GetItemContainer(object item)
        {
            ListBoxItem result = items.TryGetValue(item);
            return result;
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            if (item == null)
                return;

            //TODO disposehelper -> DisposeRecursive
            IDisposable idisposable = element as IDisposable;
            if (idisposable != null)
                idisposable.Dispose();

            items.Remove(item);
            base.ClearContainerForItemOverride(element, item);
        }

        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            RaiseItemsChanged();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            ListBoxItem lbi =(ListBoxItem)element;
            items[item] = lbi;
            if (object.Equals(SelectedItem, item))
                lbi.IsSelected = true;
        }

        protected virtual void RaiseItemsChanged()
        {
            if (ItemsChanged != null)
            {
                ItemsChanged(this, EventArgs.Empty);
            }
        }

        #endregion Methods
    }
}