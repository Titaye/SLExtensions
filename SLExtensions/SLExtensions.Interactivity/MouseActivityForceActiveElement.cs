﻿namespace SLExtensions.Interactivity
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

    public class MouseActivityForceActiveElement
    {
        #region Properties

        public FrameworkElement Element
        {
            get; set;
        }

        public string ElementName
        {
            get; set;
        }

        #endregion Properties
    }
}