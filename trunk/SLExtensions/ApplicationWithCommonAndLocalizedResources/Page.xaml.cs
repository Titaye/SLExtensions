namespace ApplicationWithCommonAndLocalizedResources
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    public partial class Page : UserControl
    {
        #region Constructors

        public Page()
        {
            InitializeComponent();
            txtBlock.Text = ApplicationWithCommonAndLocalizedResources.Resources.HelloLocalized;
        }

        #endregion Constructors
    }
}