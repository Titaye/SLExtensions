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
    /// ComboBox with better layout-behavior regarding dynamic data and 
    /// ScaleTransforms
    /// </summary>
    public class FixedComboBox : ComboBox
    {
        #region Fields

        FrameworkElement _elementPopupChild;
        Popup _popup;

        #endregion Fields

        #region Methods

        public override void OnApplyTemplate()
        {
            _popup = GetTemplateChild("Popup") as Popup;
            _elementPopupChild = _popup.Child as FrameworkElement;
            base.OnApplyTemplate();
        }

        protected override void OnDropDownOpened(EventArgs e)
        {
            base.OnDropDownOpened(e);
            if (_elementPopupChild == null)
                return;
            FrameworkElement page = Application.Current.RootVisual as FrameworkElement;

            var gt = _popup.TransformToVisual(page) as MatrixTransform;
            double yOffset = 0.0;
            double yScale = 1.0;
            if (gt != null)
            {
                yOffset = gt.Matrix.OffsetY;
                yScale = gt.Matrix.M22;
            }

            var pageHeight = page.ActualHeight;
            var availableHeight = pageHeight - yOffset;
            var scaledMaxHeight = availableHeight / yScale - this.ActualHeight;

            if (_elementPopupChild.MaxHeight != double.NaN && _elementPopupChild.MaxHeight != double.PositiveInfinity)
                scaledMaxHeight = Math.Min(scaledMaxHeight, _elementPopupChild.MaxHeight);
            _elementPopupChild.MaxHeight = scaledMaxHeight > 0 ? scaledMaxHeight : 0;
        }

        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            if (_elementPopupChild != null)
            {
                _elementPopupChild.ClearValue(FrameworkElement.MinWidthProperty);
                _elementPopupChild.ClearValue(FrameworkElement.MinHeightProperty);
                _elementPopupChild.ClearValue(FrameworkElement.MaxWidthProperty);
                _elementPopupChild.ClearValue(FrameworkElement.MaxHeightProperty);
                _elementPopupChild.ClearValue(FrameworkElement.WidthProperty);
                _elementPopupChild.ClearValue(FrameworkElement.HeightProperty);

                this.InvalidateArrange();
            }
        }

        #endregion Methods
    }
}