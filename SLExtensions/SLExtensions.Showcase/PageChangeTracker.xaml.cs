namespace SLExtensions.Showcase
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    public class Data : NotifyingObject
    {
        #region Fields

        private string value;

        #endregion Fields

        #region Properties

        public string Value
        {
            get { return value; }
            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.Value));
                }
            }
        }

        #endregion Properties
    }

    public partial class PageChangeTracker : UserControl
    {
        #region Constructors

        public PageChangeTracker()
        {
            InitializeComponent();

            Data[] test = new Data[] { new Data { Value = "aaaaaa" }, new Data { Value = "bbbbb" } };
            itemsControl.ItemsSource = test;
        }

        #endregion Constructors
    }
}
