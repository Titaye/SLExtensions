using System;
using System.Collections.Generic;
using System.Text;

namespace SLExtensions.IO
{
    /// <summary>
    /// Defines the byte order.
    /// </summary>
    public enum ByteOrder
    {
        /// <summary>
        /// The most significant byte (MSB) value is stored at the memory location with the lowest address.
        /// </summary>
        BigEndian,
        /// <summary>
        /// The least significant byte (LSB) value is at the lowest address.
        /// </summary>
        LittleEndian
    }
}
