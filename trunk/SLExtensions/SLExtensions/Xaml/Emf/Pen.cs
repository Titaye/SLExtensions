namespace SLExtensions.Xaml.Emf
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows.Media;

    using SLExtensions.IO;

    /// <summary>
    /// Represents a pen.
    /// </summary>
    internal class Pen
    {
        #region Fields

        /// <summary>
        /// PS_ALTERNATE
        /// </summary>
        public const int PS_ALTERNATE = 8;

        /// <summary>
        /// PS_COSMETIC
        /// </summary>
        public const int PS_COSMETIC = 0;

        /// <summary>
        /// PS_DASH
        /// </summary>
        public const int PS_DASH = 1;

        /// <summary>
        /// PS_DASHDOT
        /// </summary>
        public const int PS_DASHDOT = 3;

        /// <summary>
        /// PS_DASHDOTDOT
        /// </summary>
        public const int PS_DASHDOTDOT = 4;

        /// <summary>
        /// PS_DOT
        /// </summary>
        public const int PS_DOT = 2;

        /// <summary>
        /// PS_ENDCAP_FLAT
        /// </summary>
        public const int PS_ENDCAP_FLAT = 512;

        /// <summary>
        /// PS_ENDCAP_MASK
        /// </summary>
        public const int PS_ENDCAP_MASK = 3840;

        /// <summary>
        /// PS_ENDCAP_ROUND
        /// </summary>
        public const int PS_ENDCAP_ROUND = 0;

        /// <summary>
        /// PS_ENDCAP_SQUARE
        /// </summary>
        public const int PS_ENDCAP_SQUARE = 256;

        /// <summary>
        /// PS_GEOMETRIC
        /// </summary>
        public const int PS_GEOMETRIC = 65536;

        /// <summary>
        /// PS_INSIDEFRAME
        /// </summary>
        public const int PS_INSIDEFRAME = 6;

        /// <summary>
        /// PS_JOIN_BEVEL
        /// </summary>
        public const int PS_JOIN_BEVEL = 4096;

        /// <summary>
        /// PS_JOIN_MASK
        /// </summary>
        public const int PS_JOIN_MASK = 61440;

        /// <summary>
        /// PS_JOIN_MITER
        /// </summary>
        public const int PS_JOIN_MITER = 8192;

        /// <summary>
        /// PS_JOIN_ROUND
        /// </summary>
        public const int PS_JOIN_ROUND = 0;

        /// <summary>
        /// PS_NULL
        /// </summary>
        public const int PS_NULL = 5;

        /// <summary>
        /// PS_SOLID
        /// </summary>
        public const int PS_SOLID = 0;

        /// <summary>
        /// PS_STYLE_MASK
        /// </summary>
        public const int PS_STYLE_MASK = 15;

        /// <summary>
        /// PS_TYPE_MASK
        /// </summary>
        public const int PS_TYPE_MASK = 983040;

        /// <summary>
        /// PS_USERSTYLE
        /// </summary>
        public const int PS_USERSTYLE = 7;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Pen"/> class.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        public Pen(byte[] parameters)
        {
            this.Style = EmfConverter.ReadInt16(parameters, 0);
            this.Width = EmfConverter.ReadInt16(parameters, 2);
            this.Color = EmfConverter.ReadColor(parameters, 4);
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
        /// Gets the style.
        /// </summary>
        /// <value>The style.</value>
        public short Style
        {
            get; private set;
        }

        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <value>The width.</value>
        public short Width
        {
            get; private set;
        }

        #endregion Properties
    }
}