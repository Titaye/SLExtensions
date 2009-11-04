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
    /// <summary>
    /// Separates UI Elements
    /// </summary>
    public class Separator : Control
    {
        public Separator()
        {
            DefaultStyleKey = typeof(Separator);
            this.Loaded += new RoutedEventHandler(Separator_Loaded);
        }

        void Separator_Loaded(object sender, RoutedEventArgs e)
        {
            if (!AutomaticOrientation)
                return;

            var parentAsMSStackPanel = Parent as System.Windows.Controls.StackPanel;
            if (parentAsMSStackPanel != null)
            {
                Orientation = parentAsMSStackPanel.Orientation;
                return;
            }

            var parentAsStackPanel = Parent as StackPanel;
            if (parentAsStackPanel != null)
            {
                Orientation = parentAsStackPanel.Orientation;
                return;
            }

            var parentAsGrid = Parent as Grid;
            if (parentAsGrid != null)
            {
                if (VerticalAlignment == VerticalAlignment.Bottom ||
                    VerticalAlignment == VerticalAlignment.Top)
                    Orientation = Orientation.Vertical;
                else Orientation = Orientation.Horizontal;
                return;
            }
            var parentAsDockPanel = Parent as DockPanel;
            if (parentAsDockPanel != null)
            {
                var dock = DockPanel.GetDock(this);
                if (dock == Dock.Bottom || dock == Dock.Top)
                    Orientation = Orientation.Vertical;
                else
                    Orientation = Orientation.Horizontal;
                return;
            }
        }

        #region Properties

        /// <summary>
        /// If the parent of the separator is a StackPanel, Grid or DockPanel, automatically set the Orientation property
        /// </summary>
        public bool AutomaticOrientation
        {
            get { return (bool)GetValue(AutomaticOrientationProperty); }
            set { SetValue(AutomaticOrientationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AutomaticOrientationProperty =
            DependencyProperty.Register("AutomaticOrientation", typeof(bool), typeof(Separator), new PropertyMetadata(true,
                (sender, args) =>
                {
                    var asOwning = sender as Separator;
                    if (asOwning == null)
                        return;
                    var oldValue = args.OldValue == null ? default(bool) : (bool)args.OldValue;
                    var newValue = args.NewValue == null ? default(bool) : (bool)args.NewValue;

                    asOwning.OnAutomaticOrientationChanged(oldValue, newValue);
                }));
        protected virtual void OnAutomaticOrientationChanged(bool oldAutomaticOrientation, bool newAutomaticOrientation)
        {

        }

        /// <summary>
        /// Orientation of the item container (for example, for a StatusBar separator, should be Horizontal)
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(Separator), new PropertyMetadata(Orientation.Horizontal,
                (sender, args) =>
                {
                    var asOwning = sender as Separator;
                    if (asOwning == null)
                        return;
                    var oldValue = args.OldValue == null ? default(Orientation) : (Orientation)args.OldValue;
                    var newValue = args.NewValue == null ? default(Orientation) : (Orientation)args.NewValue;

                    asOwning.OnOrientationChanged(oldValue, newValue);
                }));
        protected virtual void OnOrientationChanged(Orientation oldOriantation, Orientation newOriantation)
        {

        }

        #endregion
    }
}
