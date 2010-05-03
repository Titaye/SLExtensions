namespace SLExtensions
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
    using System.Windows.Data;
    using System.ComponentModel;
    using SLExtensions.ComponentModel;

    public class BindingListener : FrameworkElement
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

        public void EnsureBindingSource(FrameworkElement source)
        {

            if (source == null)
            {
                ClearValue(DataContextProperty);
                return;
            }

            var binding = GetBindingExpression(ValueProperty);
            if (binding == null)
                return;

            SetBinding(DataContextProperty, new Binding("DataContext") { Source = source });

            var oldBinding = binding.ParentBinding;
            object newBindingSource = null;

            if (oldBinding.ElementName != null)
            {
                Binding bd = new Binding();
                bd.ElementName = oldBinding.ElementName;
                source.SetBinding(BindingHelperProperty, bd);
                newBindingSource = source.GetValue(BindingHelperProperty) as FrameworkElement;
                source.ClearValue(BindingHelperProperty);
            }
            else if (oldBinding.RelativeSource != null)
            {
                Binding bd = new Binding();
                bd.RelativeSource = oldBinding.RelativeSource;
                source.SetBinding(BindingHelperProperty, bd);
                newBindingSource = source.GetValue(BindingHelperProperty) as FrameworkElement;
                source.ClearValue(BindingHelperProperty);
            }

            if (newBindingSource != null)
            {
                ClearValue(ValueProperty);
                Binding newBinding = new Binding();
                newBinding.BindsDirectlyToSource = oldBinding.BindsDirectlyToSource;
                newBinding.Converter = oldBinding.Converter;
                newBinding.ConverterCulture = oldBinding.ConverterCulture;
                newBinding.ConverterParameter = oldBinding.ConverterParameter;
                newBinding.Mode = oldBinding.Mode;
                newBinding.NotifyOnValidationError = oldBinding.NotifyOnValidationError;
                newBinding.Path = oldBinding.Path;
                newBinding.Source = newBindingSource;
                newBinding.UpdateSourceTrigger = oldBinding.UpdateSourceTrigger;
                newBinding.ValidatesOnExceptions = oldBinding.ValidatesOnExceptions;
                SetBinding(ValueProperty, newBinding);
            }
            //}
        }

        private static readonly DependencyProperty BindingHelperProperty =
            DependencyProperty.RegisterAttached("BindingHelper", typeof(FrameworkElement), typeof(BindingListener), null);



        #endregion Methods
    }
}