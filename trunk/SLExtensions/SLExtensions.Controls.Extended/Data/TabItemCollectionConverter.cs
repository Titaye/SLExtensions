namespace SLExtensions.Data
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    using SLExtensions.Collections.ObjectModel;

    public class TabItemCollectionConverter : IValueConverter
    {
        #region Properties

        public DataTemplate ContentTemplate
        {
            get;
            set;
        }

        public DataTemplate HeaderTemplate
        {
            get;
            set;
        }

        public Style Style
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;

            ObservableCollection<TabItem> tabs = new ObservableCollection<TabItem>();

            FillList(value, tabs);

            INotifyCollectionChanged sourceNotifyCollectionChanged = value as INotifyCollectionChanged;
            if (sourceNotifyCollectionChanged != null)
            {
                sourceNotifyCollectionChanged.CollectionChanged += (o, args) =>
                {
                    switch (args.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                            foreach (var item in args.NewItems)
                            {
                                tabs.Insert(args.NewStartingIndex, CreateTabItem(item));
                            }
                            break;
                        case NotifyCollectionChangedAction.Remove:
                            for (int i = 0; i < args.OldItems.Count; i++)
                            {
                                tabs.RemoveAt(args.OldStartingIndex);
                            }
                            break;
                        default:
                        case NotifyCollectionChangedAction.Replace:
                        case NotifyCollectionChangedAction.Reset:
                            tabs.Clear();
                            FillList(value, tabs);
                            break;
                    }
                };
            }

            return tabs;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        private TabItem CreateTabItem(object item)
        {
            TabItem tabUi = new TabItem();
            tabUi.DataContext = item;

            if (Style != null)
                tabUi.Style = Style;

            if (HeaderTemplate != null)
            {
                FrameworkElement fe = (FrameworkElement)HeaderTemplate.LoadContent();
                fe.DataContext = item;
                tabUi.Header = fe;
            }
            else
            {
                tabUi.Header = item;
            }

            if (ContentTemplate != null)
            {
                FrameworkElement fe = (FrameworkElement)ContentTemplate.LoadContent();
                fe.DataContext = item;
                tabUi.Content = fe;
            }
            else
            {
                tabUi.Content = item;
            }
            return tabUi;
        }

        private void FillList(object value, ObservableCollection<TabItem> tabs)
        {
            foreach (var item in (IEnumerable)value)
            {
                TabItem tabUi = CreateTabItem(item);

                tabs.Add(tabUi);
            }
        }

        #endregion Methods
    }
}