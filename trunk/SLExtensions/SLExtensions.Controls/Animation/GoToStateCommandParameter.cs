namespace SLExtensions.Controls.Animation
{
    using System;
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

    [TypeConverterAttribute(typeof(GoToStateCommandParameterTypeConverter))]
    public class GoToStateCommandParameter
    {
        #region Properties

        public string ElementName
        {
            get; set;
        }

        public string StateName
        {
            get; set;
        }

        #endregion Properties
    }
}