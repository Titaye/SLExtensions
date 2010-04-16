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

    public class MessageEventArgs : EventArgs
    {
        #region Constructors

        public MessageEventArgs(string message)
        {
            this.Message = message;
        }

        #endregion Constructors

        #region Properties

        public string Message
        {
            get; private set;
        }

        #endregion Properties
    }
}