﻿namespace LocalizationSample
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

    using SLExtensions.Bootstrapping;

    public partial class App : BootstrapApplication
    {
        #region Fields

        ProgressBar _progressbar;
        Grid _rootProxy;

        #endregion Fields

        #region Constructors

        public App()
        {
            this.Startup += this.Application_Startup;
            this.Exit += this.Application_Exit;
            this.UnhandledException += this.Application_UnhandledException;

            InitializeComponent();
        }

        #endregion Constructors

        #region Properties

        protected override IEnumerable<Uri> Xaps
        {
            get
            {
                yield return new Uri(new Uri(Host.Source.AbsoluteUri), "ApplicationWithCommonAndLocalizedResources.xap");
            }
        }

        #endregion Properties

        #region Methods

        protected override void OnApplicationReady(StartupEventArgs e)
        {
            base.OnApplicationReady(e);
            _rootProxy.Children.Clear();
            _rootProxy.Children.Add(new ApplicationWithCommonAndLocalizedResources.Page());
        }

        private void Application_Exit(object sender, EventArgs e)
        {
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            _rootProxy = new Grid();
            _progressbar = new ProgressBar
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 400,
                Height = 10,
                Minimum = 0,
                Maximum = 1,
                Value = 0
            };
            _rootProxy.Children.Add(_progressbar);
            this.Progress += new EventHandler<BootstrapEventArgs>(App_Progress);
            RootVisual = _rootProxy;
            // this.RootVisual = new Page();
        }

        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            // If the app is running outside of the debugger then report the exception using
            // the browser's exception mechanism. On IE this will display it a yellow alert
            // icon in the status bar and Firefox will display a script error.
            if (!System.Diagnostics.Debugger.IsAttached)
            {

                // NOTE: This will allow the application to continue running after an exception has been thrown
                // but not handled.
                // For production applications this error handling should be replaced with something that will
                // report the error to the website and stop the application.
                e.Handled = true;
                Deployment.Current.Dispatcher.BeginInvoke(delegate { ReportErrorToDOM(e); });
            }
        }

        void App_Progress(object sender, BootstrapEventArgs e)
        {
            _progressbar.Value = e.OverallProgress;
        }

        private void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
        {
            try
            {
                string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
                errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

                System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight 2 Application " + errorMsg + "\");");
            }
            catch (Exception)
            {
            }
        }

        #endregion Methods
    }
}