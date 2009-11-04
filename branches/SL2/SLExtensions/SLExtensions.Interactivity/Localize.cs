namespace SLExtensions.Interactivity
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Resources;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    using Microsoft.Expression.Interactivity;

    using SLExtensions.Globalization;

    public class Localize : Behavior<FrameworkElement>
    {
        #region Fields

        private bool convertXaml;
        private DependencyProperty dependencyProperty;
        private string key;
        private string propertyName;
        private string resourceManagerKey;

        #endregion Fields

        #region Properties

        public bool ConvertXaml
        {
            get
            {
                return this.convertXaml;
            }

            set
            {
                if (this.convertXaml != value)
                {
                    this.convertXaml = value;
                    Refresh();
                }
            }
        }

        public string Key
        {
            get
            {
                return this.key;
            }

            set
            {
                if (this.key != value)
                {
                    this.key = value;
                    Refresh();
                }
            }
        }

        public string PropertyName
        {
            get
            {
                return this.propertyName;
            }

            set
            {
                if (this.propertyName != value)
                {
                    this.propertyName = value;
                    this.dependencyProperty = null;
                    Refresh();
                }
            }
        }

        public string ResourceManagerKey
        {
            get
            {
                return this.resourceManagerKey;
            }

            set
            {
                if (this.resourceManagerKey != value)
                {
                    this.resourceManagerKey = value;
                }
            }
        }

        #endregion Properties

        #region Methods

        protected override void OnAttached()
        {
            Refresh();
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
        }

        private void Refresh()
        {
            if (AssociatedObject == null
                || string.IsNullOrEmpty(propertyName)
                || string.IsNullOrEmpty(Key)
                || string.IsNullOrEmpty(ResourceManagerKey))
                return;

            if (dependencyProperty == null)
            {
                Type t = AssociatedObject.GetType();
                var field = t.GetField(propertyName + "Property", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                if (field != null)
                {
                    dependencyProperty = field.GetValue(null) as DependencyProperty;
                }
            }

            if (dependencyProperty != null
                && Application.Current != null)
            {
                ResourceManager resourceManager = null;
                var obj = Application.Current.Resources[ResourceManagerKey];
                if (obj is ResourceLoader)
                {
                    resourceManager = ((ResourceLoader)obj).GetResourceManager();
                }
                else
                {
                    resourceManager = obj as ResourceManager;
                }

                var resourceValue = resourceManager.GetObject(Key, CultureInfo.CurrentCulture);
                if (convertXaml && resourceValue is string)
                {
                    resourceValue = XamlReader.Load((string)resourceValue);
                }
                AssociatedObject.SetValue(dependencyProperty, resourceValue);
            }
        }

        #endregion Methods
    }
}