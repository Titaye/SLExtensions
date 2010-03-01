namespace SLExtensions.Sharepoint
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

    public class CancelSharepointItemEventArgs : CancelEventArgs
    {
        #region Constructors

        public CancelSharepointItemEventArgs(TemplateDataBase item, SharepointList list)
        {
            this.Item = item;
            this.List = list;
        }

        #endregion Constructors

        #region Properties

        public TemplateDataBase Item
        {
            get; private set;
        }

        public SharepointList List
        {
            get; private set;
        }

        #endregion Properties
    }

    public class SharepointItemEventArgs : EventArgs
    {
        #region Fields

        private Action asyncCallback;

        #endregion Fields

        #region Constructors

        public SharepointItemEventArgs(TemplateDataBase item, SharepointList list, Action asyncCallback)
        {
            this.Item = item;
            this.List = list;
            this.asyncCallback = asyncCallback;
        }

        #endregion Constructors

        #region Properties

        public TemplateDataBase Item
        {
            get; private set;
        }

        public SharepointList List
        {
            get; private set;
        }

        public bool UseReadyCallback
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public void Ready()
        {
            if (this.asyncCallback != null)
            {
                UseReadyCallback = false;
                this.asyncCallback();
            }
        }

        #endregion Methods
    }
}