namespace SLExtensions.Controls
{
    /// <summary>
    /// Interface used to specify how a CoolMenu behaves
    /// </summary>
    public interface ICoolMenuBehavior
    {
        /// <summary>
        /// Intializes each element in the menu.
        /// </summary>
        /// <param name="parent">The menu containing the element.</param>
        /// <param name="element">The element to initialize.</param>
        void Initialize(CoolMenu parent, CoolMenuItem element);

        /// <summary>
        /// Fired on each element when the mouse is hovers over an element.
        /// </summary>
        /// <param name="proximity">Indicates how close the mouse is to the current element.</param>
        /// <param name="element">The element of concern.</param>
        void ApplyMouseEnterBehavior(int proximity, CoolMenuItem element);

        /// <summary>
        /// Fired on each element when the mouse is hovers over an element.
        /// </summary>
        /// <param name="element">The element of concern.</param>
        void ApplyMouseLeaveBehavior(CoolMenuItem element);

        /// <summary>
        /// Fired when the left mouse button is clicked on a particular element.
        /// </summary>
        /// <param name="selectedIndex">The index of the selected element.</param>
        /// <param name="element">The element of concern.</param>
        void ApplyMouseDownBehavior(int selectedIndex, CoolMenuItem element);

        /// <summary>
        /// Fired when the left mouse button is lifed on a particular element.
        /// </summary>
        /// <param name="selectedIndex">The index of the selected element.</param>
        /// <param name="element">The element of concern.</param>
        void ApplyMouseUpBehavior(int selectedIndex, CoolMenuItem element);
    }
}