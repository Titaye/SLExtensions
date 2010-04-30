#region Header

// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

#endregion Header

namespace SLExtensions.Controls
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary> 
    /// Names and helpers for visual states in the controls.
    /// </summary> 
    internal static class VisualStates
    {
        #region Fields

        /// <summary> 
        /// Active state group
        /// </summary>
        public const string GroupActive = "ActiveStates";

        /// <summary>
        /// Common state group 
        /// </summary>
        public const string GroupCommon = "CommonStates";

        /// <summary> 
        /// Focus state group 
        /// </summary>
        public const string GroupFocus = "FocusStates";

        /// <summary>
        /// Selection state group
        /// </summary> 
        public const string GroupSelection = "SelectionStates";

        /// <summary>
        /// Active state 
        /// </summary>
        public const string StateActive = "Active";

        /// <summary>
        /// Disabled state
        /// </summary> 
        public const string StateDisabled = "Disabled";

        /// <summary>
        /// Focused state 
        /// </summary> 
        public const string StateFocused = "Focused";

        /// <summary>
        /// Focused and Dropdown is showing state
        /// </summary> 
        public const string StateFocusedDropDown = "FocusedDropDown";

        /// <summary>
        /// Inactive state
        /// </summary> 
        public const string StateInactive = "Inactive";

        /// <summary> 
        /// MouseOver state
        /// </summary>
        public const string StateMouseOver = "MouseOver";

        /// <summary>
        /// Normal state
        /// </summary> 
        public const string StateNormal = "Normal";

        /// <summary>
        /// Pressed state 
        /// </summary> 
        public const string StatePressed = "Pressed";

        /// <summary> 
        /// Readonly state 
        /// </summary>
        public const string StateReadOnly = "ReadOnly";

        /// <summary>
        /// Selected state
        /// </summary> 
        public const string StateSelected = "Selected";

        /// <summary> 
        /// Selected and unfocused state
        /// </summary>
        public const string StateSelectedUnfocused = "SelectedUnfocused";

        /// <summary> 
        /// Unfocused state
        /// </summary>
        public const string StateUnfocused = "Unfocused";

        /// <summary>
        /// Unselected state 
        /// </summary> 
        public const string StateUnselected = "Unselected";

        #endregion Fields

        #region Methods

        /// <summary> 
        /// Use VisualStateManager to change the visual state of the control. 
        /// </summary>
        /// <param name="control"> 
        /// Control whose visual state is being changed.
        /// </param>
        /// <param name="useTransitions"> 
        /// true to use transitions when updating the visual state, false to
        /// snap directly to the new visual state.
        /// </param> 
        /// <param name="stateNames"> 
        /// Ordered list of state names and fallback states to transition into.
        /// Only the first state to be found will be used. 
        /// </param>
        public static void GoToState(Control control, bool useTransitions, params string[] stateNames)
        {
            if (control == null)
            {
                throw new ArgumentNullException("control");
            }

            if (stateNames == null)
            {
                return;
            }

            foreach (string name in stateNames)
            {
                if (VisualStateManager.GoToState(control, name, useTransitions))
                {
                    break;
                }
            }
        }

        #endregion Methods
    }
}