namespace SLExtensions.Showcase
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    public partial class PageVirtualizedStackPanel : UserControl
    {
        #region Constructors

        public PageVirtualizedStackPanel()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            List<int> source = new List<int>();
            for (int i = 0; i < 1000000; i++)
            {
                source.Add(i);
            }
            vsp.ItemsSource = source;
        }

        #endregion Methods
    }
}