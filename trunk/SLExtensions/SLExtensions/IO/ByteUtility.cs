using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SLExtensions;

namespace SLExtensions.IO
{
    /// <summary>
    /// Provides various byte utility methods.
    /// </summary>
    public static class ByteUtility
    {
        /// <summary>
        /// Validates the specified array.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="index">The index.</param>
        public static void ValidateArray(byte[] data, int index)
        {
            if (data == null) {
                throw new ArgumentNullException("data");
            }
            if (index < 0 || index >= data.Length) {
                throw new ArgumentOutOfRangeException("index");
            }
        }

        /// <summary>
        /// Validates the specified array.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        public static void ValidateArray(byte[] data, int index, int count)
        {
            ValidateArray(data, index);

            if (count < 0 || index + count > data.Length) {
                throw new ArgumentOutOfRangeException("count");
            }
        }

        /// <summary>
        /// Determines whether the specified stream contains the given data at the current position.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="data">The data.</param>
        /// <returns>
        /// 	<c>true</c> if the specified stream contains data; otherwise, <c>false</c>.
        /// </returns>
        public static bool ContainsData(Stream stream, int[] data)
        {
            byte[] streamData;

            if (ByteUtility.TryRead(stream, data.Length, out streamData)) {
                return ContainsData(streamData, data, 0);
            }
            return false;
        }

