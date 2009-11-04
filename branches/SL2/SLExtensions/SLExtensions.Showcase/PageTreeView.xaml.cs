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
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    using SLExtensions.Controls;

    public class Child1Node
    {
        #region Constructors

        public Child1Node()
        {
            Nodes = new List<Child2Node>();
        }

        #endregion Constructors

        #region Properties

        public string Id
        {
            get; set;
        }

        public List<Child2Node> Nodes
        {
            get; set;
        }

        #endregion Properties
    }

    public class Child2Node
    {
        #region Constructors

        public Child2Node()
        {
        }

        #endregion Constructors

        #region Properties

        public string Name
        {
            get; set;
        }

        #endregion Properties
    }

    [ContentProperty("Templates")]
    public class CustomTemplateSelector : DataTemplateSelector
    {
        #region Constructors

        public CustomTemplateSelector()
        {
            Templates = new List<ItemCustomTemplateSelector>();
        }

        #endregion Constructors

        #region Properties

        public List<ItemCustomTemplateSelector> Templates
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is RootNode)
            {
                return FindTemplateFromName("RootNode");
            }
            if (item is Child1Node)
            {
                return FindTemplateFromName("Child1Node");
            }
            if (item is Child2Node)
            {
                return FindTemplateFromName("Child2Node");
            }
            return base.SelectTemplate(item, container);
        }

        private DataTemplate FindTemplateFromName(string name)
        {
            ItemCustomTemplateSelector item = Templates.FirstOrDefault(i => i.ItemName == name);
            if (item != null)
                return item.Template;
            return null;
        }

        #endregion Methods
    }

    [ContentProperty("Template")]
    public class ItemCustomTemplateSelector
    {
        #region Properties

        public string ItemName
        {
            get; set;
        }

        public DataTemplate Template
        {
            get; set;
        }

        #endregion Properties
    }

    public partial class PageTreeView : UserControl
    {
        #region Constructors

        public PageTreeView()
        {
            InitializeComponent();

            List<RootNode> datasource = new List<RootNode>();
            List<Child2Node> nodes2 = new List<Child2Node>();
            for (int i = 0; i < 3; i++)
            {
                nodes2.Add(new Child2Node() { Name = "level 2 nodes (" + i + ")" });
            }

            List<Child1Node> nodes1 = new List<Child1Node>();
            for (int i = 0; i < 3; i++)
            {
                nodes1.Add(new Child1Node() { Id = "level 1 nodes (" + i + ")", Nodes = nodes2 });
            }

            for (int i = 0; i < 3; i++)
            {
                datasource.Add(new RootNode() { Data = "rootnode nodes (" + i + ")", Nodes = nodes1 });
            }

            tv.ItemsSource = datasource;
        }

        #endregion Constructors
    }

    public class RootNode
    {
        #region Constructors

        public RootNode()
        {
            Nodes = new List<Child1Node>();
        }

        #endregion Constructors

        #region Properties

        public string Data
        {
            get; set;
        }

        public List<Child1Node> Nodes
        {
            get; set;
        }

        #endregion Properties
    }
}