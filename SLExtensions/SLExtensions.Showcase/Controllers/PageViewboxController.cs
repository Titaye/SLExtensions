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

    public class PageViewboxController : NotifyingObject
    {
        #region Fields

        private Stretch stretchValue;

        #endregion Fields

        #region Properties

        public Stretch SelectedStretchValue
        {
            get { return stretchValue; }
            set
            {
                if (stretchValue != value)
                {
                    stretchValue = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.SelectedStretchValue));
                }
            }
        }

        public Stretch[] StrechValues
        {
            get
            {
                return new Stretch[] {
                    Stretch.None,
                    Stretch.Fill,
                    Stretch.Uniform,
                    Stretch.UniformToFill
                };
            }
        }

        #endregion Properties
    }
}
