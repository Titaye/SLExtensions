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
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    using SLExtensions.Controls.Media;

    public partial class PageAnimation : UserControl
    {
        #region Constructors

        public PageAnimation()
        {
            // Ensure static commands are created before xaml is processed
            StoryboardCommands.Initialize();

            InitializeComponent();
        }

        #endregion Constructors
    }
}