// <copyright file="MarginWrapper.cs" company="Ucaya">
// Distributed under Microsoft Public License (Ms-PL)
// </copyright>
// <author>Thierry Bouquain</author>
namespace SLExtensions.Controls.Animation
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

    public class AnimationWrapper : Panel
    {
        #region Fields

        /// <summary>
        /// Value depedency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty = 
            DependencyProperty.Register(
                "Value",
                typeof(double),
                typeof(AnimationWrapper),
                new PropertyMetadata((d, e) => ((AnimationWrapper)d).OnValueChanged((double)e.OldValue, (double)e.NewValue)));

        #endregion Fields

        #region Events

        public event EventHandler<DoubleEventArgs> ValueChanged;

        #endregion Events

        #region Properties

        public double Value
        {
            get
            {
                return (double)GetValue(ValueProperty);
            }

            set
            {
                SetValue(ValueProperty, value);
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// handles the ValueProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnValueChanged(double oldValue, double newValue)
        {
            if (ValueChanged != null)
            {
                ValueChanged(this, new DoubleEventArgs(newValue));
            }
        }

        #endregion Methods
    }

    public class DoubleEventArgs : EventArgs
    {
        #region Constructors

        public DoubleEventArgs(double value)
        {
            Value = value;
        }

        #endregion Constructors

        #region Properties

        public double Value
        {
            get; private set;
        }

        #endregion Properties
    }
}