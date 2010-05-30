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

    public static partial class StoryboardExtensionsCommands
    {
        #region Fields

        // Using a DependencyProperty as the backing store for BeginStoryboardCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BeginStoryboardCommandProperty = 
            DependencyProperty.RegisterAttached("BeginStoryboardCommand", typeof(Storyboard), typeof(StoryboardExtensions), new PropertyMetadata(BeginStoryboardCommandChangedCallback));

        #endregion Fields

        #region Methods

        public static Storyboard GetBeginStoryboardCommand(DependencyObject obj)
        {
            return (Storyboard)obj.GetValue(BeginStoryboardCommandProperty);
        }

        public static void SetBeginStoryboardCommand(DependencyObject obj, Storyboard value)
        {
            obj.SetValue(BeginStoryboardCommandProperty, value);
        }

        private static void BeginStoryboardCommandChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Storyboard sb = e.NewValue as Storyboard;
            if (sb != null)
            {
                CommandService.SetCommand(d, new StoryboardCommand());
                CommandService.SetCommandParameter(d, new StoryboardCommandParameter { Action = StoryboardAction.Begin, Storyboard = sb });
            }
        }

        #endregion Methods
    }
}