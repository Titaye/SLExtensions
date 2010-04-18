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
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;

    /// <summary>
    /// Creates an class for building dynamic images.
    /// </summary>
    public class EditableImage
    {
        #region Fields

        private byte[] _buffer;
        private int _height = 0;
        private bool _init = false;
        private int _rowLength;

        /// Original EditableImage and PngEncoder classes courtesy Joe Stegman.
        /// http://blogs.msdn.com/jstegman
        private int _width = 0;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Creates a new EditableImage with the specified witdth and height.
        /// </summary>
        /// <param name="width">Specifies the width of the image.  The width must not exceed 3000 pixels.</param>
        /// <param name="height">Specifies the height of the image.  The height must not exceed 3000 pixels.</param>
        public EditableImage(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// Creates a new EditableImage with the specified witdth and height.
        /// </summary>
        /// <param name="width">Specifies the width of the image.  The width must not exceed 3000 pixels.</param>
        /// <param name="height">Specifies the height of the image.  The height must not exceed 3000 pixels.</param>
        public EditableImage(WriteableBitmap wb)
        {
            this.Width = wb.PixelWidth;
            this.Height = wb.PixelHeight;
            Initialize();
            for (int y = 0; y < wb.PixelHeight; y++)
            {
                for (int x = 0; x < wb.PixelWidth; x++)
                {
                    int pix = wb.Pixels[x + (y * wb.PixelWidth)];
                    byte a = (byte)((pix >> 24) & 0xff);
                    byte r = (byte)((pix >> 16) & 0xff);
                    byte g = (byte)((pix >> 8) & 0xff);
                    byte b = (byte)(pix & 0xff);

                    SetPixel(x, y, r, g, b, a);
                }
            }
        }

        #endregion Constructors

        #region Events

        /// <summary>
        /// Event fired when the editable image encounters an error condition.
        /// </summary>
        public event EventHandler<EditableImageErrorEventArgs> ImageError;

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets or sets the height of the image.
        /// </summary>
        public int Height
        {
            get
            {
                return _height;
            }
            set
            {
                if (_init)
                {
                    OnImageError("Error: Cannot change Height after the EditableImage has been initialized");
                }
                else if ((value <= 0) || (value > 3000))
                {
                    OnImageError("Error: Height must be between 0 and 3000");
                }
                else
                {
                    _height = value;
                }
            }
        }

        /// <summary>
        /// Gets a boolean value indicating if the image is initialized.
        /// </summary>
        public bool Initialized
        {
            get { return _init; }
        }

        /// <summary>
        /// Gets or sets the width of the image.
        /// </summary>
        public int Width
        {
            get
            {
                return _width;
            }
            set
            {
                if (_init)
                {
                    OnImageError("Error: Cannot change Width after the EditableImage has been initialized");
                }
                else if ((value <= 0) || (value > 3000))
                {
                    OnImageError("Error: Width must be between 0 and 3000");
                }
                else
                {
                    _width = value;
                }
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Gets the color in the desired row/column pair.
        /// </summary>
        /// <param name="col">The column for which to get the pixel.</param>
        /// <param name="row">The row for which to get the pixel.</param>
        /// <returns>Returns a color for the specified column/row pair.</returns>
        public Color GetPixel(int col, int row)
        {
            if ((col > _width) || (col < 0))
            {
                OnImageError("Error: Column must be greater than 0 and less than the Width");
            }
            else if ((row > _height) || (row < 0))
            {
                OnImageError("Error: Row must be greater than 0 and less than the Height");
            }

            Color color = new Color();
            int start = _rowLength * row + col * 4 + 1;   // +1 is for the filter byte

            color.R = _buffer[start];
            color.G = _buffer[start + 1];
            color.B = _buffer[start + 2];
            color.A = _buffer[start + 3];

            return color;
        }

        /// <summary>
        /// Gets a PNG encoded stream from the image data.
        /// </summary>
        /// <returns>Returns a PNG encoded stream.</returns>
        public MemoryStream GetStream()
        {
            MemoryStream stream;

            if (!_init)
            {
                OnImageError("Error: Image has not been initialized");
                stream = null;
            }
            else
            {
                stream = PngEncoder.Encode(_buffer, _width, _height);
            }

            return stream;
        }

        /// <summary>
        /// Sets a pixel in the desired row/column pair with the specified color.
        /// </summary>
        /// <param name="col">The column for which to set the pixel.</param>
        /// <param name="row">The row for which to set the pixel.</param>
        /// <param name="color">The color to set for the row/column pair.</param>
        public void SetPixel(int col, int row, Color color)
        {
            SetPixel(col, row, color.R, color.G, color.B, color.A);
        }

        /// <summary>
        /// Sets a pixel in the desired row/column pair with the specified color.
        /// </summary>
        /// <param name="col">The column for which to set the pixel.</param>
        /// <param name="row">The row for which to set the pixel.</param>
        /// <param name="red">The red value.</param>
        /// <param name="green">The green value.</param>
        /// <param name="blue">The blue value.</param>
        /// <param name="alpha">The alpha transparency value.</param>
        public void SetPixel(int col, int row, byte red, byte green, byte blue, byte alpha)
        {
            if (!_init)
            {
                Initialize();
            }

            if ((col > _width) || (col < 0))
            {
                OnImageError("Error: Column must be greater than 0 and less than the Width");
            }
            else if ((row > _height) || (row < 0))
            {
                OnImageError("Error: Row must be greater than 0 and less than the Height");
            }

            // Set the pixel
            int start = _rowLength * row + col * 4 + 1; // +1 is for the filter byte
            _buffer[start] = red;
            _buffer[start + 1] = green;
            _buffer[start + 2] = blue;
            _buffer[start + 3] = alpha;
        }

        private void Initialize()
        {
            _rowLength = _width * 4 + 1;
            _buffer = new byte[_rowLength * _height];

            // Initialize
            for (int idx = 0; idx < _height; idx++)
            {
                _buffer[idx * _rowLength] = 0;      // Filter bit
            }

            _init = true;
        }

        private void OnImageError(string msg)
        {
            if (null != ImageError)
            {
                EditableImageErrorEventArgs args = new EditableImageErrorEventArgs();
                args.ErrorMessage = msg;
                ImageError(this, args);
            }
        }

        #endregion Methods
    }
}