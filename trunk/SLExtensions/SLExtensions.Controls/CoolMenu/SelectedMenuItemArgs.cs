using System;

namespace SLExtensions.Controls
{
    /// <summary>
    /// Delegate for the MenuIndexChanged and MenuItemClicked events.
    /// </summary>
    /// <param name="sender">Represents the object that fired the event.</param>
    /// <param name="e">Event data for the event.</param>
    public delegate void MenuIndexChangedHandler(object sender, SelectedMenuItemArgs e);

    /// <summary>
    /// Event data for the MenuIndexChanged and MenuItemClicked events.
    /// </summary>
    public class SelectedMenuItemArgs : EventArgs
    {
        private readonly CoolMenuItem m_cmi;
        private readonly int m_index;

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

        /// <summary>
        /// The currently selected menu item.
        /// </summary>
        public CoolMenuItem Item
        {
            get { return m_cmi; }
        }

        /// <summary>
        /// The currently selected index of the menu item.
        /// </summary>
        public int Index
        {
            get { return m_index; }
        }
    }
}
