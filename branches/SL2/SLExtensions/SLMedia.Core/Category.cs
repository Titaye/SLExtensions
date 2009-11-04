namespace SLMedia.Core
{
    using System;
    using System.Net;
    using System.Windows;
    using System.Windows.Browser;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    using SLExtensions;

    public class Category : NotifyingObject
    {
        #region Fields

        private string description;
        private string name;

        #endregion Fields

        #region Constructors

        static Category()
        {
            HtmlPage.RegisterCreateableType("Category", typeof(Category));
        }

        #endregion Constructors

        #region Properties

        [ScriptableMember]
        public string Description
        {
            get { return description; }
            set
            {
                if (description != value)
                {
                    description = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.Description));
                }
            }
        }

        [ScriptableMember]
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.Name));
                }
            }
        }

        #endregion Properties
    }
}