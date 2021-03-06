﻿namespace SLExtensions.Controls.Animation
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    public static partial class StoryboardExtensions
    {
        #region Fields

        // Using a DependencyProperty as the backing store for TargetProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TargetPropertyProperty = 
            DependencyProperty.RegisterAttached("TargetProperty", typeof(DependencyProperty), typeof(PropertyPath), new PropertyMetadata(TargetPropertyChangedCallback));

        // Using a DependencyProperty as the backing store for SBExtTarget.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty SBExtTargetProperty = 
            DependencyProperty.RegisterAttached("SBExtTarget", typeof(object), typeof(Storyboard), null);

        #endregion Fields

        #region Methods

        public static DoubleAnimationUsingKeyFrames CreateAnim(this Storyboard sb, DependencyObject target,
            string propertyPath, IEasingFunction easing, double value, TimeSpan keyTime)
        {
            var doubleAnim = (from anim in sb.Children.OfType<DoubleAnimationUsingKeyFrames>()
                     where GetSBExtTarget(anim) == target
                     let prop = Storyboard.GetTargetProperty(anim)
                     where prop.Path == propertyPath
                     select anim).FirstOrDefault();

            if (doubleAnim == null)
            {
                doubleAnim = new DoubleAnimationUsingKeyFrames();
                SetSBExtTarget(doubleAnim, target);
                Storyboard.SetTarget(doubleAnim, target);
                Storyboard.SetTargetProperty(doubleAnim, new System.Windows.PropertyPath(propertyPath));
                sb.Children.Add(doubleAnim);
            }

            EasingDoubleKeyFrame kf = new EasingDoubleKeyFrame();
            kf.EasingFunction = easing;
            kf.KeyTime = keyTime;
            kf.Value = value;
            doubleAnim.KeyFrames.Add(kf);

            return doubleAnim;
        }

        public static DependencyProperty GetTargetProperty(DependencyObject obj)
        {
            return (DependencyProperty)obj.GetValue(TargetPropertyProperty);
        }

        public static void SetTargetProperty(DependencyObject obj, DependencyProperty value)
        {
            obj.SetValue(TargetPropertyProperty, value);
        }

        private static object GetSBExtTarget(DependencyObject obj)
        {
            return (object)obj.GetValue(SBExtTargetProperty);
        }

        private static void SetSBExtTarget(DependencyObject obj, object value)
        {
            obj.SetValue(SBExtTargetProperty, value);
        }

        private static void TargetPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                d.ClearValue(Storyboard.TargetPropertyProperty);
            else
                Storyboard.SetTargetProperty((Timeline)d, new System.Windows.PropertyPath((DependencyProperty)e.NewValue));
        }

        #endregion Methods
    }
}