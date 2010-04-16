namespace SLExtensions.Showcase
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;

    public partial class PageSliderGauge : UserControl
    {
        #region Constructors

        public PageSliderGauge()
        {
            InitializeComponent();
            this.Loaded += PageSliderGauge_Loaded;
        }

        #endregion Constructors

        #region Methods

        void PageSliderGauge_Loaded(object sender, RoutedEventArgs e)
        {
            txtPercent1.Text = slider1.Percentage.ToString("p");
            txtPercent2.Text = slider2.Percentage.ToString("p");
            txtPercent3.Text = slider3.Percentage.ToString("p");
        }

        private void SliderGauge_PercentChanged1(object sender, Controls.GaugePercentageChangedEventArgs e)
        {
            txtPercent1.Text = e.Percentage.ToString("p");
        }

        private void SliderGauge_PercentChanged2(object sender, Controls.GaugePercentageChangedEventArgs e)
        {
            txtPercent2.Text = e.Percentage.ToString("p");
        }

        private void SliderGauge_PercentChanged3(object sender, Controls.GaugePercentageChangedEventArgs e)
        {
            txtPercent3.Text = e.Percentage.ToString("p");
        }

        #endregion Methods
    }
}