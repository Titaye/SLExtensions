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
using System.IO;

namespace SLExtensions.Imaging
{
    /// <summary>
    /// Bitmap File Header class used y BmpDecoder.
    /// </summary>
    public class BmpFileHeader
    {
        /// Original EditableImage and PngEncoder classes courtesy Joe Stegman.
        /// http://blogs.msdn.com/jstegman

        private const int _SIZE = 14;

        /// <summary>
        /// 
        /// </summary>
        public short BitmapType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public short NA1 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public short NA2 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int OffsetToData { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static BmpFileHeader FillFromStream(Stream stream)
        {
            byte[] buffer = new byte[_SIZE];
            BmpFileHeader header = new BmpFileHeader();

            stream.Read(buffer, 0, _SIZE);

            // Fill
            header.BitmapType = BitConverter.ToInt16(buffer, 0);
            header.Size = BitConverter.ToInt32(buffer, 2);
            header.OffsetToData = BitConverter.ToInt32(buffer, 10);

            // Return results
            return header;
        }
    }
}
