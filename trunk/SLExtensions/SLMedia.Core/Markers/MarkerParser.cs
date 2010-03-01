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

    public static class MarkerParser
    {
        #region Constructors

        static MarkerParser()
        {
            RegisteredFileParsers = new Dictionary<string, IMarkerParser>(StringComparer.OrdinalIgnoreCase);
            RegisteredFileParsers.Add(".srt", new SrtParser());
            RegisteredFileParsers.Add(".smi", new SmiParser());
        }

        #endregion Constructors

        #region Properties

        public static Dictionary<string, IMarkerParser> RegisteredFileParsers
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public static void ParseSelectorAsync(Uri filesource, IDictionary<string, object> metadata, Action<IEnumerable<IMarkerSelector>> callback)
        {
            var fileExt = System.IO.Path.GetExtension(filesource.LocalPath);
            IMarkerParser parser;
            if (callback == null
                || !RegisteredFileParsers.TryGetValue(fileExt, out parser))
                return;

            WebClient client = new WebClient();

            client.DownloadStringCompleted += (snd, e) =>
            {
                callback(parser.Parse(e.Result, metadata));
            };
            client.DownloadStringAsync(filesource);
        }

        #endregion Methods
    }
}