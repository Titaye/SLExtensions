// <copyright file="StoryboardCommands.cs" company="Ucaya">
// Distributed under Microsoft Public License (Ms-PL)
// </copyright>
// <author>Thierry Bouquain</author>
namespace SLExtensions.Controls.Media
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

    /// <summary>
    /// Provide generic commands for managing storyboards
    /// </summary>
    public static class StoryboardCommands
    {
        #region Constructors

        /// <summary>
        /// Initialize static properties
        /// </summary>
        static StoryboardCommands()
        {
            BeginStoryboard = new Command("BeginStoryboard");
            BeginStoryboard.Executed += new EventHandler<ExecutedEventArgs>(BeginStoryboard_Executed);

            StopStoryboard = new Command("StopStoryboard");
            StopStoryboard.Executed += new EventHandler<ExecutedEventArgs>(StopStoryboard_Executed);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets a generic command for starting a storyboard from a name given in parameter.
        /// </summary>
        /// <value>The begin storyboard command.</value>
        public static Command BeginStoryboard
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a generic command for stoping a storyboard from a name given in parameter.
        /// </summary>
        /// <value>The stop storyboard command.</value>
        public static Command StopStoryboard
        {
            get;
            private set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Ensure static fields are initialized
        /// </summary>
        public static void Initialize()
        {
        }

        /// <summary>
        /// Executes the begin storyboard command
        /// </summary>
        /// <param name="sender">the command source</param>
        /// <param name="e">the event argument</param>
        private static void BeginStoryboard_Executed(object sender, ExecutedEventArgs e)
        {
            Storyboard storyboard = FindStoryboard(e);
            if (storyboard != null)
            {
                storyboard.Begin();
            }
        }

        /// <summary>
        /// Finds the storyboard from the command parameter
        /// </summary>
        /// <param name="e">The ExecutedEventArgs</param>
        /// <returns>returns the storyboard if found otherwise null</returns>
        private static Storyboard FindStoryboard(ExecutedEventArgs e)
        {
            FrameworkElement element = e.Source as FrameworkElement;
            if (element != null)
            {
                string storyboardName = e.Parameter as string;
                if (string.IsNullOrEmpty(storyboardName))
                {
                    return null;
                }

                return FindStoryboardResource(element, storyboardName);
            }

            return null;
        }

        /// <summary>
        /// Finds the storyboard in visual tree resources.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="storyboardName">Name of the storyboard.</param>
        /// <returns>returns the storyboard if found otherwise null</returns>
        private static Storyboard FindStoryboardResource(FrameworkElement element, string storyboardName)
        {
            if (element == null)
            {
                return Application.Current.Resources[storyboardName] as Storyboard;
            }

            Storyboard storyboard = element.Resources[storyboardName] as Storyboard;

            if (storyboard != null)
            {
                return storyboard;
            }

            return FindStoryboardResource(element.Parent as FrameworkElement, storyboardName);
        }

        /// <summary>
        /// Executes the stop storyboard command
        /// </summary>
        /// <param name="sender">the command source</param>
        /// <param name="e">the event argument</param>
        private static void StopStoryboard_Executed(object sender, ExecutedEventArgs e)
        {
            Storyboard storyboard = FindStoryboard(e);
            if (storyboard != null)
            {
                storyboard.Stop();
            }
        }

        #endregion Methods
    }
}