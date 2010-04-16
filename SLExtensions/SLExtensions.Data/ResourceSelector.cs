using System;
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
using System.Windows.Data;
using System.Globalization;
using System.Windows.Markup;
using System.Collections.Generic;

namespace SLExtensions.Data
{
    /// <summary>
    /// Converter capable of selecting a resource from it's internal resources
    /// </summary>
    [ContentProperty("Resources")]
    public class ResourceSelector : DependencyObject,  IValueConverter
    {
        
        public ResourceSelector()
        {
        }

        #region Resources property
        /// <summary>
        /// Resources in which the selector can select
        /// </summary>
        public ResourceDictionary Resources
        {
            get { return (ResourceDictionary)GetValue(ResourcesProperty); }
            set { SetValue(ResourcesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ResourcesProperty =
            DependencyProperty.Register("Resources", typeof(ResourceDictionary), typeof(ResourceSelector), new PropertyMetadata(null,
                (sender, args)=>
                {
                  var asOwning = sender as ResourceSelector;
                  if(asOwning == null)
                    return;
                  var oldValue = args.OldValue == null ? default(ResourceDictionary) : (ResourceDictionary)args.OldValue;
                  var newValue = args.NewValue == null ? default(ResourceDictionary) : (ResourceDictionary)args.NewValue;
                  
                  asOwning.OnResourcesChanged(oldValue, newValue);
                }));
        protected virtual void OnResourcesChanged(ResourceDictionary oldResources, ResourceDictionary newResources)
        {
        }

        #endregion
        #region FallbackResourceKey property

        /// <summary>
        /// Resource key used when the selector fails to find matching resource
        /// </summary>
        public string FallbackResourceKey
        {
            get { return (string)GetValue(FallbackResourceKeyProperty); }
            set { SetValue(FallbackResourceKeyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FallbackResourceKeyProperty =
            DependencyProperty.Register("FallbackResourceKey", typeof(string), typeof(ResourceSelector), new PropertyMetadata(string.Empty,
                (sender, args)=>
                {
                  var asOwning = sender as ResourceSelector;
                  if(asOwning == null)
                    return;
                  var oldValue = args.OldValue == null ? default(string) : (string)args.OldValue;
                  var newValue = args.NewValue == null ? default(string) : (string)args.NewValue;
                  
                  asOwning.OnFallbackResourceKeyChanged(oldValue, newValue);
                }));
        protected virtual void OnFallbackResourceKeyChanged(string oldFallbackResourceKey, string newFallbackResourceKey)
        {
        }

        #endregion

        #region Behavior property
        /// <summary>
        /// Behavior of the selector (used to determine how to get a resource key from the provided value)
        /// Default value is Auto
        /// </summary>
        public ResourceSelectorBehavior Behavior
        {
            get { return (ResourceSelectorBehavior)GetValue(BehaviorProperty); }
            set { SetValue(BehaviorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BehaviorProperty =
            DependencyProperty.Register("Behavior", typeof(ResourceSelectorBehavior), typeof(ResourceSelector), new PropertyMetadata(ResourceSelectorBehavior.Auto,
                (sender, args)=>
                {
                  var asOwning = sender as ResourceSelector;
                  if(asOwning == null)
                    return;
                  var oldValue = args.OldValue == null ? default(ResourceSelectorBehavior) : (ResourceSelectorBehavior)args.OldValue;
                  var newValue = args.NewValue == null ? default(ResourceSelectorBehavior) : (ResourceSelectorBehavior)args.NewValue;
                  
                  asOwning.OnBehaviorChanged(oldValue, newValue);
                }));
        protected virtual void OnBehaviorChanged(ResourceSelectorBehavior oldBehavior, ResourceSelectorBehavior newBehavior)
        {
        }

        #endregion

        #region IValueConverter Members

        
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // if no resource dictionnary, return null
            if (Resources == null)
                return null;
            var behavior = Behavior;
            // if behavior is set as automatic, let's determine which is the most appropriate for the provided value type
            if (behavior == ResourceSelectorBehavior.Auto)
                behavior = DetermineResourceKeyBehavior(value);
            // if ExtractResource fails, let's use the fallback value
            return ExtractResource(value, behavior) ?? FallbackValue();
            
        }

        /// <summary>
        /// Extract the fallback value using the FallbackKey
        /// </summary>
        /// <returns></returns>
        private object FallbackValue()
        {
            if (Resources == null)
                return null;

            if (Resources.Contains(FallbackResourceKey))
                return Resources[FallbackResourceKey];
            return null;
        }

        /// <summary>
        /// Extract the value from the resource dictionnary using the given value and behavior
        /// </summary>
        /// <param name="value">value used to find the resource</param>
        /// <param name="behavior">behavior of the finder</param>
        /// <returns></returns>
        private object ExtractResource(object value, ResourceSelectorBehavior behavior)
        {
            if (Resources == null)
                return null;

            switch (behavior)
            {
                    // Simplest way : value as key
                case ResourceSelectorBehavior.ValueAsKey:
                    if (value == null)
                    {
                        if (Resources.Contains(string.Empty))
                            return Resources[string.Empty];
                        return null;
                    }
                    var asIProvideKey = value as IProvideResourceKey;
                    if (asIProvideKey != null)
                    {
                        if (Resources.Contains(asIProvideKey.ResourceKey))
                            return Resources[asIProvideKey.ResourceKey];
                        return null;
                    }
                    
                    var asString = value as string;
                    if (asString == null)
                        asString = System.Convert.ToString(value, CultureInfo.InvariantCulture);
                    if (asString != null)
                    {
                        if (Resources.Contains(asString))
                            return Resources[asString];
                        return null;
                    }
                    return null;
                    // More tricky to accept inheritance
                case ResourceSelectorBehavior.ValueTypeAsKey:
                    Type type = value.GetType();
                    // This particular Linq query is fabulous. Yes, I'm proud of it
                    return GetAncestorsAndSelf(type)
                        .Select(t => ExtractResourceFromType(t))
                        .Where(res => res != null)
                        .FirstOrDefault();
            }
            return null;
        }

        /// <summary>
        /// Returns the hierarchy stack of the given type
        /// </summary>
        /// <param name="t">Deepest type of the hierarchical type tree</param>
        /// <returns>Types of the hierarchy from the deepest one, to System.Object</returns>
        private IEnumerable<Type> GetAncestorsAndSelf(Type t)
        {
            for (; t != typeof(object); t = t.BaseType)
            {
                yield return t;
            }
            yield return typeof(object);
        }

        /// <summary>
        /// Try to extract the resource using the given type as a key
        /// - First trying with type.AssemblyQualifiedName
        /// - then type.FullName
        /// - then type.Name
        /// </summary>
        /// <param name="type">type to use as the key</param>
        /// <returns>The found resource or null</returns>
        private object ExtractResourceFromType(Type type)
        {
            if (Resources.Contains(type.AssemblyQualifiedName))
                return Resources[type.AssemblyQualifiedName];
            if (Resources.Contains(type.FullName))
                return Resources[type.FullName];
            if (Resources.Contains(type.Name))
                return Resources[type.Name];
            return null;
        }

        // this is internal for testability purpose
        /// <summary>
        /// Check the type of the value, and determine which behavior seems to be the most appropriate
        /// </summary>
        /// <param name="value">value to check</param>
        /// <returns>ValueAsKey or ValueTypeAsKey</returns>
        internal static ResourceSelectorBehavior DetermineResourceKeyBehavior(object value)
        {
            Type valType;
            if (value == null 
                || value is IProvideResourceKey 
                || value is string
                || ((valType = value.GetType()).IsValueType && valType.Assembly == typeof(string).Assembly)
                || valType.IsEnum)
                return ResourceSelectorBehavior.ValueAsKey;

            return ResourceSelectorBehavior.ValueTypeAsKey;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion


        
    }
}
