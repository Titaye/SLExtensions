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

    public static class CursorService
    {
        #region Fields

        // Using a DependencyProperty as the backing store for CursorManager.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CursorManagerProperty = 
            DependencyProperty.RegisterAttached("CursorManager", typeof(CursorManager), typeof(CursorService), new PropertyMetadata(CursorManagerChangedCallback));

        // Using a DependencyProperty as the backing store for Cursor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CursorProperty = 
            DependencyProperty.RegisterAttached("Cursor", typeof(FrameworkElement), typeof(CursorService), new PropertyMetadata(CursorChangedCallback));

        private static Point lastPosition;

        #endregion Fields

        #region Constructors

        static CursorService()
        {
            CursorPopup = new Popup();
            CursorPopup.IsHitTestVisible = false;
            CursorPopup.Cursor = Cursors.None;
            Canvas.SetZIndex(CursorPopup, 1000000);
        }

        #endregion Constructors

        #region Properties

        public static Popup CursorPopup
        {
            get; private set;
        }

        #endregion Properties

        #region Methods

        public static FrameworkElement GetCursor(DependencyObject obj)
        {
            return (FrameworkElement)obj.GetValue(CursorProperty);
        }

        public static void SetCursor(DependencyObject obj, FrameworkElement value)
        {
            obj.SetValue(CursorProperty, value);
        }

        private static void CursorChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement fe = d as FrameworkElement;
            if (fe == null)
                return;

            CursorManager cm = EnsureCursorManager(fe);
            cm.Cursor = e.NewValue as FrameworkElement;
        }

        private static void CursorManagerChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        private static CursorManager EnsureCursorManager(FrameworkElement fe)
        {
            if (fe == null)
                return null;

            CursorManager cm = GetCursorManager(fe);
            if (cm == null)
            {
                cm = new CursorManager(fe);
                SetCursorManager(fe, cm);
            }

            return cm;
        }

        private static CursorManager GetCursorManager(DependencyObject obj)
        {
            return (CursorManager)obj.GetValue(CursorManagerProperty);
        }

        private static void SetCursorManager(DependencyObject obj, CursorManager value)
        {
            obj.SetValue(CursorManagerProperty, value);
        }

        private static void SetCursorPosition(MouseEventArgs e, FrameworkElement cursor)
        {
            Point pos = e.GetPosition(Application.Current.RootVisual);
            if (pos != lastPosition)
            {
                CursorPopup.Child = cursor;
                CursorPopup.IsOpen = true;
                CursorPopup.VerticalOffset = pos.Y;
                CursorPopup.HorizontalOffset = pos.X;
                lastPosition = pos;
            }
        }

        #endregion Methods

        #region Nested Types

        private class CursorManager
        {
            #region Fields

            private FrameworkElement cursor;
            private FrameworkElement element;

            #endregion Fields

            #region Constructors

            public CursorManager(FrameworkElement element)
            {
                this.element = element;
                this.element.MouseEnter += new MouseEventHandler(element_MouseEnter);
                this.element.MouseLeave += new MouseEventHandler(element_MouseLeave);
                this.element.Cursor = Cursors.None;
            }

            #endregion Constructors

            #region Properties

            public FrameworkElement Cursor
            {
                get
                {
                    return this.cursor;
                }

                set
                {
                    this.cursor = value;
                    if (value != null)
                    {
                        value.IsHitTestVisible = false;
                        value.Cursor = Cursors.None;
                    }
                }
            }

            #endregion Properties

            #region Methods

            void element_MouseEnter(object sender, MouseEventArgs e)
            {
                this.element.MouseMove -= new MouseEventHandler(element_MouseMove);
                this.element.MouseMove += new MouseEventHandler(element_MouseMove);
                SetCursorPosition(e, Cursor);
            }

            void element_MouseLeave(object sender, MouseEventArgs e)
            {
                this.element.MouseMove -= new MouseEventHandler(element_MouseMove);
                CursorPopup.IsOpen = false;
            }

            void element_MouseMove(object sender, MouseEventArgs e)
            {
                SetCursorPosition(e, Cursor);
            }

            #endregion Methods
        }

        #endregion Nested Types
    }
}