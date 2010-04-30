namespace SLExtensions.Showcase
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

    using SLExtensions.Input;

    public static class Commands
    {
        #region Constructors

        static Commands()
        {
            Navigate = new Command("Navigate");
        }

        #endregion Constructors

        #region Properties

        public static Command Navigate
        {
            get; private set;
        }

        #endregion Properties
    }
}