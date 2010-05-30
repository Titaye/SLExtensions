namespace SLExtensions.Windows.Data
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Net;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    public class PagedCollection<T> : NotifyingObject, IPagedCollectionView, IEnumerable<T>, INotifyCollectionChanged
    {
        #region Fields

        private Action<PagedCollectionRequest> asyncLoadPage;
        private bool canChangePage;
        private bool isPageChanging;
        private int itemCount;
        private IEnumerable<T> page;
        private int pageIndex;
        private int pageSize;
        private int totalItemCount;

        #endregion Fields

        #region Constructors

        public PagedCollection(int totalItemCount, Action<PagedCollectionRequest> asyncLoadPage,
            IEnumerable<T> firstPage = null)
        {
            if (asyncLoadPage == null)
                throw new ArgumentNullException("asyncLoadPage");

            this.asyncLoadPage = asyncLoadPage;
            this.TotalItemCount = totalItemCount;
            pageIndex = 0;
            CanChangePage = true;
            this.Page = firstPage != null ? firstPage.ToList() : null;
        }

        #endregion Constructors

        #region Events

        public event EventHandler<DataEventArgs<Exception>> AsyncException;

        event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
        {
            add { collectionChanged += value; }
            remove { collectionChanged -= value; }
        }

        public event EventHandler<EventArgs> PageChanged;

        public event EventHandler<PageChangingEventArgs> PageChanging;

        private event NotifyCollectionChangedEventHandler collectionChanged;

        #endregion Events

        #region Properties

        public bool CanChangePage
        {
            get { return this.canChangePage; }
            set
            {
                if (this.canChangePage != value)
                {
                    this.canChangePage = value;
                    this.RaisePropertyChanged(n => n.CanChangePage);
                }
            }
        }

        public bool CanMoveNext
        {
            get { return PageIndex < PageCount - 1; }
        }

        public bool CanMovePrevious
        {
            get { return PageIndex > 0; }
        }

        public bool IsPageChanging
        {
            get { return this.isPageChanging; }
            private set
            {
                if (this.isPageChanging != value)
                {
                    this.isPageChanging = value;
                    this.RaisePropertyChanged(n => n.IsPageChanging);
                }
            }
        }

        public int ItemCount
        {
            get { return this.itemCount; }
            private set
            {
                if (this.itemCount != value)
                {
                    this.itemCount = value;
                    this.RaisePropertyChanged(n => n.ItemCount);
                }
            }
        }

        public IEnumerable<T> Page
        {
            get { return this.page; }
            set
            {
                if (this.page != value)
                {
                    this.page = value;
                    this.RaisePropertyChanged(n => n.Page);
                }
            }
        }

        public int PageCount
        {
            get
            {
                return (int)Math.Ceiling((double)TotalItemCount / PageSize);
            }
        }

        public int PageIndex
        {
            get { return this.pageIndex; }
            private set
            {
                if (this.pageIndex != value)
                {
                    this.pageIndex = value;
                    this.RaisePropertyChanged(n => n.PageIndex);
                    this.RaisePropertyChanged(n => n.CanMoveNext);
                    this.RaisePropertyChanged(n => n.CanMovePrevious);

                }
            }
        }

        public int PageSize
        {
            get { return this.pageSize; }
            set
            {
                if (this.pageSize != value)
                {
                    this.pageSize = value;
                    this.RaisePropertyChanged(n => n.PageSize);
                    this.RaisePropertyChanged(n => n.PageCount);
                    this.RaisePropertyChanged(n => n.CanMoveNext);
                    this.RaisePropertyChanged(n => n.CanMovePrevious);
                }
            }
        }

        public int TotalItemCount
        {
            get { return this.totalItemCount; }
            set
            {
                if (this.totalItemCount != value)
                {
                    this.totalItemCount = value;
                    this.RaisePropertyChanged(n => n.TotalItemCount);
                    this.RaisePropertyChanged(n => n.PageCount);
                    this.RaisePropertyChanged(n => n.CanMoveNext);
                    this.RaisePropertyChanged(n => n.CanMovePrevious);
                }
            }
        }

        #endregion Properties

        #region Methods

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)this).GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            if (Page != null)
                return Page.GetEnumerator();

            return new List<T>().GetEnumerator();
        }

        public bool MoveToFirstPage()
        {
            return MoveToPage(0);
        }

        public bool MoveToLastPage()
        {
            return MoveToPage(PageCount - 1);
        }

        public bool MoveToNextPage()
        {
            if (CanMoveNext)
                return MoveToPage(PageIndex + 1);

            return false;
        }

        public bool MoveToPage(int pageIndex)
        {
            if (!CanChangePage)
                return false;

            if (IsPageChanging)
                return false;

            if (!RaisePageChanging(pageIndex))
                return false;

            IsPageChanging = true;

            asyncLoadPage(
                new PagedCollectionRequest(
                    pageIndex * PageSize,
                    PageSize,
                    (result, error) =>
                    {
                        try
                        {
                            if (error != null)
                            {
                                if (AsyncException != null)
                                {
                                    AsyncException(this, DataEventArgs.Create(new Exception("Failed to change page", error)));
                                }
                            }
                            Page = result != null ? result.ToList() : null;
                            PageIndex = pageIndex;
                            RaiseCollectionChanged();
                            RaisePageChanged();
                        }
                        finally
                        {
                            IsPageChanging = false;
                        }
                    }));

            return true;
        }

        public bool MoveToPreviousPage()
        {
            if (CanMovePrevious)
                return MoveToPage(PageIndex - 1);
            return false;
        }

        private void RaiseCollectionChanged()
        {
            if (collectionChanged != null)
                collectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private void RaisePageChanged()
        {
            if (PageChanged != null)
                PageChanged(this, EventArgs.Empty);
        }

        private bool RaisePageChanging(int newIndex)
        {
            PageChangingEventArgs args = new PageChangingEventArgs(newIndex);
            if (PageChanging != null)
            {
                PageChanging(this, args);
                if (args.Cancel)
                    return false;
            }
            return true;
        }

        #endregion Methods

        #region Nested Types

        public class PagedCollectionRequest
        {
            #region Fields

            private Action<IEnumerable<T>, Exception> callback;

            #endregion Fields

            #region Constructors

            public PagedCollectionRequest(int skip, int pageSize,
                Action<IEnumerable<T>, Exception> callback)
            {
                if (callback == null)
                    throw new ArgumentNullException();

                this.PageSize = pageSize;
                this.Skip = skip;
                this.callback = callback;
            }

            #endregion Constructors

            #region Properties

            public int PageSize
            {
                get;
                private set;
            }

            public int Skip
            {
                get;
                private set;
            }

            #endregion Properties

            #region Methods

            public void PostError(Exception exception)
            {
                callback(null, exception);
            }

            public void PostResult(IEnumerable<T> result)
            {
                callback(result, null);
            }

            #endregion Methods
        }

        #endregion Nested Types
    }
}