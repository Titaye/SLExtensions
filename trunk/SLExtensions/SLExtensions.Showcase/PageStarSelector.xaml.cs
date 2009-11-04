using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SLExtensions.Showcase
{
    public partial class PageStarSelector : UserControl
    {
        public PageStarSelector()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(Page_Loaded);
        }

        void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Slider1.Value = StarSelector1.DisplayValue;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            StarSelector1.DisplayValue = e.NewValue;
            SelectorValue.Text = e.NewValue.ToString("0.00");
        }

        private void StarSelector1_ValueChanged(object sender, RoutedEventArgs e)
        {
            Slider1.Value = StarSelector1.Value;
        }

        private void AllowHalfStarSelection_Click(object sender, RoutedEventArgs e)
        {
            StarSelector1.AllowHalfStarSelection = AllowHalfStarSelection.IsChecked.HasValue && AllowHalfStarSelection.IsChecked.Value;
        }

        private void SliderMaxStars_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (SliderMaxStars != null)
            {
                StarSelector1.Maximum = (int)SliderMaxStars.Value;
                MaximumStarsValue.Text = SliderMaxStars.Value.ToString("0");
            }
        }

        private void ReadOnlyControl_Click(object sender, RoutedEventArgs e)
        {
            StarSelector1.ReadOnly = ReadOnlyControl.IsChecked.HasValue && ReadOnlyControl.IsChecked.Value;
        }

        private void DisableControl_Click(object sender, RoutedEventArgs e)
        {
            StarSelector1.Disabled = DisableControl.IsChecked.HasValue && DisableControl.IsChecked.Value;
        }
    }
}
