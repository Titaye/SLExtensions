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

    public class ReCaptchaResponse : NotifyingObject
    {
        #region Fields

        private string response;

        #endregion Fields

        #region Properties

        public string Response
        {
            get { return response; }
            set
            {
                if (response != value)
                {
                    response = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.Response));
                }
            }
        }

        #endregion Properties
    }
}