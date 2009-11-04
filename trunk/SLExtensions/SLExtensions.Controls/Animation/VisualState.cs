namespace SLExtensions.Controls.Animation
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    public static class VisualState
    {
        #region Fields

        // Using a DependencyProperty as the backing store for LoadedState.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LoadedStateProperty = 
            DependencyProperty.RegisterAttached("LoadedState", typeof(string), typeof(VisualState), new PropertyMetadata(PropertyChangedCallback));

        // Using a DependencyProperty as the backing store for MouseOverState.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MouseOverStateProperty = 
            DependencyProperty.RegisterAttached("MouseOverState", typeof(string), typeof(VisualState), new PropertyMetadata(PropertyChangedCallback));

        // Using a DependencyProperty as the backing store for NormalState.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NormalStateProperty = 
            DependencyProperty.RegisterAttached("NormalState", typeof(string), typeof(VisualState), new PropertyMetadata(PropertyChangedCallback));
        public static readonly DependencyProperty subscriptionProperty = 
            DependencyProperty.RegisterAttached("subscription", typeof(subscription), typeof(VisualState), null);

        #endregion Fields

        #region Methods

        public static string GetLoadedState(DependencyObject obj)
        {
            return (string)obj.GetValue(LoadedStateProperty);
        }

        public static string GetMouseOverState(DependencyObject obj)
        {
            return (string)obj.GetValue(MouseOverStateProperty);
        }

        public static string GetNormalState(DependencyObject obj)
        {
            return (string)obj.GetValue(NormalStateProperty);
        }

        public static bool GoToState(FrameworkElement element, bool useTransitions, params string[] stateNames)
        {
            VisualStateManager.SetCustomVisualStateManager(element, new customVisualStateManager());
            if (stateNames != null)
            {
                foreach (string str in stateNames)
                {
                    if (customVisualStateManager.GoToState(element, str, useTransitions))
                        return true;
                }
            }
            return false;
        }

        public static void GoToState(Control control, bool useTransitions, params string[] stateNames)
        {
            if (control == null)
            {
                throw new ArgumentNullException("control");
            }
            if (stateNames != null)
            {
                foreach (string str in stateNames)
                {
                    if (VisualStateManager.GoToState(control, str, useTransitions))
                    {
                        return;
                    }
                }
            }
        }

        public static void SetLoadedState(DependencyObject obj, string value)
        {
            obj.SetValue(LoadedStateProperty, value);
        }

        public static void SetMouseOverState(DependencyObject obj, string value)
        {
            obj.SetValue(MouseOverStateProperty, value);
        }

        public static void SetNormalState(DependencyObject obj, string value)
        {
            obj.SetValue(NormalStateProperty, value);
        }

        private static subscription EnsureSubscription(FrameworkElement frameworkElement)
        {
            if (frameworkElement == null)
            {
                return null;
            }

            subscription subs = frameworkElement.GetValue(subscriptionProperty) as subscription;
            if (subs == null)
            {
                subs = new subscription(frameworkElement);
                frameworkElement.SetValue(subscriptionProperty, subs);
            }
            return subs;
        }

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            subscription subs = EnsureSubscription(d as FrameworkElement);

            if (subs != null)
            {
                subs.RefreshProperties();
            }
        }

        #endregion Methods

        #region Nested Types

        private class customVisualStateManager : VisualStateManager
        {
            #region Methods

            public static bool GoToState(FrameworkElement element, string stateName, bool useTransitions)
            {
                System.Windows.VisualState state;
                VisualStateGroup group;
                if (stateName == null)
                {
                    throw new ArgumentNullException("stateName");
                }

                IEnumerable<VisualStateGroup> visualStateGroups = GetVisualStateGroups(element).OfType<VisualStateGroup>();
                if (visualStateGroups == null)
                {
                    return false;
                }

                if (!TryGetState(visualStateGroups, stateName, out group, out state))
                {
                    if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(element))
                        throw new Exception("Unable to find state " + stateName + " for element " + element.Name);
                    else
                        return false;
                }

                customVisualStateManager customVisualStateManager = GetCustomVisualStateManager(element) as customVisualStateManager;
                if (customVisualStateManager != null)
                {
                    return customVisualStateManager.GoToStateCore(null, element, stateName, group, state, useTransitions);
                }

                return false;
            }

            internal static bool TryGetState(IEnumerable<VisualStateGroup> groups, string stateName, out VisualStateGroup group, out System.Windows.VisualState state)
            {
                var item = (from g in groups
                            from s in g.States.OfType<System.Windows.VisualState>()
                            where s.Name == stateName
                            select new
                    {
                        grp = g,
                        state = s
                    }).FirstOrDefault();
                if (item == null)
                {
                    group = null;
                    state = null;
                    return false;
                }
                group = item.grp;
                state = item.state;
                return true;
            }

            protected override bool GoToStateCore(Control control, FrameworkElement templateRoot, string stateName, VisualStateGroup group, System.Windows.VisualState state, bool useTransitions)
            {
                return base.GoToStateCore(control, templateRoot, stateName, group, state, useTransitions);
            }

            #endregion Methods
        }

        private class subscription
        {
            #region Fields

            private FrameworkElement element;
            private customVisualStateManager vsm;

            #endregion Fields

            #region Constructors

            public subscription(FrameworkElement element)
            {
                this.element = element;
                vsm = new customVisualStateManager();
                VisualStateManager.SetCustomVisualStateManager(element, vsm);
            }

            #endregion Constructors

            #region Methods

            public void RefreshProperties()
            {
                string mouseOverState = GetMouseOverState(element);
                if (!string.IsNullOrEmpty(mouseOverState))
                {
                    element.MouseEnter += new MouseEventHandler(element_MouseEnter);
                    element.MouseLeave += new MouseEventHandler(element_MouseLeave);
                }
                else
                {
                    element.MouseEnter -= new MouseEventHandler(element_MouseEnter);
                    element.MouseLeave -= new MouseEventHandler(element_MouseLeave);
                }

                string loadedState = GetLoadedState(element);
                if (!string.IsNullOrEmpty(loadedState))
                {
                    element.Loaded += new RoutedEventHandler(element_Loaded);
                }
                else
                {
                    element.Loaded -= new RoutedEventHandler(element_Loaded);
                }
            }

            void element_Loaded(object sender, RoutedEventArgs e)
            {
                string state = GetLoadedState(element);
                if (!string.IsNullOrEmpty(state))
                {
                    foreach (var item in state.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        customVisualStateManager.GoToState(element, item, true);
                    }
                }
            }

            void element_MouseEnter(object sender, MouseEventArgs e)
            {
                string state = GetMouseOverState(element);
                if (!string.IsNullOrEmpty(state))
                    customVisualStateManager.GoToState(element, state, true);
            }

            void element_MouseLeave(object sender, MouseEventArgs e)
            {
                string state = GetNormalState(element);
                if (!string.IsNullOrEmpty(state))
                    customVisualStateManager.GoToState(element, state, true);
            }

            #endregion Methods
        }

        #endregion Nested Types
    }
}