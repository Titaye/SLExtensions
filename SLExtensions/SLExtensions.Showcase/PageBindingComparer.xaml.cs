namespace SLExtensions.Showcase
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    public class CustomCompare : IComparer
    {
        #region Methods

        public int Compare(object x, object y)
        {
            return ((int)x).CompareTo((int)y);
        }

        #endregion Methods
    }

    public class ImageObj : INotifyPropertyChanged
    {
        #region Fields

        private int id;
        private int oldId;
        private string oldTitre;
        private string titre;

        #endregion Fields

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Properties

        public int Id
        {
            get { return id; }
            set
            {
                if (id != value)
                {
                    id = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.Id));
                }
            }
        }

        public int OldId
        {
            get { return oldId; }
            set
            {
                if (oldId != value)
                {
                    oldId = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.OldId));
                }
            }
        }

        public string OldTitre
        {
            get { return oldTitre; }
            set
            {
                if (oldTitre != value)
                {
                    oldTitre = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.OldTitre));
                }
            }
        }

        public string Titre
        {
            get { return titre; }
            set
            {
                if (titre != value)
                {
                    titre = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.Titre));
                }
            }
        }

        #endregion Properties

        #region Methods

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Methods
    }

    public class ListImageObj : List<ImageObj>
    {
    }

    public partial class PageBindingComparer : UserControl
    {
        #region Constructors

        public PageBindingComparer()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        private void TextBoxId_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext != null)
            {
                try
                {
                    ((ImageObj)((FrameworkElement)sender).DataContext).Id = Int32.Parse(((TextBox)sender).Text);
                }
                catch { }
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext != null)
            {
                ((ImageObj)((FrameworkElement)sender).DataContext).Titre = ((TextBox)sender).Text;
            }
        }

        #endregion Methods
    }
}
