namespace SLExtensions.Sharepoint
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    public class ColumnBinderToggleButton : ColumnBinder
    {
        #region Fields

        // Using a DependencyProperty as the backing store for ChoiceValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChoiceValueProperty = 
            DependencyProperty.RegisterAttached("ChoiceValue", typeof(string), typeof(ColumnBinderToggleButton), new PropertyMetadata(ChoiceValueChangedCallback));

        private ToggleButton btn;
        private bool isChecked;

        #endregion Fields

        #region Constructors

        public ColumnBinderToggleButton(ToggleButton btn)
            : base(btn)
        {
            this.btn = btn;
            Binding = (BindingExpression)this.btn.SetBinding(ToggleButton.IsCheckedProperty,
                new System.Windows.Data.Binding("IsChecked")
                {
                    Source = this,
                    Mode = System.Windows.Data.BindingMode.TwoWay,
                    ValidatesOnExceptions = true,
                    NotifyOnValidationError = true
                });
        }

        #endregion Constructors

        #region Properties

        public bool IsChecked
        {
            get { return this.isChecked; }
            set
            {
                if (this.isChecked != value)
                {
                    this.isChecked = value;
                    var values = new List<string>(ReadValuesFromItem());
                    if (value && !values.Contains(GetChoiceValue(AssociatedObject)))
                    {
                        values.Add(GetChoiceValue(AssociatedObject));
                    }
                    else if (!value)
                    {
                        values.Remove(GetChoiceValue(AssociatedObject));
                    }
                    this.RaisePropertyChanged(n => n.IsChecked);
                    Value = string.Join(ChoiceColumnInfo.ChoiceSeparator, values.ToArray());
                }
                Validate();
            }
        }

        #endregion Properties

        #region Methods

        public static string GetChoiceValue(DependencyObject obj)
        {
            return (string)obj.GetValue(ChoiceValueProperty);
        }

        public static void SetChoiceValue(DependencyObject obj, string value)
        {
            obj.SetValue(ChoiceValueProperty, value);
        }

        protected string[] ReadValuesFromItem()
        {
            if (Item == null || Item.Data == null)
                return new string[0];

            var value = Convert.ToString(Item.Data.TryGetValue(GetColumnName(AssociatedObject)));
            return value.Split(new string[] { ChoiceColumnInfo.ChoiceSeparator }, StringSplitOptions.RemoveEmptyEntries);
        }

        protected override void RefreshData()
        {
            SetSkipValidation(true);
            try
            {
                base.RefreshData();
                IsChecked = ReadValuesFromItem().Contains(GetChoiceValue(AssociatedObject));
            }
            finally
            {
                SetSkipValidation(false);
            }
        }

        private static void ChoiceValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColumnBinderToggleButton binder = GetColumnBinder(d) as ColumnBinderToggleButton;
            if (binder != null)
                binder.RefreshData();
        }

        #endregion Methods
    }
}