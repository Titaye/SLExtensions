namespace SLExtensions.Input
{
    using System;
    using System.Windows.Browser;

    public class StateEventArgs : EventArgs
    {
        #region Properties

        public ScriptObject State
        {
            get; set;
        }

        #endregion Properties
    }
}