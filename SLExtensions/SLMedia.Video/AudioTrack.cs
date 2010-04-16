namespace SLMedia.Video
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

    using SLExtensions;

    public class AudioTrack : NotifyingObject
    {
        #region Fields

        private bool isActive;

        #endregion Fields

        #region Properties

        public int Index
        {
            get; set;
        }

        public bool IsActive
        {
            get { return this.isActive; }
            set
            {
                if (this.isActive != value)
                {
                    this.isActive = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.IsActive));
                }
            }
        }

        public string Title
        {
            get; set;
        }

        #endregion Properties
    }
}