namespace SLExtensions.Showcase.Controllers
{
    using System;
    using System.Collections;
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

    public class PageAnimatingFillPanelController : INotifyPropertyChanged
    {
        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Properties

        public IEnumerable<TagCloudData> TagCloud
        {
            get
            {
                Random rnd = new Random();
                return new TagCloudData[] {
                    new TagCloudData( ".NET", rnd.Next(8, 20)),
                    new TagCloudData( ".NET 2.0", rnd.Next(8, 20)),
                    new TagCloudData( ".NET 3.5", rnd.Next(8, 20)),
                    new TagCloudData( "Ajax", rnd.Next(8, 20)),
                    new TagCloudData( "AJAX Control", rnd.Next(8, 20)),
                    new TagCloudData( "Toolkit", rnd.Next(8, 20)),
                    new TagCloudData( "ASP.NET", rnd.Next(8, 20)),
                    new TagCloudData( "ASP.NET 2.0", rnd.Next(8, 20)),
                    new TagCloudData( "blog", rnd.Next(8, 20)),
                    new TagCloudData( "C#", rnd.Next(8, 20)),
                    new TagCloudData( "C# 2.0", rnd.Next(8, 20)),
                    new TagCloudData( "CMS", rnd.Next(8, 20)),
                    new TagCloudData( "Controls", rnd.Next(8, 20)),
                    new TagCloudData( "Database", rnd.Next(8, 20)),
                    new TagCloudData( "DNN", rnd.Next(8, 20)),
                    new TagCloudData( "DotNetNuke", rnd.Next(8, 20)),
                    new TagCloudData( "Enterprise Library", rnd.Next(8, 20)),
                    new TagCloudData( "Framework", rnd.Next(8, 20)),
                    new TagCloudData( "game", rnd.Next(8, 20)),
                    new TagCloudData( "Library", rnd.Next(8, 20)),
                    new TagCloudData( "LINQ", rnd.Next(8, 20)),
                    new TagCloudData( "MOSS", rnd.Next(8, 20)),
                    new TagCloudData( "MVC", rnd.Next(8, 20)),
                    new TagCloudData( "orm", rnd.Next(8, 20)),
                    new TagCloudData( "patterns & practices", rnd.Next(8, 20)),
                    new TagCloudData( "powershell", rnd.Next(8, 20)),
                    new TagCloudData( "RSS", rnd.Next(8, 20)),
                    new TagCloudData( "Sharepoint", rnd.Next(8, 20)),
                    new TagCloudData( "Silverlight", rnd.Next(8, 20)),
                    new TagCloudData( "SQL", rnd.Next(8, 20)),
                    new TagCloudData( "SQL Server", rnd.Next(8, 20)),
                    new TagCloudData( "TFS", rnd.Next(8, 20)),
                    new TagCloudData( "Tools", rnd.Next(8, 20)),
                    new TagCloudData( "utility", rnd.Next(8, 20)),
                    new TagCloudData( "VB.NET", rnd.Next(8, 20)),
                    new TagCloudData( "Visual Studio", rnd.Next(8, 20)),
                    new TagCloudData( "watch", rnd.Next(8, 20)),
                    new TagCloudData( "wcf", rnd.Next(8, 20)),
                    new TagCloudData( "Windows Forms", rnd.Next(8, 20)),
                    new TagCloudData( "winforms", rnd.Next(8, 20)),
                    new TagCloudData( "WPF", rnd.Next(8, 20)),
                    new TagCloudData( "xml", rnd.Next(8, 20)),
                    new TagCloudData( "XNA", rnd.Next(8, 20))
                };
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

    public class TagCloudData
    {
        #region Constructors

        public TagCloudData(string name, double size)
        {
            this.Name = name;
            this.Size = size;
        }

        #endregion Constructors

        #region Properties

        public string Name
        {
            get; set;
        }

        public double Size
        {
            get; set;
        }

        #endregion Properties
    }
}