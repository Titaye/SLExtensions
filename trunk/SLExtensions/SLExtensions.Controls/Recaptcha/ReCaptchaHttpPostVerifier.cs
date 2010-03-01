namespace SLExtensions.Controls
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

    using SLExtensions;

    public class ReCaptchaHttpPostVerifier : IReCaptchaVerifier
    {
        #region Events

        public event EventHandler<MessageEventArgs> Failed;

        public event EventHandler Success;

        #endregion Events

        #region Properties

        public string Url
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public void Verify(string response, string challenge)
        {
            Uri url;
            if (!Uri.TryCreate(Url, UriKind.RelativeOrAbsolute, out url))
                return;

            WebClient wc = new WebClient();
            wc.UploadStringCompleted += new UploadStringCompletedEventHandler(wc_UploadStringCompleted);
            wc.SendHtmlForm(url,
                new KeyValuePair<string, string>[]
                {
                    new KeyValuePair<string, string>("challenge", challenge),
                    new KeyValuePair<string, string>("response", response)
                });
        }

        protected virtual void OnFailed(string error)
        {
            if (Failed != null)
            {
                Failed(this, new MessageEventArgs(error));
            }
        }

        protected virtual void OnSuccess()
        {
            if (Success != null)
            {
                Success(this, EventArgs.Empty);
            }
        }

        void wc_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            string result = e.Result;
            string error = string.Empty;
            if (!string.IsNullOrEmpty(result))
            {
                string[] lines = result.Split('\n');
                if (lines.Length > 0 && StringComparer.OrdinalIgnoreCase.Compare(lines[0], "true") == 0)
                    OnSuccess();

                if (lines.Length > 1)
                    error = lines[1];
            }
            OnFailed(error);
        }

        #endregion Methods
    }
}