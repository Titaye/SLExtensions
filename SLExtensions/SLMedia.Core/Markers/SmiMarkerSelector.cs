namespace SLMedia.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    public class SmiMarkerSelector : MarkerSelector
    {
        #region Fields

        BackgroundWorker parserWorker;
        private bool webContentLoaded;

        #endregion Fields

        #region Constructors

        public SmiMarkerSelector()
            : base()
        {
        }

        public SmiMarkerSelector(Uri source, string language)
            : base(new KeyValuePair<string, object>(MarkerMetadata.Language, language))
        {
            this.Language = language;
            this.Source = source;
        }

        #endregion Constructors

        #region Properties

        public Encoding Encoding
        {
            get; set;
        }

        public string Language
        {
            get;
            set;
        }

        public Uri Source
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        protected override void OnIsActiveChanged()
        {
            base.OnIsActiveChanged();

            LoadWebContent();
        }

        private void LoadWebContent()
        {
            if (IsActive
                && Source != null
                && !webContentLoaded)
            {
                WebClient client = new WebClient();
                client.OpenReadCompleted += (snd, e) =>
                {
                    if (e.Cancelled
                        || e.Error != null)
                        return;
                    var data = new byte[e.Result.Length];
                    e.Result.Read(data, 0, data.Length);
                    string content;
                    var encoding = Encoding ?? Encoding.UTF8;

                    content = encoding.GetString(data, 0, data.Length);

                    webContentLoaded = true;
                    parserWorker = new BackgroundWorker();
                    parserWorker.DoWork += delegate
                    {
                        var markers = SmiParser.ParseSmiFile(content);
                        if (markers != null
                            && markers.Count > 0)
                        {
                            Deployment.Current.Dispatcher.BeginInvoke(() =>
                            {
                                this.Markers = markers.First().Value;
                            });
                        }
                    };
                    parserWorker.RunWorkerAsync();
                };
                client.OpenReadAsync(Source);
            }
        }

        #endregion Methods
    }
}
