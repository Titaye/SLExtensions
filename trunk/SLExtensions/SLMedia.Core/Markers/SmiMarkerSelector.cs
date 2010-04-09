namespace SLMedia.Core
{
    using System;
    using System.Net;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.Collections.Generic;
using System.ComponentModel;
    using System.Text;

    public class SmiMarkerSelector : MarkerSelector
    {
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

        #region Properties

        public Uri Source { get; set; }

        public Encoding Encoding { get; set; }

        public string Language
        {
            get;
            set;
        }

        private bool webContentLoaded;


        #endregion Properties

        protected override void OnIsActiveChanged()
        {
            base.OnIsActiveChanged();

            LoadWebContent();
        }

        BackgroundWorker parserWorker;

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
    }
}