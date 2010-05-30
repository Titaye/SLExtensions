namespace SLExtensions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    public class NotifyingDictionary<T> : INotifyCollectionChanged
    {
        #region Fields

        private Dictionary<string, WeakReference> dictionary;

        #endregion Fields

        #region Constructors

        public NotifyingDictionary()
        {
            dictionary = new Dictionary<string, WeakReference>();
        }

        #endregion Constructors

        #region Events

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion Events

        #region Properties

        public int Count
        {
            get { return dictionary.Count; }
        }

        #endregion Properties

        #region Indexers

        public virtual T this[string key]
        {
            get
            {
                WeakReference weakref = (WeakReference)this.dictionary[key];
                if (weakref.Target == null)
                    throw new System.Collections.Generic.KeyNotFoundException();
                return (T) weakref.Target;
            }
            set
            {
                WeakReference weakref = new WeakReference(value);
                dictionary[key] = weakref;
                RaiseCollectionChanged();
            }
        }

        #endregion Indexers

        #region Methods

        public void Add(string key, T value)
        {
            WeakReference weakref = new WeakReference(value);
            dictionary.Add(key, weakref);
            RaiseCollectionChanged();
        }

        public void Clear()
        {
            this.dictionary.Clear();
            RaiseCollectionChanged();
        }

        public bool ContainsKey(string key)
        {
            return dictionary.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            return dictionary.Remove(key);
        }

        public bool TryGetValue(string key, out T value)
        {
            WeakReference weakref;
            if (!dictionary.TryGetValue(key, out weakref))
            {
                value = default(T);
                return false;
            }
            if(weakref.Target == null)
            {
                value = default(T);
                dictionary.Remove(key);
                return false;
            }

            value = (T)weakref.Target;
            return true;
        }

        private void RaiseCollectionChanged()
        {
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        #endregion Methods
    }
}