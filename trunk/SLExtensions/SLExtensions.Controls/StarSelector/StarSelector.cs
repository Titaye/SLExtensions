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
using System.Collections.Generic;

namespace SLExtensions.Controls
{
    public class StarSelector : Control
    {
        private const string STATE_NORMAL = "StateNormal";
        private const string STATE_READONLY = "StateReadOnly";
        private const string STATE_DISABLED = "StateDisabled";
        private const string STATE_MOUSEDOWN = "StateMouseDown";
        private const string STATE_MOUSEUP = "StateMouseUp";

        private readonly IList<Star> m_stars = new List<Star>();

        private bool m_ratingMode = false;
        private double m_ratingModeValue = 0;

        private Panel m_starContainer;
        private Panel m_rootElement;

        public event RoutedEventHandler ValueChanged;

        public StarSelector()
        {
            this.DefaultStyleKey = typeof(StarSelector);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            m_rootElement = GetTemplateChild("LayoutRoot") as Panel;
            m_starContainer = GetTemplateChild("StarContainer") as Panel;
            m_starContainer.MouseEnter += starContainer_MouseEnter;
            m_starContainer.MouseLeave += starContainer_MouseLeave;
            m_starContainer.MouseLeftButtonDown += m_starContainer_MouseLeftButtonDown;
            m_starContainer.MouseLeftButtonUp += m_starContainer_MouseLeftButtonUp;

            InitializeStars();
            UpdateVisuals();
        }

        private void InitializeStars()
        {
            if (m_rootElement == null)
                return;

            m_starContainer.Children.Clear();
            m_stars.Clear();
            for (int i = 0; i < this.Maximum; ++i)
            {
                Star s = new Star();
                s.MouseMove += s_MouseMove;
                s.MouseLeftButtonUp += s_MouseLeftButtonUp;
                if (this.StarTemplate != null)
                    s.Template = this.StarTemplate;
                m_starContainer.Children.Add(s);
                m_stars.Add(s);
            }
        }

        void s_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.ReadOnly || this.Disabled)
                return;

