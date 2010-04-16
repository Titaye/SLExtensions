// <copyright file="SwappableContentControl.cs" company="Ucaya">
// Distributed under Microsoft Public License (Ms-PL)
// </copyright>
// <author>Thierry Bouquain</author>
namespace SLExtensions.Controls
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    [ContentProperty("Content")]
    [TemplatePart(Name = RootElementName, Type = typeof(Panel))]
    [TemplateVisualState(Name = "ShowPresenter1", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "ShowPresenter2", GroupName = "CommonStates")]
    public class SwappableContentControl : Control
    {
        #region Fields

        /// <summary>
        /// Content depedency property.
        /// </summary>
        public static readonly DependencyProperty ContentProperty = 
            DependencyProperty.Register(
                "Content",
                typeof(object),
                typeof(SwappableContentControl),
                new PropertyMetadata((d, e) => ((SwappableContentControl)d).OnContentChanged((object)e.OldValue, (object)e.NewValue)));

        /// <summary>
        /// ContentTemplate depedency property.
        /// </summary>
        public static readonly DependencyProperty ContentTemplateProperty = 
            DependencyProperty.Register(
                "ContentTemplate",
                typeof(DataTemplate),
                typeof(SwappableContentControl),
                new PropertyMetadata((d, e) => ((SwappableContentControl)d).OnContentTemplateChanged((DataTemplate)e.OldValue, (DataTemplate)e.NewValue)));

        /// <summary>
        /// Presenter1Content depedency property.
        /// </summary>
        public static readonly DependencyProperty Presenter1ContentProperty = 
            DependencyProperty.Register(
                "Presenter1Content",
                typeof(object),
                typeof(SwappableContentControl),
                new PropertyMetadata((d, e) => ((SwappableContentControl)d).OnPresenter1ContentChanged((object)e.OldValue, (object)e.NewValue)));

        /// <summary>
        /// Presenter2Content depedency property.
        /// </summary>
        public static readonly DependencyProperty Presenter2ContentProperty = 
            DependencyProperty.Register(
                "Presenter2Content",
                typeof(object),
                typeof(SwappableContentControl),
                new PropertyMetadata((d, e) => ((SwappableContentControl)d).OnPresenter2ContentChanged((object)e.OldValue, (object)e.NewValue)));

        private const string RootElementName = "LayoutRoot";

        private bool isLoaded = false;
        private bool isPresenter1Turn;
        private Panel rootElement;

        #endregion Fields

        #region Constructors

        public SwappableContentControl()
        {
            DefaultStyleKey = typeof(SwappableContentControl);
            isPresenter1Turn = true;
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

        public object Presenter1Content
        {
            get
            {
                return (object)GetValue(Presenter1ContentProperty);
            }

            set
            {
                SetValue(Presenter1ContentProperty, value);
            }
        }

        public object Presenter2Content
        {
            get
            {
                return (object)GetValue(Presenter2ContentProperty);
            }

            set
            {
                SetValue(Presenter2ContentProperty, value);
            }
        }

        #endregion Properties

        #region Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            rootElement = GetTemplateChild(RootElementName) as Panel;

            var sbShowPrensenter1 = (from g in VisualStateManager.GetVisualStateGroups(rootElement).OfType<VisualStateGroup>()
                    where g.Name == "CommonStates"
                    from state in g.States.OfType<VisualState>()
                    where state.Name == "ShowPresenter1"
                    select state.Storyboard).FirstOrDefault();

            if (sbShowPrensenter1 != null)
            {
                sbShowPrensenter1.Completed += new EventHandler(sbShowPrensenter1_Completed);
            }

            var sbShowPrensenter2 = (from g in VisualStateManager.GetVisualStateGroups(rootElement).OfType<VisualStateGroup>()
                                     where g.Name == "CommonStates"
                                     from state in g.States.OfType<VisualState>()
                                     where state.Name == "ShowPresenter2"
                                     select state.Storyboard).FirstOrDefault();

            if (sbShowPrensenter2 != null)
            {
                sbShowPrensenter2.Completed += new EventHandler(sbShowPrensenter2_Completed);
            }

            isLoaded = true;

            if(Content != null)
                OnContentChanged(null, Content);
        }

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
        /// handles the ContentProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnContentChanged(object oldValue, object newValue)
        {
            if (!isLoaded)
                return;

            if (Presenter2Content == newValue)
                Presenter2Content = null;

            if (Presenter1Content == newValue)
                Presenter1Content = null;

            if (isPresenter1Turn)
            {
                Presenter1Content = newValue;
                this.GoToState(true, "ShowPresenter1");
            }
            else
            {
                Presenter2Content = newValue;
                this.GoToState(true, "ShowPresenter2");
            }

            isPresenter1Turn = !isPresenter1Turn;
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
        /// handles the Presenter1ContentProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnPresenter1ContentChanged(object oldValue, object newValue)
        {
        }

        /// <summary>
        /// handles the Presenter2ContentProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnPresenter2ContentChanged(object oldValue, object newValue)
        {
        }

        void sbShowPrensenter1_Completed(object sender, EventArgs e)
        {
            if (Presenter2Content != null)
            {
                Presenter2Content = null;
            }
        }

        void sbShowPrensenter2_Completed(object sender, EventArgs e)
        {
            if (Presenter1Content != null)
            {
                Presenter1Content = null;
            }
        }

        #endregion Methods
    }
}