// <copyright file="Tween.cs" company="First Floor Software">
// Distributed under Microsoft Public License (Ms-PL)
// </copyright>
// <author>Koen Zwikstra</author>
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

    /// <summary>
    /// Provides the tween attached property.
    /// </summary>
    public static class Tween
    {
        #region Fields

        public static readonly DependencyProperty FromProperty = 
            DependencyProperty.RegisterAttached("From", typeof(double), typeof(Tween), new PropertyMetadata(OnTweenChanged));
        public static readonly DependencyProperty IsInitializeProperty = 
            DependencyProperty.RegisterAttached("IsInitialize", typeof(bool), typeof(Tween), new PropertyMetadata(OnTweenChanged));
        public static readonly DependencyProperty ToProperty = 
            DependencyProperty.RegisterAttached("To", typeof(double), typeof(Tween), new PropertyMetadata(OnTweenChanged));
        public static readonly DependencyProperty TypeProperty = 
            DependencyProperty.RegisterAttached("Type", typeof(EquationType), typeof(Tween), new PropertyMetadata(OnTweenChanged));

        #endregion Fields

        #region Delegates

        private delegate double Equation(params double[] args);

        #endregion Delegates

        #region Methods

        public static DoubleAnimationUsingKeyFrames AddAnimation(Storyboard sb, DependencyObject target, PropertyPath property, TimeSpan duration, EquationType type, double from, double to)
        {
            DoubleAnimationUsingKeyFrames anim = new DoubleAnimationUsingKeyFrames();
            sb.Children.Add(anim);

            Storyboard.SetTarget(anim, target);
            Storyboard.SetTargetProperty(anim, property);

            anim.Duration = new Duration(duration);

            Tween.SetFrom(anim, from);
            Tween.SetTo(anim, to);
            Tween.SetType(anim, type);

            Tween.SetIsInitialize(anim, true);

            return anim;
        }

        public static Storyboard CreateAnim(DependencyObject target, EquationType type, TimeSpan duration, PropertyPath property, double from, double to)
        {
            Storyboard sb = new Storyboard();

            AddAnimation(sb, target, property, duration, type, from, to);

            return sb;
        }

        public static Storyboard CreateAnim(DependencyObject target, EquationType type, TimeSpan duration, params TweenParameter[] parameters)
        {
            Storyboard sb = new Storyboard();

            foreach (var p in parameters)
            {
                AddAnimation(sb, target, p.Property
                    , p.Duration.HasValue ? p.Duration.Value : duration
                    , p.Type.HasValue ? p.Type.Value : type
                    , p.From, p.To);
            }

            return sb;
        }

        // For backward compatibility
        public static Storyboard CreateAnim(DependencyObject target, PropertyPath property, EquationType type, double to, TimeSpan duration)
        {
            return CreateAnim(target, type, duration, property, 0, to);
        }

        public static Transform FindTransform(FrameworkElement target, string transformName)
        {
            if ((target.RenderTransform != null) && (target.RenderTransform is TransformGroup))
            {
                if (((TransformGroup)target.RenderTransform).Children.Count > 0)
                {
                    for (int i = 0; i < ((TransformGroup)target.RenderTransform).Children.Count; i++)
                    {
                        if (((TransformGroup)target.RenderTransform).Children[i].GetType().Name == transformName)
                        {
                            return (Transform)((TransformGroup)target.RenderTransform).Children[i];
                        }
                    }
                }
            }
            return null;
        }

        public static TransformGroup FindTransformGroup(FrameworkElement target)
        {
            if ((target.RenderTransform != null) && (target.RenderTransform is TransformGroup))
            {
                return (TransformGroup)target.RenderTransform;
            }
            // if TransformGroup doesn't exist then return a new one
            return new TransformGroup();
        }

        /// <summary>
        /// Gets the tween's starting value.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns></returns>
        public static double GetFrom(DependencyObject o)
        {
            return (double)o.GetValue(FromProperty);
        }

        /// <summary>
        /// Gets the tween's is initialize.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns></returns>
        public static bool GetIsInitialize(DependencyObject o)
        {
            return (bool)o.GetValue(IsInitializeProperty);
        }

        public static ScaleTransform GetTargetScaleTransform(FrameworkElement target)
        {
            return (ScaleTransform)FindTransform(target, "ScaleTransform");
        }

        public static SkewTransform GetTargetSkewTransform(FrameworkElement target)
        {
            return (SkewTransform)FindTransform(target, "SkewTransform");
        }

        public static TranslateTransform GetTargetTranslateTransform(FrameworkElement target)
        {
            return (TranslateTransform)FindTransform(target, "TranslateTransform");
        }

        /// <summary>
        /// Gets the tween's ending value.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns></returns>
        public static double GetTo(DependencyObject o)
        {
            return (double)o.GetValue(ToProperty);
        }

        /// <summary>
        /// Gets the tween type.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns></returns>
        public static EquationType GetType(DependencyObject o)
        {
            return (EquationType)o.GetValue(TypeProperty);
        }

        public static ScaleTransform InitElementScale(FrameworkElement target)
        {
            // if there's an existing ScaleTransform on the target FrameworkElement find it and return it
            ScaleTransform st;
            st = (ScaleTransform)FindTransform(target, "ScaleTransform");
            if (st != null)
                return st;

            // if we're here then a RenderTransform doesn't exist for the FrameworkElement, so create one
            // first check for an existing TransformGroup
            TransformGroup tg = FindTransformGroup(target);
            st = new ScaleTransform();

            tg.Children.Add(st);
            target.RenderTransform = tg;
            return st;
        }

        public static TranslateTransform InitElementTranslate(FrameworkElement target)
        {
            // if there's an existing ScaleTransform on the target FrameworkElement find it and return it
            TranslateTransform tt;
            tt = (TranslateTransform)FindTransform(target, "TranslateTransform");
            if (tt != null)
                return tt;

            // if we're here then a RenderTransform doesn't exist for the FrameworkElement, so create one
            // first check for an existing TransformGroup
            TransformGroup tg = FindTransformGroup(target);
            tt = new TranslateTransform();

            tg.Children.Add(tt);
            target.RenderTransform = tg;
            return tt;
        }

        public static Storyboard PrepareAnim(Storyboard sb, DependencyObject target, EquationType type, TimeSpan duration, params TweenParameter[] parameters)
        {
            foreach (var p in parameters)
            {
                AddAnimation(sb, target, p.Property
                    , p.Duration.HasValue ? p.Duration.Value : duration
                    , p.Type.HasValue ? p.Type.Value : type
                    , p.From, p.To);
            }

            return sb;
        }

        /// <summary>
        /// Sets the tween's starting value.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="value">The value.</param>
        public static void SetFrom(DependencyObject o, double value)
        {
            o.SetValue(FromProperty, value);
        }

        /// <summary>
        /// Sets the tween's is initialize.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="value">The value.</param>
        public static void SetIsInitialize(DependencyObject o, bool value)
        {
            o.SetValue(IsInitializeProperty, value);
        }

        /// <summary>
        /// Sets the tween's ending value.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="value">The value.</param>
        public static void SetTo(DependencyObject o, double value)
        {
            o.SetValue(ToProperty, value);
        }

        /// <summary>
        /// Sets the tween type.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="value">The value.</param>
        public static void SetType(DependencyObject o, EquationType value)
        {
            o.SetValue(TypeProperty, value);
        }

        public static Storyboard UpdateAnim(Storyboard sb, params TweenParameter[] parameters)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                TweenParameter p = parameters[i];
                DoubleAnimationUsingKeyFrames anim = (DoubleAnimationUsingKeyFrames)sb.Children[i];

                UpdateAnimation(anim, p.From, p.To);
            }

            return sb;
        }

        public static DoubleAnimationUsingKeyFrames UpdateAnimation(DoubleAnimationUsingKeyFrames anim, double from, double to)
        {
            Tween.SetFrom(anim, from);
            Tween.SetTo(anim, to);

            Tween.SetIsInitialize(anim, true);

            return anim;
        }

        private static void OnTweenChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DoubleAnimationUsingKeyFrames animation = o as DoubleAnimationUsingKeyFrames;

            if (animation != null && e.Property != Tween.IsInitializeProperty)
            {

                EquationType type = GetType(animation);
                double from = GetFrom(animation);
                double to = GetTo(animation);

                Equation equation = (Equation)Delegate.CreateDelegate(typeof(Equation), typeof(Equations).GetMethod(type.ToString(), new Type[] { typeof(double[]) }));

                double total = animation.Duration.TimeSpan.TotalMilliseconds;

                bool isInitialize = GetIsInitialize(animation);

                if (!isInitialize)
                {

                    animation.KeyFrames.Clear();

                    for (double i = 0; i < total; i += 50)
                    {
                        LinearDoubleKeyFrame frame = new LinearDoubleKeyFrame();

                        frame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(i));

                        frame.Value = equation(i, from, to - from, total);

                        animation.KeyFrames.Add(frame);
                    }

                    // add final key frame
                    LinearDoubleKeyFrame finalFrame = new LinearDoubleKeyFrame();
                    finalFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(total));
                    finalFrame.Value = to;
                    animation.KeyFrames.Add(finalFrame);
                }
                else
                {
                    int frameIndex = 0;

                    int countFrame = animation.KeyFrames.Count;

                    for (double i = 0; i < total; i += 50)
                    {
                        if (frameIndex < countFrame)
                        {
                            LinearDoubleKeyFrame frame = (LinearDoubleKeyFrame)animation.KeyFrames[frameIndex];

                            frame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(i));

                            frame.Value = equation(i, from, to - from, total);

                            frameIndex++;
                        }
                    }

                    // final key frame
                    if (countFrame > 0)
                    {
                        LinearDoubleKeyFrame finalFrame = (LinearDoubleKeyFrame)animation.KeyFrames[animation.KeyFrames.Count - 1];
                        finalFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(total));
                        finalFrame.Value = to;
                    }
                }
            }
        }

        #endregion Methods

        #region Other

        // This old OnTweenChanged method...
        //private static void OnTweenChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        //{
        //    DoubleAnimationUsingKeyFrames animation = o as DoubleAnimationUsingKeyFrames;
        //    if (animation != null) {
        //        animation.KeyFrames.Clear();
        //        EquationType type = GetType(animation);
        //        double from = GetFrom(animation);
        //        double to = GetTo(animation);
        //        Equation equation = (Equation)Delegate.CreateDelegate(typeof(Equation), typeof(Equations).GetMethod(type.ToString(), new Type[] { typeof(double[])} ));
        //        double total = animation.Duration.TimeSpan.TotalMilliseconds;
        //        for (double i = 0; i < total; i += 50) {
        //            LinearDoubleKeyFrame frame = new LinearDoubleKeyFrame();
        //            frame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(i));
        //            frame.Value = equation(i, from, to - from, total);
        //            animation.KeyFrames.Add(frame);
        //        }
        //        // add final key frame
        //        LinearDoubleKeyFrame finalFrame = new LinearDoubleKeyFrame();
        //        finalFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(total));
        //        finalFrame.Value = to;
        //        animation.KeyFrames.Add(finalFrame);
        //    }
        //}
        // This old CreateAnim method...
        //public static Storyboard CreateAnim(DependencyObject target, PropertyPath property, EquationType type, double to, TimeSpan duration)
        //{
        //    Storyboard sb = new Storyboard();
        //    DoubleAnimationUsingKeyFrames anim = new DoubleAnimationUsingKeyFrames();
        //    sb.Children.Add(anim);
        //    Storyboard.SetTarget(anim, target);
        //    Storyboard.SetTargetProperty(anim, property);
        //    anim.Duration = new Duration(duration);
        //    Tween.SetTo(anim, to);
        //    Tween.SetType(anim, type);
        //    return sb;
        //}

        #endregion Other
    }
}