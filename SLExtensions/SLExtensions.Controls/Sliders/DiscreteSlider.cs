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

    public class DiscreteSlider : SLExtensions.Controls.Slider
    {
        #region Fields

        private bool m_busy;

        #endregion Fields

        #region Methods

        /// <summary>
        ///  the OnValueChanged slider method to return a discrete value proportional to SmallChange
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected override void OnValueChanged(double oldValue, double newValue)
        {
            if (!m_busy)
            {
                m_busy = true;
                try
                {
                    if (SmallChange != 0)
                    {
                        double newDiscreteValue = (int)(Math.Round(newValue / SmallChange)) * SmallChange;
                        if (newDiscreteValue != oldValue)
                        {
                            Value = newDiscreteValue;
                            base.OnValueChanged(oldValue, newDiscreteValue);
                            //m_discreteValue = newDiscreteValue;
                        }
                    }
                    else
                    {
                        base.OnValueChanged(oldValue, newValue);
                    }
                }
                finally
                {
                    m_busy = false;
                }
            }
        }

        #endregion Methods
    }
}