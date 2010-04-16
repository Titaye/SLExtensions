namespace SLExtensions.Showcase.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    public class PageSwappableContentController : INotifyPropertyChanged
    {
        #region Fields

        private IEnumerable<SwappableContentData> data;
        private SwappableContentData selectedData;

        #endregion Fields

        #region Constructors

        public PageSwappableContentController()
        {
            List<SwappableContentData> data = new List<SwappableContentData>();
            data.Add(new SwappableContentData() { Name = "Red", Color = new SolidColorBrush(Colors.Red) });
            data.Add(new SwappableContentData() { Name = "Blue", Color = new SolidColorBrush(Colors.Blue) });
            Data = data;
        }

        #endregion Constructors

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Properties

        public IEnumerable<SwappableContentData> Data
        {
            get { return data; }
            set
            {
                if (data != value)
                {
                    data = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.Data));
                }
            }
        }

        public SwappableContentData SelectedData
        {
            get { return selectedData; }
            set
            {
                if (selectedData != value)
                {
                    selectedData = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.SelectedData));
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

    public class SwappableContentData
    {
        #region Properties

        public SolidColorBrush Color
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }

        #endregion Properties
    }
}
