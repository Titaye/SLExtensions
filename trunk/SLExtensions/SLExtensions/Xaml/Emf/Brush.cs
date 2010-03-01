namespace SLExtensions.Xaml.Emf
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows.Media;

    using SLExtensions.IO;

    /// <summary>
    /// Represents a brush
    /// </summary>
    internal class Brush
    {
        #region Fields

        /// <summary>
        /// Hatched brush 
        /// </summary>
        public const short BS_HATCHED = 2;

        /// <summary>
        /// Hollow brush
        /// </summary>
        public const short BS_NULL = 1;

        /// <summary>
        /// Solid brush
        /// </summary>
        public const short BS_SOLID = 0;

        /// <summary>
        /// A 45-degree upward, left-to-right hatch
        /// </summary>
        public const short HS_BDIAGONAL = 3;

        /// <summary>
        /// Horizontal and vertical cross-hatch
        /// </summary>
        public const short HS_CROSS = 4;

        /// <summary>
        /// 45-degree crosshatch
        /// </summary>
        public const short HS_DIAGCROSS = 5;

        /// <summary>
        /// A 45-degree downward, left-to-right hatch
        /// </summary>
        public const short HS_FDIAGONAL = 2;

        /// <summary>
        /// Horizontal hatch
        /// </summary>
        public const short HS_HORIZONTAL = 0;

        /// <summary>
        /// Vertical hatch
        /// </summary>
        public const short HS_VERTICAL = 1;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Brush"/> class.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        public Brush(byte[] parameters)
        {
            this.Style = EmfConverter.ReadInt16(parameters, 0);
            this.Color = EmfConverter.ReadColor(parameters, 2);
            this.Hatch = EmfConverter.ReadInt16(parameters, 6);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the color.
        /// </summary>
        /// <value>The color.</value>
        public Color Color
        {
            get; private set;
        }

        /// <summary>
        /// Gets the hatch.
        /// </summary>
        /// <value>The hatch.</value>
        public short Hatch
        {
            get; private set;
        }

        /// <summary>
        /// Gets the style.
        /// </summary>
        /// <value>The style.</value>
        public short Style
        {
            get; private set;
        }

        #endregion Properties
    }
}