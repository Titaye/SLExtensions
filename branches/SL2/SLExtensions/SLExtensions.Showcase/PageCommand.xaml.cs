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

    using SLExtensions.Input;

    public partial class PageCommand : UserControl, IDisposable
    {
        #region Constructors

        public PageCommand()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        public void Dispose()
        {
            IDisposable disposable = Resources["controller"] as IDisposable;
            disposable.Dispose();
        }

        #endregion Methods
    }
}