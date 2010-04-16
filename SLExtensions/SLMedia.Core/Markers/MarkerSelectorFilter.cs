namespace SLMedia.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    using SLExtensions;

    public class MarkerSelectorFilter : DependencyObject, INotifyPropertyChanged
    {
        #region Fields

        /// <summary>
        /// FilterActiveOnly depedency property.
        /// </summary>
        public static readonly DependencyProperty FilterActiveOnlyProperty = 
            DependencyProperty.Register(
                "FilterActiveOnly",
                typeof(bool),
                typeof(MarkerSelectorFilter),
                new PropertyMetadata((d, e) => ((MarkerSelectorFilter)d).OnFilterActiveOnlyChanged((bool)e.OldValue, (bool)e.NewValue)));

        /// <summary>
        /// Key depedency property.
        /// </summary>
        public static readonly DependencyProperty KeyProperty = 
            DependencyProperty.Register(
                "Key",
                typeof(string),
                typeof(MarkerSelectorFilter),
                new PropertyMetadata((d, e) => ((MarkerSelectorFilter)d).OnKeyChanged((string)e.OldValue, (string)e.NewValue)));

        /// <summary>
        /// MarkerSelectors depedency property.
        /// </summary>
        public static readonly DependencyProperty MarkerSelectorsProperty = 
            DependencyProperty.Register(
                "MarkerSelectors",
                typeof(IEnumerable<IMarkerSelector>),
                typeof(MarkerSelectorFilter),
                new PropertyMetadata((d, e) => ((MarkerSelectorFilter)d).OnMarkerSelectorsChanged((IEnumerable<IMarkerSelector>)e.OldValue, (IEnumerable<IMarkerSelector>)e.NewValue)));

        /// <summary>
        /// Value depedency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty = 
            DependencyProperty.Register(
                "Value",
                typeof(object),
                typeof(MarkerSelectorFilter),
                new PropertyMetadata((d, e) => ((MarkerSelectorFilter)d).OnValueChanged((object)e.OldValue, (object)e.NewValue)));

        private IEnumerable<IMarkerSelector> lastCollection;
        private List<IMarkerSelector> subscribedSelectors = new List<IMarkerSelector>();

        #endregion Fields

        #region Constructors

        public MarkerSelectorFilter()
        {
            Value = new IMarkerSelector[0];
        }

        #endregion Constructors

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Properties

        public bool FilterActiveOnly
        {
            get
            {
                return (bool)GetValue(FilterActiveOnlyProperty);
            }

            set
            {
                SetValue(FilterActiveOnlyProperty, value);
            }
        }

        public string Key
        {
            get
            {
                return (string)GetValue(KeyProperty);
            }

            set
            {
                SetValue(KeyProperty, value);
            }
        }

        public IEnumerable<IMarkerSelector> MarkerSelectors
        {
            get
            {
                return (IEnumerable<IMarkerSelector>)GetValue(MarkerSelectorsProperty);
            }

            set
            {
                SetValue(MarkerSelectorsProperty, value);
            }
        }

        public object Value
        {
            get
            {
                return (object)GetValue(ValueProperty);
            }

            set
            {
                SetValue(ValueProperty, value);
            }
        }

        #endregion Properties

        #region Methods

        public void Refresh()
        {
            if (lastCollection != null)
            {
                UnbindSelectors(lastCollection);
                lastCollection = null;
            }

            var newSelectors = MarkerSelectors;
            if (newSelectors != null)
                BindSelectors(newSelectors);

            FilterList();
        }

        protected virtual void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        private void BindSelectors(IEnumerable<IMarkerSelector> newCollection)
        {
            lastCollection = newCollection;

            foreach (var mrkSelector in lastCollection)
            {
                SubscribeSelector(mrkSelector);
            }

            var notifyCollection = lastCollection as INotifyCollectionChanged;
            if (notifyCollection != null)
                notifyCollection.CollectionChanged += MarkerSources_CollectionChanged;
        }

        private void FilterList()
        {
            if (lastCollection != null)
            {
                Value = lastCollection
                            .Where(s => (!FilterActiveOnly || s.IsActive) && s.Metadata.ContainsKey(Key))
                            .ToArray();
            }
            else
                Value = new IMarkerSelector[0];
        }

        void MarkerSources_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                foreach (var mrkSelector in subscribedSelectors.ToArray())
                {
                    UnSubscribeSelector(mrkSelector);
                }
                subscribedSelectors.Clear();

                foreach (var mrkSelector in lastCollection)
                {
                    SubscribeSelector(mrkSelector);
                }
            }
            else
            {
                if (e.OldItems != null)
                {
                    foreach (var mrkSelector in e.OldItems.OfType<IMarkerSelector>())
                    {
                        UnSubscribeSelector(mrkSelector);
                    }
                }

                if (e.NewItems != null)
                {
                    foreach (var mrkSelector in e.NewItems.OfType<IMarkerSelector>())
                    {
                        SubscribeSelector(mrkSelector);
                    }
                }

            }

            FilterList();
        }

        /// <summary>
        /// handles the FilterActiveOnlyProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnFilterActiveOnlyChanged(bool oldValue, bool newValue)
        {
            FilterList();
        }

        /// <summary>
        /// handles the KeyProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnKeyChanged(string oldValue, string newValue)
        {
            Refresh();
        }

        /// <summary>
        /// handles the MarkerSelectorsProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnMarkerSelectorsChanged(IEnumerable<IMarkerSelector> oldValue, IEnumerable<IMarkerSelector> newValue)
        {
            Refresh();
        }

        /// <summary>
        /// handles the ValueProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnValueChanged(object oldValue, object newValue)
        {
            OnPropertyChanged("Value");
        }

        private void SubscribeSelector(IMarkerSelector selector)
        {
            subscribedSelectors.Add(selector);
            selector.IsActiveChanged += mrkSelector_IsActiveChanged;
        }

        private void UnSubscribeSelector(IMarkerSelector selector)
        {
            subscribedSelectors.Remove(selector);
            selector.IsActiveChanged -= mrkSelector_IsActiveChanged;
        }

        private void UnbindSelectors(IEnumerable<IMarkerSelector> lastCollection)
        {
            foreach (var mrkSelector in lastCollection)
            {
                UnSubscribeSelector(mrkSelector);
            }

            var notifyCollection = lastCollection as INotifyCollectionChanged;
            if(notifyCollection != null)
                notifyCollection.CollectionChanged -= MarkerSources_CollectionChanged;
        }

        void mrkSelector_IsActiveChanged(object sender, EventArgs e)
        {
            FilterList();
        }

        #endregion Methods
    }
}