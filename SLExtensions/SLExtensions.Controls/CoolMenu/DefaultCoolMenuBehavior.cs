namespace SLExtensions.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Media.Animation;

    /// <summary>
    /// Represents the default behavior for the CoolMenu control.
    /// </summary>
    public class DefaultCoolMenuBehavior : ICoolMenuBehavior
    {
        #region Fields

        private readonly double[] m_sizes = new[] { 1, 0.75, 0.65, 0.50 };

        private bool m_mouseCaptured;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the DefaultCoolMenuBehavior class.
        /// </summary>
        public DefaultCoolMenuBehavior()
        {
            BounceEnabled = true;
            MaxItemHeight = 110;
            MaxItemWidth = 110;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Flag to enable or disable the bounce effect when a menu item is clicked.
        /// </summary>
        public bool BounceEnabled
        {
            get; set;
        }

        /// <summary>
        /// The maximum height of a menu item.
        /// </summary>
        public double MaxItemHeight
        {
            get; set;
        }

        /// <summary>
        /// The maximum width of a menu item.
        /// </summary>
        public double MaxItemWidth
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Applies the mouse down behavior.
        /// </summary>
        /// <param name="selectedIndex">The selected item index.</param>
        /// <param name="element">The element of concern.</param>
        public virtual void ApplyMouseDownBehavior(int selectedIndex, CoolMenuItem element)
        {
            if (!BounceEnabled)
                return;

            m_mouseCaptured = element.CaptureMouse();

            var da = new DoubleAnimationUsingKeyFrames();
            var k1 = new SplineDoubleKeyFrame
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(100)),
                Value = this.MaxItemHeight * 0.30
            };
            da.KeyFrames.Add(k1);

            Storyboard sb = new Storyboard();
            Storyboard.SetTarget(da, element);
            Storyboard.SetTargetProperty(da,
                new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(TranslateTransform.Y)"));
            sb.Children.Add(da);
            sb.Begin();
        }

        /// <summary>
        /// Applies the mouse enter behavior.
        /// </summary>
        /// <param name="proximity">The proximity of the element which is selected.</param>
        /// <param name="element">The element of concern.</param>
        public virtual void ApplyMouseEnterBehavior(int proximity, CoolMenuItem element)
        {
            if (proximity >= m_sizes.Length || proximity < 0)
                proximity = m_sizes.Length - 1;

            TimeSpan speed = TimeSpan.FromMilliseconds(100);
            DoubleAnimation daWidth = new DoubleAnimation { To = m_sizes[proximity] * MaxItemWidth, Duration = new Duration(speed) };
            DoubleAnimation daHeight = new DoubleAnimation { To = m_sizes[proximity] * MaxItemHeight, Duration = new Duration(speed) };
            Storyboard sb = new Storyboard();
            Storyboard.SetTarget(daWidth, element);
            Storyboard.SetTarget(daHeight, element);
            Storyboard.SetTargetProperty(daHeight, new PropertyPath("(UIElement.Height)"));
            Storyboard.SetTargetProperty(daWidth, new PropertyPath("(UIElement.Width)"));
            sb.Children.Add(daWidth);
            sb.Children.Add(daHeight);
            sb.Begin();
        }

        /// <summary>
        /// Does nothing in this implementation.
        /// </summary>
        /// <param name="element">The element of concern.</param>
        public virtual void ApplyMouseLeaveBehavior(CoolMenuItem element)
        {
            // Do nothing.
        }

        /// <summary>
        /// Applies the mouse up behavior.
        /// </summary>
        /// <param name="selectedIndex">The selected item index.</param>
        /// <param name="element">The element of concern.</param>
        public virtual void ApplyMouseUpBehavior(int selectedIndex, CoolMenuItem element)
        {
            if (!m_mouseCaptured || !BounceEnabled)
                return;

            var da = new DoubleAnimationUsingKeyFrames();
            var k2 = new SplineDoubleKeyFrame
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(100)),
                Value = 0
            };
            var k3 = new SplineDoubleKeyFrame
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(200)),
                Value = this.MaxItemHeight * 0.10
            };
            var k4 = new SplineDoubleKeyFrame
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(250)),
                Value = 0
            };
            da.KeyFrames.Add(k2);
            da.KeyFrames.Add(k3);
            da.KeyFrames.Add(k4);

            Storyboard sb = new Storyboard();
            Storyboard.SetTarget(da, element);
            Storyboard.SetTargetProperty(da,
                new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(TranslateTransform.Y)"));
            sb.Children.Add(da);
            sb.Begin();

            element.ReleaseMouseCapture();
            m_mouseCaptured = false;
        }

        /// <summary>
        /// Initializes each element in the cool menu.
        /// </summary>
        /// <param name="parent">The parent CoolMenu.</param>
        /// <param name="element">The element of concern.</param>
        public virtual void Initialize(CoolMenu parent, CoolMenuItem element)
        {
            element.Height = MaxItemHeight * m_sizes[m_sizes.Length - 1];
            element.Width = MaxItemWidth * m_sizes[m_sizes.Length - 1];
        }

        #endregion Methods
    }
}