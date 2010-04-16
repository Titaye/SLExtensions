namespace SLExtensions.Test
{
    using System;
    using System.Collections.Generic;
    using System.Windows;

    using Microsoft.Silverlight.Testing;

    public partial class App : Application
    {
        #region Constructors

        public App()
        {
            this.Startup += this.Application_Startup;
            this.Exit += this.Application_Exit;
            this.UnhandledException += this.Application_UnhandledException;

            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        private void Application_Exit(object sender, EventArgs e)
        {
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Load the main control
            this.RootVisual = (UIElement)UnitTestSystem.CreateTestPage(this);
        }

        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
        }

        #endregion Methods
    }
}