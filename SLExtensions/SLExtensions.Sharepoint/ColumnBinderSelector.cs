namespace SLExtensions.Sharepoint
{
    using System;
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

    public class ColumnBinderSelector : ColumnBinder
    {
        #region Constructors

        public ColumnBinderSelector(Selector tb)
            : base(tb)
        {
            Binding = (BindingExpression)tb.SetBinding(Selector.SelectedItemProperty,
                new System.Windows.Data.Binding("Value")
                {
                    Source = this,
                    Mode = System.Windows.Data.BindingMode.TwoWay,
                    ValidatesOnExceptions = true,
                    NotifyOnValidationError = true
                });
        }

        #endregion Constructors
    }
}