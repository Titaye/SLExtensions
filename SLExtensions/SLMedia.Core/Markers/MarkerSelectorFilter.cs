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
    using SLExtensions.Collections.ObjectModel;

    public class MarkerSelectorFilter : FrameworkElement, INotifyPropertyChanged
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
        /// Item depedency property.
        /// </summary>
        public static readonly DependencyProperty ItemProperty = 
            DependencyProperty.Register(
                "Item",
                typeof(IMediaItem),
                typeof(MarkerSelectorFilter),
                new PropertyMetadata((d, e) => ((MarkerSelectorFilter)d).OnItemChanged((IMediaItem)e.OldValue, (IMediaItem)e.NewValue)));

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
        /// Value depedency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty = 
            DependencyProperty.Register(
                "Value",
                typeof(object),
                typeof(MarkerSelectorFilter),
                new PropertyMetadata((d, e) => ((MarkerSelectorFilter)d).OnValueChanged((object)e.OldValue, (object)e.NewValue)));

        private ObservableCollection<IMarkerSelector> lastCollection;
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

        public IMediaItem Item
        {
            get
            {
                return (IMediaItem)GetValue(ItemProperty);
            }

            set
            {
                SetValue(ItemProperty, value);
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
            var item = Item;
            if (item != null)
            {
                lastCollection = item.MarkerSelectors;
                foreach (var mrkSelector in lastCollection)
                {
                    subscribedSelectors.Add(mrkSelector);
                    mrkSelector.IsActiveChanged += new EventHandler(mrkSelector_IsActiveChanged);
                }
                lastCollection.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(MarkerSources_CollectionChanged);
            }
            else
            {
                if (lastCollection != null)
                {
                    foreach (var mrkSelector in lastCollection)
                    {
                        subscribedSelectors.Remove(mrkSelector);
                        mrkSelector.IsActiveChanged -= new EventHandler(mrkSelector_IsActiveChanged);
                    }
                    lastCollection.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(MarkerSources_CollectionChanged);
                }
                lastCollection = null;
            }

            FilterList();
        }

        protected virtual void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
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
                foreach (var mrkSelector in subscribedSelectors)
                {
                    mrkSelector.IsActiveChanged -= new EventHandler(mrkSelector_IsActiveChanged);
                }
                subscribedSelectors.Clear();

                foreach (var mrkSelector in lastCollection)
                {
                    subscribedSelectors.Add(mrkSelector);
                    mrkSelector.IsActiveChanged += new EventHandler(mrkSelector_IsActiveChanged);
                }
            }
            else
            {
                if (e.OldItems != null)
                {
                    foreach (var mrkSelector in e.OldItems.OfType<IMarkerSelector>())
                    {
                        subscribedSelectors.Remove(mrkSelector);
                        mrkSelector.IsActiveChanged -= new EventHandler(mrkSelector_IsActiveChanged);
                    }
                }

                if (e.NewItems != null)
                {
                    foreach (var mrkSelector in e.NewItems.OfType<IMarkerSelector>())
                    {
                        subscribedSelectors.Add(mrkSelector);
                        mrkSelector.IsActiveChanged += new EventHandler(mrkSelector_IsActiveChanged);
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
        /// handles the ItemProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnItemChanged(IMediaItem oldValue, IMediaItem newValue)
        {
            Refresh();
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
        /// handles the ValueProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnValueChanged(object oldValue, object newValue)
        {
            OnPropertyChanged("Value");
        }

        void mrkSelector_IsActiveChanged(object sender, EventArgs e)
        {
            FilterList();
        }

        #endregion Methods
    }
}