namespace SLExtensions.Interactivity
{
    using System;
    using System.Net;
    using System.Windows;
    using System.Windows.Automation.Peers;
    using System.Windows.Automation.Provider;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Interactivity;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    public class MouseWheelScrollBehavior : Behavior<UIElement>
    {
        #region Fields

        /// <summary>
        /// IsScrollInversed depedency property.
        /// </summary>
        public static readonly DependencyProperty IsScrollInversedProperty = 
            DependencyProperty.Register(
                "IsScrollInversed",
                typeof(bool),
                typeof(MouseWheelScrollBehavior),
                null);

        /// <summary>
        /// Orientation depedency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty = 
            DependencyProperty.Register(
                "Orientation",
                typeof(Orientation),
                typeof(MouseWheelScrollBehavior),
                null);

        #endregion Fields

        #region Properties

        public bool IsScrollInversed
        {
            get
            {
                return (bool)GetValue(IsScrollInversedProperty);
            }

            set
            {
                SetValue(IsScrollInversedProperty, value);
            }
        }

        public Orientation Orientation
        {
            get
            {
                return (Orientation)GetValue(OrientationProperty);
            }

            set
            {
                SetValue(OrientationProperty, value);
            }
        }

        #endregion Properties

        #region Methods

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.MouseWheel += new MouseWheelEventHandler(AssociatedObject_MouseWheel);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.MouseWheel -= new MouseWheelEventHandler(AssociatedObject_MouseWheel);
        }

        void AssociatedObject_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Orientation orientation = Orientation;

            AutomationPeer peer = FrameworkElementAutomationPeer.FromElement(AssociatedObject);
            if (peer == null)
                peer = FrameworkElementAutomationPeer.CreatePeerForElement(AssociatedObject);

            IScrollProvider scrollProvider = peer.GetPattern(PatternInterface.Scroll) as IScrollProvider;
            if (scrollProvider == null)
                return;

            var scrollAmount = IsScrollInversed ? System.Windows.Automation.ScrollAmount.SmallIncrement : System.Windows.Automation.ScrollAmount.SmallDecrement;
            if (e.Delta < 0)
            {
                if (scrollAmount == System.Windows.Automation.ScrollAmount.SmallIncrement)
                    scrollAmount = System.Windows.Automation.ScrollAmount.SmallDecrement;
                else
                    scrollAmount = System.Windows.Automation.ScrollAmount.SmallIncrement;
            }

            if (orientation == Orientation.Vertical)
            {
                if (!scrollProvider.VerticallyScrollable)
                    return;

                if (e.Delta > 0)
                {
                    scrollProvider.Scroll(System.Windows.Automation.ScrollAmount.NoAmount, scrollAmount);
                }
                else if (e.Delta < 0)
                {
                    scrollProvider.Scroll(System.Windows.Automation.ScrollAmount.NoAmount, scrollAmount);
                }
            }
            else
            {
                if (!scrollProvider.HorizontallyScrollable)
                    return;

                if (e.Delta > 0)
                {
                    scrollProvider.Scroll(scrollAmount, System.Windows.Automation.ScrollAmount.NoAmount);
                }
                else if (e.Delta < 0)
                {
                    scrollProvider.Scroll(scrollAmount, System.Windows.Automation.ScrollAmount.NoAmount);
                }
            }
            e.Handled = true;
        }

        #endregion Methods
    }
}