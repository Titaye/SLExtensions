namespace SLExtensions.Controls.Tween
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

    public class TweenParameter
    {
        #region Properties

        public TimeSpan? Duration
        {
            get; set;
        }

        public double From
        {
            get; set;
        }

        public PropertyPath Property
        {
            get; set;
        }

        public double To
        {
            get; set;
        }

        public EquationType? Type
        {
            get; set;
        }

        #endregion Properties
    }
}