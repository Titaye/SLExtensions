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

namespace SLExtensions.Controls
{
    public enum StarSelectState { NotSelected, HalfSelected, Selected }

    public class Star : Control
    {
        private const string StateNormal = "StateNormal";
        private const string StateHalfSelected = "StateHalfSelected";
        private const string StateFullSelected = "StateFullSelected";
        private const string StateDisabled = "StateDisabled";
        private const string StateNotDisabled = "StateNotDisabled";

        private UIElement m_fullItem;
        private UIElement m_halfItem;

        public Star()
        {
            DefaultStyleKey = typeof(Star);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            m_fullItem = GetTemplateChild("FullItem") as UIElement;
            m_halfItem = GetTemplateChild("HalfItem") as UIElement;
            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            if (m_halfItem == null || m_fullItem == null)
                return;

            switch (Value)
            {
                default:
                case StarSelectState.NotSelected:
                    VisualStateManager.GoToState(this, StateNormal, true);
                    break;
                case StarSelectState.HalfSelected:
                    VisualStateManager.GoToState(this, StateHalfSelected, true);
                    break;
                case StarSelectState.Selected:
                    VisualStateManager.GoToState(this, StateFullSelected, true);
                    break;
            }

            if (this.Disabled)
            {
                VisualStateManager.GoToState(this, StateDisabled, true);
            }
            else
            {
                VisualStateManager.GoToState(this, StateNotDisabled, true);
            }
        }

        #region Value Dependency Property

        /// <summary>
        /// Value
        /// </summary>
        public StarSelectState Value
        {
            get { return (StarSelectState)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Value Dependency Property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value",
                typeof(StarSelectState),
                typeof(Star),
                new PropertyMetadata(StarSelectState.NotSelected, ValueChanged));

        private static void ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Star c = d as Star;
            if (c != null)
            {
                c.UpdateVisuals();
            }
        }

        #endregion

        #region Disabled Dependency Property

        /// <summary>
        /// Disabled
        /// </summary>
        public bool Disabled
        {
            get { return (bool)GetValue(DisabledProperty); }
            set { SetValue(DisabledProperty, value); }
        }

        /// <summary>
        /// Disabled Dependency Property.
        /// </summary>
        public static readonly DependencyProperty DisabledProperty =
            DependencyProperty.Register(
                "Disabled",
                typeof(bool),
                typeof(Star),
                new PropertyMetadata(false, DisabledChanged));

        private static void DisabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Star c = d as Star;
            if (c != null)
            {
                c.UpdateVisuals();
            }
        }

        #endregion
    }
}
