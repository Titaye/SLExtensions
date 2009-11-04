namespace SLExtensions.Showcase.Controllers
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    using SLExtensions.Input;

    public class OpenFileDialogInfo : INotifyPropertyChanged
    {
        #region Fields

        private bool enableMultipleSelection;
        private string filter;

        #endregion Fields

        #region Constructors

        public OpenFileDialogInfo()
        {
            EnableMultipleSelection = false;
            Filter = "Text files|*.txt";
        }

        #endregion Constructors

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Properties

        public bool EnableMultipleSelection
        {
            get { return enableMultipleSelection; }
            set
            {
                if (enableMultipleSelection != value)
                {
                    enableMultipleSelection = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.EnableMultipleSelection));
                }
            }
        }

        public string Filter
        {
            get { return filter; }
            set
            {
                if (filter != value)
                {
                    filter = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.Filter));
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

        #endregion Methods
    }

    public class PageCommandController : INotifyPropertyChanged, IDisposable
    {
        #region Fields

        private OpenFileDialogInfo openFileInfo;

        #endregion Fields

        #region Constructors

        public PageCommandController()
        {
            OpenFileInfo = new OpenFileDialogInfo();
            TestCommands.OpenCommand.Executed += new EventHandler<ExecutedEventArgs>(OpenCommand_Executed);
        }

        #endregion Constructors

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Properties

        public OpenFileDialogInfo OpenFileInfo
        {
            get { return openFileInfo; }
            set
            {
                if (openFileInfo != value)
                {
                    openFileInfo = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.OpenFileInfo));
                }
            }
        }

        #endregion Properties

        #region Methods

        public void Dispose()
        {
            TestCommands.OpenCommand.Executed -= new EventHandler<ExecutedEventArgs>(OpenCommand_Executed);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        void OpenCommand_Executed(object sender, ExecutedEventArgs e)
        {
            OpenFileDialogInfo info = e.Parameter as OpenFileDialogInfo;
            if (info != null)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = info.EnableMultipleSelection;
                ofd.Filter = info.Filter;
                ofd.ShowDialog();

            }
        }

        #endregion Methods
    }

    public static class TestCommands
    {
        #region Constructors

        static TestCommands()
        {
            OpenCommand = new Command("Open");
        }

        #endregion Constructors

        #region Properties

        public static Command OpenCommand
        {
            get;
            private set;
        }

        #endregion Properties
    }
}