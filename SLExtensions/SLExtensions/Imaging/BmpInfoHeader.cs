namespace SLExtensions.Imaging
{
    using System;
    using System.IO;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    /// <summary>
    /// Bitmap Info Header class used y BmpDecoder.
    /// </summary>
    public class BmpInfoHeader
    {
        #region Fields

        /// Original EditableImage and PngEncoder classes courtesy Joe Stegman.
        /// http://blogs.msdn.com/jstegman
        private const int _SIZE = 40;

        #endregion Fields

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public short BitsPerPixel
        {
            get; set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int ColorCount
        {
            get; set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Compression
        {
            get; set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int HeaderSize
        {
            get; set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Height
        {
            get; set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int ImageSize
        {
            get; set;
        }

        /// <summary>
        /// 
        /// </summary>
        public short NA1
        {
            get; set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int NA2
        {
            get; set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int NA3
        {
            get; set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int NA4
        {
            get; set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Width
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static BmpInfoHeader FillFromStream(Stream stream)
        {
            byte[] buffer = new byte[_SIZE];
            BmpInfoHeader header = new BmpInfoHeader();

            stream.Read(buffer, 0, _SIZE);

            // Fill
            header.HeaderSize = BitConverter.ToInt32(buffer, 0);
            header.Width = BitConverter.ToInt32(buffer, 4);
            header.Height = BitConverter.ToInt32(buffer, 8);
            header.BitsPerPixel = BitConverter.ToInt16(buffer, 14);
            header.Compression = BitConverter.ToInt32(buffer, 16);
            header.ImageSize = BitConverter.ToInt32(buffer, 20);
            header.ColorCount = BitConverter.ToInt32(buffer, 32);

            // Fix for no ImageSize in the header
            if (header.ImageSize == 0)
            {
                int rowSize = 4 * (int)Math.Ceiling(header.Width * (header.BitsPerPixel / 32));
                int fileSize = header.HeaderSize + (4 * (int)Math.Pow(2, header.BitsPerPixel)) + rowSize + header.Height;

                header.ImageSize = (int)fileSize;
            }

            if (header.ColorCount == 0)
            {
                header.ColorCount = (1 << header.BitsPerPixel);
            }

            // Return results
            return header;
        }

        #endregion Methods
    }
}