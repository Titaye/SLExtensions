namespace SLExtensions.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Windows;
    using System.Windows.Browser;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;
    using System.Windows.Threading;

    [TemplatePart(Name = playElementName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = stopElementName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = inputElementName, Type = typeof(TextBox))]
    [TemplatePart(Name = mediaElementName, Type = typeof(MediaElement))]
    [TemplatePart(Name = showAudioElementName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = showImageElementName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = reloadElementName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = verifyElementName, Type = typeof(FrameworkElement))]
    [TemplateVisualState(Name = audioStateName, GroupName = captchaStatesGroup)]
    [TemplateVisualState(Name = imageStateName, GroupName = captchaStatesGroup)]
    public class ReCaptcha : Control
    {
        #region Fields

        /// <summary>
        /// CaptchaType depedency property.
        /// </summary>
        public static readonly DependencyProperty CaptchaTypeProperty = 
            DependencyProperty.Register(
                "CaptchaType",
                typeof(ReCaptchaType),
                typeof(ReCaptcha),
                new PropertyMetadata((d, e) => ((ReCaptcha)d).OnCaptchaTypeChanged((ReCaptchaType)e.OldValue, (ReCaptchaType)e.NewValue)));

        /// <summary>
        /// ImageSource depedency property.
        /// </summary>
        public static readonly DependencyProperty ImageSourceProperty = 
            DependencyProperty.Register(
                "ImageSource",
                typeof(ImageSource),
                typeof(ReCaptcha),
                null);

        /// <summary>
        /// ImageUrl depedency property.
        /// </summary>
        public static readonly DependencyProperty ImageUrlProperty = 
            DependencyProperty.Register(
                "ImageUrl",
                typeof(string),
                typeof(ReCaptcha),
                new PropertyMetadata((d, e) => ((ReCaptcha)d).OnImageUrlChanged((string)e.OldValue, (string)e.NewValue)));

        /// <summary>
        /// Lang depedency property.
        /// </summary>
        public static readonly DependencyProperty LangProperty = 
            DependencyProperty.Register(
                "Lang",
                typeof(string),
                typeof(ReCaptcha),
                new PropertyMetadata("en", (d, e) => ((ReCaptcha)d).OnLangChanged((string)e.OldValue, (string)e.NewValue)));

        /// <summary>
        /// PublicKey depedency property.
        /// </summary>
        public static readonly DependencyProperty PublicKeyProperty = 
            DependencyProperty.Register(
                "PublicKey",
                typeof(string),
                typeof(ReCaptcha),
                new PropertyMetadata((d, e) => ((ReCaptcha)d).OnPublicKeyChanged((string)e.OldValue, (string)e.NewValue)));

        /// <summary>
        /// Response depedency property.
        /// </summary>
        public static readonly DependencyProperty ResponseProperty = 
            DependencyProperty.Register(
                "Response",
                typeof(string),
                typeof(ReCaptcha),
                new PropertyMetadata((d, e) => ((ReCaptcha)d).OnResponseChanged((string)e.OldValue, (string)e.NewValue)));

        /// <summary>
        /// Verifier depedency property.
        /// </summary>
        public static readonly DependencyProperty VerifierProperty = 
            DependencyProperty.Register(
                "Verifier",
                typeof(IReCaptchaVerifier),
                typeof(ReCaptcha),
                new PropertyMetadata((d, e) => ((ReCaptcha)d).OnVerifierChanged((IReCaptchaVerifier)e.OldValue, (IReCaptchaVerifier)e.NewValue)));

        /// <summary>
        /// WaveUri depedency property.
        /// </summary>
        public static readonly DependencyProperty WaveUriProperty = 
            DependencyProperty.Register(
                "WaveUri",
                typeof(Uri),
                typeof(ReCaptcha),
                new PropertyMetadata((d, e) => ((ReCaptcha)d).OnWaveUriChanged((Uri)e.OldValue, (Uri)e.NewValue)));

        /// <summary>
        /// WaveUrl depedency property.
        /// </summary>
        public static readonly DependencyProperty WaveUrlProperty = 
            DependencyProperty.Register(
                "WaveUrl",
                typeof(string),
                typeof(ReCaptcha),
                new PropertyMetadata((d, e) => ((ReCaptcha)d).OnWaveUrlChanged((string)e.OldValue, (string)e.NewValue)));

        private const string audioStateName = "Audio";
        private const string captchaStatesGroup = "CaptchaStates";
        private const string imageStateName = "Image";
        private const string inputElementName = "inputElement";
        private const string mediaElementName = "mediaElement";
        private const string playElementName = "playElement";
        private const string recaptchaJavascript = @"var Recaptcha = {
        _plugin : null,
        _managedObject : null,

        set_Plugin : function(value)
        {
        _plugin = value;
        },

        get_Plugin : function()
        {
        return _plugin;
        },

        set_ManagedObject : function(name)
        {
        if(_plugin != null)
            _managedObject = _plugin.content[name];
        },

        challenge_callback : function()
        {
        if(_managedObject != null && RecaptchaState)
        {
            _managedObject.OnChallengeReceived(
                RecaptchaState.challenge,
                RecaptchaState.timeout,
                RecaptchaState.server,
                RecaptchaState.site,
                RecaptchaState.error_message,
                RecaptchaState.programming_error,
                RecaptchaState.is_incorrect,
                'image');
        }
        },

        finish_reload: function(new_challenge, type) {
        if(_managedObject != null)
        {
            _managedObject.OnReload(
                new_challenge,
                type);
        }
        },

        };";
        private const string reloadElementName = "reloadElement";
        private const string showAudioElementName = "showAudioElement";
        private const string showImageElementName = "showImageElement";
        private const string stopElementName = "stopElement";
        private const string verifyElementName = "verifyElement";

        private TextBox inputElement;
        private bool isLoaded;
        private RecaptchaState lastState;
        private MediaElement mediaElement;
        private FrameworkElement playElement;
        private ScriptObject recaptchaJSObject;
        private FrameworkElement reloadElement;
        private FrameworkElement showAudioElement;
        private FrameworkElement showImageElement;
        private FrameworkElement stopElement;
        private DispatcherTimer timerReload;
        private FrameworkElement verifyElement;

        #endregion Fields

        #region Constructors

        public ReCaptcha()
        {
            DefaultStyleKey = typeof(ReCaptcha);
            timerReload = new DispatcherTimer();
            timerReload.Tick += new EventHandler(timerReload_Tick);
            this.Loaded += new RoutedEventHandler(ReCaptcha_Loaded);
        }

        #endregion Constructors

        #region Properties

        public Uri ApiServer
        {
            get;
            private set;
        }

        public ReCaptchaType CaptchaType
        {
            get
            {
                return (ReCaptchaType)GetValue(CaptchaTypeProperty);
            }

            set
            {
                SetValue(CaptchaTypeProperty, value);
            }
        }

        public ImageSource ImageSource
        {
            get
            {
                return (ImageSource)GetValue(ImageSourceProperty);
            }

            set
            {
                SetValue(ImageSourceProperty, value);
            }
        }

        public string ImageUrl
        {
            get
            {
                return (string)GetValue(ImageUrlProperty);
            }

            set
            {
                SetValue(ImageUrlProperty, value);
            }
        }

        public string Lang
        {
            get
            {
                return (string)GetValue(LangProperty);
            }

            set
            {
                SetValue(LangProperty, value);
            }
        }

        public string PublicKey
        {
            get
            {
                return (string)GetValue(PublicKeyProperty);
            }

            set
            {
                SetValue(PublicKeyProperty, value);
            }
        }

        public string Response
        {
            get
            {
                return (string)GetValue(ResponseProperty);
            }

            set
            {
                SetValue(ResponseProperty, value);
            }
        }

        public IReCaptchaVerifier Verifier
        {
            get
            {
                return (IReCaptchaVerifier)GetValue(VerifierProperty);
            }

            set
            {
                SetValue(VerifierProperty, value);
            }
        }

        public Uri WaveUri
        {
            get
            {
                return (Uri)GetValue(WaveUriProperty);
            }

            set
            {
                SetValue(WaveUriProperty, value);
            }
        }

        public string WaveUrl
        {
            get
            {
                return (string)GetValue(WaveUrlProperty);
            }

            set
            {
                SetValue(WaveUrlProperty, value);
            }
        }

        #endregion Properties

        #region Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            playElement = GetTemplateChild(playElementName) as FrameworkElement;
            stopElement = GetTemplateChild(stopElementName) as FrameworkElement;
            inputElement = GetTemplateChild(inputElementName) as TextBox;
            showAudioElement = GetTemplateChild(showAudioElementName) as FrameworkElement;
            showImageElement = GetTemplateChild(showImageElementName) as FrameworkElement;
            reloadElement = GetTemplateChild(reloadElementName) as FrameworkElement;
            mediaElement = GetTemplateChild(mediaElementName) as MediaElement;
            verifyElement = GetTemplateChild(verifyElementName) as FrameworkElement;

            if (mediaElement != null)
            {
                ButtonBase btnPlay = playElement as ButtonBase;
                if (btnPlay != null)
                {
                    btnPlay.Click += delegate { Play(); };
                }
                else if (playElement != null)
                {
                    playElement.MouseLeftButtonUp += delegate { Play(); };
                }

                ButtonBase btnStop = stopElement as ButtonBase;
                if (btnStop != null)
                {
                    btnStop.Click += delegate { Stop(); };
                }
                else if (stopElement != null)
                {
                    stopElement.MouseLeftButtonDown += delegate { Stop(); };
                }
            }

            if (inputElement != null)
            {
                inputElement.TextChanged += delegate { Response = inputElement.Text; };
                inputElement.KeyDown += (s, e) =>
                {
                    if (e.Key == Key.Enter)
                        Verify();
                };
            }

            if (showAudioElement != null)
            {
                ButtonBase btnShowAudio = showAudioElement as ButtonBase;
                if (btnShowAudio != null)
                {
                    btnShowAudio.Click += delegate { CaptchaType = ReCaptchaType.Audio; };
                }
                else if (showAudioElement != null)
                {
                    showAudioElement.MouseLeftButtonUp += delegate { CaptchaType = ReCaptchaType.Audio; };
                }
            }

            if (showImageElement != null)
            {
                ButtonBase btnShowImage = showImageElement as ButtonBase;
                if (btnShowImage != null)
                {
                    btnShowImage.Click += delegate { CaptchaType = ReCaptchaType.Image; };
                }
                else if (showImageElement != null)
                {
                    showImageElement.MouseLeftButtonUp += delegate { CaptchaType = ReCaptchaType.Image; };
                }
            }

            if (reloadElement != null)
            {
                ButtonBase btnReload = reloadElement as ButtonBase;
                if (btnReload != null)
                {
                    btnReload.Click += delegate { Reload(); };
                }
                else if (reloadElement != null)
                {
                    reloadElement.MouseLeftButtonUp += delegate { Reload(); };
                }
            }
            if (verifyElement != null)
            {
                ButtonBase btnVerify = verifyElement as ButtonBase;
                if (btnVerify != null)
                {
                    btnVerify.Click += delegate { Verify(); };
                }
                else if (reloadElement != null)
                {
                    verifyElement.MouseLeftButtonUp += delegate { Verify(); };
                }
            }
        }

        [ScriptableMember]
        public void OnChallengeReceived(string challenge
            , int timeout
            , string server
            , string site
            , string error_message
            , string programming_error
            , bool is_incorrect)
        {
            RecaptchaState state = new RecaptchaState();
            state.Challenge = challenge;
            state.Timeout = timeout;
            state.Server = server;
            state.Site = site;
            state.ErrorMessage = error_message;
            state.ProgrammingError = programming_error;
            state.IsIncorrect = is_incorrect;
            lastState = state;
            timerReload.Interval = TimeSpan.FromMilliseconds((state.Timeout - 60 * 5) * 1000);
            timerReload.Start();
            if (CaptchaType == ReCaptchaType.Audio)
            {
                timerReload.Stop();
                Reload(ReCaptchaReloadReason.ChangeTypeAudio);
            }
            else
            {
                SetCaptcha(state, CaptchaType);
            }
        }

        [ScriptableMember]
        public void OnReload(string challenge
            , string type)
        {
            timerReload.Start();
            lastState.Challenge = challenge;
            SetCaptcha(lastState, CaptchaType);
        }

        public void Play()
        {
            if (mediaElement != null)
            {
                mediaElement.Stop();
                mediaElement.Play();
            }
        }

        public void Reload()
        {
            Reload(ReCaptchaReloadReason.User);

            if (inputElement != null)
            {
                inputElement.Text = string.Empty;
            }
        }

        public void Reload(ReCaptchaReloadReason reason)
        {
            if (lastState == null
                || !isLoaded)
                return;

            char creason;
            switch (reason)
            {
                case ReCaptchaReloadReason.Timeout:
                    creason = 't';
                    break;
                default:
                case ReCaptchaReloadReason.User:
                    creason = 'r';
                    break;
                case ReCaptchaReloadReason.ChangeTypeAudio:
                    creason = 'a';
                    break;
                case ReCaptchaReloadReason.ChangeTypeText:
                    creason = 'v';
                    break;
            }

            string url = string.Format("{0}reload?c={1}&k={2}&reason={3}&type={4}&lang={5}"
                , lastState.Server
                , lastState.Challenge
                , lastState.Site
                , creason
                , CaptchaType == ReCaptchaType.Audio ? "audio" : "image"
                , Lang);

            AddScript(new Uri(url));
        }

        public void Stop()
        {
            if (mediaElement != null)
            {
                mediaElement.Stop();
            }
        }

        public void Verify()
        {
            Verify(Response);
        }

        public void Verify(string response)
        {
            if (lastState == null
                || Verifier == null)
                return;

            Verifier.Verify((response ?? string.Empty).Trim(), lastState.Challenge);
        }

        private void AddScript(Uri scriptUrl)
        {
            if (System.ComponentModel.DesignerProperties.IsInDesignTool)
                return;

            HtmlElement scriptElement = HtmlPage.Document.CreateElement("script");
            scriptElement.SetAttribute("type", "text/javascript");
            scriptElement.SetAttribute("src", scriptUrl.ToString());

            HtmlElement parent = null;
            ScriptObjectCollection headers = HtmlPage.Document.GetElementsByTagName("head");
            if (headers != null && headers.Count > 0)
            {
                parent = headers[0] as HtmlElement;
            }

            if (parent == null)
            {
                parent = HtmlPage.Document.Body;
            }
            parent.AppendChild(scriptElement);
        }

        private void AddScriptContent(string script)
        {
            if (System.ComponentModel.DesignerProperties.IsInDesignTool)
                return;

            HtmlElement scriptElement = HtmlPage.Document.CreateElement("script");
            scriptElement.SetProperty("text", script);
            scriptElement.SetAttribute("type", "text/javascript");

            HtmlElement parent = null;
            ScriptObjectCollection headers = HtmlPage.Document.GetElementsByTagName("head");
            if (headers != null && headers.Count > 0)
            {
                parent = headers[0] as HtmlElement;
            }

            if (parent == null)
            {
                parent = HtmlPage.Document.Body;
            }
            parent.AppendChild(scriptElement);
        }

        private void Challenge()
        {
            if (!isLoaded)
                return;

            Uri challengeUri = new Uri(ApiServer, string.Format("/challenge?k={0}&ajax=1&cachestop={1}", PublicKey, Guid.NewGuid()));
            AddScript(challengeUri);
        }

        /// <summary>
        /// handles the CaptchaTypeProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnCaptchaTypeChanged(ReCaptchaType oldValue, ReCaptchaType newValue)
        {
            if (newValue == ReCaptchaType.Image)
                VisualStateManager.GoToState(this, imageStateName, true);
            else
                VisualStateManager.GoToState(this, audioStateName, true);

            ImageUrl = null;
            WaveUrl = null;
            Reload(newValue == ReCaptchaType.Audio ? ReCaptchaReloadReason.ChangeTypeAudio : ReCaptchaReloadReason.ChangeTypeText);
        }

        /// <summary>
        /// handles the ImageUrlProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnImageUrlChanged(string oldValue, string newValue)
        {
            if (string.IsNullOrEmpty(newValue))
                ImageSource = null;
            else
                ImageSource = new BitmapImage(new Uri(newValue));
        }

        /// <summary>
        /// handles the LangProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnLangChanged(string oldValue, string newValue)
        {
            Reload(ReCaptchaReloadReason.Timeout);
        }

        /// <summary>
        /// handles the PublicKeyProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnPublicKeyChanged(string oldValue, string newValue)
        {
            Challenge();
        }

        /// <summary>
        /// handles the ResponseProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnResponseChanged(string oldValue, string newValue)
        {
        }

        /// <summary>
        /// handles the VerifierProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnVerifierChanged(IReCaptchaVerifier oldValue, IReCaptchaVerifier newValue)
        {
            if (oldValue != null)
            {
                oldValue.Failed -= new EventHandler<MessageEventArgs>(verifier_Failed);
            }
            if (newValue != null)
            {
                newValue.Failed += new EventHandler<MessageEventArgs>(verifier_Failed);
            }
        }

        /// <summary>
        /// handles the WaveUriProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnWaveUriChanged(Uri oldValue, Uri newValue)
        {
        }

        /// <summary>
        /// handles the WaveUrlProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnWaveUrlChanged(string oldValue, string newValue)
        {
            Uri uri;
            if (Uri.TryCreate(newValue, UriKind.RelativeOrAbsolute, out uri))
                WaveUri = uri;
            else
                WaveUri = null;
        }

        void ReCaptcha_Loaded(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, imageStateName, false);

            if (System.ComponentModel.DesignerProperties.IsInDesignTool)
                return;

            AddScriptContent(recaptchaJavascript);

            recaptchaJSObject = HtmlPage.Window.GetProperty("Recaptcha") as ScriptObject;
            recaptchaJSObject.Invoke("set_Plugin", HtmlPage.Plugin);

            HtmlPage.RegisterScriptableObject("Recaptcha", this);
            recaptchaJSObject.Invoke("set_ManagedObject", "Recaptcha");

            isLoaded = true;
            RefreshApiServer();
            if (!string.IsNullOrEmpty(PublicKey))
                Challenge();
        }

        private void RefreshApiServer()
        {
            Uri url;
            if (Application.Current.Host.Source.Scheme == Uri.UriSchemeHttps)
            {
                url = new Uri("https://api-secure.recaptcha.net/");
            }
            else
            {
                url = new Uri("http://api.recaptcha.net/");
            }
            ApiServer = url;
        }

        private void SetCaptcha(RecaptchaState state, ReCaptchaType type)
        {
            if (string.IsNullOrEmpty(state.Challenge))
            {
                WaveUrl = null;
                ImageUrl = null;
            }
            else
            {
                if (type == ReCaptchaType.Image)
                {
                    ImageUrl = state.Server + "image?c=" + state.Challenge;
                    WaveUrl = null;
                }
                else
                {
                    ImageUrl = null;
                    WaveUrl = (state.Server + "image?c=" + state.Challenge).Replace("https://", "http://");
                }
            }
        }

        void timerReload_Tick(object sender, EventArgs e)
        {
            timerReload.Stop();
            Reload(ReCaptchaReloadReason.Timeout);
        }

        void verifier_Failed(object sender, MessageEventArgs e)
        {
            Reload();

            if (inputElement != null)
            {
                inputElement.Focus();
            }
        }

        #endregion Methods

        #region Nested Types

        private class RecaptchaState
        {
            #region Properties

            public string Challenge
            {
                get;
                set;
            }

            public string ErrorMessage
            {
                get;
                set;
            }

            public bool IsIncorrect
            {
                get;
                set;
            }

            public string ProgrammingError
            {
                get;
                set;
            }

            public string Server
            {
                get;
                set;
            }

            public string Site
            {
                get;
                set;
            }

            public int Timeout
            {
                get;
                set;
            }

            #endregion Properties
        }

        #endregion Nested Types
    }
}
