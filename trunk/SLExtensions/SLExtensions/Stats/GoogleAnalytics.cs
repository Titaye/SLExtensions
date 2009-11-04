namespace SLExtensions.Stats
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Windows;
    using System.Windows.Browser;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    public static class GoogleAnalytics
    {
        #region Fields

        private static ScriptObject gat;
        private static Dictionary<string, ScriptObject> trackers = new Dictionary<string, ScriptObject>();

        #endregion Fields

        #region Methods

        public static void TrackPageView(string trackerId, string url)
        {
            if (string.IsNullOrEmpty(trackerId))
                return;

            EnsureInitialized();
            ScriptObject tracker = EnsureTracker(trackerId);
            if (tracker != null)
            {
                tracker.Invoke("_trackPageview", url);
            }
        }

        private static void EnsureInitialized()
        {
            gat = (ScriptObject)HtmlPage.Window.GetProperty("_gat");
            if (gat == null)
            {
                HtmlElement scriptElement = HtmlPage.Document.CreateElement("script");
                scriptElement.SetAttribute("type", "text/javascript");

                string host = HtmlPage.Document.DocumentUri.Scheme == Uri.UriSchemeHttps ? "https://ssl." : "http://www.";
                string src = host + "google-analytics.com/ga.js";
                scriptElement.SetAttribute("src", src);
                HtmlPage.Document.Body.AppendChild(scriptElement);
                gat = (ScriptObject)HtmlPage.Window.GetProperty("_gat");
            }
        }

        private static ScriptObject EnsureTracker(string trackerId)
        {
            ScriptObject tracker = null;
            if (gat != null && !trackers.TryGetValue(trackerId, out tracker))
            {
                tracker = (ScriptObject)gat.Invoke("_getTracker", trackerId);
                trackers[trackerId] = tracker;
            }
            return tracker;
        }

        #endregion Methods
    }
}