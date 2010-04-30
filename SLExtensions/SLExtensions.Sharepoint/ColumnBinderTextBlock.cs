namespace SLExtensions.Sharepoint
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

    public class ColumnBinderTextBlock : ColumnBinder
    {
        #region Constructors

        public ColumnBinderTextBlock(TextBlock tb)
            : base(tb)
        {
            tb.SetBinding(TextBlock.TextProperty, new System.Windows.Data.Binding("Value") { Source = this });
        }

        #endregion Constructors
    }
}