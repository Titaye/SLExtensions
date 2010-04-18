namespace SLExtensions.Globalization
{
    using System;
    using System.Net;
    using System.Resources;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    public class ResourceLoader
    {
        #region Properties

        public string ResourceManagerType
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public ResourceManager GetResourceManager()
        {
            return new ResourceManager(Type.GetType(ResourceManagerType));
        }

        #endregion Methods
    }
}