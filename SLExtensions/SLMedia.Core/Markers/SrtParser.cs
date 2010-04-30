namespace SLMedia.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    using SLMedia.Core;

    public class SrtParser : IMarkerParser
    {
        #region Fields

        private const string contentGroupName = "content";
        private const string endGroupName = "end";
        private const string srtPattern = @"^\d+\r?\n(?<start>\d\d:\d\d:\d\d(,\d\d\d)?)\s--\>\s(?<end>\d\d:\d\d:\d\d(,\d\d\d)?)\r?\n(?<content>(.+\n?)*)";
        private const string srtSplitPattern = @"(\r?\n){2}";

        //private const string strPattern = @"^\d+\r?\n((?<time>\d\d:\d\d:\d\d(,\d\d\d)?)(?:\s--\>\s)?){2}\r?\n(?<content>.*)(\r?\n){2}";
        private const string startGroupName = "start";

        private static readonly Regex srtRegex = new Regex(srtPattern, RegexOptions.Multiline | RegexOptions.ExplicitCapture);
        private static readonly Regex srtSplitRegex = new Regex(srtSplitPattern, RegexOptions.Multiline | RegexOptions.ExplicitCapture);

        #endregion Fields

        #region Methods

        public static SrtMarker[] ParseSrtFile(string content)
        {
            TimeSpan start = TimeSpan.Zero;
            TimeSpan end = TimeSpan.Zero;

            var r = from s in srtSplitRegex.Split(content)
                    from m in srtRegex.Matches(s).OfType<Match>()
                    where TimeSpan.TryParse(m.Groups[startGroupName].Value.Replace(',', '.'), out start)
                    && TimeSpan.TryParse(m.Groups[endGroupName].Value.Replace(',', '.'), out end)
                    select new SrtMarker { Position = start, Duration = end - start, Content = m.Groups[contentGroupName].Value.TrimEnd('\r', '\n') };

            return r.ToArray();
        }

        public IEnumerable<IMarkerSelector> Parse(string content, IDictionary<string, object> metadata)
        {
            var selector = new MarkerSelector();
            if (metadata != null)
            {
                foreach (var md in metadata)
                {
                    selector.Metadata.Add(md.Key, md.Value);
                }
            }

            foreach (var mrk in ParseSrtFile(content))
            {
                selector.Markers.Add(mrk);
            }

            return new IMarkerSelector[] { selector };
        }

        #endregion Methods
    }
}