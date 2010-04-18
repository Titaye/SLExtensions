namespace SLExtensions.Controls
{
    using System.Windows;
    using System.Windows.Media;

    public class Clipper
    {
        #region Fields

        public static readonly DependencyProperty ClipToBoundsProperty = 
            DependencyProperty.RegisterAttached("ClipToBounds", typeof(bool), typeof(Clipper),
                                                new PropertyMetadata(ClipToBoundsChangedCallback));

        internal static readonly DependencyProperty FrameworkElementExtensionsProperty = 
            DependencyProperty.RegisterAttached("FrameworkElementExtensions", typeof(Clipper),
                                                typeof(Clipper), null);

        private FrameworkElement element;

        #endregion Fields

        #region Constructors

        private Clipper(FrameworkElement attachedObject)
        {
            element = attachedObject as FrameworkElement;
            if (element != null)
            {
                element.SizeChanged += ElementSizeChanged;
            }
        }

        #endregion Constructors

        #region Methods

        public static bool GetClipToBounds(DependencyObject obj)
        {
            return (bool)obj.GetValue(ClipToBoundsProperty);
        }

        public static void SetClipToBounds(DependencyObject obj, bool value)
        {
            obj.SetValue(ClipToBoundsProperty, value);
        }

        public void RefreshClip()
        {
            if (GetClipToBounds(element))
            {
                element.Clip = new RectangleGeometry { Rect = new Rect(0, 0, element.ActualWidth, element.ActualHeight) };
            }
            else
                element.Clip = null;
        }

        // Using a DependencyProperty as the backing store for ClipToBounds.  This enables animation, styling, binding, etc...
        internal static Clipper GetFrameworkElementExtensions(DependencyObject obj)
        {
            var element = obj as FrameworkElement;
            if (element == null)
                return null;

            var fe = obj.GetValue(FrameworkElementExtensionsProperty) as Clipper;
            if (fe == null)
            {
                fe = new Clipper(element);
                obj.SetValue(FrameworkElementExtensionsProperty, fe);
            }
            return fe;
        }

        // Using a DependencyProperty as the backing store for ClipToBounds.  This enables animation, styling, binding, etc...
        private static void ClipToBoundsChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Clipper ext = GetFrameworkElementExtensions(d);
            if (ext != null)
                ext.RefreshClip();
        }

        private void ElementSizeChanged(object sender, SizeChangedEventArgs e)
        {
            RefreshClip();
        }

        #endregion Methods
    }
}