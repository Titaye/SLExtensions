namespace SLExtensions.Globalization
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Linq;
    using System.Resources;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.Collections;
    using System.Globalization;
    using System.ComponentModel;

    public class Localizer
    {
        static Localizer()
        {
            CachedResourceNames = new Dictionary<ResourceManager, Dictionary<string, string[]>>();
        }
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
                    if (localizer != null)
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

        public static Dictionary<ResourceManager, Dictionary<string, string[]>> CachedResourceNames { get; set; }

        public virtual void Localize(DependencyObject d)
        {
            ResourceManager rm = GetResourceManager(d);

            if (rm == null)
            {
                if (Application.Current.RootVisual != null)
                    rm = GetResourceManager(Application.Current.RootVisual);
            }

            if (rm == null)
            {
                rm = DefaultResourceManager;
            }

            var app = Application.Current;
            if (rm == null && app != null && app.RootVisual != null)
            {
                var rootVisualAssembly = app.RootVisual.GetType().Assembly;
                var resourceName = rootVisualAssembly.ToString().Split(',').First() + ".Resources";
                if (rootVisualAssembly.GetManifestResourceNames().Contains(resourceName + ".resources"))
                    rm = new ResourceManager(resourceName, rootVisualAssembly);
                else
                    return;
            }

            string key = GetKey(d);
            if (!string.IsNullOrEmpty(key))
            {
                var resourceNames = GetResourceNames(rm);
                if (resourceNames != null)
                {
                    string[] propertyNames;
                    if (resourceNames.TryGetValue(key, out propertyNames))
                    {
                        Type objectType = d.GetType();
                        var propToAssign = from p in objectType.GetProperties()
                                           where propertyNames.Contains(p.Name)
                                           && p.CanWrite
                                           select p;

                        foreach (var prop in propToAssign)
                        {
                            var val = rm.GetObject(key + "_" + prop.Name);
                            if (prop.PropertyType.IsAssignableFrom(val.GetType()))
                            {
                                prop.SetValue(d, val, null);
                            }
                            else if (prop.PropertyType.IsEnum)
                            {
                                var eval = Enum.Parse(prop.PropertyType, Convert.ToString(val), false);
                                if (eval != null)
                                {
                                    prop.SetValue(d, eval, null);
                                }
                            }
                            else
                            {
                                try
                                {
                                    System.Convert.ChangeType(val, prop.PropertyType, CultureInfo.CurrentCulture);
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                }
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

        private Dictionary<string, string[]> GetResourceNames(ResourceManager rm)
        {
            Dictionary<string, string[]> result;
            if (!CachedResourceNames.TryGetValue(rm, out result))
            {
                result = (from dicEntry in rm.GetResourceSet(System.Globalization.CultureInfo.CurrentCulture, true, true)
                                        .OfType<DictionaryEntry>()
                          let keyParts = ((string)((DictionaryEntry)dicEntry).Key).Split(new[] { '_' }, 2)
                          where keyParts.Length == 2
                          let kv = new { k = keyParts[0], v = keyParts[1] }
                          group kv by kv.k into g
                          select g)
                        .ToDictionary(i => i.Key, i => i.Select(_ => _.v).ToArray());
                CachedResourceNames.Add(rm, result);
            }

            return result;
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



        public static string GetKey(DependencyObject obj)
        {
            return (string)obj.GetValue(KeyProperty);
        }

        public static void SetKey(DependencyObject obj, string value)
        {
            obj.SetValue(KeyProperty, value);
        }

        // Using a DependencyProperty as the backing store for Key.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.RegisterAttached("Key", typeof(string), typeof(Localizer), new PropertyMetadata(OnPropertyChanged));
    }
}
