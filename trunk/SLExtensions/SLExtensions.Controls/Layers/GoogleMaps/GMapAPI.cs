namespace SLExtensions.Controls.Layers.GoogleMaps
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Browser;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.Windows.Threading;

    public class GMapAPI
    {
        #region Fields

        private static GMapAPI instance;

        private ScriptObject gload;
        private bool isReady = false;
        private DispatcherTimer timer = new DispatcherTimer();

        #endregion Fields

        #region Constructors

        public GMapAPI()
        {
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += new EventHandler(timer_Tick);

            gload = (ScriptObject)HtmlPage.Window.GetProperty("GLoad");
            if (gload == null)
            {
                HtmlElement scriptElement = HtmlPage.Document.CreateElement("script");
                scriptElement.SetAttribute("type", "text/javascript");

                string src = "http://maps.google.com/maps?file=api&v=2";
                scriptElement.SetAttribute("src", src);
                HtmlPage.Document.Body.AppendChild(scriptElement);
                gload = (ScriptObject)HtmlPage.Window.GetProperty("GLoad");
                if (gload == null)
                {
                    timer.Start();
                }
                else
                {
                    SetReady();
                }
            }
        }

        #endregion Constructors

        #region Events

        public event EventHandler Ready
        {
            add
            {
                if (!isReady)
                {
                    // If not ready, store event
                    readyEvent += value;
                }
                else
                {
                    // If ready, raise the event
                    value(this, EventArgs.Empty);
                }
            }
            remove
            {
                readyEvent -= value;
            }
        }

        private event EventHandler readyEvent;

        #endregion Events

        #region Properties

        public static GMapAPI Instance
        {
            get
            {
                Initialize();
                return instance;
            }
        }

        public string[] DefaultMapUrls
        {
            get; private set;
        }

        public string[] RoadOnlyMapUrls
        {
            get; private set;
        }

        public string[] SatelliteMapUrls
        {
            get; private set;
        }

        public string[] TerrainUrls
        {
            get; private set;
        }

        #endregion Properties

        #region Methods

        // http://maps.google.com/maps?file=api&v=2
        public static void Initialize()
        {
            if (instance == null)
                instance = new GMapAPI();
        }

        private string ReplaceHexa(Match match)
        {
            char c = (char)int.Parse(match.Groups["hexa"].Value, System.Globalization.NumberStyles.HexNumber);
            return c.ToString();
        }

        private void SetReady()
        {
            isReady = true;
            if (readyEvent != null)
            {
                readyEvent(this, EventArgs.Empty);
            }
            string gloadContent = (string)gload.Invoke("toString");

            Regex urlsRegex = new Regex(@"\[[^\]]*", RegexOptions.Multiline);
            Regex url = new Regex("\"(?<url>[^\"]*)\",?");
            Regex hexa = new Regex("\\\\x(?<hexa>[0-9a-fA-F]{2})");
            Regex lng = new Regex(@"&hl=\w\w");
            var urlGroups = (from m in urlsRegex.Matches(gloadContent).OfType<Match>()
                             select m.Value).ToList();
            if (urlGroups.Count >= 4)
            {
                DefaultMapUrls = (from urlMatches in url.Matches(urlGroups[0]).OfType<Match>()
                                  select lng.Replace(hexa.Replace(urlMatches.Groups["url"].Value, ReplaceHexa), "")).ToArray();

                SatelliteMapUrls = (from urlMatches in url.Matches(urlGroups[1]).OfType<Match>()
                                    select lng.Replace(hexa.Replace(urlMatches.Groups["url"].Value, ReplaceHexa), "")).ToArray();

                RoadOnlyMapUrls = (from urlMatches in url.Matches(urlGroups[2]).OfType<Match>()
                                   select lng.Replace(hexa.Replace(urlMatches.Groups["url"].Value, ReplaceHexa), "")).ToArray();

                TerrainUrls = (from urlMatches in url.Matches(urlGroups[3]).OfType<Match>()
                               select lng.Replace(hexa.Replace(urlMatches.Groups["url"].Value, ReplaceHexa), "")).ToArray();

            }
            //from urlMatches in url.Matches(grp).OfType<Match>()
            //select urlMatches.Groups["url"];

            //apiCallback\((\s*\[(?<urls1>(\s*"[^"]*",)*|[^\]]*))\],|[^,]*,)*
        }

        void timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            gload = (ScriptObject)HtmlPage.Window.GetProperty("GLoad");
            if (gload == null)
            {
                timer.Start();
            }
            else
            {
                SetReady();
            }
        }

        #endregion Methods
    }
}