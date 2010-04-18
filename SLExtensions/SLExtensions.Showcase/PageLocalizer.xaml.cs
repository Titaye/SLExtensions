namespace SLExtensions.Showcase
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Navigation;
    using System.Windows.Shapes;

    public partial class PageLocalizer : Page
    {
        #region Constructors

        public PageLocalizer()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboLang == null)
                return;

            if (comboLang.SelectedIndex == 0)
            {
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-us");
                Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
            }
            else
            {
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("fr-fr");
                Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
            }
            SLExtensions.Globalization.Localizer.RefreshLocalization();
        }

        #endregion Methods
    }
}