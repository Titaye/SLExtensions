namespace SLExtensions.Globalization
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    public class LocalizerEventListener
    {
        #region Constructors

        public LocalizerEventListener(DependencyObject obj)
        {
            DependencyObject = obj;
        }

        #endregion Constructors

        #region Properties

        public DependencyObject DependencyObject
        {
            get; private set;
        }

        #endregion Properties

        #region Methods

        public void Localizer_LocalizationRefreshed(object sender, EventArgs e)
        {
            Localizer.Localize(DependencyObject);
        }

        #endregion Methods
    }
}