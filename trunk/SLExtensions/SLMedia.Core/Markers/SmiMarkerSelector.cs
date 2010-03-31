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

    public class SmiMarkerSelector : MarkerSelector
    {
        public SmiMarkerSelector() : base()
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

        public string Language
        {
            get; set;
        }

        private bool webContentLoaded;
        

        #endregion Properties

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
                client.DownloadStringCompleted += (snd, e) =>
                {
                    if (!e.Cancelled && e.Error != null)
                    {
                        var markers = SmiParser.ParseSmiFile(e.Result);
                        if (markers != null
                            && markers.Count > 0)
                        {
                            this.Markers = markers.First().Value;
                        }
                        webContentLoaded = true;
                    }
                };
                client.DownloadStringAsync(Source);
            }
        }
    }
}