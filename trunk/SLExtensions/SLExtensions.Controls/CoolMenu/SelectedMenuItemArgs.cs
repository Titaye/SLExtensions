namespace SLExtensions.Controls
{
    using System;

    #region Delegates

    /// <summary>
    /// Delegate for the MenuIndexChanged and MenuItemClicked events.
    /// </summary>
    /// <param name="sender">Represents the object that fired the event.</param>
    /// <param name="e">Event data for the event.</param>
    public delegate void MenuIndexChangedHandler(object sender, SelectedMenuItemArgs e);

    #endregion Delegates

    /// <summary>
    /// Event data for the MenuIndexChanged and MenuItemClicked events.
    /// </summary>
    public class SelectedMenuItemArgs : EventArgs
    {
        #region Fields

        private readonly CoolMenuItem m_cmi;
        private readonly int m_index;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Creates a new instance of the SelectedMenuItemArgs class.
        /// </summary>
        /// <param name="menuItem">The currently selected menu item</param>
        /// <param name="menuIndex">The index of the currently selected menu item.</param>
        public SelectedMenuItemArgs(CoolMenuItem menuItem, int menuIndex)
        {
            m_cmi = menuItem;
            m_index = menuIndex;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// The currently selected index of the menu item.
        /// </summary>
        public int Index
        {
            get { return m_index; }
        }

        /// <summary>
        /// The currently selected menu item.
        /// </summary>
        public CoolMenuItem Item
        {
            get { return m_cmi; }
        }

        #endregion Properties
    }
}