        /// <summary>
        /// Determines whether the specified input contains data.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="data">The data.</param>
        /// <param name="index">The index.</param>
        /// <returns>
        /// 	<c>true</c> if the specified input contains data; otherwise, <c>false</c>.
        /// </returns>
        public static bool ContainsData(byte[] input, int[] data, int index)
        {
            if (input == null) {
                throw new ArgumentNullException("input");
            }
            if (data == null) {
                throw new ArgumentNullException("data");
            }
            if (index + data.Length <= input.Length) {
                for (int i = 0; i < data.Length; i++) {
                    if (input[index + i] != data[i]) {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Reads the specified number of bytes from specified stream and return them.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        /// <exception cref="IOException">End of stream reached</exception>
        public static byte[] Read(Stream stream, int count)
        {
            byte[] data;
            if (!TryRead(stream, count, out data)) {
                throw new IOException(Resource.ExceptionEndOfStream);
            }

            return data;
        }

        /// <summary>
        /// Tries seeking to specified offset.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="origin">The origin.</param>
        /// <returns></returns>
        public static bool TrySeek(Stream stream, long offset, SeekOrigin origin)
        {
            if (stream == null) {
                throw new ArgumentNullException("stream");
            }

            try {
                stream.Seek(offset, origin);
                return true;
            }
            catch (IOException) {
                return false;
            }
        }

        /// <summary>
        /// Try reading the specified number of bytes from specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="count">The count.</param>
        /// <param name="data">The data.</param>
        /// <returns>False if the specified number of bytes could not be read</returns>
        public static bool TryRead(Stream stream, int count, out byte[] data)
        {
            if (stream == null) {
                throw new ArgumentNullException("stream");
            }

            data = new byte[count];
            return stream.Read(data, 0, count) == count;
        }

        /// <summary>
        /// Reads a byte from specified stream and return it.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        /// <exception cref="IOException">End of stream reached</exception>
        public static byte ReadByte(Stream stream)
        {
            if (stream == null) {
                throw new ArgumentNullException("stream");
            }

            int b = stream.ReadByte();
            if (b == -1) {
                throw new IOException(Resource.ExceptionEndOfStream);
            }
            return (byte)b;
        }

        /// <summary>
        /// Reads a signed short (2 bytes) from given array using specified byte order.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="index">The index.</param>
        /// <param name="order">The byte order.</param>
        /// <returns></returns>
        public static short ReadInt16(byte[] data, int index, ByteOrder order)
        {
            ValidateArray(data, index, 2);

            if (order == ByteOrder.BigEndian) {
                return (short)(data[index] * 0x100 + data[checked(index + 1)]);
            }
            else if (order == ByteOrder.LittleEndian) {
                return (short)(data[index] + data[checked(index + 1)] * 0x100);
            }
            else{
                throw new ArgumentOutOfRangeException("order");
            }
        }

        /// <summary>
        /// Reads a signed short (2 bytes) from given array using specified byte order.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="index">The index.</param>
        /// <param name="order">The byte order.</param>
        /// <returns></returns>
        public static int ReadUInt16(byte[] data, int index, ByteOrder order)
        {
            ValidateArray(data, index, 2);

            if (order == ByteOrder.BigEndian) {
                return (ushort)(data[index] * 0x100 + data[checked(index + 1)]);
            }
            else if (order == ByteOrder.LittleEndian) {
                return (ushort)(data[index] + data[checked(index + 1)] * 0x100);
            }
            else{
                throw new ArgumentOutOfRangeException("order");
            }
        }

        /// <summary>
        /// Reads a signed integer (4 bytes) from given array using specified byte order.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="index">The index.</param>
        /// <param name="order">The byte order.</param>
        /// <returns></returns>
        public static int ReadInt32(byte[] data, int index, ByteOrder order)
        {
            ValidateArray(data, index, 4);

            if (order == ByteOrder.BigEndian) {
                return (int)(data[index] * 0x1000000 + data[checked(index + 1)] * 0x10000 + data[checked(index + 2)] * 0x100 + data[checked(index + 3)]);
            }
            else if (order == ByteOrder.LittleEndian) {
                return (int)(data[index] + data[checked(index + 1)] * 0x100 + data[checked(index + 2)] * 0x10000 + data[checked(index + 3)] * 0x1000000);
            }
            else{
                throw new ArgumentOutOfRangeException("order");
            }
        }

        /// <summary>
        /// Reads an unsigned integer (4 bytes) from given array using specified byte order.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="index">The index.</param>
        /// <param name="order">The byte order.</param>
        /// <returns></returns>
        public static long ReadUInt32(byte[] data, int index, ByteOrder order)
        {
            ValidateArray(data, index, 4);

            if (order == ByteOrder.BigEndian) {
                return (uint)(data[index] * 0x1000000 + data[checked(index + 1)] * 0x10000 + data[checked(index + 2)] * 0x100 + data[checked(index + 3)]);
            }
            else if (order == ByteOrder.LittleEndian) {
                return (uint)(data[index] + data[checked(index + 1)] * 0x100 + data[checked(index + 2)] * 0x10000 + data[checked(index + 3)] * 0x1000000);
            }
            else {
                throw new ArgumentOutOfRangeException("order");
            }
        }

        /// <summary>
        /// Reads a signed 64 bit integer (8 bytes) from given array using specified byte order.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="index">The index.</param>
        /// <param name="order">The byte order.</param>
        /// <returns></returns>
        public static long ReadInt64(byte[] data, int index, ByteOrder order)
        {
            ValidateArray(data, index, 8);

            if (order == ByteOrder.BigEndian) {
                return (long)(data[index] * 0x100000000000000 + data[checked(index + 1)] * 0x1000000000000 + data[checked(index + 2)] * 0x10000000000 + data[checked(index + 3)] * 0x100000000 + data[checked(index + 4)] * 0x1000000 + data[checked(index + 5)] * 0x10000 + data[checked(index + 6)] * 0x100 + data[checked(index + 7)]);
            }
            else if (order == ByteOrder.LittleEndian) {
                return (long)(data[index] + data[checked(index + 1)] * 0x100 + data[checked(index + 2)] * 0x10000 + data[checked(index + 3)] * 0x1000000 + data[checked(index + 4)] * 0x100000000 + data[checked(index + 5)] * 0x10000000000 + data[checked(index + 6)] * 0x1000000000000 + data[checked(index + 7)] * 0x100000000000000);
            }
            else {
                throw new ArgumentOutOfRangeException("order");
            }
        }

        /// <summary>
        /// Reads a float value.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="index">The index.</param>
        /// <param name="order">The order.</param>
        /// <returns></returns>
        public static float ReadFloat(byte[] data, int index, ByteOrder order)
        {
            ValidateArray(data, index);

            if (order == ByteOrder.BigEndian) {
                throw new NotImplementedException();
            }
            else if (order == ByteOrder.LittleEndian) {
                throw new NotImplementedException();
            }
            else {
                throw new ArgumentOutOfRangeException("order");
            }
        }

        /// <summary>
        /// Reads a double value.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="index">The index.</param>
        /// <param name="order">The order.</param>
        /// <returns></returns>
        public static double ReadDouble(byte[] data, int index, ByteOrder order)
        {
            ValidateArray(data, index);

            if (order == ByteOrder.BigEndian) {
                throw new NotImplementedException();
            }
            else if (order == ByteOrder.LittleEndian) {
                throw new NotImplementedException();
            }
            else {
                throw new ArgumentOutOfRangeException("order");
            }
        }

        /// <summary>
        /// Reads a string using specified encoding.
        /// </summary>
        /// <param name="encoding">The encoding.</param>
        /// <param name="data">The data.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public static string ReadString(Encoding encoding, byte[] data, int index)
        {
            return ReadString(encoding, data, index, data.Length - index);
        }

        /// <summary>
        /// Reads a string using specified encoding.
        /// </summary>
        /// <param name="encoding">The encoding.</param>
        /// <param name="data">The data.</param>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        public static string ReadString(Encoding encoding, byte[] data, int index, int count)
        {
            if (encoding == null) {
                throw new ArgumentNullException("encoding");
            }
            ValidateArray(data, index, count);

            return encoding.GetString(data, index, count);
        }

        /// <summary>
        /// Reads a terminated string using specified encoding.
        /// </summary>
        /// <param name="encoding">The encoding.</param>
        /// <param name="terminator">The terminator.</param>
        /// <param name="data">The data.</param>
        /// <param name="index">The index.</param>
        /// <param name="endIndex">The end index.</param>
        /// <returns></returns>
        public static string ReadTerminatedString(Encoding encoding, byte terminator, byte[] data, int index, out int endIndex)
        {
            return ReadTerminatedString(encoding, terminator, data, index, int.MaxValue, out endIndex);
        }

        /// <summary>
        /// Reads a terminated string using specified encoding.
        /// </summary>
        /// <param name="encoding">The encoding.</param>
        /// <param name="terminator">The terminator.</param>
        /// <param name="data">The data.</param>
        /// <param name="index">The index.</param>
        /// <param name="maxCount">The max count of the string to read.</param>
        /// <param name="endIndex">The end index.</param>
        /// <returns></returns>
        /// <remarks>
        /// Warning: single byte terminators only work correcly for single byte encodings!
        /// </remarks>
        public static string ReadTerminatedString(Encoding encoding, byte terminator, byte[] data, int index, int maxCount, out int endIndex)
        {
            if (encoding == null) {
                throw new ArgumentNullException("encoding");
            }
            if (maxCount < 0) {
                throw new ArgumentOutOfRangeException("maxCount");
            }
            ValidateArray(data, index);

            int i = index;
            while ((i < data.Length) && (i - index < maxCount) && (data[i] != terminator)) {
                i++;
            }

            endIndex = i;

            return ReadString(encoding, data, index, i - index);
        }

        /// <summary>
        /// Reads all terminated strings using specified encoding from the given data.
        /// </summary>
        /// <param name="encoding">The encoding.</param>
        /// <param name="terminator">The terminator.</param>
        /// <param name="data">The data.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public static string[] ReadTerminatedStrings(Encoding encoding, char terminator, byte[] data, int index)
        {
            return ReadTerminatedStrings(encoding, terminator, data, index, int.MaxValue);
        }

        /// <summary>
        /// Reads all \0 terminated strings using specified encoding from the given data.
        /// </summary>
        /// <param name="encoding">The encoding.</param>
        /// <param name="terminator">The terminator.</param>
        /// <param name="data">The data.</param>
        /// <param name="index">The index.</param>
        /// <param name="maxCount">The max number of bytes to read.</param>
        /// <returns></returns>
        public static string[] ReadTerminatedStrings(Encoding encoding, char terminator, byte[] data, int index, int maxCount)
        {
            if (encoding == null) {
                throw new ArgumentNullException("encoding");
            }
            if (maxCount < 0) {
                throw new ArgumentOutOfRangeException("maxCount");
            }
            ValidateArray(data, index);

            // read characters not bytes!
            char[] chars = encoding.GetChars(data, index, Math.Min(data.Length - index, maxCount));
            List<string> strings = new List<string>();

            int position = 0;
            for (int i = 0; i < chars.Length; i++) {
                if (chars[i] == terminator) {
                    strings.Add(new string(chars, position, i - position));
                    position = i + 1;
                }
                else if (i == chars.Length - 1) {
                    strings.Add(new string(chars, position, i + 1 - position));
                }
            }

            return strings.ToArray();
        }

        /// <summary>
        /// Reads the stream until a byte not equal to 0 is read.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public static void SkipNull(Stream stream)
        {
            if (stream == null) {
                throw new ArgumentNullException("stream");
            }
            while (stream.ReadByte() == 0) {
            }
            stream.Seek(-1, SeekOrigin.Current);
        }

        /// <summary>
        /// Reads an deflated string.
        /// </summary>
        /// <param name="encoding">The encoding.</param>
        /// <param name="data">The data.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public static string ReadDeflatedString(Encoding encoding, byte[] data, int index)
        {
            if (encoding == null) {
                throw new ArgumentNullException("encoding");
            }

            throw new NotSupportedException();

            //index = checked(index + 2);    // skip Compression flags code and Additional flags bits (2 bytes)

            //ValidateArray(data, index);

            //try {
            //    using (MemoryStream mStream = new MemoryStream(data, index, data.Length - index)) {
            //        using (DeflateStream dStream = new DeflateStream(mStream, CompressionMode.Decompress)) {
            //            using (MemoryStream stream = new MemoryStream()) {
            //                byte[] buffer = new byte[256];

            //                while (true) {
            //                    int bytesRead = dStream.Read(buffer, 0, buffer.Length);
            //                    if (bytesRead != 0) {
            //                        stream.Write(buffer, 0, bytesRead);
            //                    }
            //                    else {
            //                        return encoding.GetString(stream.ToArray());
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
            //catch (InvalidDataException e) {
            //    throw new MetadataReadException(Properties.Resources.ExceptionDecompressFailed, e);
            //}
        }

        /// <summary>
        /// Truncates the specified string value.
        /// </summary>
        /// <param name="encoding">The encoding.</param>
        /// <param name="value">The value.</param>
        /// <param name="maxByteCount">The max byte count.</param>
        /// <returns></returns>
        /// <remarks>
        /// This methods might return giberish for multibyte encodings.
        /// </remarks>
        public static string Truncate(Encoding encoding, string value, int maxByteCount)
        {
            if (encoding == null) {
                throw new ArgumentNullException("encoding");
            }
            if (value == null) {
                throw new ArgumentNullException("value");
            }
            if (maxByteCount < 0) {
                throw new ArgumentOutOfRangeException("maxByteCount");
            }

			byte[] data = encoding.GetBytes(value);

            return encoding.GetString(data, 0, Math.Min(data.Length, maxByteCount));
        }

        /// <summary>
        /// Gets the number of bytes for specified string in given encoding.
        /// </summary>
        /// <param name="encoding">The encoding.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static int GetByteCount(Encoding encoding, string value)
        {
            if (encoding == null) {
                throw new ArgumentNullException("encoding");
            }
            if (value == null) {
                throw new ArgumentNullException("value");
            }

            return encoding.GetByteCount(value);
        }
    }
}
