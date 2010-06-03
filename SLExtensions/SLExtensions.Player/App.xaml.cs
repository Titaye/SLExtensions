namespace SLExtensions.Player
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Windows;
    using System.Windows.Browser;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;

    using SLExtensions.Controls.Animation;
    using SLExtensions.Input;

    using SLMedia.Core;
    using SLMedia.PlaylistProvider.MediaRSS;
    using SLMedia.PlaylistProvider.MSNVideo;

    public partial class App : Application
    {
        #region Fields

        private IDictionary<string, string> initParams;
        private MediaController mediaController;
        private string provider;
        private IDictionary<string, IEnumerable<string>> queryParameters;
        private string sourceRequestParam;
        private IEnumerable<string> sources;

        #endregion Fields

        #region Constructors

        public App()
        {
            this.Startup += this.Application_Startup;
            this.Exit += this.Application_Exit;
            this.UnhandledException += this.Application_UnhandledException;

            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        private void Application_Exit(object sender, EventArgs e)
        {
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            FrameworkElement currentPlayer = null;
            string queryString = HtmlPage.Document.DocumentUri.Query;
            queryParameters = new Dictionary<string, IEnumerable<string>>();
            if (!string.IsNullOrEmpty(queryString))
            {
                queryString = queryString.TrimStart('?');
                var r = from s in queryString.Split('&')
                        let parts = s.Split('=').Select(str => HttpUtility.UrlDecode(str)).ToArray()
                        select new
                        {
                            k = parts.Length == 1 ? string.Empty : parts.First(),
                            val = parts.Length == 1 ? (IEnumerable<string>)parts : parts.Skip(1)
                        };

                queryParameters = r.ToDictionary(i => i.k, s => s.val);
            }

            initParams = new Dictionary<string, string>();
            if (e.InitParams != null)
            {
                initParams = e.InitParams;
                string playerType;

                // Get query string parameters for passing mediaitem to the player
                e.InitParams.TryGetValue("sourceRequestParam", out sourceRequestParam);

                // Initialize provider for retreiving or parsing a playlist
                e.InitParams.TryGetValue("provider", out provider);

                // Get the player type from initparams or query string
                if (!e.InitParams.TryGetValue("player", out playerType))
                {
                    IEnumerable<string> playerTypes;
                    if (queryParameters.TryGetValue("player", out playerTypes))
                        playerType = playerTypes.FirstOrDefault();
                }

                // get mediaitem or playlist sources
                string url;
                if (initParams.TryGetValue("url", out url))
                {
                    sources = new string[] { url };
                }
                else
                {
                    queryParameters.TryGetValue(sourceRequestParam, out sources);
                }

                // Initialise player from its type
                if (!string.IsNullOrEmpty(playerType))
                {
                    switch (playerType.ToLower())
                    {
                        case "videosimple":
                            {
                                VideoPlayer player = new VideoPlayer();
                                currentPlayer = player;
                                mediaController = player.Controller;
                                player.Loaded += new RoutedEventHandler(player_Loaded);
                                //Load ScriptCommands
                                string strScriptCommandsUrl;
                                if (initParams.TryGetValue("scriptcommandsurl", out strScriptCommandsUrl))
                                {
                                    mediaController.ScriptCommandsUrl = strScriptCommandsUrl;
                                }
                            } break;
                        case "videowebslice":
                            {
                                VideoPlayerWebSlice player = new VideoPlayerWebSlice();
                                currentPlayer = player;
                                mediaController = player.Controller;
                                player.Loaded += new RoutedEventHandler(player_Loaded);
                                //Load ScriptCommands
                                string strScriptCommandsUrl;
                                if (initParams.TryGetValue("scriptcommandsurl", out strScriptCommandsUrl))
                                {
                                    mediaController.ScriptCommandsUrl = strScriptCommandsUrl;
                                }

                                string videoLinkUrl;
                                if (initParams.TryGetValue("videolinkurl", out videoLinkUrl))
                                {
                                    player.VideoLinkUri = new Uri(videoLinkUrl);
                                }

                            } break;
                        case "video":
                            {
                                if (string.IsNullOrEmpty(provider))
                                {
                                    // No provider, we assume the url given are linked to media items instead of playlists
                                    if (sources.Count() <= 1)
                                    {
                                        VideoPlayer player = new VideoPlayer();
                                        currentPlayer = player;
                                        mediaController = player.Controller;
                                        player.Loaded += new RoutedEventHandler(player_Loaded);
                                        //Load ScriptCommands
                                        string strScriptCommandsUrl;
                                        if (initParams.TryGetValue("scriptcommandsurl", out strScriptCommandsUrl))
                                        {
                                            mediaController.ScriptCommandsUrl = strScriptCommandsUrl;
                                        }

                                    }
                                    else
                                    {
                                        VideoPlayerPlaylist player = new VideoPlayerPlaylist();
                                        currentPlayer = player;
                                        mediaController = player.Controller;
                                        player.Loaded += new RoutedEventHandler(player_Loaded);

                                    }

                                }
                                else
                                {
                                    VideoPlayerPlaylist player = new VideoPlayerPlaylist();
                                    currentPlayer = player;
                                    mediaController = player.Controller;
                                    player.Loaded += new RoutedEventHandler(player_Loaded);
                                    //Load ScriptCommands
                                    string strScriptCommandsUrl;
                                    if (initParams.TryGetValue("scriptcommandsurl", out strScriptCommandsUrl))
                                    {
                                        mediaController.ScriptCommandsUrl = strScriptCommandsUrl;
                                    }
                                }
                            } break;
                        case "photo":
                            {
                                PhotoPlayer player = new PhotoPlayer();
                                currentPlayer = player;
                                mediaController = player.Controller;
                                player.Loaded += new RoutedEventHandler(player_Loaded);
                            } break;
                        case "deepzoom":
                            {
                                DeepZoomPlayer player = new DeepZoomPlayer();
                                currentPlayer = player;
                                mediaController = player.Controller;
                                player.Loaded += new RoutedEventHandler(player_Loaded);
                            } break;
                    }
                }
            }

            if (currentPlayer != null)
                this.RootVisual = currentPlayer;
        }

        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            // If the app is running outside of the debugger then report the exception using
            // the browser's exception mechanism. On IE this will display it a yellow alert
            // icon in the status bar and Firefox will display a script error.
            if (!System.Diagnostics.Debugger.IsAttached)
            {

                // NOTE: This will allow the application to continue running after an exception has been thrown
                // but not handled.
                // For production applications this error handling should be replaced with something that will
                // report the error to the website and stop the application.
                e.Handled = true;

                try
                {
                    string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
                    errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

                    System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight 2 Application " + errorMsg + "\");");
                }
                catch (Exception)
                {
                }
            }
        }

        void player_Loaded(object sender, RoutedEventArgs e)
        {
            if (mediaController != null)
            {
                if (!string.IsNullOrEmpty(provider))
                {
                    switch (provider.ToLower())
                    {
                        case "mediarss":
                            {
                                MediaRssProvider rssProvider = new MediaRssProvider();
                                string url;
                                Uri uri;
                                if (initParams.TryGetValue("url", out url)
                                    && Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out uri))
                                {
                                    rssProvider.ContentSource = uri;
                                    mediaController.PlaylistSource = rssProvider;
                                }
                            }
                            break;
                        case "msnvideo":
                            {
                                MSNVideoProvider msnProvider = new MSNVideoProvider();
                                string url;
                                Uri uri;
                                if (initParams.TryGetValue("url", out url) && Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out uri))
                                {
                                    msnProvider.ContentSource = uri;
                                    mediaController.PlaylistSource = msnProvider;
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
                else
                {

                    foreach (var source in sources)
                    {
                        SLMedia.Core.MediaItem item = new SLMedia.Core.MediaItem();
                        item.Source = source;
                        mediaController.Playlist.Add(item);
                        mediaController.CurrentItem = item;
                    }

                    //if (mediaController is DeepZoomController)
                    //{

                    //}
                    //else
                    //{
                    //}
                }
            }
        }

        #endregion Methods
    }
}