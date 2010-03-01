namespace SLExtensions.Showcase
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    using SLExtensions.Collections.ObjectModel;
    using SLExtensions.Data;

    public partial class PageResourceSelector : UserControl
    {
        #region Fields

        private ObservableCollection<object> _itemsSource = new ObservableCollection<object>();

        #endregion Fields

        #region Constructors

        public PageResourceSelector()
        {
            InitializeComponent();
            dynTplItemsControl.ItemsSource = _itemsSource;
        }

        #endregion Constructors

        #region Methods

        private void OnAddTypeA(object sender, RoutedEventArgs e)
        {
            _itemsSource.Add(new TypeA());
        }

        private void OnAddTypeB(object sender, RoutedEventArgs e)
        {
            _itemsSource.Add(new TypeB());
        }

        #endregion Methods
    }

    public class Person : IProvideResourceKey
    {
        #region Properties

        public double Age
        {
            get; set;
        }

        public string FirstName
        {
            get; set;
        }

        public string ResourceKey
        {
            get
            {
                if (Age < 2)
                    return "Baby";
                else if (Age < 10)
                    return "Child";
                else if (Age < 20)
                    return "Teenager";
                else return "Adult";
            }
        }

        #endregion Properties
    }

    public class TypeA
    {
    }

    public class TypeB
    {
    }
}