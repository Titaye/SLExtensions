namespace SLExtensions
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Net;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    public class NotifyingObject : INotifyPropertyChanged
    {
        #region Constructors

        public NotifyingObject()
        {
            SyncContext = SynchronizationContext.Current;
        }

        #endregion Constructors

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Properties

        public SynchronizationContext SyncContext
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        public void RaisePropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (SyncContext == null
                || SynchronizationContext.Current == SyncContext)
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
            else
            {
                SyncContext.Post(delegate
                {
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }, null);
            }
        }

        #endregion Methods
    }
}
