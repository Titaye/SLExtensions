namespace SLExtensions
{
    using System;
    using System.ComponentModel;
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

    using SLExtensions.ComponentModel;

    public class BindingListener : DependencyObject
    {
        #region Fields

        /// <summary>
        /// Value depedency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty = 
            DependencyProperty.Register(
                "Value",
                typeof(object),
                typeof(BindingListener),
                new PropertyMetadata((d, e) => ((BindingListener)d).OnValueChanged((object)e.OldValue, (object)e.NewValue)));

        private static readonly DependencyProperty BindingHelperProperty = 
            DependencyProperty.RegisterAttached("BindingHelper", typeof(FrameworkElement), typeof(BindingListener), null);

        #endregion Fields

        #region Constructors

        public BindingListener()
        {
        }

        #endregion Constructors

        #region Events

        public event EventHandler<PropertyValueChangedEventArgs> ValueChanged;

        #endregion Events

        #region Properties

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

        /// <summary>
        /// handles the ValueProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnValueChanged(object oldValue, object newValue)
        {
            if (ValueChanged != null)
            {
                ValueChanged(this, new PropertyValueChangedEventArgs(oldValue, newValue));
            }
        }

        #endregion Methods
    }
}