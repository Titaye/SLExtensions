namespace SLMedia.Core
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

    public class Marker : NotifyingObject, IMarker
    {
        #region Fields

        private bool isActive;

        #endregion Fields

        #region Properties

        public Duration Duration
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

        public TimeSpan Position
        {
            get; set;
        }

        #endregion Properties
    }
}