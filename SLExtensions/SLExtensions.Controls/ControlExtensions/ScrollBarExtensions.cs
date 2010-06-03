namespace SLExtensions.Controls
{
    using System;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    /// <summary>
    /// Provide behavior extensions to a ScrollBar control. When registered add CanIncrease and CanDecrease attached property
    /// </summary>
    public static class ScrollBarExtensions
    {
        #region Fields

        public static readonly DependencyProperty CanDecreaseProperty = 
            DependencyProperty.RegisterAttached("CanDecrease", typeof(bool), typeof(ScrollBarExtensions), null);
        public static readonly DependencyProperty CanIncreaseProperty = 
            DependencyProperty.RegisterAttached("CanIncrease", typeof(bool), typeof(ScrollBarExtensions), null);

        // Using a DependencyProperty as the backing store for Epsillon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EpsillonProperty = 
            DependencyProperty.RegisterAttached("Epsillon", typeof(double), typeof(ScrollBarExtensions), new PropertyMetadata(0.001d));
        public static readonly DependencyProperty RegisterScrollbarProperty = 
            DependencyProperty.RegisterAttached("RegisterScrollbar", typeof(ScrollBar), typeof(ScrollBarExtensions), new PropertyMetadata(RegisterScrollbarChangedCallback));

        #endregion Fields

        #region Methods

        /// <summary>
        /// true when the Value of a ScrollBar is greater than the Minimum + the Epsillon value
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool GetCanDecrease(DependencyObject obj)
        {
            return (bool)obj.GetValue(CanDecreaseProperty);
        }

        /// <summary>
        /// true when the Value of a ScrollBar is loawer than the Maximum - the Epsillon value
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool GetCanIncrease(DependencyObject obj)
        {
            return (bool)obj.GetValue(CanIncreaseProperty);
        }

        /// <summary>
        /// Get the epsillon value arround the Maximum or Minimum value that activate or deactive the CanIncrease and CanDecrease property
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double GetEpsillon(DependencyObject obj)
        {
            return (double)obj.GetValue(EpsillonProperty);
        }

        /// <summary>
        /// Get the registered ScrollBar
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ScrollBar GetRegisterScrollbar(DependencyObject obj)
        {
            return (ScrollBar)obj.GetValue(RegisterScrollbarProperty);
        }

        public static void SetCanDecrease(DependencyObject obj, bool value)
        {
            obj.SetValue(CanDecreaseProperty, value);
        }

        public static void SetCanIncrease(DependencyObject obj, bool value)
        {
            obj.SetValue(CanIncreaseProperty, value);
        }

        /// <summary>
        /// Set the epsillon value arround the Maximum or Minimum value that activate or deactive the CanIncrease and CanDecrease property
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetEpsillon(DependencyObject obj, double value)
        {
            obj.SetValue(EpsillonProperty, value);
        }

        /// <summary>
        /// Register a ScrollBar
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetRegisterScrollbar(DependencyObject obj, ScrollBar value)
        {
            obj.SetValue(RegisterScrollbarProperty, value);
        }

        private static void RefreshValue(ScrollBar sb)
        {
            var epsillon = GetEpsillon(sb);
            SetCanDecrease(sb, (sb.Value - sb.Minimum) > epsillon);
            SetCanIncrease(sb, (sb.Maximum - sb.Value) > epsillon);
        }

        private static void RegisterScrollbarChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sb = (ScrollBar)e.OldValue;
            if (sb != null)
            {
                sb.ValueChanged -= storyBoardPropertyChanged;
                sb.RemoveChangedHandler("Maximum", storyBoardDependecyPropertyChanged);
                sb.RemoveChangedHandler("Minimum", storyBoardDependecyPropertyChanged);
            }

            sb = (ScrollBar)e.NewValue;
            if (sb != null)
            {
                sb.ValueChanged += storyBoardPropertyChanged;
                sb.AddChangedHandler("Maximum", storyBoardDependecyPropertyChanged);
                sb.AddChangedHandler("Minimum", storyBoardDependecyPropertyChanged);
                RefreshValue(sb);
            }
        }

        static void storyBoardDependecyPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            RefreshValue((ScrollBar)sender);
        }

        static void storyBoardPropertyChanged(object sender, EventArgs e)
        {
            RefreshValue((ScrollBar)sender);
        }

        #endregion Methods
    }
}