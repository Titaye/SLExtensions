using System;
using SLExtensions;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SLExtensions.Controls.ControlExtensions
{
    /// <summary>
    /// Forward properties from a DataTemplate to the ContentPresenter ancestor. Usefull in ItemsControl when ItemsPanel is a Canvas or Grid for settings Canvas.Left / Top or Grid.Row / Column
    /// </summary>
    public class ContentPresenterExtensions
    {
        public ContentPresenterExtensions(FrameworkElement element)
        {
            this.Element = element;
            this.Element.Loaded += new RoutedEventHandler(Element_Loaded);
        }

        void Element_Loaded(object sender, RoutedEventArgs e)
        {
            SetProperties();
        }

        private static ContentPresenter FindPresenter(FrameworkElement element)
        {
            //TODO: add attached property with maxdistance ?
            return element.FirstVisualAncestorOfType<ContentPresenter>(1);
        }

        private bool HasValue<T>(DependencyProperty prop, out T value)
        {
            value = default(T);
            object obj = Element.GetValue(prop);
            if (obj != null)
            {
                value = (T)obj;
                return true;
            }
            return false;
        }

        private void SetProperties()
        {
            var presenter = FindPresenter(Element);
            if (presenter == null)
                return;

            int val;
            if(HasValue(GridColumnProperty, out val))
                Grid.SetColumn(presenter, val);

            if (HasValue(GridRowProperty, out val))
                Grid.SetRow(presenter, val);

            double dval;
            if (HasValue(CanvasLeftProperty, out dval))
                Canvas.SetLeft(presenter, dval);

            if (HasValue(CanvasTopProperty, out dval))
                Canvas.SetTop(presenter, dval);

        }

        public FrameworkElement Element { get; private set; }

        public static int GetGridColumn(DependencyObject obj)
        {
            return (int)obj.GetValue(GridColumnProperty);
        }

        public static void SetGridColumn(DependencyObject obj, int value)
        {
            obj.SetValue(GridColumnProperty, value);
        }

        // Using a DependencyProperty as the backing store for GridColumn.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GridColumnProperty =
            DependencyProperty.RegisterAttached("GridColumn", typeof(int), typeof(ContentPresenterExtensions), new PropertyMetadata(GridColumnChangedCallback));

        private static void GridColumnChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GetExtensions(d).SetProperties();
        }



        public static int GetGridRow(DependencyObject obj)
        {
            return (int)obj.GetValue(GridRowProperty);
        }

        public static void SetGridRow(DependencyObject obj, int value)
        {
            obj.SetValue(GridRowProperty, value);
        }

        // Using a DependencyProperty as the backing store for GridRow.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GridRowProperty =
            DependencyProperty.RegisterAttached("GridRow", typeof(int), typeof(ContentPresenterExtensions), new PropertyMetadata(GridRowChangedCallback));

        private static void GridRowChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GetExtensions(d).SetProperties();
        }




        public static double GetCanvasLeft(DependencyObject obj)
        {
            return (double)obj.GetValue(CanvasLeftProperty);
        }

        public static void SetCanvasLeft(DependencyObject obj, double value)
        {
            obj.SetValue(CanvasLeftProperty, value);
        }

        // Using a DependencyProperty as the backing store for CanvasLeft.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanvasLeftProperty =
            DependencyProperty.RegisterAttached("CanvasLeft", typeof(double), typeof(ContentPresenterExtensions), new PropertyMetadata(CanvasLeftChangedCallback));

        private static void CanvasLeftChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GetExtensions(d).SetProperties();
        }



        public static double GetCanvasTop(DependencyObject obj)
        {
            return (double)obj.GetValue(CanvasTopProperty);
        }

        public static void SetCanvasTop(DependencyObject obj, double value)
        {
            obj.SetValue(CanvasTopProperty, value);
        }

        // Using a DependencyProperty as the backing store for CanvasTop.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanvasTopProperty =
            DependencyProperty.RegisterAttached("CanvasTop", typeof(double), typeof(ContentPresenterExtensions), new PropertyMetadata(CanvasTopChangedCallback));

        private static void CanvasTopChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GetExtensions(d).SetProperties();
        }





        // Using a DependencyProperty as the backing store for GridRow.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty ItemsPresenterExtensionsProperty =
            DependencyProperty.RegisterAttached("ItemsPresenterExtensions", typeof(ContentPresenterExtensions), typeof(ContentPresenterExtensions), null);

        private static ContentPresenterExtensions GetExtensions(DependencyObject obj)
        {
            FrameworkElement fe = obj as FrameworkElement;
            if (fe == null)
                throw new NotSupportedException("ItemsPresenterExtensions must be assigned to a frameworkelement");

            var ext = fe.GetValue(ItemsPresenterExtensionsProperty) as ContentPresenterExtensions;
            if (ext == null)
            {
                ext = new ContentPresenterExtensions(fe);
                fe.SetValue(ItemsPresenterExtensionsProperty, ext);
            }
            return ext;
        }

    }
}
