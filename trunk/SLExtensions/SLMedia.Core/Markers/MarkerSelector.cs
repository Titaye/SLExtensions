namespace SLMedia.Core
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

    public class MarkerSelector : NotifyingObject, IMarkerSelector
    {
        #region Fields

        private IMarker activeMarker;
        private bool isActive;
        private bool isEnabled;

        #endregion Fields

        #region Constructors

        public MarkerSelector()
        {
            Markers = new List<IMarker>();
            Metadata = new Dictionary<string, object>();
        }

        #endregion Constructors

        #region Events

        public event EventHandler IsActiveChanged;

        #endregion Events

        #region Properties

        public IMarker ActiveMarker
        {
            get { return this.activeMarker; }
            set
            {
                if (this.activeMarker != value)
                {
                    this.activeMarker = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.ActiveMarker));
                }
            }
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
                    OnIsActiveChanged();
                }
            }
        }

        public bool IsEnabled
        {
            get { return this.isEnabled; }
            set
            {
                if (this.isEnabled != value)
                {
                    this.isEnabled = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.IsEnabled));
                }
            }
        }

        public IList<IMarker> Markers
        {
            get;
            private set;
        }

        public IDictionary<string, object> Metadata
        {
            get;
            private set;
        }

        #endregion Properties

        #region Methods

        protected virtual void OnIsActiveChanged()
        {
            if (IsActiveChanged != null)
            {
                IsActiveChanged(this, EventArgs.Empty);
            }
        }

        #endregion Methods
    }
}