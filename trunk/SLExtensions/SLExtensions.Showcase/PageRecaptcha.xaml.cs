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

    public partial class PageRecaptcha : UserControl
    {
        #region Constructors

        public PageRecaptcha()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        private void ReCaptchaHttpPostVerifier_Failed(object sender, SLExtensions.Controls.MessageEventArgs e)
        {
            MessageBox.Show(e.Message, "Failed", MessageBoxButton.OK);
        }

        private void ReCaptchaHttpPostVerifier_Success(object sender, EventArgs e)
        {
            MessageBox.Show("Success");
        }

        #endregion Methods
    }
}