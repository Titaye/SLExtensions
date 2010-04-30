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

    public class DefaultResourceManager
    {
        #region Fields

        private string type;

        #endregion Fields

        #region Properties

        public string Type
        {
            get { return type; }
            set
            {
                if (type != value)
                {
                    type = value;

                    if (string.IsNullOrEmpty(type))
                        Localizer.DefaultResourceManager = null;
                    else
                    {
                        Type t = System.Type.GetType(type);
                        if (t != null)
                            Localizer.DefaultResourceManager = new ResourceManager(t);
                        else
                            Localizer.DefaultResourceManager = null;
                    }
                }
            }
        }

        #endregion Properties
    }
}