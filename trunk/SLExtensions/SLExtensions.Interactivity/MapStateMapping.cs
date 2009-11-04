namespace SLExtensions.Interactivity
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

    public class MapStateMapping
    {
        #region Properties

        public bool Else
        {
            get; set;
        }

        public bool IsNotNull
        {
            get; set;
        }

        public string StateName
        {
            get; set;
        }

        public string Value
        {
            get; set;
        }

        #endregion Properties
    }
}