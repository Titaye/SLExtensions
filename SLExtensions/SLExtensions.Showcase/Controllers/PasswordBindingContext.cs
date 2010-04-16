namespace SLExtensions.Showcase.Controllers
{
    using System;
    using System.ComponentModel;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    public class PasswordBindingContext : INotifyPropertyChanged
    {
        #region Fields

        private string _text;

        #endregion Fields

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Properties

        public string Text
        {
            get { return _text; }
            set
            {
                if (_text != value)
                {
                    _text = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("Text"));
                }
            }
        }

        #endregion Properties
    }
}