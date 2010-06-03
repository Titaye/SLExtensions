namespace SLExtensions.ComponentModel
{
    using System;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    public class PropertyValueChangedEventArgs : EventArgs
    {
        #region Constructors

        public PropertyValueChangedEventArgs(object oldValue, object newValue)
        {
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }

        #endregion Constructors

        #region Properties

        public object NewValue
        {
            get; private set;
        }

        public object OldValue
        {
            get; private set;
        }

        #endregion Properties

        public static PropertyValueChangedEventArgs<T> Create<T>(T oldValue, T newValue)
        {
            return new PropertyValueChangedEventArgs<T>(oldValue, newValue);
        }
    }

    public class PropertyValueChangedEventArgs<T> : EventArgs
    {
        #region Constructors

        public PropertyValueChangedEventArgs(T oldValue, T newValue)
        {
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }

        #endregion Constructors

        #region Properties

        public T NewValue
        {
            get;
            private set;
        }

        public T OldValue
        {
            get;
            private set;
        }

        #endregion Properties

    }
}