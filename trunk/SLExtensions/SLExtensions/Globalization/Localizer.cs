namespace SLExtensions.Globalization
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Resources;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    public class Localizer
    {
        #region Fields

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentKeyProperty = 
            DependencyProperty.RegisterAttached("ContentKey", typeof(string), typeof(Localizer), new PropertyMetadata(OnPropertyChanged));

        // Using a DependencyProperty as the backing store for Localizer.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LocalizerProperty = 
            DependencyProperty.RegisterAttached("Localizer", typeof(Localizer), typeof(Localizer), new PropertyMetadata(LocalizerChangedCallback));

        // Using a DependencyProperty as the backing store for ResourceManager.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ResourceManagerProperty = 
            DependencyProperty.RegisterAttached("ResourceManager", typeof(ResourceManager), typeof(Localizer), new PropertyMetadata(OnPropertyChanged));

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextKeyProperty = 
            DependencyProperty.RegisterAttached("TextKey", typeof(string), typeof(Localizer), new PropertyMetadata(OnPropertyChanged));

        // Using a DependencyProperty as the backing store for Tooltip.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TooltipKeyProperty = 
            DependencyProperty.RegisterAttached("TooltipKey", typeof(string), typeof(Localizer), new PropertyMetadata(OnPropertyChanged));

        private static ResourceManager defaultResourceManager;
        private static List<WeakReference> localizedControls = new List<WeakReference>();

        #endregion Fields

        #region Properties

        public static ResourceManager DefaultResourceManager
        {
            get
            {
                return defaultResourceManager;
            }
            set
            {
                if (defaultResourceManager != value)
                {
                    defaultResourceManager = value;
                    RefreshLocalization();
                }
            }
        }

        #endregion Properties

        #region Methods

        public static string GetContentKey(DependencyObject obj)
        {
            return (string)obj.GetValue(ContentKeyProperty);
        }

        public static Localizer GetLocalizer(DependencyObject obj)
        {
            return (Localizer)obj.GetValue(LocalizerProperty);
        }

        public static ResourceManager GetResourceManager(DependencyObject obj)
        {
            return (ResourceManager)obj.GetValue(ResourceManagerProperty);
        }

        public static string GetTextKey(DependencyObject obj)
        {
            return (string)obj.GetValue(TextKeyProperty);
        }

        public static string GetTooltipKey(DependencyObject obj)
        {
            return (string)obj.GetValue(TooltipKeyProperty);
        }

        public static void RefreshLocalization()
        {
            for (int i = localizedControls.Count - 1; i >= 0; i--)
            {
                DependencyObject controlToBeRefreshed = localizedControls[i].Target as DependencyObject;
                if (controlToBeRefreshed == null)
                    localizedControls.RemoveAt(i);
                else
                {
                    var localizer = GetLocalizer(controlToBeRefreshed);
                    if(localizer != null)
                        localizer.Localize(controlToBeRefreshed);
                    else
                        localizedControls.RemoveAt(i);
                }
            }
        }

        public static void SetContentKey(DependencyObject obj, string value)
        {
            obj.SetValue(ContentKeyProperty, value);
        }

        public static void SetLocalizer(DependencyObject obj, Localizer value)
        {
            obj.SetValue(LocalizerProperty, value);
        }

        public static void SetResourceManager(DependencyObject obj, ResourceManager value)
        {
            obj.SetValue(ResourceManagerProperty, value);
        }

        public static void SetTextKey(DependencyObject obj, string value)
        {
            obj.SetValue(TextKeyProperty, value);
        }

        public static void SetTooltipKey(DependencyObject obj, string value)
        {
            obj.SetValue(TooltipKeyProperty, value);
        }

        public virtual void Localize(DependencyObject d)
        {
            ResourceManager rm = GetResourceManager(d);

            if (rm == null)
            {
                if(Application.Current.RootVisual != null)
                    rm = GetResourceManager(Application.Current.RootVisual);
            }

            if (rm == null)
            {
                rm = DefaultResourceManager;
            }

            if(rm == null)
            {
                return;
            }

            string textKey = GetTextKey(d);
            if (textKey != null)
            {
                TextBlock tb = d as TextBlock;
                if (tb != null)
                {
                    tb.Text = rm.GetString(textKey, System.Globalization.CultureInfo.CurrentCulture);
                }

                TextBox tbox = d as TextBox;
                if (tbox != null)
                {
                    tbox.Text = rm.GetString(textKey, System.Globalization.CultureInfo.CurrentCulture);
                }
            }

            string tooltipKey = GetTooltipKey(d);
            if (tooltipKey != null)
            {
                ToolTipService.SetToolTip(d, rm.GetString(tooltipKey, System.Globalization.CultureInfo.CurrentCulture));
            }

            string contentKey = GetContentKey(d);
            if (contentKey != null)
            {
                ContentControl cc = d as ContentControl;
                if (cc != null)
                {
                    cc.Content = rm.GetString(contentKey, System.Globalization.CultureInfo.CurrentCulture);
                }
            }
        }

        private static void LocalizerChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Localizer newLocalizer = e.NewValue as Localizer;
            if (newLocalizer != null)
                newLocalizer.Localize(d);
        }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Localizer localizer = GetLocalizer(d);
            if (localizer == null)
            {
                localizer = new Localizer();
                localizedControls.Add(new WeakReference(d));
                SetLocalizer(d, localizer);
            }

            localizer.Localize(d);
        }

        #endregion Methods
    }
}