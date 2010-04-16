namespace SLExtensions.Sharepoint
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    public class ColumnBinder : NotifyingObject, IDisposable
    {
        #region Fields

        // Using a DependencyProperty as the backing store for ColumnBinder.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnBinderProperty = 
            DependencyProperty.RegisterAttached("ColumnBinder", typeof(ColumnBinder), typeof(ColumnBinder), new PropertyMetadata(ColumnBinderChangedCallback));

        // Using a DependencyProperty as the backing store for ColumnName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnNameProperty = 
            DependencyProperty.RegisterAttached("ColumnName", typeof(string), typeof(ColumnBinder), new PropertyMetadata(ColumnNameChangedCallback));

        // Using a DependencyProperty as the backing store for Item.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemProperty = 
            DependencyProperty.RegisterAttached("Item", typeof(TemplateDataBase), typeof(ColumnBinder), new PropertyMetadata(ItemChangedCallback));

        sharepointEventListener evtListener;
        private TemplateDataBase item;
        int skipValidationCounter;
        private object value;

        #endregion Fields

        #region Constructors

        static ColumnBinder()
        {
            TypeResolver = new Dictionary<Type, RetreiveColumnBinderDelegate>();

            TypeResolver.Add(typeof(TextBox), (obj) => new ColumnBinderTextBox((TextBox)obj));
            TypeResolver.Add(typeof(TextBlock), (obj) => new ColumnBinderTextBlock((TextBlock)obj));
            TypeResolver.Add(typeof(ToggleButton), (obj) => new ColumnBinderToggleButton((ToggleButton)obj));
            TypeResolver.Add(typeof(Selector), (obj) => new ColumnBinderSelector((Selector)obj));
        }

        public ColumnBinder(DependencyObject associatedObject)
        {
            this.AssociatedObject = associatedObject;
            evtListener = new sharepointEventListener(this);
        }

        #endregion Constructors

        #region Delegates

        public delegate ColumnBinder RetreiveColumnBinderDelegate(DependencyObject associatedObject);

        #endregion Delegates

        #region Properties

        public static Dictionary<Type, RetreiveColumnBinderDelegate> TypeResolver
        {
            get; set;
        }

        public DependencyObject AssociatedObject
        {
            get; set;
        }

        public TemplateDataBase Item
        {
            get { return this.item; }
            set
            {
                if (this.item != value)
                {
                    evtListener.Detach();

                    this.item = value;

                    if (this.Item != null)
                    {
                        evtListener.Attach();
                    }

                    this.RaisePropertyChanged(n => n.Item);
                }
            }
        }

        public virtual object Value
        {
            get { return this.value; }
            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    this.RaisePropertyChanged(n => n.Value);
                }

                if (Item != null && Item.Data != null)
                {
                    var colName = GetColumnName(AssociatedObject);
                    var originalValue = Item.Data.TryGetValue(colName);
                    if (!object.Equals(originalValue, this.Value))
                    {
                        Item.Data[colName] = this.value;
                        SharepointEvents.RaiseItemValueUpdated(Item, colName);
                    }

                }
                Validate();
            }
        }

        protected BindingExpression Binding
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public static ColumnBinder GetColumnBinder(DependencyObject obj)
        {
            return (ColumnBinder)obj.GetValue(ColumnBinderProperty);
        }

        public static string GetColumnName(DependencyObject obj)
        {
            return (string)obj.GetValue(ColumnNameProperty);
        }

        public static TemplateDataBase GetItem(DependencyObject obj)
        {
            return (TemplateDataBase)obj.GetValue(ItemProperty);
        }

        public static void SetColumnBinder(DependencyObject obj, ColumnBinder value)
        {
            obj.SetValue(ColumnBinderProperty, value);
        }

        public static void SetColumnName(DependencyObject obj, string value)
        {
            obj.SetValue(ColumnNameProperty, value);
        }

        public static void SetItem(DependencyObject obj, TemplateDataBase value)
        {
            obj.SetValue(ItemProperty, value);
        }

        public virtual void Dispose()
        {
        }

        internal virtual bool HasFocus()
        {
            for (DependencyObject obj2 = FocusManager.GetFocusedElement() as DependencyObject; obj2 != null; obj2 = VisualTreeHelper.GetParent(obj2))
            {
                if (object.ReferenceEquals(obj2, AssociatedObject))
                {
                    return true;
                }
            }
            return false;
        }

        protected virtual void ItemValueUpdated(object value)
        {
            RefreshData();
            if (Binding != null)
            {
                Binding.UpdateSource();
            }
        }

        protected virtual void RefreshData()
        {
            SetSkipValidation(true);
            try
            {
                if (Item != null && Item.Data != null)
                    Value = Item.Data.TryGetValue(GetColumnName(AssociatedObject));
            }
            finally
            {
                SetSkipValidation(false);
            }
        }

        protected void SetSkipValidation(bool skip)
        {
            if (skip)
                skipValidationCounter++;
            else
                skipValidationCounter--;
        }

        protected void Validate()
        {
            var colName = GetColumnName(AssociatedObject);
            if (skipValidationCounter == 0 && Item != null)
            {

                SharepointList list = Store.ReadListMetadata<SharepointList>(Item.ListName);
                var c = (from col in list.Columns
                         where colName == col.Name
                         select col).FirstOrDefault();
                if (c != null && c.IsRequired
                    && (Value == null
                    || (Value is string && string.Empty.Equals(Value))))
                {
                    throw new RequiredFieldException("Obligatoire");
                }
            }
        }

        private static void ColumnBinderChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColumnBinder colBinder = e.OldValue as ColumnBinder;
            if (colBinder != null)
            {
                colBinder.Dispose();
            }

            colBinder = e.NewValue as ColumnBinder;

            if (colBinder != null)
            {
                colBinder.Item = GetItem(d);
                colBinder.RefreshData();
            }
        }

        private static void ColumnNameChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColumnBinder binder = null;
            Type associatedObjectType = d.GetType();
            var dlg = (from i in TypeResolver
                       where i.Key.IsAssignableFrom(associatedObjectType)
                       select i).OrderBy(a => a.Key == associatedObjectType ? 0 : 1).Select(a => a.Value).FirstOrDefault();

            if (dlg != null)
            {
                binder = dlg(d);
            }

            if (binder != null)
                SetColumnBinder(d, binder);
        }

        private static void ItemChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColumnBinder binder = GetColumnBinder(d);
            if (binder != null)
            {
                binder.Item = e.NewValue as TemplateDataBase;
                binder.RefreshData();
            }
        }

        void SharepointEvents_IsValid(object sender, CancelSharepointItemEventArgs e)
        {
            if (e.Item.ListName == this.Item.ListName
                && (e.Item.Id != null && (this.Item.Id == e.Item.Id)
                || e.Item.Data == this.Item.Data))
            {
                if (Binding != null)
                {
                    try
                    {
                        Binding.UpdateSource();
                    }
                    catch
                    {

                    }
                    e.Cancel = e.Cancel || Validation.GetHasError(AssociatedObject);
                }
            }
        }

        private void SharepointEvents_ItemValueUpdated(object sender, SharepointItemValueEventArgs e)
        {
            var colName = GetColumnName(AssociatedObject);
            if (!string.IsNullOrEmpty(colName) && item.ListName == e.Item.ListName
                && e.Keys.Contains(colName)
                && (e.Item.Id != null && (this.Item.Id == e.Item.Id)
                || e.Item.Data == this.Item.Data))
            {
                ItemValueUpdated(e.Item.Data.TryGetValue(colName));
            }
        }

        #endregion Methods

        #region Nested Types

        private class sharepointEventListener
        {
            #region Fields

            private WeakReference weakReference;

            #endregion Fields

            #region Constructors

            public sharepointEventListener(ColumnBinder binder)
            {
                weakReference = new WeakReference(binder);
            }

            #endregion Constructors

            #region Methods

            public void Attach()
            {
                SharepointEvents.ItemValueUpdated += new EventHandler<SharepointItemValueEventArgs>(SharepointEvents_ItemValueUpdated);
                SharepointEvents.IsValid += new EventHandler<CancelSharepointItemEventArgs>(SharepointEvents_IsValid);
            }

            public void Detach()
            {
                SharepointEvents.ItemValueUpdated -= new EventHandler<SharepointItemValueEventArgs>(SharepointEvents_ItemValueUpdated);
                SharepointEvents.IsValid -= new EventHandler<CancelSharepointItemEventArgs>(SharepointEvents_IsValid);
            }

            void SharepointEvents_IsValid(object sender, CancelSharepointItemEventArgs e)
            {
                ColumnBinder binder = weakReference.Target as ColumnBinder;
                if (binder == null)
                {
                    SharepointEvents.IsValid -= new EventHandler<CancelSharepointItemEventArgs>(SharepointEvents_IsValid);
                }
                else
                {
                    binder.SharepointEvents_IsValid(sender, e);
                }
            }

            void SharepointEvents_ItemValueUpdated(object sender, SharepointItemValueEventArgs e)
            {
                ColumnBinder binder = weakReference.Target as ColumnBinder;
                if (binder == null)
                {
                    SharepointEvents.ItemValueUpdated -= new EventHandler<SharepointItemValueEventArgs>(SharepointEvents_ItemValueUpdated);
                }
                else
                {
                    binder.SharepointEvents_ItemValueUpdated(sender, e);
                }
            }

            #endregion Methods
        }

        #endregion Nested Types
    }
}