            this.Value = m_ratingModeValue;
            if (this.SetDisplayValueOnClick)
            {
                this.DisplayValue = this.Value;
            }
            RaiseValueChanged();
        }


        void m_starContainer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.ReadOnly || this.Disabled)
                return;

            VisualStateManager.GoToState(this, STATE_MOUSEUP, true);
        }

        void m_starContainer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.ReadOnly || this.Disabled)
                return;

            VisualStateManager.GoToState(this, STATE_MOUSEDOWN, true);
        }

        void starContainer_MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.ReadOnly || this.Disabled)
                return;

            VisualStateManager.GoToState(this, STATE_NORMAL, true);
            m_ratingMode = false;
            UpdateVisuals();
        }

        void starContainer_MouseEnter(object sender, MouseEventArgs e)
        {
            if (this.ReadOnly || this.Disabled)
                return;

            m_ratingMode = true;
            UpdateVisuals();
        }

        void s_MouseMove(object sender, MouseEventArgs e)
        {
            if (!m_ratingMode || this.ReadOnly || this.Disabled)
                return;

            Star s = sender as Star;

            int tempVal = m_stars.IndexOf(s);
            if (!this.AllowHalfStarSelection)
            {
                m_ratingModeValue = tempVal + 1;
            }
            else
            {
                Point pos = e.GetPosition(s);
                if (pos.X > s.ActualWidth / 2)
                {
                    m_ratingModeValue = tempVal + 1;
                }
                else
                {
                    m_ratingModeValue = tempVal + 0.5;
                }
            }
            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            double useVal = this.DisplayValue;
            if (m_ratingMode)
                useVal = m_ratingModeValue;

            double rounded = Math.Round((useVal) * 2, 0) / 2;
            int wholePart = (int)Math.Floor(rounded);
            double fractionPart = rounded - wholePart;

            for (int i = 1; i <= m_stars.Count; i++)
            {
                if (i <= wholePart)
                {
                    m_stars[i - 1].Value = StarSelectState.Selected;
                }
                else if (i == wholePart + 1 && fractionPart == 0.5)
                {
                    m_stars[i - 1].Value = StarSelectState.HalfSelected;
                }
                else
                {
                    m_stars[i - 1].Value = StarSelectState.NotSelected;
                }

                m_stars[i - 1].Disabled = this.Disabled;
            }
        }

        private void RaiseValueChanged()
        {
            if (ValueChanged != null)
                ValueChanged(this, new RoutedEventArgs());
        }

        #region Value Dependency Property

        /// <summary>
        /// Value
        /// </summary>
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Value Dependency Property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value",
                typeof(double),
                typeof(StarSelector),
                new PropertyMetadata(ValuePropertyChanged));

        private static void ValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StarSelector c = d as StarSelector;
            if (c != null)
            {
                c.UpdateVisuals();
            }
        }

        #endregion

        #region DisplayValue Dependency Property

        /// <summary>
        /// DisplayValue
        /// </summary>
        public double DisplayValue
        {
            get { return (double)GetValue(DisplayValueProperty); }
            set { SetValue(DisplayValueProperty, value); }
        }

        /// <summary>
        /// DisplayValue Dependency Property.
        /// </summary>
        public static readonly DependencyProperty DisplayValueProperty =
            DependencyProperty.Register(
                "DisplayValue",
                typeof(double),
                typeof(StarSelector),
                new PropertyMetadata(new PropertyChangedCallback(DisplayValueChanged)));

        private static void DisplayValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StarSelector c = d as StarSelector;
            if (c != null)
            {
                c.UpdateVisuals();
            }
        }

        #endregion

        #region Maximum Dependency Property

        /// <summary>
        /// Maximum
        /// </summary>
        public int Maximum
        {
            get { return (int)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        /// <summary>
        /// Maximum Dependency Property.
        /// </summary>
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(
                "Maximum",
                typeof(int),
                typeof(StarSelector),
                new PropertyMetadata(4, MaximumChanged));

        private static void MaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StarSelector c = d as StarSelector;
            if (c != null)
            {
                c.InitializeStars();
                c.UpdateVisuals();
            }
        }

        #endregion

        #region SetDisplayValueOnClick Dependency Property

        /// <summary>
        /// SetDisplayValueOnClick
        /// </summary>
        public bool SetDisplayValueOnClick
        {
            get { return (bool)GetValue(SetDisplayValueOnClickProperty); }
            set { SetValue(SetDisplayValueOnClickProperty, value); }
        }

        /// <summary>
        /// SetDisplayValueOnClick Dependency Property.
        /// </summary>
        public static readonly DependencyProperty SetDisplayValueOnClickProperty =
            DependencyProperty.Register(
                "SetDisplayValueOnClick",
                typeof(bool),
                typeof(StarSelector),
                new PropertyMetadata(false));

        #endregion

        #region AllowHalfStarSelection Dependency Property

        /// <summary>
        /// AllowHalfStarSelection
        /// </summary>
        public bool AllowHalfStarSelection
        {
            get { return (bool)GetValue(AllowHalfStarSelectionProperty); }
            set { SetValue(AllowHalfStarSelectionProperty, value); }
        }

        /// <summary>
        /// AllowHalfStarSelection Dependency Property.
        /// </summary>
        public static readonly DependencyProperty AllowHalfStarSelectionProperty =
            DependencyProperty.Register(
                "AllowHalfStarSelection",
                typeof(bool),
                typeof(StarSelector),
                new PropertyMetadata(false));



        #endregion

        #region ReadOnly Dependency Property

        /// <summary>
        /// ReadOnly
        /// </summary>
        public bool ReadOnly
        {
            get { return (bool)GetValue(ReadOnlyProperty); }
            set { SetValue(ReadOnlyProperty, value); }
        }

        /// <summary>
        /// ReadOnly Dependency Property.
        /// </summary>
        public static readonly DependencyProperty ReadOnlyProperty =
            DependencyProperty.Register(
                "ReadOnly",
                typeof(bool),
                typeof(StarSelector),
                new PropertyMetadata(false, ReadOnlyPropertyChanged));

        private static void ReadOnlyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StarSelector c = d as StarSelector;
            if (c != null)
            {
                VisualStateManager.GoToState(c, STATE_READONLY, true);
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
                typeof(StarSelector),
                new PropertyMetadata(false, DisabledChanged));

        private static void DisabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StarSelector c = d as StarSelector;
            if (c != null)
            {
                c.UpdateVisuals();
                VisualStateManager.GoToState(c, STATE_DISABLED, true);
            }
        }

        #endregion

        #region StarTemplate Dependency Property

        /// <summary>
        /// StarTemplate
        /// </summary>
        public ControlTemplate StarTemplate
        {
            get { return (ControlTemplate)GetValue(StarTemplateProperty); }
            set { SetValue(StarTemplateProperty, value); }
        }

        /// <summary>
        /// Disabled Dependency Property.
        /// </summary>
        public static readonly DependencyProperty StarTemplateProperty =
            DependencyProperty.Register(
                "StarTemplate",
                typeof(ControlTemplate),
                typeof(StarSelector),
                new PropertyMetadata(StarTemplatePropertyChanged));

        private static void StarTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //StarSelector c = d as StarSelector;
            //if (c != null)
            //{
            //    c.UpdateVisuals();
            //}
        }

        #endregion
    }
}
