namespace SLExtensions.Controls.Animation
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

    #region Enumerations

    public enum StoryboardAction
    {
        Begin,
        Stop
    }

    #endregion Enumerations

    public class StoryboardCommand : Command<StoryboardCommandParameter>
    {
        #region Constructors

        public StoryboardCommand()
            : base(ExecuteAction)
        {
        }

        #endregion Constructors

        #region Methods

        private static void ExecuteAction(StoryboardCommandParameter prm)
        {
            if (prm == null || prm.Storyboard == null)
                return;

            switch (prm.Action)
            {
                case StoryboardAction.Begin:
                    prm.Storyboard.Begin();
                    break;
                case StoryboardAction.Stop:
                    prm.Storyboard.Stop();
                    break;
            }
        }

        #endregion Methods
    }

    public class StoryboardCommandParameter
    {
        #region Properties

        public StoryboardAction Action
        {
            get; set;
        }

        public Storyboard Storyboard
        {
            get; set;
        }

        #endregion Properties
    }
}