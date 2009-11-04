namespace SLExtensions.Showcase.Controllers
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

    using SLExtensions.Collections.ObjectModel;
    using SLExtensions.Input;

    public class PageFixedComboBoxController : NotifyingObject
    {
        #region Fields

        private readonly ObservableCollection<string> _itemsSource = new ObservableCollection<string>();

        private string _addItem;
        private Command _addItemCommand;

        #endregion Fields

        #region Constructors

        public PageFixedComboBoxController()
        {
            AddItem = string.Format("AddItem_{0}", GetHashCode());
            _addItemCommand = new Command(AddItem);
            _addItemCommand.Executed += new EventHandler<ExecutedEventArgs>(_addItemCommand_Executed);
        }

        #endregion Constructors

        #region Properties

        public string AddItem
        {
            get { return _addItem; }
            set
            {
                if (_addItem != value)
                {
                    _addItem = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.AddItem));
                }
            }
        }

        public ObservableCollection<string> ItemsSource
        {
            get { return _itemsSource; }
        }

        #endregion Properties

        #region Methods

        void _addItemCommand_Executed(object sender, ExecutedEventArgs e)
        {
            _itemsSource.Add(string.Format("Item {0}", _itemsSource.Count));
        }

        #endregion Methods
    }
}
