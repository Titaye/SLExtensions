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

    public class SharepointItemValueEventArgs : EventArgs
    {
        #region Constructors

        public SharepointItemValueEventArgs(TemplateDataBase item, params string[] keys)
        {
            this.Item = item;
            this.Keys = keys;
        }

        #endregion Constructors

        #region Properties

        public TemplateDataBase Item
        {
            get; private set;
        }

        public string[] Keys
        {
            get; private set;
        }

        #endregion Properties
    }
}