namespace SLExtensions
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
    /// Contains various animation extension methods for use in Silverlight applications.
    /// </summary>
    public static class AnimationExtensions
    {
        #region Methods

        /// <summary>
        /// Extension method to quickly animate a DependencyObject.  Simply provide the propertyPath and a timeline.
        /// </summary>
        /// <param name="element">The element to animate.</param>
        /// <param name="propertyPath">The property to animate.</param>
        /// <param name="timeline">The animation to apply to the element.</param>
        /// <param name="completed">The event handler to fire once the animation completes.</param>
        public static void Animate(this DependencyObject element, string propertyPath, Timeline timeline, EventHandler completed)
        {
            Storyboard sb = new Storyboard();
            if (completed != null)
                sb.Completed += completed;

            sb.Children.Add(timeline);
            Storyboard.SetTarget(sb, element);
            Storyboard.SetTargetProperty(sb, new PropertyPath(propertyPath));
            sb.Begin();
        }

        /// <summary>
        /// Extension method to quickly animate a DependencyObject.  Simply provide the propertyPath and a timeline.
        /// </summary>
        /// <param name="element">The element to animate.</param>
        /// <param name="propertyPath">The property to animate.</param>
        /// <param name="timeline">The animation to apply to the element.</param>
        public static void Animate(this DependencyObject element, string propertyPath, Timeline timeline)
        {
            Animate(element, propertyPath, timeline, null);
        }

        /// <summary>
        /// Extension method to quickly animate a DependencyObject with a DoubleAnimation.  Simply provide the propertyPath, toValue, and handler to fire once the animation completes.
        /// </summary>
        /// <param name="element">The element to animate.</param>
        /// <param name="propertyPath">The property to animate.</param>
        /// <param name="duration">The duration of the animation in milliseconds.</param>
        /// <param name="toValue">The value to animate to.</param>
        /// <param name="completed">The event to fire once the animation completes.</param>
        public static void AnimateDouble(this DependencyObject element, string propertyPath, int duration, double toValue, EventHandler completed)
        {
            DoubleAnimation ani = new DoubleAnimation();
            ani.To = toValue;
            ani.Duration = TimeSpan.FromMilliseconds(duration);
            Animate(element, propertyPath, ani, completed);
        }

        /// <summary>
        /// Extension method to quickly animate a DependencyObject with a DoubleAnimation.  Simply provide the propertyPath, toValue, and handler to fire once the animation completes.
        /// </summary>
        /// <param name="element">The element to animate.</param>
        /// <param name="propertyPath">The property to animate.</param>
        /// <param name="duration">The duration of the animation.</param>
        /// <param name="toValue">The value to animate to.</param>
        public static void AnimateDouble(this DependencyObject element, string propertyPath, int duration, double toValue)
        {
            AnimateDouble(element, propertyPath, duration, toValue, null);
        }

        /// <summary>
        /// This extension method is used to find a Storyboard declared inside of the VisualStateManager.
        /// Returns null if the storyboard cannot be found.
        /// </summary>
        /// <param name="parent">The parent object containing the VisualStateManager declaration.</param>
        /// <param name="groupName">The group name containing the storyboard.</param>
        /// <param name="stateName">The name of the state or storyboard.</param>
        /// <returns></returns>
        public static Storyboard FindStoryboard(this FrameworkElement parent, string groupName, string stateName)
        {
            var vsgs = VisualStateManager.GetVisualStateGroups(parent);
            foreach (VisualStateGroup vsg in vsgs)
            {
                if (vsg.Name != groupName)
                    continue;
                foreach (VisualState vs in vsg.States)
                {
                    if (vs.Name == stateName)
                        return vs.Storyboard;
                }
            }
            return null;
        }

        #endregion Methods
    }
}