// <copyright file="VirtualizedListBoxItem.cs" company="Ucaya">
// Distributed under Microsoft Public License (Ms-PL)
// </copyright>
// <author>Thierry Bouquain</author>
namespace SLExtensions.Controls
{
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

    [TemplateVisualState(Name = "Normal", GroupName = "CommonStates"),
    TemplateVisualState(Name = "Unselected", GroupName = "SelectionStates"),
    TemplateVisualState(Name = "Unfocused", GroupName = "FocusStates"),
    TemplateVisualState(Name = "Focused", GroupName = "FocusStates"),
    TemplateVisualState(Name = "Selected", GroupName = "SelectionStates"),
    TemplateVisualState(Name = "SelectedUnfocused", GroupName = "SelectionStates"),
    TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates")]
    public class VirtualizedListBoxItem : Control
    {
        #region Fields

        /// <summary>
        /// Content depedency property.
        /// </summary>
        public static readonly DependencyProperty ContentProperty = 
            DependencyProperty.Register(
                "Content",
                typeof(object),
                typeof(VirtualizedListBoxItem),
                new PropertyMetadata((d, e) => ((VirtualizedListBoxItem)d).OnContentChanged((object)e.OldValue, (object)e.NewValue)));

        /// <summary>
        /// ContentTemplate depedency property.
        /// </summary>
        public static readonly DependencyProperty ContentTemplateProperty = 
            DependencyProperty.Register(
                "ContentTemplate",
                typeof(DataTemplate),
                typeof(VirtualizedListBoxItem),
                new PropertyMetadata((d, e) => ((VirtualizedListBoxItem)d).OnContentTemplateChanged((DataTemplate)e.OldValue, (DataTemplate)e.NewValue)));

        /// <summary>
        /// IsFocused depedency property.
        /// </summary>
        public static readonly DependencyProperty IsFocusedProperty = 
            DependencyProperty.Register(
                "IsFocused",
                typeof(bool),
                typeof(VirtualizedListBoxItem),
                new PropertyMetadata((d, e) => ((VirtualizedListBoxItem)d).OnIsFocusedChanged((bool)e.OldValue, (bool)e.NewValue)));

        /// <summary>
        /// IsMouseOver depedency property.
        /// </summary>
        public static readonly DependencyProperty IsMouseOverProperty = 
            DependencyProperty.Register(
                "IsMouseOver",
                typeof(bool),
                typeof(VirtualizedListBoxItem),
                new PropertyMetadata((d, e) => ((VirtualizedListBoxItem)d).OnIsMouseOverChanged((bool)e.OldValue, (bool)e.NewValue)));

        /// <summary>
        /// IsSelected depedency property.
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty = 
            DependencyProperty.Register(
                "IsSelected",
                typeof(bool),
                typeof(VirtualizedListBoxItem),
                new PropertyMetadata((d, e) => ((VirtualizedListBoxItem)d).OnIsSelectedChanged((bool)e.OldValue, (bool)e.NewValue)));

        #endregion Fields

        #region Constructors

        public VirtualizedListBoxItem()
        {
            DefaultStyleKey = typeof(VirtualizedListBoxItem);
            this.MouseEnter += delegate { IsMouseOver = true; };
            this.MouseLeave += delegate { IsMouseOver = false; };
        }

        #endregion Constructors

        #region Properties

        public object Content
        {
            get
            {
                return (object)GetValue(ContentProperty);
            }

            set
            {
                SetValue(ContentProperty, value);
            }
        }

        public DataTemplate ContentTemplate
        {
            get
            {
                return (DataTemplate)GetValue(ContentTemplateProperty);
            }

            set
            {
                SetValue(ContentTemplateProperty, value);
            }
        }

        public bool IsFocused
        {
            get
            {
                return (bool)GetValue(IsFocusedProperty);
            }

            set
            {
                SetValue(IsFocusedProperty, value);
            }
        }

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

        public bool IsSelected
        {
            get
            {
                return (bool)GetValue(IsSelectedProperty);
            }

            set
            {
                SetValue(IsSelectedProperty, value);
            }
        }

        #endregion Properties

        #region Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.ChangeVisualState(false);
        }

        internal void ChangeVisualState(bool useTransitions)
        {
            if (this.IsMouseOver)
            {
                GoToState(useTransitions, new string[] { "MouseOver", "Normal" });
            }
            else
            {
                GoToState(useTransitions, new string[] { "Normal" });
            }
            if (this.IsSelected)
            {
                //if (ListBox.GetIsSelectionActive(this.ParentListBox))
                //{
                    GoToState(useTransitions, new string[] { "Selected", "Unselected" });
                //}
                //else
                //{
                    GoToState(useTransitions, new string[] { "SelectedUnfocused", "Selected", "Unselected" });
                //}
            }
            else
            {
                GoToState(useTransitions, new string[] { "Unselected" });
            }
            if (this.IsFocused)
            {
                GoToState(useTransitions, new string[] { "Focused", "Unfocused" });
            }
            else
            {
                GoToState(useTransitions, new string[] { "Unfocused" });
            }
        }

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
        /// handles the ContentProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnContentChanged(object oldValue, object newValue)
        {
        }

        /// <summary>
        /// handles the ContentTemplateProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnContentTemplateChanged(DataTemplate oldValue, DataTemplate newValue)
        {
        }

        /// <summary>
        /// handles the IsFocusedProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnIsFocusedChanged(bool oldValue, bool newValue)
        {
            ChangeVisualState(true);
        }

        /// <summary>
        /// handles the IsMouseOverProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnIsMouseOverChanged(bool oldValue, bool newValue)
        {
            ChangeVisualState(true);
        }

        /// <summary>
        /// handles the IsSelectedProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnIsSelectedChanged(bool oldValue, bool newValue)
        {
            ChangeVisualState(true);
        }

        #endregion Methods
    }
}