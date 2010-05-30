namespace SLExtensions.Controls
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

    public class TabPanel : System.Windows.Controls.Primitives.TabPanel
    {
        #region Fields

        /// <summary>
        /// IsMultiRow depedency property.
        /// </summary>
        public static readonly DependencyProperty IsMultiRowProperty = 
            DependencyProperty.Register(
                "IsMultiRow",
                typeof(bool),
                typeof(TabPanel),
                new PropertyMetadata((d, e) => ((TabPanel)d).OnIsMultiRowChanged((bool)e.OldValue, (bool)e.NewValue)));

        #endregion Fields

        #region Properties

        public bool IsMultiRow
        {
            get
            {
                return (bool)GetValue(IsMultiRowProperty);
            }

            set
            {
                SetValue(IsMultiRowProperty, value);
            }
        }

        #endregion Properties

        #region Methods

        protected override Size ArrangeOverride(Size finalSize)
        {
            return base.ArrangeOverride(finalSize);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (IsMultiRow)
                return base.MeasureOverride(availableSize);
            else
            {
                var val = base.MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity));
                return val;
            }
        }

        /// <summary>
        /// handles the IsMultiRowProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnIsMultiRowChanged(bool oldValue, bool newValue)
        {
            InvalidateMeasure();
        }

        #endregion Methods
    }
}