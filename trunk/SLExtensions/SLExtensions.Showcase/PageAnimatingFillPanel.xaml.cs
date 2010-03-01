namespace SLExtensions.Showcase
{
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

    using SLExtensions.Controls;

    public partial class PageAnimatingFillPanel : UserControl
    {
        #region Fields

        Random rnd = new Random();

        #endregion Fields

        #region Constructors

        public PageAnimatingFillPanel()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Border brd = (Border)sender;
            switch (rnd.Next(2))
            {
                case 0:
                    brd.Width = 512;
                    brd.Height = 288;
                    break;
                case 1:
                    brd.Width = 320;
                    brd.Height = 180;
                    break;
                //case 2:
                //    brd.Width = rnd.Next(500);
                //    brd.Height = rnd.Next(500);
                //    break;

            }
            (brd.Parent as AnimatingFillPanel).StartElement = null;
            //(brd.Parent as Panel).InvalidateMeasure();
        }

        private void Border_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            (((FrameworkElement)sender).Parent as Panel).InvalidateMeasure();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            Grid grd = (Grid)btn.Parent;
            Border brd = (Border)grd.Parent;

            AnimatingFillPanel fillPanel = (AnimatingFillPanel)brd.Parent;
            fillPanel.StartElement = brd;

            Storyboard sb = new Storyboard();

            DoubleAnimation anim = new DoubleAnimation();
            sb.Children.Add(anim);
            anim.Duration = TimeSpan.FromMilliseconds(200);
            Storyboard.SetTarget(anim, brd);
            anim.To = 1024;
            Storyboard.SetTargetProperty(anim, new PropertyPath("Width"));

            anim = new DoubleAnimation();
            sb.Children.Add(anim);
            anim.Duration = TimeSpan.FromMilliseconds(200);
            Storyboard.SetTarget(anim, brd);
            anim.To = 768;
            Storyboard.SetTargetProperty(anim, new PropertyPath("Height"));
            sb.Begin();
            //brd.Width = fillPanel.ActualWidth;
            //brd.Height = fillPanel.ActualHeight;
        }

        #endregion Methods
    }
}