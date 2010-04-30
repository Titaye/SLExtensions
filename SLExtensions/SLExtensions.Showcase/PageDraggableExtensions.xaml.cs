// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="PageDraggableExtensions.xaml.cs" company="Raw Engineering">
//   Distributed under Ms-PL License.
// </copyright>
// <author>Kunal Shetye</author>
// <summary>
//   Defines the PageDraggableExtensions type.
//   Used to demonstrate the Draggable Extension methods.
// </summary>
// ---------------------------------------------------------------------------------------------------------------------
namespace SLExtensions.Showcase
{
    using System.Windows;
    using System.Windows.Controls;

    using SLExtensions;

    public partial class PageDraggableExtensions : UserControl
    {
        #region Fields

        private const string strGridDisabled = "Disable Grid Container Dragging";
        private const string strGridEnabled = "Enable Grid Container Dragging";
        private const string strLoginFormDisabled = "Disable Login-Form Draggable Dragging";
        private const string strLoginFormEnabled = "Enable Login-Form Draggable Dragging";
        private const string strStackPanelDisabled = "Disabled StackPanel Container Dragging";
        private const string strStackPanelEnabled = "Enable StackPanel Container Dragging";

        #endregion Fields

        #region Constructors

        public PageDraggableExtensions()
        {
            InitializeComponent();  // Init page components
            InitButtonContent();    // Set the initial button text
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Triggered when Login or cancel buttons are pressed on the spDemoLoginForm
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void btnDemoLoginButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            MessageBox.Show(
                string.Format(
                    "You clicked : {0}\r\nWith Username : {1} and Password : {2}",
                    ((Button) sender).Name,
                    txtUsername.Text,
                    txtPassword.Text));
        }

        /// <summary>
        /// Triggered when btnGrid is Clicked
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void btnGrid_Click(
            object sender,
            RoutedEventArgs e)
        {
            // Check if DemoGrid is already draggable
            if(!spDemoGrid.IsAlreadyDraggable())
            {
                spDemoGrid.Draggable(spSampleContent);      // Make draggable
                btnGrid.Content = strGridDisabled;          // Change Button Text
            }
            else
            {
                spDemoGrid.StopDraggable();                 // Stop Dragging
                btnGrid.Content = strGridEnabled;
            }
        }

        /// <summary>
        /// Triggered when btnLoginForm is Clicked
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void btnLoginForm_Click(
            object sender,
            RoutedEventArgs e)
        {
            // Check if LoginForm is already draggable
            if(!spDemoLoginForm.IsAlreadyDraggable())
            {
                spDemoLoginForm.Draggable(spSampleContent); // Make draggable
                btnLoginForm.Content = strLoginFormDisabled;// Change Button Text
            }
            else
            {
                spDemoLoginForm.StopDraggable();            // Stop Dragging
                btnLoginForm.Content = strLoginFormEnabled; // Change Button Text
            }
        }

        /// <summary>
        /// Triggered when btnStackPanel Button is Clicked
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void btnStackPanel_Click(
            object sender,
            RoutedEventArgs e)
        {
            // Check if DemoStackPanel is already draggable
            if(!spDemoStackPanel.IsAlreadyDraggable())
            {
                spDemoStackPanel.Draggable(spSampleContent);    // Make draggable
                btnStackPanel.Content = strStackPanelDisabled; // Change Button Text
            }
            else
            {
                spDemoStackPanel.StopDraggable();             // Stop Dragging
                btnStackPanel.Content = strStackPanelEnabled; // Change Button Text
            }
        }

        private void InitButtonContent()
        {
            btnStackPanel.Content = strStackPanelEnabled;   // Set to strStackPanelEnabled
            btnGrid.Content = strGridEnabled;               // Set to strStackPanelEnabled
            btnLoginForm.Content = strLoginFormEnabled;     // Set to strLoginFormEnabled
        }

        #endregion Methods
    }
}