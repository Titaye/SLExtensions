namespace SLMedia.Core
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

    public class WebClientMarkerSource : IMarkerSource
    {
        #region Properties

        public IDictionary<string, object> Metadata
        {
            get; set;
        }

        public IMarkerParser Parser
        {
            get; set;
        }

        public Uri Source
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public void LoadAsync(Action<System.Collections.Generic.IEnumerable<IMarkerSelector>> endLoadCallback
            , IDictionary<string, object> metadata)
        {
            if (Source == null)
                return;

            IMarkerParser parser = Parser;
            if (Parser == null)
            {
                Uri src = Source;
                if (!src.IsAbsoluteUri)
                    src = new Uri(new Uri("http://test/"), Source);

                var ext = System.IO.Path.GetExtension(src.LocalPath);
                MarkerParser.RegisteredFileParsers.TryGetValue(ext, out parser);
            }

            if (parser != null)
            {
                WebClient client = new WebClient();
                client.DownloadStringCompleted += (snd, e) =>
                {
                    if (endLoadCallback != null)
                    {

                        endLoadCallback(parser.Parse(e.Result, metadata));
                    }
                };
                client.DownloadStringAsync(Source);
            }
        }

        #endregion Methods
    }
}