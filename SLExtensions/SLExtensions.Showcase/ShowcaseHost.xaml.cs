namespace SLExtensions.Showcase
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    using SLExtensions.Showcase.Controllers;

    public partial class ShowcaseHost : UserControl
    {
        #region Constructors

        public ShowcaseHost()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            controller = (ShowcaseController)Resources["controller"];
        }

        #endregion Methods
    }
}