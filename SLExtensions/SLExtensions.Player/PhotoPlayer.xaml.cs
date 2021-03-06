﻿namespace SLExtensions.Player
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

    using SLMedia.Core;
    using SLMedia.Picture;

    public partial class PhotoPlayer : UserControl
    {
        #region Fields

        private Thickness descriptionDesignMargin;

        #endregion Fields

        #region Constructors

        public PhotoPlayer()
        {
            InitializeComponent();
            Controller = Resources["controller"] as PictureController;
        }

        #endregion Constructors

        #region Properties

        public PictureController Controller
        {
            get; private set;
        }

        #endregion Properties

        #region Methods

        protected override Size ArrangeOverride(Size finalSize)
        {
            Size size = base.ArrangeOverride(finalSize);
            GridClip.RadiusX = border.CornerRadius.TopLeft;
            GridClip.RadiusY = border.CornerRadius.TopLeft;
            GridClip.Rect = new Rect(0, 0, grid.ActualWidth, grid.ActualHeight);
            return size;
        }

        void gridDescription_MouseEnter(object sender, MouseEventArgs e)
        {
            //TODO:
            //Storyboard sb = new Storyboard();
            //DoubleAnimation anim = new DoubleAnimation();
            //sb.Children.Add(anim);
            //Storyboard.SetTarget(anim, descriptionMarginWrapper);
            //Storyboard.SetTargetProperty(anim, new PropertyPath("(MarginWrapper.MarginTop)"));
            //anim.To = Math.Min(descriptionDesignMargin.Top, (descriptionDesignMargin.Top - description.ActualHeight ));
            //anim.Duration = new Duration(TimeSpan.FromMilliseconds(200));
            //sb.Begin();
        }

        void gridDescription_MouseLeave(object sender, MouseEventArgs e)
        {
            //TODO:
            //Storyboard sb = new Storyboard();
            //DoubleAnimation anim = new DoubleAnimation();
            //sb.Children.Add(anim);
            //Storyboard.SetTarget(anim, descriptionMarginWrapper);
            //Storyboard.SetTargetProperty(anim, new PropertyPath("(MarginWrapper.MarginTop)"));
            //anim.To = descriptionDesignMargin.Top;
            //anim.Duration = new Duration(TimeSpan.FromMilliseconds(200));
            //sb.Begin();
        }

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
        }

        #endregion Methods
    }
}