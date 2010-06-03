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
    using System.Windows.Browser;

    [ScriptableType]
    public class MarkerContent : Marker
    {
        #region Properties
        [ScriptableMember]
        public string Content
        {
            get; set;
        }

        #endregion Properties
    }
}