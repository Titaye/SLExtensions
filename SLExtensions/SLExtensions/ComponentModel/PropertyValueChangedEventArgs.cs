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
    }
}