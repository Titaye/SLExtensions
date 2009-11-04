namespace SLExtensions.Showcase.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Browser;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    using SLExtensions.Stats;

    public class ShowcaseController : INotifyPropertyChanged
    {
        #region Fields

        private List<string> contributors;
        private object currentPage;
        private List<ShowcaseItem> items;
        private List<string> lastAdditions;
        private ShowcaseItem lastPage;
        private List<ShowcaseLink> links;

        #endregion Fields

        #region Constructors

        public ShowcaseController()
        {
            Commands.Navigate.Executed += new EventHandler<SLExtensions.Input.ExecutedEventArgs>(Navigate_Executed);
            // added for BootStrapping hosted version
            if (Application.Current is App)
                ((App)Application.Current).Navigate += new EventHandler<SLExtensions.Input.StateEventArgs>(ShowcaseController_Navigate);
            items = new List<ShowcaseItem>();
            LastAdditions = new List<string>();
            Contributors = new List<string>();
            Links = new List<ShowcaseLink>();
        }

        #endregion Constructors

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Properties

        public List<string> Contributors
        {
            get { return contributors; }
            set
            {
                if (contributors != value)
                {
                    contributors = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.Contributors));
                }
            }
        }

        public object CurrentPage
        {
            get { return currentPage; }
            set
            {
                if (currentPage != value)
                {
                    IDisposable disposable = currentPage as IDisposable;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }

                    currentPage = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.CurrentPage));
                }
            }
        }

        public List<ShowcaseItem> Items
        {
            get { return items; }
            set
            {
                if (items != value)
                {
                    items = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.Items));
                }
            }
        }

        public List<string> LastAdditions
        {
            get { return lastAdditions; }
            set
            {
                if (lastAdditions != value)
                {
                    lastAdditions = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.LastAdditions));
                }
            }
        }

        public List<ShowcaseLink> Links
        {
            get { return links; }
            set
            {
                if (links != value)
                {
                    links = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.Links));
                }
            }
        }

        #endregion Properties

        #region Methods

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        void Navigate_Executed(object sender, SLExtensions.Input.ExecutedEventArgs e)
        {
            ShowcaseItem item = e.Parameter as ShowcaseItem;
            if (lastPage != item)
            {
                lastPage = item;
                if (item != null)
                {
                    int idx = (Items.IndexOf(item) + 1);
                    if (Application.Current is App)
                        ((App)Application.Current).AddHistoryPoint("Page", idx.ToString(), "Page:" + item.Title);
                    Type t = Type.GetType(item.Page);

                    GoogleAnalytics.TrackPageView("UA-6095100-1", "/Showcase/" + idx + "_" + HttpUtility.UrlEncode(item.Title));
                    CurrentPage = Activator.CreateInstance(t);
                }
                else
                {
                    if (Application.Current is App)
                        ((App)Application.Current).AddHistoryPoint("Page", "", "Page:");
                    CurrentPage = null;
                }
            }
        }

        void ShowcaseController_Navigate(object sender, SLExtensions.Input.StateEventArgs e)
        {
            ScriptObject so = e.State;
            object o = so.GetProperty("Page");
            if (o != null && o is string)
            {
                Commands.Navigate.Execute(Items[int.Parse((string)o) - 1]);
            }
        }

        #endregion Methods
    }
}
