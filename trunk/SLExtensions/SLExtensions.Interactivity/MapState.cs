namespace SLExtensions.Interactivity
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Browser;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Interactivity;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    [ContentProperty("Mappings")]
    public class MapState : Behavior<FrameworkElement>
    {
        #region Fields

        // Using a DependencyProperty as the backing store for AttachedValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AttachedValueProperty = 
            DependencyProperty.RegisterAttached("AttachedValue", typeof(object), typeof(MapState), new PropertyMetadata(AttachedValueChangedCallback));

        /// <summary>
        /// Property depedency property.
        /// </summary>
        public static readonly DependencyProperty PropertyProperty = 
            DependencyProperty.Register(
                "Property",
                typeof(string),
                typeof(MapState),
                new PropertyMetadata((d, e) => ((MapState)d).OnPropertyChanged((string)e.OldValue, (string)e.NewValue)));

        /// <summary>
        /// Source depedency property.
        /// </summary>
        public static readonly DependencyProperty SourceProperty = 
            DependencyProperty.Register(
                "Source",
                typeof(object),
                typeof(MapState),
                new PropertyMetadata((d, e) => ((MapState)d).OnSourceChanged((object)e.OldValue, (object)e.NewValue)));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty UseTransitionsProperty = DependencyProperty.Register("UseTransitions", typeof(bool), typeof(MapState), new PropertyMetadata(true));

        /// <summary>
        /// Value depedency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty = 
            DependencyProperty.Register(
                "Value",
                typeof(object),
                typeof(MapState),
                new PropertyMetadata((d, e) => ((MapState)d).OnValueChanged((object)e.OldValue, (object)e.NewValue)));

        private BindingListener bindingListener;
        private Binding boundValue;
        private bool isAttached = false;
        private PropertyInfo propertyInfo = null;

        #endregion Fields

        #region Constructors

        public MapState()
        {
            Mappings = new List<MapStateMapping>();
            bindingListener = new BindingListener();
            bindingListener.ValueChanged += delegate { this.Value = bindingListener.Value; };
        }

        #endregion Constructors

        #region Properties

        public Binding BoundValue
        {
            get { return this.boundValue; }
            set
            {
                if (this.boundValue != value)
                {
                    this.boundValue = value;
                    if (value == null)
                        bindingListener.ClearValue(BindingListener.ValueProperty);
                    else
                    {
                        bindingListener.EnsureBindingSource(AssociatedObject);
                        bindingListener.SetBinding(BindingListener.ValueProperty, value);
                    }
                }
            }
        }

        public List<MapStateMapping> Mappings
        {
            get;
            private set;
        }

        public string Property
        {
            get
            {
                return (string)GetValue(PropertyProperty);
            }

            set
            {
                SetValue(PropertyProperty, value);
            }
        }

        public object Source
        {
            get
            {
                return (object)GetValue(SourceProperty);
            }

            set
            {
                SetValue(SourceProperty, value);
            }
        }

        /// <summary>
        /// True if transitions should be used for the state change.
        /// </summary>
        public bool UseTransitions
        {
            get { return (bool)this.GetValue(MapState.UseTransitionsProperty); }
            set { this.SetValue(MapState.UseTransitionsProperty, value); }
        }

        public object Value
        {
            get
            {
                return (object)GetValue(ValueProperty);
            }

            set
            {
                SetValue(ValueProperty, value);
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Hooks up necessary handlers for the state changes.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            bindingListener.EnsureBindingSource(AssociatedObject);

            isAttached = true;

            // Launch control visual state refresh. If the associated control is not loaded, some exceptions can be thrown by the VisualStateManager
            RefreshState();
            FrameworkElement element = this.AssociatedObject;
            if (element != null)
            {
                // Catch the loaded event to refresh the state whe the control is loaded
                element.Loaded += new RoutedEventHandler(Element_Loaded);
            }
            else
            {
                Dispatcher.BeginInvoke(delegate
                {
                    this.OnAttached();
                });
            }
        }

        private static void AttachedValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MapState ms = d as MapState;
            if (ms == null)
            {
                BehaviorCollection col = System.Windows.Interactivity.Interaction.GetBehaviors(d);
                if (col != null)
                {
                    ms = col.OfType<MapState>().FirstOrDefault();
                }
            }

            if (ms != null)
            {
                ms.Value = e.NewValue;
            }
        }

        private void Element_Loaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            element.Loaded -= new RoutedEventHandler(Element_Loaded);
            RefreshState();
        }

        /// <summary>
        /// handles the PropertyProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnPropertyChanged(string oldValue, string newValue)
        {
            if (Source == null)
                propertyInfo = null;
            else
                propertyInfo = Source.GetType().GetProperty(newValue);

            RefreshState();
        }

        /// <summary>
        /// handles the SourceProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnSourceChanged(object oldValue, object newValue)
        {
            propertyInfo = null;
            INotifyPropertyChanged notifyingObject = oldValue as INotifyPropertyChanged;
            if (notifyingObject != null)
            {
                notifyingObject.PropertyChanged -= new PropertyChangedEventHandler(notifyingObject_PropertyChanged);
            }
            notifyingObject = newValue as INotifyPropertyChanged;
            if (notifyingObject != null)
            {
                notifyingObject.PropertyChanged += new PropertyChangedEventHandler(notifyingObject_PropertyChanged);
            }
            if (newValue != null && !string.IsNullOrEmpty(Property))
            {
                propertyInfo = newValue.GetType().GetProperty(Property);
            }
            RefreshState();
        }

        /// <summary>
        /// handles the ValueProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnValueChanged(object oldValue, object newValue)
        {
            RefreshState();
        }

        private void RefreshState()
        {
            // Do not refresh at desing time
            if (System.ComponentModel.DesignerProperties.IsInDesignTool
                || !isAttached)
            {
                return;
            }

            object propValue;
            if (Source != null && propertyInfo != null)
            {
                propValue = propertyInfo.GetValue(Source, null);
            }
            else
            {
                propValue = Value;
            }

            if (propValue == null)
            {
                var nullMappings = from m in Mappings
                                   where m.Value == null
                                   || ((m.Value is string) && string.Empty.Equals(m.Value))
                                   select m.StateName;

                try
                {
                    var ctrl = AssociatedObject as Control;
                    if (ctrl != null)
                    {
                        foreach (var state in nullMappings)
                        {
                            if (VisualStateManager.GoToState(ctrl, state, true))
                                break;
                        }
                    }
                    else
                    {
                        SLExtensions.Controls.Animation.VisualState.GoToState(AssociatedObject, true, nullMappings.ToArray());
                    }
                    //foreach (var state in nullMappings)
                    //{
                    //    if (VisualStateManager.GoToState(AssociatedObject.FirstVisualAncestorOfType<Control>(), state, UseTransitions))
                    //        break;
                    //}
                }
                catch
                {
                }
            }
            else
            {
                MapStateMapping mapping = null;
                foreach (var item in Mappings)
                {
                    var value = item.Value;
                    if (value != null)
                    {
                        if (object.Equals(value, propValue))
                        {
                            mapping = item;
                            break;
                        }

                        if (propValue is Enum)
                        {
                            try
                            {
                                var val2 = Enum.Parse(propValue.GetType(), value, true);
                                if (object.Equals(val2, propValue))
                                {
                                    mapping = item;
                                    break;
                                }
                            }
                            catch
                            {
                            }
                        }
                        else if (propValue is IConvertible)
                        {
                            try
                            {
                                var val2 = Convert.ChangeType(propValue, value.GetType(), CultureInfo.InvariantCulture);
                                if (object.Equals(val2, value))
                                {
                                    mapping = item;
                                    break;
                                }
                            }
                            catch
                            {
                            }

                            try
                            {
                                var val2 = Convert.ChangeType(value, propValue.GetType(), CultureInfo.InvariantCulture);
                                if (object.Equals(val2, propValue))
                                {
                                    mapping = item;
                                    break;
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
                }

                if (mapping == null)
                {
                    mapping = Mappings.FirstOrDefault(m => m.IsNotNull == true);
                }

                if (mapping == null)
                {
                    mapping = Mappings.Where(m => m.Else).FirstOrDefault();
                }

                if (mapping != null)
                {
                    try
                    {
                        var ctrl = AssociatedObject as Control;
                        if (ctrl != null)
                        {
                            VisualStateManager.GoToState(ctrl, mapping.StateName, UseTransitions);
                        }
                        else
                            SLExtensions.Controls.Animation.VisualState.GoToState(AssociatedObject, UseTransitions, mapping.StateName);
                        //VisualStateManager.GoToState(AssociatedObject.FirstVisualAncestorOfType<Control>(), mapping.StateName, UseTransitions);
                        Debug.WriteLine("MapState : (" + propValue + ") -> " + mapping.StateName);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("MapState : (" + propValue + ") -> " + mapping.StateName + " failed " + ex);
                    }
                }
            }
        }

        void notifyingObject_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Property)
                RefreshState();
        }

        #endregion Methods
    }
}