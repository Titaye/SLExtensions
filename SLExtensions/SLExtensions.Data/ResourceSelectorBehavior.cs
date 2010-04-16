namespace SLExtensions.Data
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

    #region Enumerations

    /// <summary>
    /// Possible values for the Behavior property of the ResourceSelector.
    /// It is used to define how to select a resource at binding time.
    /// </summary>
    public enum ResourceSelectorBehavior
    {
        /// <summary>
        /// Try to define which strategy should be used, using the provided value type.
        ///  - If type is a value type from mscorlib (or a string), it will use "ValueAsKey"
        ///  - If type implements IProvideResourceKey, it will use "ValueAsKey" using the IProvideResourceKey implementation
        ///  - Else, it will use "ValueTypeAsKey"
        /// </summary>
        Auto,
        /// <summary>
        /// - If the value implements IProvideResourceKey, it will use its implementation to find the resource
        /// - Else, it will use System.Convert.ToString(value, invariantCulture)
        /// </summary>
        ValueAsKey,
        /// <summary>
        /// Use the type of the provided value as a key. First try will be made with FullyQualifiedName, then FullName, then Name
        /// </summary>
        ValueTypeAsKey
    }

    #endregion Enumerations
}