namespace SLExtensions.Showcase
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Browser;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    using SLExtensions.Controls.Media;
    using SLExtensions.Input;

    public partial class App : Application
    {
        #region Constructors

        public App()
        {
            this.Startup += this.Application_Startup;
            this.Exit += this.Application_Exit;
            this.UnhandledException += this.Application_UnhandledException;

            InitializeComponent();
            SLExtensions.Controls.ScrollableScrollViewer.Initialize();
            SLExtensions.Controls.ScrollableVirtualizedListBox.Initialize();
        }

        #endregion Constructors

        #region Events

        /// <summary>
        /// Events
        /// </summary>
        public event EventHandler<StateEventArgs> Navigate;

        #endregion Events

        #region Methods

        /// <summary>
        /// Call to Sys.Application.addHistoryPoint
        /// </summary>
        public void AddHistoryPoint(string stateKey, string stateValue, string title)
        {
            string addHistoryScript = "Sys.Application.addHistoryPoint({{ {0}:'{1}' }}, '{2}');";
            HtmlPage.Window.Eval(string.Format(addHistoryScript, stateKey, stateValue, title));
        }

        /// <summary>
        /// Handler for Sys.Application navigate event
        /// </summary>
        [ScriptableMember]
        public void HandleNavigate(ScriptObject state)
        {
            if (Navigate != null)
            {
                Navigate(this, new StateEventArgs() { State = state });
            }
        }

        private void Application_Exit(object sender, EventArgs e)
        {
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Load the main control
            var host = new ShowcaseHost();
            this.RootVisual = host;

            InitScripts();
            StoryboardCommands.Initialize();
        }

        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
        }

        /// <summary>
        /// Register event hander through proxy method
        /// </summary>
        private void InitScripts()
        {
            HtmlPage.RegisterScriptableObject("App", this);
            string pluginId = HtmlPage.Plugin.Id;
            string initScript = @"
                var __navigateHandler = new Function('obj','args','document.getElementById(\'" + pluginId + @"\').content.App.HandleNavigate(args.get_state())');
                Sys.Application.add_navigate(__navigateHandler);
                __navigateHandler(this, new Sys.HistoryEventArgs(Sys.Application._state));
             ";
            HtmlPage.Window.Eval(initScript);
        }

        #endregion Methods
    }
}