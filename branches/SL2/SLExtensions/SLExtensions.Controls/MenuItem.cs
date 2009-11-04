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
using System.Windows.Controls.Primitives;
using System.Collections;

namespace SLExtensions.Controls
{
    [TemplatePart(Name = PopupName, Type = typeof(Popup))]
    [TemplatePart(Name = HeaderControlName, Type = typeof(ContentControl))]
    [TemplatePart(Name = SubMenuListName, Type = typeof(ListBox))]
    [TemplateVisualState(GroupName = "CommonStates", Name = "Normal")]
    [TemplateVisualState(GroupName = "CommonStates", Name = "MouseOver")]
    public class MenuItem : Control
    {
        public MenuItem()
        {
            DefaultStyleKey = typeof(MenuItem);
            Loaded += delegate
            {
                this.UpdateVisualState(true);
            };

            this.LostFocus += new RoutedEventHandler(MenuItem_LostFocus);
        }

        void MenuItem_LostFocus(object sender, RoutedEventArgs e)
        {
            if (popup != null)
                popup.IsOpen = false;
        }

        #region Header

        public object Header
        {
            get
            {
                return (object)GetValue(HeaderProperty);
            }

            set
            {
                SetValue(HeaderProperty, value);
            }
        }

        /// <summary>
        /// Header depedency property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(
                "Header",
                typeof(object),
                typeof(MenuItem),
                new PropertyMetadata((d, e) => ((MenuItem)d).OnHeaderChanged((object)e.OldValue, (object)e.NewValue)));

        /// <summary>
        /// handles the HeaderProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnHeaderChanged(object oldValue, object newValue)
        {

        }

        #endregion Header

        #region HeaderTemplate

        public DataTemplate HeaderTemplate
        {
            get
            {
                return (DataTemplate)GetValue(HeaderTemplateProperty);
            }

            set
            {
                SetValue(HeaderTemplateProperty, value);
            }
        }

        /// <summary>
        /// HeaderTemplate depedency property.
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register(
                "HeaderTemplate",
                typeof(DataTemplate),
                typeof(MenuItem),
                new PropertyMetadata((d, e) => ((MenuItem)d).OnHeaderTemplateChanged((DataTemplate)e.OldValue, (DataTemplate)e.NewValue)));

        /// <summary>
        /// handles the HeaderTemplateProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnHeaderTemplateChanged(DataTemplate oldValue, DataTemplate newValue)
        {

        }

        #endregion HeaderTemplate

        #region ItemsSource

        public IEnumerable ItemsSource
        {
            get
            {
                return (IEnumerable)GetValue(ItemsSourceProperty);
            }

            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        /// <summary>
        /// ItemsSource depedency property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                "ItemsSource",
                typeof(IEnumerable),
                typeof(MenuItem),
                new PropertyMetadata((d, e) => ((MenuItem)d).OnItemsSourceChanged((IEnumerable)e.OldValue, (IEnumerable)e.NewValue)));

        /// <summary>
        /// handles the ItemsSourceProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {

        }

        #endregion ItemsSource

        private Popup popup;
        private ContentControl headerControl;
        private ListBox subMenu;
        private const string PopupName = "Popup";
        private const string HeaderControlName = "HeaderControl";
        private const string SubMenuListName = "SubMenuList";

        public override void OnApplyTemplate()
        {
            popup = GetTemplateChild(PopupName) as Popup;
            
            headerControl = GetTemplateChild(HeaderControlName) as ContentControl;

            subMenu = GetTemplateChild(SubMenuListName) as ListBox;

            

            if (headerControl != null)
            {
                headerControl.MouseEnter += new MouseEventHandler(headerControl_MouseEnter);
                headerControl.MouseLeave += new MouseEventHandler(headerControl_MouseLeave);
                headerControl.MouseLeftButtonDown += new MouseButtonEventHandler(headerControl_MouseLeftButtonDown);
            }
            
            base.OnApplyTemplate();
        }

        private bool isInnerControl(FrameworkElement element)
        {
            FrameworkElement parent = element;
            while (parent != null)
            {
                if (parent == this || parent == subMenu)
                    return true;
                parent = parent.Parent as FrameworkElement;
            }
            return false;
        }

        void headerControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Focus();
            if (popup != null)
            {
                if (!popup.IsOpen)
                    popup.IsOpen = true;
                else
                    popup.IsOpen = false;
            }
        }

        void headerControl_MouseLeave(object sender, MouseEventArgs e)
        {
            IsMouseOver = true;
        }

        void headerControl_MouseEnter(object sender, MouseEventArgs e)
        {
            IsMouseOver = false;            
        }

        #region IsMouseOver

        public bool IsMouseOver
        {
            get
            {
                return (bool)GetValue(IsMouseOverProperty);
            }

            set
            {
                SetValue(IsMouseOverProperty, value);
            }
        }

        /// <summary>
        /// IsMouseOver depedency property.
        /// </summary>
        public static readonly DependencyProperty IsMouseOverProperty =
            DependencyProperty.Register(
                "IsMouseOver",
                typeof(bool),
                typeof(MenuItem),
                new PropertyMetadata((d, e) => ((MenuItem)d).OnIsMouseOverChanged((bool)e.OldValue, (bool)e.NewValue)));

        /// <summary>
        /// handles the IsMouseOverProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnIsMouseOverChanged(bool oldValue, bool newValue)
        {
            UpdateVisualState(true);
        }

        #endregion IsMouseOver

        


        /// <summary>
        /// Goes to the specified visual states. Stop on first state name found
        /// </summary>
        /// <param name="useTransitions">if set to <c>true</c> uses transitions.</param>
        /// <param name="stateNames">The state names.</param>
        private void GoToState(bool useTransitions, params string[] stateNames)
        {
            if (stateNames != null)
            {
                foreach (string str in stateNames)
                {
                    if (VisualStateManager.GoToState(this, str, useTransitions))
                    {
                        return;
                    }
                }
            }
        }

                /// <summary>
        /// Updates the visual state.
        /// </summary>
        /// <param name="useTransitions">if set to <c>true</c> uses transitions.</param>
        private void UpdateVisualState(bool useTransitions)
        {
            if (IsMouseOver)
            {
                GoToState(true, "MouseOver", "Normal");
            }
            else
            {
                GoToState(true, "Normal");
            }
        }
    }
}
