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
    /// Decorates another control with a Border and InnerBorder
    /// </summary>
    public class Decorator : ContentControl
    {
        public Decorator()
        {
            this.DefaultStyleKey = typeof(Decorator);
        }

        #region Properties
        public Thickness InnerBorderThickness
        {
            get { return (Thickness)GetValue(InnerBorderThicknessProperty); }
            set { SetValue(InnerBorderThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InnerBorderThicknessProperty =
            DependencyProperty.Register("InnerBorderThickness", typeof(Thickness), typeof(Decorator), new PropertyMetadata(new Thickness(1),
                (sender, args) =>
                {
                    var asOwning = sender as Decorator;
                    if (asOwning == null)
                        return;
                    var oldValue = args.OldValue == null ? default(Thickness) : (Thickness)args.OldValue;
                    var newValue = args.NewValue == null ? default(Thickness) : (Thickness)args.NewValue;

                    asOwning.OnInnerBorderThicknessChanged(oldValue, newValue);
                }));
        protected virtual void OnInnerBorderThicknessChanged(Thickness oldInnerBorderThickness, Thickness newInnerBorderThickness)
        {

        }

        public Brush InnerBorderBrush
        {
            get { return (Brush)GetValue(InnerBorderBrushProperty); }
            set { SetValue(InnerBorderBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InnerBorderBrushProperty =
            DependencyProperty.Register("InnerBorderBrush", typeof(Brush), typeof(Decorator), new PropertyMetadata(null,
                (sender, args) =>
                {
                    var asOwning = sender as Decorator;
                    if (asOwning == null)
                        return;
                    var oldValue = args.OldValue == null ? default(Brush) : (Brush)args.OldValue;
                    var newValue = args.NewValue == null ? default(Brush) : (Brush)args.NewValue;

                    asOwning.OnInnerBorderBrushChanged(oldValue, newValue);
                }));
        protected virtual void OnInnerBorderBrushChanged(Brush oldInnerBorderBrush, Brush newInnerBorderBrush)
        {

        }
        #endregion
    }
}
