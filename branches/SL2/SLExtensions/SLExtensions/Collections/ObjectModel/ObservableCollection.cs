// <copyright file="ObservableCollection.cs" company="Ucaya">
// Distributed under Microsoft Public License (Ms-PL)
// </copyright>
// <author>Thierry Bouquain</author>
namespace SLExtensions.Collections.ObjectModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    /// <summary>
    /// Represents a dynamic data collection that provides notifications when items
    /// get added, removed, or when the entire list is refreshed.
    /// </summary>
    /// <typeparam name="T">type of the collection</typeparam>
    public class ObservableCollection<T> : System.Collections.ObjectModel.ObservableCollection<T>
    {
        #region Fields

        /// <summary>
        /// Locks the CollectionChanged event.
        /// </summary>
        private bool lockCollectionChangeEvent = false;
        int lockCount = 0;

        #endregion Fields

        #region Constructors

        public ObservableCollection()
        {
        }

        public ObservableCollection(IEnumerable<T> items)
        {
            AddRange(items);
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Adds and removes a range of items to the collection in one shot.
        /// </summary>
        /// <param name="rangeToAdd">The range to add.</param>
        /// <param name="rangeToemove">The range to remove.</param>
        public void AddAndRemoveRange(IEnumerable<T> rangeToAdd, IEnumerable<T> rangeToRemove)
        {
            bool itemChanged = false;

            this.lockCollectionChangeEvent = true;
            try
            {
                foreach (T item in rangeToAdd.ToArray())
                {
                    this.Add(item);
                    itemChanged = true;
                }

                foreach (T item in rangeToRemove.ToArray())
                {
                    this.Remove(item);
                    itemChanged = true;
                }
            }
            finally
            {
                this.lockCollectionChangeEvent = false;
            }

            if (itemChanged)
                this.OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        /// Adds a range of items to the collection in one shot.
        /// </summary>
        /// <param name="range">The range.</param>
        public void AddRange(IEnumerable<T> range)
        {
            bool itemAdded = false;

            this.lockCollectionChangeEvent = true;
            try
            {
                foreach (T item in range.ToArray())
                {
                    this.Add(item);
                    itemAdded = true;
                }
            }
            finally
            {
                this.lockCollectionChangeEvent = false;
            }

            if (itemAdded)
                this.OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
        }

        public void LockCollectionChanged()
        {
            lockCount++;
        }

        /// <summary>
        /// Removes a range of items to the collection in one shot.
        /// </summary>
        /// <param name="range">The range.</param>
        public void RemoveRange(IEnumerable<T> range)
        {
            bool itemRemoved = false;

            this.lockCollectionChangeEvent = true;
            try
            {
                foreach (T item in range.ToArray())
                {
                    this.Remove(item);
                    itemRemoved = true;
                }
            }
            finally
            {
                this.lockCollectionChangeEvent = false;
            }

            if (itemRemoved)
                this.OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        /// Replace the whole collection in one shot.
        /// </summary>
        /// <param name="range">The new colletion content.</param>
        public void Replace(IEnumerable<T> range)
        {
            bool itemChanged = false;

            this.lockCollectionChangeEvent = true;
            try
            {
                itemChanged = this.Count != 0;

                this.Clear();

                if (range != null)
                {
                    foreach (T item in range.ToArray())
                    {
                        this.Add(item);
                        itemChanged = true;
                    }
                }
            }
            finally
            {
                this.lockCollectionChangeEvent = false;
            }

            if (itemChanged)
                this.OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        /// Reset the content of the list.
        /// </summary>
        /// <param name="newList">The new list content.</param>
        public void Reset(IEnumerable<T> newList)
        {
            this.lockCollectionChangeEvent = true;
            try
            {
                List<T> newItems = newList.ToList();
                this.Clear();
                foreach (T item in newItems)
                {
                    this.Add(item);
                }
            }
            finally
            {
                this.lockCollectionChangeEvent = false;
            }

            this.OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
        }

        public void UnlockCollectionChanged()
        {
            bool needRefresh = false;
            if (lockCount == 1)
            {
                needRefresh = true;
            }
            lockCount--;
            if (needRefresh)
            {
                OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Collections.ObjectModel.ObservableCollection`1.CollectionChanged"/> event with the provided event data.
        /// </summary>
        /// <param name="e">The event data to report in the event.</param>
        protected override void OnCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (lockCount == 0 && !this.lockCollectionChangeEvent)
            {
                base.OnCollectionChanged(e);
            }
        }

        #endregion Methods
    }
}