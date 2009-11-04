namespace SLExtensions.Bootstrapping
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Resources;
    using System.Windows.Shapes;
    using System.Xml;

    #region Enumerations

    public enum BootstrapSteps
    {
        LoadingCore,
        LoadingCommonResources,
        LoadingLocalizedResources
    }

    #endregion Enumerations

    public class BootstrapApplication : Application
    {
        #region Fields

        private readonly Uri _manifestUri = new Uri("AppManifest.xaml", UriKind.Relative);

        int _currentProcessItem;
        ProcessItem[] _processItems;
        private StartupEventArgs _startupArgs;
        WebClient _webClient = new WebClient();

        #endregion Fields

        #region Constructors

        protected BootstrapApplication()
        {
            _webClient.OpenReadCompleted += new OpenReadCompletedEventHandler(webClient_OpenReadCompleted);
            _webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(webClient_DownloadProgressChanged);

            this.Startup += new StartupEventHandler(BootstrapApplication_Startup);
        }

        #endregion Constructors

        #region Events

        public event EventHandler<BootstrapEventArgs> Progress;

        #endregion Events

        #region Properties

        /// <summary>
        /// If true, for each xap named "package.xap" defined in Xaps property, the bootstrapper
        /// will try to load a "package.&lt;locale&gt;.xap" file. &lt;locale&gt; will first be replaced with "fr-FR", then if failed, it will be "fr" (assuming current UI culture is fr-FR). 
        /// </summary>
        protected virtual bool DownloadLocalizedResources
        {
            get { return true; }
        }

        /// <summary>
        /// Xaps uris containing code assemblies
        /// </summary>
        protected virtual IEnumerable<Uri> Xaps
        {
            get { yield break; }
        }

        #endregion Properties

        #region Methods

        protected virtual void OnApplicationReady(StartupEventArgs e)
        {
        }

        void BootstrapApplication_Startup(object sender, StartupEventArgs e)
        {
            _startupArgs = e;
            List<ProcessItem> processItems = new List<ProcessItem>(
                from coreUri in Xaps
                select new ProcessItem { Step = BootstrapSteps.LoadingCore, XapUri = coreUri });
            CultureInfo curentCulture = CultureInfo.CurrentUICulture;
            bool hasParentCulture = false;
            if (curentCulture.Name.Contains('-'))
                hasParentCulture = true;
            if (this.DownloadLocalizedResources)
            {
                foreach(var coreUri in Xaps)
                {
                    processItems.Add(new ProcessItem{ Step= BootstrapSteps.LoadingLocalizedResources, XapUri=LocalizeUri(coreUri, curentCulture)});
                    if (hasParentCulture)
                    {
                        processItems.Add(new ProcessItem { Step = BootstrapSteps.LoadingLocalizedResources, XapUri = LocalizeUri(coreUri, curentCulture.Parent) });
                    }
                }
            }
            _processItems = processItems.ToArray();
            _currentProcessItem = 0;
            ConsumeItem();
        }

        void ConsumeItem()
        {
            if (_currentProcessItem >= _processItems.Length)
            {
                OnApplicationReady(_startupArgs);
                return;
            }

            _webClient.OpenReadAsync(_processItems[_currentProcessItem].XapUri);
        }

        private IEnumerable<Uri> GetAssemblyUrisFromDeploymentXamlStream(Stream stream)
        {
            using (var xmlReader = XmlReader.Create(stream))
            {
                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType == XmlNodeType.Element
                        && xmlReader.Name == "AssemblyPart")
                        yield return new Uri(xmlReader.GetAttribute("Source"), UriKind.Relative);
                }
            }
        }

        private Uri LocalizeUri(Uri uri, CultureInfo culture)
        {
            string localized = string.Format("{0}.{1}.xap", uri.AbsoluteUri.Substring(0, uri.AbsoluteUri.Length -4), culture);
            return new Uri(localized, UriKind.Absolute);
        }

        private void RaiseProgress(BootstrapEventArgs args)
        {
            if (Progress != null)
                Progress(this, args);
        }

        void TryLoadXap(Stream xapStream)
        {
            try
            {
                StreamResourceInfo xapSri = new StreamResourceInfo(xapStream, "application/x-silverlight-app");
                var manifestSri = Application.GetResourceStream(xapSri, _manifestUri);

                foreach (var asmUri in GetAssemblyUrisFromDeploymentXamlStream(manifestSri.Stream))
                {
                    var assemblySri = Application.GetResourceStream(xapSri, asmUri);
                    var asmPart = new AssemblyPart();
                    asmPart.Load(assemblySri.Stream);
                }
            }
            catch (Exception ex)
            {
                // TODO : notify error
            }
        }

        void webClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            double currentStepProgress = e.ProgressPercentage / 100.0;
            double overallProgress = (_currentProcessItem + currentStepProgress) * (1.0 / _processItems.Length);
            RaiseProgress(new BootstrapEventArgs(_processItems[_currentProcessItem].Step, overallProgress, currentStepProgress));
        }

        void webClient_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error != null || e.Cancelled)
            {
                // todo : decide what to do on error

                _currentProcessItem++;
                ConsumeItem();
                return;
            }
            TryLoadXap(e.Result);
            _currentProcessItem++;
            ConsumeItem();
        }

        #endregion Methods

        #region Nested Types

        private class ProcessItem
        {
            #region Properties

            public BootstrapSteps Step
            {
                get; set;
            }

            public Uri XapUri
            {
                get; set;
            }

            #endregion Properties
        }

        #endregion Nested Types
    }

    public class BootstrapEventArgs : EventArgs
    {
        #region Constructors

        internal BootstrapEventArgs(BootstrapSteps step, double overallProgress, double stepProgress)
        {
            Step = step;
            OverallProgress = overallProgress;
            StepProgress = stepProgress;
        }

        #endregion Constructors

        #region Properties

        public double OverallProgress
        {
            get; private set;
        }

        public BootstrapSteps Step
        {
            get; private set;
        }

        public double StepProgress
        {
            get; private set;
        }

        #endregion Properties
    }
}