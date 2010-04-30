namespace SLExtensions
{
    using System;
    using System.Linq;
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

    public static class ApplicationExtensions
    {
        #region Methods

        public static string GetParam(this Application application, string name)
        {
            return GetParam(application, name, null);
        }

        public static string GetParam(this Application application, string name, StringComparer stringComparer)
        {
            stringComparer = stringComparer ?? StringComparer.CurrentCulture;
            return (from child in HtmlPage.Plugin.Children
                    let nameAttribute = child.GetProperty("name") as string
                    let valueAttribute = child.GetProperty("value") as string
                    where stringComparer.Compare(name, nameAttribute) == 0
                    select valueAttribute as string).FirstOrDefault();
        }

        #endregion Methods
    }
}