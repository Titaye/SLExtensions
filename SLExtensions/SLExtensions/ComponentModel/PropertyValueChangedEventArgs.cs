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

namespace SLExtensions.ComponentModel
{
    public class PropertyValueChangedEventArgs : EventArgs
    {
        public PropertyValueChangedEventArgs(object oldValue, object newValue)
        {
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }

        public object OldValue { get; private set; }
        public object NewValue { get; private set; }
    }
}
