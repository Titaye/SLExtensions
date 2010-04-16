namespace SLExtensions.Controls
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    [TemplateVisualState(GroupName = ModificationStatesGroupName, Name = ModifiedStateName)]
    [TemplateVisualState(GroupName = ModificationStatesGroupName, Name = UnchangedStateName)]
    [TemplatePart(Name = CommitButtonName, Type = typeof(Button))]
    [TemplatePart(Name = ReverButtonName, Type = typeof(Button))]
    public class ChangeTracker : Control
    {
        #region Fields

        /// <summary>
        /// ChangeTracker depedency property.
        /// </summary>
        private static readonly DependencyProperty ChangeTrackerProperty = 
            DependencyProperty.Register(
                "ChangeTracker",
                typeof(object),
                typeof(ChangeTracker),
                new PropertyMetadata((d, e) => ((ChangeTracker)d).OnChangeTrackerChanged((object)e.OldValue, (object)e.NewValue)));

        /// <summary>
        /// IsModified depedency property.
        /// </summary>
        private static readonly DependencyProperty IsModifiedProperty = 
            DependencyProperty.Register(
                "IsModified",
                typeof(bool),
                typeof(ChangeTracker),
                new PropertyMetadata((d, e) => ((ChangeTracker)d).OnIsModifiedChanged((bool)e.OldValue, (bool)e.NewValue)));

        private const string CommitButtonName = "CommitButton";
        private const string ModificationStatesGroupName = "ModificationStates";
        private const string ModifiedStateName = "Modified";
        private const string ReverButtonName = "RevertButton";
        private const string UnchangedStateName = "Unchanged";

        private Button commitButton;
        private bool lockChanges = false;
        private Dictionary<string, object> originalProperties = new Dictionary<string, object>();
        private Dictionary<string, object> properties = new Dictionary<string, object>();
        private Button revertButton;

        #endregion Fields

        #region Constructors

        public ChangeTracker()
        {
            DefaultStyleKey = typeof(ChangeTracker);

            SetBinding(ChangeTrackerProperty, new System.Windows.Data.Binding());
            this.Loaded += new RoutedEventHandler(ChangeTracker_Loaded);
        }

        #endregion Constructors

        #region Events

        public event EventHandler ChangesCommitted;

        public event EventHandler<CancelEventArgs> ChangesCommitting;

        public event EventHandler ChangesReverted;

        public event EventHandler DataContextChanged;

        public event EventHandler ModificationOccured;

        #endregion Events

        #region Properties

        public bool IsModified
        {
            get
            {
                return (bool)GetValue(IsModifiedProperty);
            }

            private set
            {
                SetValue(IsModifiedProperty, value);
            }
        }

        #endregion Properties

        #region Methods

        public void Commit()
        {
            if (DataContext == null)
                return;

            if (!OnChangesCommitting())
                return;

            foreach (var item in properties)
            {
                originalProperties[item.Key] = item.Value;
            }
            IsModified = false;
            OnChangesCommitted();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            commitButton = GetTemplateChild(CommitButtonName) as Button;
            revertButton = GetTemplateChild(ReverButtonName) as Button;

            if(commitButton != null)
                commitButton.Click += new RoutedEventHandler(commitButton_Click);

            if(revertButton != null)
                revertButton.Click += new RoutedEventHandler(revertButton_Click);
        }

        public void Revert()
        {
            if (DataContext == null)
                return;

            lockChanges = true;
            try
            {
                Type t = DataContext.GetType();

                foreach (var item in properties)
                {
                    t.GetProperty(item.Key).SetValue(DataContext, originalProperties[item.Key], null);
                }
                properties.Clear();
            }
            finally
            {
                lockChanges = false;
            }
            IsModified = false;
            OnChangesReverted();
        }

        protected virtual void OnChangesCommitted()
        {
            if (ChangesCommitted != null)
            {
                ChangesCommitted(this, EventArgs.Empty);
            }
        }

        protected virtual bool OnChangesCommitting()
        {
            CancelEventArgs e = new CancelEventArgs();
            if (ChangesCommitting != null)
            {
                ChangesCommitting(this, e);
            }
            return !e.Cancel;
        }

        protected virtual void OnChangesReverted()
        {
            if (ChangesReverted != null)
            {
                ChangesReverted(this, EventArgs.Empty);
            }
        }

        protected virtual void OnDataContextChanged()
        {
            if (DataContextChanged != null)
            {
                DataContextChanged(this, EventArgs.Empty);
            }
            ClearPropertyValues();
        }

        protected virtual void OnModificationOccured()
        {
            if (ModificationOccured != null)
            {
                ModificationOccured(this, EventArgs.Empty);
            }
        }

        void ChangeTracker_Loaded(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, UnchangedStateName, false);
        }

        private void ClearPropertyValues()
        {
            properties = new Dictionary<string, object>();
            originalProperties = new Dictionary<string, object>();
            if (DataContext != null)
            {
                Type t = DataContext.GetType();
                foreach (var prop in t.GetProperties())
                {
                    originalProperties[prop.Name] = prop.GetValue(DataContext, null);
                }
            }
            IsModified = false;
        }

        /// <summary>
        /// handles the ChangeTrackerProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnChangeTrackerChanged(object oldValue, object newValue)
        {
            OnDataContextChanged();
            INotifyPropertyChanged notify = newValue as INotifyPropertyChanged;
            if (notify != null)
                notify.PropertyChanged += new PropertyChangedEventHandler(notify_PropertyChanged);

            notify = oldValue as INotifyPropertyChanged;
            if (notify != null)
                notify.PropertyChanged -= new PropertyChangedEventHandler(notify_PropertyChanged);

            ClearPropertyValues();
        }

        /// <summary>
        /// handles the IsModifiedProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnIsModifiedChanged(bool oldValue, bool newValue)
        {
            if (newValue)
                VisualStateManager.GoToState(this, ModifiedStateName, true);
            else
                VisualStateManager.GoToState(this, UnchangedStateName, true);
        }

        void commitButton_Click(object sender, RoutedEventArgs e)
        {
            Commit();
        }

        void notify_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (lockChanges)
                return;

            properties[e.PropertyName] = DataContext.GetType().GetProperty(e.PropertyName).GetValue(DataContext, null);
            IsModified = true;
            OnModificationOccured();
        }

        void revertButton_Click(object sender, RoutedEventArgs e)
        {
            Revert();
        }

        #endregion Methods
    }
}