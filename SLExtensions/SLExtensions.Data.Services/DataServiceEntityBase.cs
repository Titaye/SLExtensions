namespace SLExtensions.Data.Services
{
    using System;
    using System.ComponentModel;
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
    /// Base class for DataService Data Contract classes to implement 
    /// base functionality that is needed like INotifyPropertyChanged.  
    /// Add the base class in the partial class to add the implementation.
    /// </summary>
    public abstract class DataServiceEntityBase : INotifyPropertyChanged
    {
        #region Fields

        /// <summary>
        /// The handler for the registrants of the interface's event 
        /// </summary>
        PropertyChangedEventHandler _propertyChangedHandler;

        #endregion Fields

        #region Events

        /// <summary>
        /// The interface used to notify changes on the entity.
        /// </summary>
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add
              {
            _propertyChangedHandler += value;
              }
              remove
              {
            _propertyChangedHandler -= value;
              }
        }

        #endregion Events

        #region Methods

        /// <summary>
        /// Allow inheritors to fire the event more simply.
        /// </summary>
        /// <param name="propertyName"></param>
        protected void FirePropertyChanged(string propertyName)
        {
            if (_propertyChangedHandler != null)
              {
            _propertyChangedHandler(this, new PropertyChangedEventArgs(propertyName));
              }
        }

        #endregion Methods
    }
}