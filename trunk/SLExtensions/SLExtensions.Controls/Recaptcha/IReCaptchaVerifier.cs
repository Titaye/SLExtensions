namespace SLExtensions.Controls
{
    using System;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    public interface IReCaptchaVerifier
    {
        #region Events

        event EventHandler<MessageEventArgs> Failed;

        event EventHandler Success;

        #endregion Events

        #region Methods

        void Verify(string response, string challenge);

        #endregion Methods
    }
}