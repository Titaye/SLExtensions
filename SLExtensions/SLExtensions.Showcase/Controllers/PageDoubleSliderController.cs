namespace SLExtensions.Showcase.Controllers
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

    public class PageDoubleSliderController : NotifyingObject
    {
        #region Fields

        private double maxValue;
        private double minValue;

        #endregion Fields

        #region Properties

        public double MaxValue
        {
            get { return maxValue; }
            set
            {
                if (maxValue != value)
                {
                    maxValue = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.MaxValue));
                }
            }
        }

        public double MinValue
        {
            get { return minValue; }
            set
            {
                if (minValue != value)
                {
                    minValue = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.MinValue));
                }
            }
        }

        #endregion Properties
    }
}
