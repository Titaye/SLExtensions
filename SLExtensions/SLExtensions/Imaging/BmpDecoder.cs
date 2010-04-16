namespace SLExtensions.Imaging
{
    using System;
    using System.IO;
    using System.Net;
    using System.Windows;
    using System.Windows.Browser;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    /// <summary>
    /// Bitmap decoder for decoding bitmaps into instances of EditableImage.
    /// </summary>
    public class BmpDecoder
    {
        #region Fields

        private const int _BLUEMASK = 0x001F;
        private const int _GREENMASK = 0x07E0;

        /// Original EditableImage and PngEncoder classes courtesy Joe Stegman.
        /// http://blogs.msdn.com/jstegman
        private const int _REDMASK = 0xF800;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Decodes a bitmap stream into an editable image.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static EditableImage Decode(Stream stream)
        {
            EditableImage image = null;

            // See http://en.wikipedia.org/wiki/BMP_file_format

            try
            {
                byte[] buffer;
                BmpFileHeader fHeader;
                BmpInfoHeader iHeader;

                // Read File Header
                fHeader = BmpFileHeader.FillFromStream(stream);

                // Validate file type
                if (fHeader.BitmapType != 19778)
                {
                    throw new Exception("Invalid BMP file");
                }

                // Read Info Header
                iHeader = BmpInfoHeader.FillFromStream(stream);

                // Data
                if ((iHeader.Compression == 0) && (iHeader.BitsPerPixel == 24))
                {
                    // Read bits into the buffer
                    buffer = new byte[iHeader.ImageSize];
                    stream.Read(buffer, 0, iHeader.ImageSize);

                    // Standard RGB file
                    image = Read24BitBmp(buffer, iHeader);
                }
                else if ((iHeader.Compression == 0) && (iHeader.BitsPerPixel <= 8))
                {
                    int count = iHeader.ColorCount * 4;       // 4 bytes per color
                    Color[] palette;

                    // Read colors
                    buffer = new byte[count];
                    stream.Read(buffer, 0, count);

                    palette = FillColorPalette(buffer, iHeader.ColorCount);

                    // Read data
                    buffer = new byte[iHeader.ImageSize];
                    stream.Read(buffer, 0, iHeader.ImageSize);

                    image = ReadPaletteBmp(buffer, palette, iHeader, iHeader.BitsPerPixel);
                }
                else if ((iHeader.Compression == 3) && (iHeader.BitsPerPixel == 16))
                {
                    // Special 565 16 bit encoding - see http://www.virtualdub.org/blog/pivot/entry.php?id=177
                    int remainder = fHeader.OffsetToData - (int)stream.Position;
                    int rMask;
                    int bMask;
                    int gMask;

                    // Read remainder
                    buffer = new byte[remainder];
                    stream.Read(buffer, 0, remainder);

                    // Read masks
                    rMask = BitConverter.ToInt32(buffer, 0);
                    gMask = BitConverter.ToInt32(buffer, 4);
                    bMask = BitConverter.ToInt32(buffer, 8);

                    if ((_REDMASK != rMask) || (_GREENMASK != gMask) || (_BLUEMASK != bMask))
                    {
                        // Not 565 format
                        throw new Exception("Unsupported 16 bit format: " + rMask.ToString("X2") + ", " + bMask.ToString("X2") + ", " + gMask.ToString("X2"));
                    }

                    // Read data
                    remainder = iHeader.Height * iHeader.Width * 2;     // 2 bytes per pixel
                    buffer = new byte[remainder];
                    stream.Read(buffer, 0, remainder);

                    // Convert to an image
                    image = Read565Bmp(buffer, iHeader);
                }
                else
                {
                    throw new Exception("Unsupported format (compression: " + iHeader.Compression.ToString() + ", Bits per pixel: " + iHeader.BitsPerPixel.ToString() + ")");
                }
            }
            catch (Exception ex)
            {
                HtmlPage.Window.Alert("Error parsing BMP file: " + ex.Message);
            }

            return image;
        }

        private static Color[] FillColorPalette(byte[] buffer, int count)
        {
            Color[] colors = new Color[count];
            int baseIdx;
            byte alpha;

            for (int idx = 0; idx < count; idx++)
            {
                baseIdx = idx * 4;
                alpha = buffer[baseIdx + 3];
                colors[idx] = Color.FromArgb(((alpha == 0) ? (byte)255 : alpha), buffer[baseIdx + 2], buffer[baseIdx + 1], buffer[baseIdx]);
            }

            return colors;
        }

        private static EditableImage Read24BitBmp(byte[] buffer, BmpInfoHeader header)
        {
            int alignment = (header.Width * 3) % 4;       // Rows are aligned on 4 byte boundaries
            int rowbase = 0;
            int offset;
            int realRow;

            EditableImage image = new EditableImage(header.Width, header.Height);

            if (alignment != 0)
            {
                alignment = 4 - alignment;                        // Calculate row padding
            }

            for (int row = 0; row < header.Height; row++)
            {
                rowbase = (row * ((header.Width * 3) + alignment));
                for (int col = 0; col < header.Width; col++)
                {
                    offset = rowbase + (col * 3);
                    realRow = header.Height - row - 1;          // Reverse row
                    if (offset >= buffer.Length)
                    {
                        HtmlPage.Window.Alert("Error - outside of bounds and not sure why");
                    }
                    image.SetPixel(col, realRow, buffer[offset + 2], buffer[offset + 1], buffer[offset], 255);
                }
            }

            return image;
        }

        private static EditableImage Read4BitBmp(byte[] buffer, Color[] palette, BmpInfoHeader header)
        {
            int width = (header.Width + 1) / 2;
            int alignment = width % 4;       // Rows are aligned on 4 byte boundaries
            int rowbase = 0;
            int colbase = 0;
            int offset;
            int realRow;
            Color color1;
            Color color2;

            EditableImage image = new EditableImage(header.Width, header.Height);

            if (alignment != 0)
            {
                alignment = 4 - alignment;                        // Calculate row padding
            }

            for (int row = 0; row < header.Height; row++)
            {
                rowbase = (row * (width + alignment));
                for (int col = 0; col < width; col++)
                {
                    colbase = col * 2;
                    offset = rowbase + col;
                    realRow = header.Height - row - 1;          // Reverse row
                    color1 = palette[(buffer[offset]) >> 4];
                    color2 = palette[(buffer[offset]) & 0x0F];
                    image.SetPixel(colbase, realRow, color1.R, color1.G, color1.B, 255);
                    image.SetPixel(colbase + 1, realRow, color2.R, color2.G, color2.B, 255);
                }
            }

            return image;
        }

        private static EditableImage Read565Bmp(byte[] buffer, BmpInfoHeader header)
        {
            int rowbase = 0;
            int offset;
            int realRow;
            short color;
            byte red;
            byte green;
            byte blue;
            int scaleR = 256 / 32;
            int scaleG = 256 / 64;

            EditableImage image = new EditableImage(header.Width, header.Height);

            for (int row = 0; row < header.Height; row++)
            {
                rowbase = (row * header.Width * 2);
                for (int col = 0; col < header.Width; col++)
                {
                    offset = rowbase + (col * 2);
                    realRow = header.Height - row - 1;          // Reverse row

                    // Get color and convert
                    color = BitConverter.ToInt16(buffer, offset);
                    red = (byte)(((color & _REDMASK) >> 11) * scaleR);
                    green = (byte)(((color & _GREENMASK) >> 5) * scaleG);
                    blue = (byte)(((color & _BLUEMASK)) * scaleR);

                    // Set pixel
                    image.SetPixel(col, realRow, red, green, blue, 255);
                }
            }

            return image;
        }

        private static EditableImage Read8BitBmp(byte[] buffer, Color[] palette, BmpInfoHeader header)
        {
            int alignment = header.Width % 4;       // Rows are aligned on 4 byte boundaries
            int rowbase = 0;
            int offset;
            int realRow;
            Color color;

            EditableImage image = new EditableImage(header.Width, header.Height);

            if (alignment != 0)
            {
                alignment = 4 - alignment;                        // Calculate row padding
            }

            for (int row = 0; row < header.Height; row++)
            {
                rowbase = (row * (header.Width + alignment));
                for (int col = 0; col < header.Width; col++)
                {
                    offset = rowbase + col;
                    realRow = header.Height - row - 1;          // Reverse row
                    color = palette[buffer[offset]];
                    image.SetPixel(col, realRow, color.R, color.G, color.B, color.A);
                }
            }

            return image;
        }

        private static EditableImage ReadPaletteBmp(byte[] buffer, Color[] palette, BmpInfoHeader header, int bpp)
        {
            int ppb = 8 / bpp;                    // Pixels per byte (bits per pixel)
            int width = (header.Width + ppb - 1) / ppb;
            int alignment = width % 4;          // Rows are aligned on 4 byte boundaries
            int mask = (0xFF >> (8 - bpp));       // Bit mask
            int rowbase;
            int colbase;
            int offset;
            int realRow;
            Color color;

            EditableImage image = new EditableImage(header.Width, header.Height);

            if (alignment != 0)
            {
                alignment = 4 - alignment;                        // Calculate row padding
            }

            for (int row = 0; row < header.Height; row++)
            {
                rowbase = (row * (width + alignment));
                for (int col = 0; col < width; col++)
                {
                    offset = rowbase + col;
                    colbase = col * ppb;
                    realRow = header.Height - row - 1;                  // Reverse row
                    for (int shift = 0; ((shift < ppb) && ((colbase + shift) < header.Width)); shift++)
                    {
                        color = palette[((buffer[offset]) >> (8 - bpp - (shift * bpp))) & mask];
                        image.SetPixel(colbase + shift, realRow, color.R, color.G, color.B, 255);
                    }
                }
            }

            return image;
        }

        #endregion Methods
    }
}