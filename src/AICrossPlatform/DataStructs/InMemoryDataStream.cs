using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace AI.DataStructs
{
    /// <summary>
    /// Class for simple IO operations
    /// </summary>
    [Serializable]
    [DebuggerDisplay("Length = {_data.Length}")]
    public class InMemoryDataStream
    {
        #region Поля и свойства
        private byte[] _data;
        private int _position = 0;

        /// <summary>
        /// Tells if data in the stream is zipped
        /// </summary>
        public bool IsZipped { get; private set; } = false;
        /// <summary>
        /// Tells if data in the stream is encrypted
        /// </summary>
        public bool IsEncrypted { get; private set; } = false;
        /// <summary>
        /// Tells if stream is opened for reading
        /// </summary>
        public bool IsForReading { get; private set; }
        /// <summary>
        /// Tells if stream is opened for writing
        /// </summary>
        public bool IsForWriting { get; private set; }

        /// <summary>
        /// AES algorithm initialization vector
        /// </summary>
        public static byte[] IV { get; set; } = { 0, 32, 27, 12, 13, 91, 1, 141, 200, 210, 211, 212, 213, 214, 115, 16 };
        #endregion

        #region Конструкторы
        /// <summary>
        /// Creates DataStream for writing data
        /// </summary>
        public InMemoryDataStream()
        {
            IsForReading = false;
            IsForWriting = true;
            _data = new byte[0];
        }
        /// <summary>
        /// Creates DataStream for reading data from file
        /// </summary>
        /// <param name="path">Путь до файла</param>
        /// <param name="isEncrypted"></param>
        /// <param name="isZipped"></param>
        public InMemoryDataStream(string path, bool isEncrypted = false, bool isZipped = false)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("File does not exist", path);
            }

            IsEncrypted = isEncrypted;
            IsZipped = isZipped;
            IsForWriting = false;
            IsForReading = true;

            using FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            using MemoryStream ms = new MemoryStream();
            fs.CopyTo(ms);
            _data = ms.ToArray();
        }
        /// <summary>
        /// Creates DataStream for reading data from byte array
        /// </summary>
        /// <param name="data"></param>
        /// <param name="isEncrypted"></param>
        /// <param name="isZipped"></param>
        public InMemoryDataStream(byte[] data, bool isEncrypted = false, bool isZipped = false)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (data.Length == 0)
            {
                throw new ArgumentException("Data is empty", nameof(data));
            }

            IsEncrypted = isEncrypted;
            IsZipped = isZipped;
            IsForWriting = false;
            IsForReading = true;

            _data = data;
        }
        /// <summary>
        /// Creates DataStream for reading data from System.IO.Stream
        /// </summary>
        /// <param name="stream">Поток</param>
        /// <param name="isEncrypted"></param>
        /// <param name="isZipped"></param>
        public InMemoryDataStream(Stream stream, bool isEncrypted = false, bool isZipped = false)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            IsEncrypted = isEncrypted;
            IsZipped = isZipped;
            IsForWriting = false;
            IsForReading = true;

            using MemoryStream ms = new MemoryStream();
            stream.CopyTo(ms);
            _data = ms.ToArray();
        }
        #endregion

        #region Запись

        #region Числа
        /// <summary>
        /// Writes int to the stream
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public InMemoryDataStream Write(int n)
        {
            WriteInternal(BitConverter.GetBytes(n));
            return this;
        }
        /// <summary>
        /// Writes short to the stream
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public InMemoryDataStream Write(short n)
        {
            WriteInternal(BitConverter.GetBytes(n));
            return this;
        }
        /// <summary>
        /// Writes byte to the stream
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public InMemoryDataStream Write(byte n)
        {
            WriteInternal(new[] { n });
            return this;
        }
        /// <summary>
        /// Writes double to the stream
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public InMemoryDataStream Write(double n)
        {
            WriteInternal(BitConverter.GetBytes(n));
            return this;
        }
        /// <summary>
        /// Writes float to the stream
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public InMemoryDataStream Write(float n)
        {
            WriteInternal(BitConverter.GetBytes(n));
            return this;
        }
        /// <summary>
        /// Writes long to the stream
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public InMemoryDataStream Write(long n)
        {
            WriteInternal(BitConverter.GetBytes(n));
            return this;
        }
        #endregion

        #region Строки
        /// <summary>
        /// Writes string in utf-8 encoding to the stream
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public InMemoryDataStream Write(string str)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }

            _ = Write(str, Encoding.UTF8);
            return this;
        }
        /// <summary>
        /// Writes string in utf-8 encoding to the stream
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public InMemoryDataStream WriteOnlyContent(string str)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }

            _ = WriteOnlyContent(str, Encoding.UTF8);
            return this;
        }
        /// <summary>
        /// Writes string in custom encoding to the stream
        /// </summary>
        /// <param name="str"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public InMemoryDataStream Write(string str, Encoding encoding)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }

            if (encoding == null)
            {
                throw new ArgumentNullException(nameof(encoding));
            }

            byte[] arr = encoding.GetBytes(str);
            _ = Write(arr.Length);
            WriteInternal(arr);
            return this;
        }
        /// <summary>
        /// Writes string in custom encoding to the stream
        /// </summary>
        /// <param name="str"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public InMemoryDataStream WriteOnlyContent(string str, Encoding encoding)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }

            if (encoding == null)
            {
                throw new ArgumentNullException(nameof(encoding));
            }

            byte[] arr = encoding.GetBytes(str);
            WriteInternal(arr);
            return this;
        }
        #endregion

        #region Массивы
        /// <summary>
        /// Writes byte array to the stream
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public InMemoryDataStream Write(byte[] arr)
        {
            if (arr == null)
            {
                throw new ArgumentNullException(nameof(arr));
            }

            _ = Write(arr.Length);
            WriteInternal(arr);
            return this;
        }
        /// <summary>
        /// Writes double array to the stream
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public InMemoryDataStream Write(double[] arr)
        {
            if (arr == null)
            {
                throw new ArgumentNullException(nameof(arr));
            }

            _ = Write(arr.Length);
            _ = WriteOnlyContent(arr);
            return this;
        }
        /// <summary>
        /// Writes float array to the stream
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public InMemoryDataStream Write(float[] arr)
        {
            if (arr == null)
            {
                throw new ArgumentNullException(nameof(arr));
            }

            _ = Write(arr.Length);
            _ = WriteOnlyContent(arr);
            return this;
        }
        /// <summary>
        /// Writes int array to the stream
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public InMemoryDataStream Write(int[] arr)
        {
            if (arr == null)
            {
                throw new ArgumentNullException(nameof(arr));
            }

            _ = Write(arr.Length);
            _ = WriteOnlyContent(arr);
            return this;
        }
        /// <summary>
        /// Writes short array to the stream
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public InMemoryDataStream Write(short[] arr)
        {
            if (arr == null)
            {
                throw new ArgumentNullException(nameof(arr));
            }

            _ = Write(arr.Length);
            _ = WriteOnlyContent(arr);
            return this;
        }
        /// <summary>
        /// Writes long array to the stream
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public InMemoryDataStream Write(long[] arr)
        {
            if (arr == null)
            {
                throw new ArgumentNullException(nameof(arr));
            }

            _ = Write(arr.Length);
            _ = WriteOnlyContent(arr);
            return this;
        }
        /// <summary>
        /// Writes char array to the stream
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public InMemoryDataStream Write(char[] arr)
        {
            if (arr == null)
            {
                throw new ArgumentNullException(nameof(arr));
            }

            _ = Write(arr.Length);
            _ = WriteOnlyContent(arr);
            return this;
        }
        /// <summary>
        /// Writes double array content to the stream
        /// </summary>
        /// <param name="dat"></param>
        /// <returns></returns>
        public InMemoryDataStream WriteOnlyContent(double[] dat)
        {
            if (dat == null)
            {
                throw new ArgumentNullException(nameof(dat));
            }

            List<byte> btsL = new List<byte>(8 * dat.Length);

            for (int i = 0; i < dat.Length; i++)
            {
                btsL.AddRange(BitConverter.GetBytes(dat[i]));
            }

            WriteInternal(btsL.ToArray());

            return this;
        }
        /// <summary>
        /// Writes float array content to the stream
        /// </summary>
        /// <param name="dat"></param>
        /// <returns></returns>
        public InMemoryDataStream WriteOnlyContent(float[] dat)
        {
            if (dat == null)
            {
                throw new ArgumentNullException(nameof(dat));
            }

            List<byte> btsL = new List<byte>(4 * dat.Length);

            for (int i = 0; i < dat.Length; i++)
            {
                btsL.AddRange(BitConverter.GetBytes(dat[i]));
            }

            WriteInternal(btsL.ToArray());

            return this;
        }
        /// <summary>
        /// Writes int array content to the stream
        /// </summary>
        /// <param name="dat"></param>
        /// <returns></returns>
        public InMemoryDataStream WriteOnlyContent(int[] dat)
        {
            if (dat == null)
            {
                throw new ArgumentNullException(nameof(dat));
            }

            List<byte> btsL = new List<byte>(4 * dat.Length);

            for (int i = 0; i < dat.Length; i++)
            {
                btsL.AddRange(BitConverter.GetBytes(dat[i]));
            }

            WriteInternal(btsL.ToArray());

            return this;
        }
        /// <summary>
        /// Writes long array content to the stream
        /// </summary>
        /// <param name="dat"></param>
        /// <returns></returns>
        public InMemoryDataStream WriteOnlyContent(long[] dat)
        {
            if (dat == null)
            {
                throw new ArgumentNullException(nameof(dat));
            }

            List<byte> btsL = new List<byte>(8 * dat.Length);

            for (int i = 0; i < dat.Length; i++)
            {
                btsL.AddRange(BitConverter.GetBytes(dat[i]));
            }

            WriteInternal(btsL.ToArray());

            return this;
        }
        /// <summary>
        /// Writes short array content to the stream
        /// </summary>
        /// <param name="dat"></param>
        /// <returns></returns>
        public InMemoryDataStream WriteOnlyContent(short[] dat)
        {
            if (dat == null)
            {
                throw new ArgumentNullException(nameof(dat));
            }

            List<byte> btsL = new List<byte>(2 * dat.Length);

            for (int i = 0; i < dat.Length; i++)
            {
                btsL.AddRange(BitConverter.GetBytes(dat[i]));
            }

            WriteInternal(btsL.ToArray());

            return this;
        }
        /// <summary>
        /// Writes char array content to the stream
        /// </summary>
        /// <param name="dat"></param>
        /// <returns></returns>
        public InMemoryDataStream WriteOnlyContent(char[] dat)
        {
            if (dat == null)
            {
                throw new ArgumentNullException(nameof(dat));
            }

            List<byte> btsL = new List<byte>(2 * dat.Length);

            for (int i = 0; i < dat.Length; i++)
            {
                btsL.AddRange(BitConverter.GetBytes(dat[i]));
            }

            WriteInternal(btsL.ToArray());

            return this;
        }
        #endregion

        /// <summary>
        /// Writes char to the stream
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public InMemoryDataStream Write(char ch)
        {
            WriteInternal(BitConverter.GetBytes(ch));
            return this;
        }

        #endregion

        #region Чтение

        #region Числа
        /// <summary>
        /// Reads int from the stream
        /// </summary>
        /// <returns></returns>
        public int ReadInt()
        {
            return BitConverter.ToInt32(ReadInternal(sizeof(int)), 0);
        }
        /// <summary>
        /// Reads int from the stream
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public InMemoryDataStream ReadInt(out int result)
        {
            result = ReadInt();
            return this;
        }
        /// <summary>
        /// Reads long from the stream
        /// </summary>
        /// <returns></returns>
        public long ReadLong()
        {
            return BitConverter.ToInt64(ReadInternal(sizeof(long)), 0);
        }
        /// <summary>
        /// Reads long from the stream
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public InMemoryDataStream ReadLong(out long result)
        {
            result = ReadLong();
            return this;
        }
        /// <summary>
        /// Reads short from the stream
        /// </summary>
        /// <returns></returns>
        public short ReadShort()
        {
            return BitConverter.ToInt16(ReadInternal(sizeof(short)), 0);
        }
        /// <summary>
        /// Reads short from the stream
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public InMemoryDataStream ReadShort(out short result)
        {
            result = ReadShort();
            return this;
        }
        /// <summary>
        /// Reads byte from the stream
        /// </summary>
        /// <returns></returns>
        public byte ReadByte()
        {
            return ReadInternal(1)[0];
        }
        /// <summary>
        /// Reads byte from the stream
        /// </summary>
        /// <returns></returns>
        public InMemoryDataStream ReadByte(out byte result)
        {
            result = ReadByte();
            return this;
        }
        /// <summary>
        /// Reads double from the stream
        /// </summary>
        /// <returns></returns>
        public double ReadDouble()
        {
            return BitConverter.ToDouble(ReadInternal(sizeof(double)), 0);
        }
        /// <summary>
        /// Reads double from the stream
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public InMemoryDataStream ReadDouble(out double result)
        {
            result = ReadDouble();
            return this;
        }
        /// <summary>
        /// Reads float from the stream
        /// </summary>
        /// <returns></returns>
        public float ReadFloat()
        {
            return BitConverter.ToSingle(ReadInternal(sizeof(float)), 0);
        }
        /// <summary>
        /// Reads float from the stream
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public InMemoryDataStream ReadFloat(out float result)
        {
            result = ReadFloat();
            return this;
        }
        #endregion

        #region Строки
        /// <summary>
        /// Reads string in utf-8 encoding from the stream
        /// </summary>
        /// <returns></returns>
        public string ReadString()
        {
            return ReadString(Encoding.UTF8);
        }
        /// <summary>
        /// Reads string in utf-8 encoding from the stream
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public InMemoryDataStream ReadString(out string result)
        {
            result = ReadString();
            return this;
        }
        /// <summary>
        /// Reads string in custom encoding from the stream
        /// </summary>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public string ReadString(Encoding encoding)
        {
            int len = ReadInt();
            return encoding.GetString(ReadInternal(len));
        }
        /// <summary>
        /// Reads string in custom encoding from the stream
        /// </summary>
        /// <param name="result"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public InMemoryDataStream ReadString(out string result, Encoding encoding)
        {
            result = ReadString(encoding);
            return this;
        }
        #endregion

        #region Массивы
        /// <summary>
        /// Reads byte array from the stream
        /// </summary>
        /// <returns></returns>
        public byte[] ReadBytes()
        {
            return ReadInternal(ReadInt());
        }
        /// <summary>
        /// Reads byte array from the stream
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public InMemoryDataStream ReadBytes(out byte[] result)
        {
            result = ReadBytes();
            return this;
        }
        /// <summary>
        /// Reads double array from the stream
        /// </summary>
        /// <returns></returns>
        public double[] ReadDoubles()
        {
            return ReadDoubles(ReadInt());
        }
        /// <summary>
        /// Reads double array from the stream
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public InMemoryDataStream ReadDoubles(out double[] result)
        {
            result = ReadDoubles();
            return this;
        }
        /// <summary>
        /// Reads float array from the stream
        /// </summary>
        /// <returns></returns>
        public float[] ReadFloats()
        {
            return ReadFloats(ReadInt());
        }
        /// <summary>
        /// Reads float array from the stream
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public InMemoryDataStream ReadFloats(out float[] result)
        {
            result = ReadFloats();
            return this;
        }
        /// <summary>
        /// Reads int array from the stream
        /// </summary>
        /// <returns></returns>
        public int[] ReadInts()
        {
            return ReadInts(ReadInt());
        }
        /// <summary>
        /// Reads int array from the stream
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public InMemoryDataStream ReadInts(out int[] result)
        {
            result = ReadInts();
            return this;
        }
        /// <summary>
        /// Reads short array from the stream
        /// </summary>
        /// <returns></returns>
        public short[] ReadShorts()
        {
            return ReadShorts(ReadInt());
        }
        /// <summary>
        /// Reads short array from the stream
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public InMemoryDataStream ReadShorts(out short[] result)
        {
            result = ReadShorts();
            return this;
        }
        /// <summary>
        /// Reads long array from the stream
        /// </summary>
        /// <returns></returns>
        public long[] ReadLongs()
        {
            return ReadLongs(ReadInt());
        }
        /// <summary>
        /// Reads long array from the stream
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public InMemoryDataStream ReadLongs(out long[] result)
        {
            result = ReadLongs();
            return this;
        }
        /// <summary>
        /// Reads char array from the stream
        /// </summary>
        /// <returns></returns>
        public char[] ReadChars()
        {
            return ReadChars(ReadInt());
        }
        /// <summary>
        /// Reads char array from the stream
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public InMemoryDataStream ReadChars(out char[] result)
        {
            result = ReadChars();
            return this;
        }
        /// <summary>
        /// Reads double array of a given length from the stream
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public double[] ReadDoubles(int length)
        {
            if (length == 0)
            {
                return new double[0];
            }

            byte[] arr = ReadInternal(length * sizeof(double));
            double[] vect = new double[length];

            using (MemoryStream fs = new MemoryStream(arr, true))
            {
                BinaryReader br = new BinaryReader(fs);

                for (int i = 0; i < length; i++)
                {
                    vect[i] = br.ReadDouble();
                }
            }

            return vect;
        }
        /// <summary>
        /// Reads double array of a given length from the stream
        /// </summary>
        /// <param name="length"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public InMemoryDataStream ReadDoubles(int length, out double[] result)
        {
            result = ReadDoubles(length);
            return this;
        }
        /// <summary>
        /// Reads float array of a given length from the stream
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public float[] ReadFloats(int length)
        {
            if (length == 0)
            {
                return new float[0];
            }

            byte[] arr = ReadInternal(length * sizeof(float));
            float[] vect = new float[length];

            using (MemoryStream fs = new MemoryStream(arr, true))
            {
                BinaryReader br = new BinaryReader(fs);

                for (int i = 0; i < length; i++)
                {
                    vect[i] = br.ReadSingle();
                }

            }

            return vect;
        }
        /// <summary>
        /// Reads double array of a given length from the stream
        /// </summary>
        /// <param name="length"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public InMemoryDataStream ReadFloats(int length, out float[] result)
        {
            result = ReadFloats(length);
            return this;
        }
        /// <summary>
        /// Reads int array of a given length from the stream
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public int[] ReadInts(int length)
        {
            if (length == 0)
            {
                return new int[0];
            }

            byte[] arr = ReadInternal(length * sizeof(int));
            int[] vect = new int[length];

            using (MemoryStream fs = new MemoryStream(arr, true))
            {
                BinaryReader br = new BinaryReader(fs);

                for (int i = 0; i < length; i++)
                {
                    vect[i] = br.ReadInt32();
                }

            }

            return vect;
        }
        /// <summary>
        /// Reads int array of a given length from the stream
        /// </summary>
        /// <param name="length"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public InMemoryDataStream ReadInts(int length, out int[] result)
        {
            result = ReadInts(length);
            return this;
        }
        /// <summary>
        /// Reads long array of a given length from the stream
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public long[] ReadLongs(int length)
        {
            if (length == 0)
            {
                return new long[0];
            }

            byte[] arr = ReadInternal(length * sizeof(long));
            long[] vect = new long[length];

            using (MemoryStream fs = new MemoryStream(arr, true))
            {
                BinaryReader br = new BinaryReader(fs);

                for (int i = 0; i < length; i++)
                {
                    vect[i] = br.ReadInt64();
                }

            }

            return vect;
        }
        /// <summary>
        /// Reads long array of a given length from the stream
        /// </summary>
        /// <param name="length"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public InMemoryDataStream ReadLongs(int length, out long[] result)
        {
            result = ReadLongs(length);
            return this;
        }
        /// <summary>
        /// Reads short array of a given length from the stream
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public short[] ReadShorts(int length)
        {
            if (length == 0)
            {
                return new short[0];
            }

            byte[] arr = ReadInternal(length * sizeof(short));
            short[] vect = new short[length];

            using (MemoryStream fs = new MemoryStream(arr, true))
            {
                BinaryReader br = new BinaryReader(fs);

                for (int i = 0; i < length; i++)
                {
                    vect[i] = br.ReadInt16();
                }

            }

            return vect;
        }
        /// <summary>
        /// Reads short array of a given length from the stream
        /// </summary>
        /// <param name="length"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public InMemoryDataStream ReadShorts(int length, out short[] result)
        {
            result = ReadShorts(length);
            return this;
        }
        /// <summary>
        /// Reads char array of a given length from the stream
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public char[] ReadChars(int length)
        {
            if (length == 0)
            {
                return new char[0];
            }

            byte[] arr = ReadInternal(length * sizeof(char));
            char[] vect = new char[length];

            using (MemoryStream fs = new MemoryStream(arr, true))
            {
                BinaryReader br = new BinaryReader(fs, Encoding.Unicode);


                for (int i = 0; i < length; i++)
                {
                    vect[i] = br.ReadChar();
                }

            }

            return vect;
        }
        /// <summary>
        /// Reads char array of a given length from the stream
        /// </summary>
        /// <param name="length"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public InMemoryDataStream ReadChars(int length, out char[] result)
        {
            result = ReadChars(length);
            return this;
        }
        #endregion

        /// <summary>
        /// Reads char from the stream
        /// </summary>
        /// <returns></returns>
        public char ReadChar()
        {
            return BitConverter.ToChar(ReadInternal(sizeof(char)), 0);
        }
        /// <summary>
        /// Reads char from the stream
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public InMemoryDataStream ReadChar(out char result)
        {
            result = ReadChar();
            return this;
        }

        #endregion

        #region Попытки прочитать

        #region Числа
        /// <summary>
        /// Tries to read int from the stream. Returns if operation succeeded
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TryReadInt(out int result)
        {
            if (IsForWriting || IsZipped || IsEncrypted || _position >= _data.Length - 1 || _position + sizeof(int) > _data.Length)
            {
                result = default;
                return false;
            }

            int prevPos = _position;
            byte[] bytes = ReadInternal(sizeof(int));

            try
            {
                result = BitConverter.ToInt32(bytes, 0);
                return true;
            }
            catch
            {
                _position = prevPos;
                result = default;
                return false;
            }
        }
        /// <summary>
        /// Tries to read int from the stream
        /// </summary>
        /// <param name="result"></param>
        /// <param name="succeeded"></param>
        /// <returns></returns>
        public InMemoryDataStream TryReadInt(out int result, out bool succeeded)
        {
            succeeded = TryReadInt(out result);
            return this;
        }
        /// <summary>
        /// Tries to read long from the stream. Returns if operation succeeded
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TryReadLong(out long result)
        {
            if (IsForWriting || IsZipped || IsEncrypted || _position >= _data.Length - 1 || _position + sizeof(long) > _data.Length)
            {
                result = default;
                return false;
            }

            int prevPos = _position;
            byte[] bytes = ReadInternal(sizeof(long));

            try
            {
                result = BitConverter.ToInt64(bytes, 0);
                return true;
            }
            catch
            {
                _position = prevPos;
                result = default;
                return false;
            }
        }
        /// <summary>
        /// Tries to read long from the stream
        /// </summary>
        /// <param name="result"></param>
        /// <param name="succeeded"></param>
        /// <returns></returns>
        public InMemoryDataStream TryReadLong(out long result, out bool succeeded)
        {
            succeeded = TryReadLong(out result);
            return this;
        }
        /// <summary>
        /// Tries to read short from the stream. Returns if operation succeeded
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TryReadShort(out short result)
        {
            if (IsForWriting || IsZipped || IsEncrypted || _position >= _data.Length - 1 || _position + sizeof(short) > _data.Length)
            {
                result = default;
                return false;
            }

            int prevPos = _position;
            byte[] bytes = ReadInternal(sizeof(short));

            try
            {
                result = BitConverter.ToInt16(bytes, 0);
                return true;
            }
            catch
            {
                _position = prevPos;
                result = default;
                return false;
            }
        }
        /// <summary>
        /// Tries to read short from the stream
        /// </summary>
        /// <param name="result"></param>
        /// <param name="succeeded"></param>
        /// <returns></returns>
        public InMemoryDataStream TryReadShort(out short result, out bool succeeded)
        {
            succeeded = TryReadShort(out result);
            return this;
        }
        /// <summary>
        /// Tries to read byte from the stream. Returns if operation succeeded
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TryReadByte(out byte result)
        {
            if (IsForWriting || IsZipped || IsEncrypted || _position >= _data.Length - 1 || _position + sizeof(byte) > _data.Length)
            {
                result = default;
                return false;
            }

            byte[] bytes = ReadInternal(sizeof(byte));

            result = bytes[0];
            return true;
        }
        /// <summary>
        /// Tries to read byte from the stream
        /// </summary>
        /// <param name="result"></param>
        /// <param name="succeeded"></param>
        /// <returns></returns>
        public InMemoryDataStream TryReadByte(out byte result, out bool succeeded)
        {
            succeeded = TryReadByte(out result);
            return this;
        }
        /// <summary>
        /// Tries to read double from the stream. Returns if operation succeeded
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TryReadDouble(out double result)
        {
            if (IsForWriting || IsZipped || IsEncrypted || _position >= _data.Length - 1 || _position + sizeof(double) > _data.Length)
            {
                result = default;
                return false;
            }

            int prevPos = _position;
            byte[] bytes = ReadInternal(sizeof(double));

            try
            {
                result = BitConverter.ToDouble(bytes, 0);
                return true;
            }
            catch
            {
                _position = prevPos;
                result = default;
                return false;
            }
        }
        /// <summary>
        /// Tries to read double from the stream
        /// </summary>
        /// <param name="result"></param>
        /// <param name="succeeded"></param>
        /// <returns></returns>
        public InMemoryDataStream TryReadDouble(out double result, out bool succeeded)
        {
            succeeded = TryReadDouble(out result);
            return this;
        }
        /// <summary>
        /// Tries to read float from the stream. Returns if operation succeeded
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TryReadFloat(out float result)
        {
            if (IsForWriting || IsZipped || IsEncrypted || _position >= _data.Length - 1 || _position + sizeof(float) > _data.Length)
            {
                result = default;
                return false;
            }

            int prevPos = _position;
            byte[] bytes = ReadInternal(sizeof(float));

            try
            {
                result = BitConverter.ToSingle(bytes, 0);
                return true;
            }
            catch
            {
                _position = prevPos;
                result = default;
                return false;
            }
        }
        /// <summary>
        /// Tries to read float from the stream
        /// </summary>
        /// <param name="result"></param>
        /// <param name="succeeded"></param>
        /// <returns></returns>
        public InMemoryDataStream TryReadFloat(out float result, out bool succeeded)
        {
            succeeded = TryReadFloat(out result);
            return this;
        }
        #endregion

        #region Строки
        /// <summary>
        /// Tries to read string in utf-8 encoding from the stream. Returns if operation succeeded
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TryReadString(out string result)
        {
            return TryReadString(out result, Encoding.UTF8);
        }
        /// <summary>
        /// Tries to read string in utf-8 encoding from the stream
        /// </summary>
        /// <param name="result"></param>
        /// <param name="succeeded"></param>
        /// <returns></returns>
        public InMemoryDataStream TryReadString(out string result, out bool succeeded)
        {
            succeeded = TryReadString(out result);
            return this;
        }
        /// <summary>
        /// Tries to read string in custom encoding from the stream. Returns if operation succeeded
        /// </summary>
        /// <param name="result"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public bool TryReadString(out string result, Encoding encoding)
        {
            if (IsForWriting || IsZipped || IsEncrypted || _position >= _data.Length - 1 || _position + sizeof(int) > _data.Length - 1)
            {
                result = string.Empty;
                return false;
            }

            int prevPos = _position;
            int length = ReadInt();

            if (length <= 0 || _position >= _data.Length - 1 || _position + length > _data.Length - 1)
            {
                _position = prevPos;
                result = string.Empty;
                return false;
            }

            try
            {
                result = encoding.GetString(ReadInternal(length));
                return true;
            }
            catch
            {
                _position = prevPos;
                result = string.Empty;
                return false;
            }
        }
        /// <summary>
        /// Tries to read string custom encoding from the stream
        /// </summary>
        /// <param name="result"></param>
        /// <param name="encoding"></param>
        /// <param name="succeeded"></param>
        /// <returns></returns>
        public InMemoryDataStream TryReadString(out string result, Encoding encoding, out bool succeeded)
        {
            succeeded = TryReadString(out result, encoding);
            return this;
        }
        #endregion

        /// <summary>
        /// Tries to read char from the stream. Returns if operation succeeded
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TryReadChar(out char result)
        {
            if (IsForWriting || IsZipped || IsEncrypted || _position >= _data.Length - 1 || _position + sizeof(char) > _data.Length)
            {
                result = default;
                return false;
            }

            int prevPos = _position;
            byte[] bytes = ReadInternal(sizeof(char));

            try
            {
                result = BitConverter.ToChar(bytes, 0);
                return true;
            }
            catch
            {
                _position = prevPos;
                result = default;
                return false;
            }
        }
        /// <summary>
        /// Tries to read char from the stream
        /// </summary>
        /// <param name="result"></param>
        /// <param name="succeeded"></param>
        /// <returns></returns>
        public InMemoryDataStream TryReadChar(out char result, out bool succeeded)
        {
            succeeded = TryReadChar(out result);
            return this;
        }

        #endregion

        #region Пропуски
        /// <summary>
        /// Skip bytes of count equal to next int in the stream
        /// </summary>
        /// <returns></returns>
        public InMemoryDataStream Skip()
        {
            _ = ReadInternal(ReadInt());
            return this;
        }
        /// <summary>
        /// Skip given count of bytes
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public InMemoryDataStream Skip(int count)
        {
            if (_position >= _data.Length - 1)
            {
                throw new InvalidOperationException("The end of stream was reached");
            }

            if (IsForWriting)
            {
                throw new InvalidOperationException("Stream is opened for writing");
            }

            if (IsZipped)
            {
                throw new InvalidOperationException("Data is zipped");
            }

            if (IsEncrypted)
            {
                throw new InvalidOperationException("Data is encrypted");
            }

            _position += count;
            return this;
        }
        /// <summary>
        /// Skips next int in the stream if the value is equal to given
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public InMemoryDataStream SkipIfEqual(int value)
        {
            int next = ReadInt();

            if (next != value)
            {
                throw new InvalidOperationException($"Next value in the stream is \"{next}\", but expected \"{value}\"");
            }

            return this;
        }
        /// <summary>
        /// Skips next long in the stream if the value is equal to given
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public InMemoryDataStream SkipIfEqual(long value)
        {
            long next = ReadLong();

            if (next != value)
            {
                throw new InvalidOperationException($"Next value in the stream is \"{next}\", but expected \"{value}\"");
            }

            return this;
        }
        /// <summary>
        /// Skips next short in the stream if the value is equal to given
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public InMemoryDataStream SkipIfEqual(short value)
        {
            short next = ReadShort();

            if (next != value)
            {
                throw new InvalidOperationException($"Next value in the stream is \"{next}\", but expected \"{value}\"");
            }

            return this;
        }
        /// <summary>
        /// Skips next double in the stream if the value is equal to given
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public InMemoryDataStream SkipIfEqual(double value)
        {
            double next = ReadDouble();

            if (next != value)
            {
                throw new InvalidOperationException($"Next value in the stream is \"{next}\", but expected \"{value}\"");
            }

            return this;
        }
        /// <summary>
        /// Skips next float in the stream if the value is equal to given
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public InMemoryDataStream SkipIfEqual(float value)
        {
            float next = ReadFloat();

            if (next != value)
            {
                throw new InvalidOperationException($"Next value in the stream is \"{next}\", but expected \"{value}\"");
            }

            return this;
        }
        /// <summary>
        /// Skips next char in the stream if the value is equal to given
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public InMemoryDataStream SkipIfEqual(char value)
        {
            char next = ReadChar();

            if (next != value)
            {
                throw new InvalidOperationException($"Next value in the stream is \"{next}\", but expected \"{value}\"");
            }

            return this;
        }

        /// <summary>
        /// Skips next string in utf-8 encoding in the stream if the value is equal to given
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public InMemoryDataStream SkipIfEqual(string value)
        {
            return SkipIfEqual(value, Encoding.UTF8);
        }
        /// <summary>
        /// Skips next string in custom encoding in the stream if the value is equal to given
        /// </summary>
        /// <param name="value"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public InMemoryDataStream SkipIfEqual(string value, Encoding encoding)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (encoding == null)
            {
                throw new ArgumentNullException(nameof(encoding));
            }

            string next = ReadString(encoding);

            if (next != value)
            {
                throw new InvalidOperationException($"Next value in the stream is \"{next}\", but expected \"{value}\"");
            }

            return this;
        }
        #endregion

        #region CheckIfEqual

        #region Числа
        /// <summary>
        /// Checks if the next value in the stream is equal to given. Position in the stream is not changed
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public bool CheckIfEqual(int n)
        {
            int prevPos = _position;
            bool tryRes = TryReadInt(out int read);

            if (!tryRes)
            {
                return false;
            }

            _position = prevPos;
            return read == n;
        }
        /// <summary>
        /// Checks if the next value in the stream is equal to given. Position in the stream is not changed
        /// </summary>
        /// <param name="n"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public InMemoryDataStream CheckIfEqual(int n, out bool result)
        {
            result = CheckIfEqual(n);
            return this;
        }
        /// <summary>
        /// Checks if the next value in the stream is equal to given. Position in the stream is not changed
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public bool CheckIfEqual(long n)
        {
            int prevPos = _position;
            bool tryRes = TryReadLong(out long read);

            if (!tryRes)
            {
                return false;
            }

            _position = prevPos;
            return read == n;
        }
        /// <summary>
        /// Checks if the next value in the stream is equal to given. Position in the stream is not changed
        /// </summary>
        /// <param name="n"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public InMemoryDataStream CheckIfEqual(long n, out bool result)
        {
            result = CheckIfEqual(n);
            return this;
        }
        /// <summary>
        /// Checks if the next value in the stream is equal to given. Position in the stream is not changed
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public bool CheckIfEqual(short n)
        {
            int prevPos = _position;
            bool tryRes = TryReadShort(out short read);

            if (!tryRes)
            {
                return false;
            }

            _position = prevPos;
            return read == n;
        }
        /// <summary>
        /// Checks if the next value in the stream is equal to given. Position in the stream is not changed
        /// </summary>
        /// <param name="n"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public InMemoryDataStream CheckIfEqual(short n, out bool result)
        {
            result = CheckIfEqual(n);
            return this;
        }
        /// <summary>
        /// Checks if the next value in the stream is equal to given. Position in the stream is not changed
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public bool CheckIfEqual(byte n)
        {
            int prevPos = _position;
            bool tryRes = TryReadByte(out byte read);

            if (!tryRes)
            {
                return false;
            }

            _position = prevPos;
            return read == n;
        }
        /// <summary>
        /// Checks if the next value in the stream is equal to given. Position in the stream is not changed
        /// </summary>
        /// <param name="n"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public InMemoryDataStream CheckIfEqual(byte n, out bool result)
        {
            result = CheckIfEqual(n);
            return this;
        }
        /// <summary>
        /// Checks if the next value in the stream is equal to given. Position in the stream is not changed
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public bool CheckIfEqual(double n)
        {
            int prevPos = _position;
            bool tryRes = TryReadDouble(out double read);

            if (!tryRes)
            {
                return false;
            }

            _position = prevPos;
            return read == n;
        }
        /// <summary>
        /// Checks if the next value in the stream is equal to given. Position in the stream is not changed
        /// </summary>
        /// <param name="n"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public InMemoryDataStream CheckIfEqual(double n, out bool result)
        {
            result = CheckIfEqual(n);
            return this;
        }
        /// <summary>
        /// Checks if the next value in the stream is equal to given. Position in the stream is not changed
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public bool CheckIfEqual(float n)
        {
            int prevPos = _position;
            bool tryRes = TryReadFloat(out float read);

            if (!tryRes)
            {
                return false;
            }

            _position = prevPos;
            return read == n;
        }
        /// <summary>
        /// Checks if the next value in the stream is equal to given. Position in the stream is not changed
        /// </summary>
        /// <param name="n"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public InMemoryDataStream CheckIfEqual(float n, out bool result)
        {
            result = CheckIfEqual(n);
            return this;
        }
        #endregion

        #region Строки
        /// <summary>
        /// Checks if the next string in utf-8 encoding in the stream is equal to given. Position in the stream is not changed
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public bool CheckIfEqual(string str)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }

            return CheckIfEqual(str, Encoding.UTF8);
        }
        /// <summary>
        /// Checks if the next string in utf-8 encoding in the stream is equal to given. Position in the stream is not changed
        /// </summary>
        /// <param name="str"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public InMemoryDataStream CheckIfEqual(string str, out bool result)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }

            result = CheckIfEqual(str);
            return this;
        }
        /// <summary>
        /// Checks if the next string in custom encoding in the stream is equal to given. Position in the stream is not changed
        /// </summary>
        /// <param name="str"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public bool CheckIfEqual(string str, Encoding encoding)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }

            if (encoding == null)
            {
                throw new ArgumentNullException(nameof(encoding));
            }

            int prevPos = _position;
            bool tryRes = TryReadString(out string read, encoding);

            if (!tryRes)
            {
                return false;
            }

            _position = prevPos;
            return read == str;
        }
        /// <summary>
        /// Checks if the next string in custom encoding in the stream is equal to given. Position in the stream is not changed
        /// </summary>
        /// <param name="str"></param>
        /// <param name="encoding"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public InMemoryDataStream CheckIfEqual(string str, Encoding encoding, out bool result)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }

            if (encoding == null)
            {
                throw new ArgumentNullException(nameof(encoding));
            }

            result = CheckIfEqual(str, encoding);
            return this;
        }
        #endregion

        /// <summary>
        /// Checks if the next value in the stream is equal to given. Position in the stream is not changed
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public bool CheckIfEqual(char ch)
        {
            int prevPos = _position;
            bool tryRes = TryReadChar(out char read);

            if (!tryRes)
            {
                return false;
            }

            _position = prevPos;
            return read == ch;
        }
        /// <summary>
        /// Checks if the next value in the stream is equal to given. Position in the stream is not changed
        /// </summary>
        /// <param name="ch"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public InMemoryDataStream CheckIfEqual(char ch, out bool result)
        {
            result = CheckIfEqual(ch);
            return this;
        }

        #endregion

        #region NullIfEqual

        #region Числа
        /// <summary>
        /// Returns null if the next value in the stream is equal to the given. If not, position in the stream is not changed
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public InMemoryDataStream NullIfEqual(int n)
        {
            int prevPos = _position;
            bool success = TryReadInt(out int read);

            if (!success)
            {
                return this;
            }

            if (n == read)
            {
                return null;
            }

            _position = prevPos;
            return this;
        }
        /// <summary>
        /// Returns null if the next value in the stream is equal to the given. If not, position in the stream is not changed
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public InMemoryDataStream NullIfEqual(long n)
        {
            int prevPos = _position;
            bool success = TryReadLong(out long read);

            if (!success)
            {
                return this;
            }

            if (n == read)
            {
                return null;
            }

            _position = prevPos;
            return this;
        }
        /// <summary>
        /// Returns null if the next value in the stream is equal to the given. If not, position in the stream is not changed
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public InMemoryDataStream NullIfEqual(short n)
        {
            int prevPos = _position;
            bool success = TryReadShort(out short read);

            if (!success)
            {
                return this;
            }

            if (n == read)
            {
                return null;
            }

            _position = prevPos;
            return this;
        }
        /// <summary>
        /// Returns null if the next value in the stream is equal to the given. If not, position in the stream is not changed
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public InMemoryDataStream NullIfEqual(byte n)
        {
            int prevPos = _position;
            bool success = TryReadByte(out byte read);

            if (!success)
            {
                return this;
            }

            if (n == read)
            {
                return null;
            }

            _position = prevPos;
            return this;
        }
        /// <summary>
        /// Returns null if the next value in the stream is equal to the given. If not, position in the stream is not changed
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public InMemoryDataStream NullIfEqual(double n)
        {
            int prevPos = _position;
            bool success = TryReadDouble(out double read);

            if (!success)
            {
                return this;
            }

            if (n == read)
            {
                return null;
            }

            _position = prevPos;
            return this;
        }
        /// <summary>
        /// Returns null if the next value in the stream is equal to the given. If not, position in the stream is not changed
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public InMemoryDataStream NullIfEqual(float n)
        {
            int prevPos = _position;
            bool success = TryReadFloat(out float read);

            if (!success)
            {
                return this;
            }

            if (n == read)
            {
                return null;
            }

            _position = prevPos;
            return this;
        }
        #endregion

        #region Строки
        /// <summary>
        /// Returns null if the next value in the stream is equal to the given. If not, position in the stream is not changed
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public InMemoryDataStream NullIfEqual(string str)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }

            return NullIfEqual(str, Encoding.UTF8);
        }
        /// <summary>
        /// Returns null if the next value in the stream is equal to the given. If not, position in the stream is not changed
        /// </summary>
        /// <param name="str"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public InMemoryDataStream NullIfEqual(string str, Encoding encoding)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }

            if (encoding == null)
            {
                throw new ArgumentNullException(nameof(encoding));
            }

            int prevPos = _position;
            bool resOfTry = TryReadString(out string read, encoding);

            if (!resOfTry)
            {
                return this;
            }

            if (str == read)
            {
                return null;
            }

            _position = prevPos;
            return this;
        }
        #endregion

        /// <summary>
        /// Returns null if the next value in the stream is equal to the given. If not, position in the stream is not changed
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public InMemoryDataStream NullIfEqual(char ch)
        {
            int prevPos = _position;
            bool success = TryReadChar(out char read);

            if (!success)
            {
                return this;
            }

            if (ch == read)
            {
                return null;
            }

            _position = prevPos;
            return this;
        }

        #endregion

        #region Управление
        /// <summary>
        /// Zips data in the stream
        /// </summary>
        /// <returns></returns>
        public InMemoryDataStream Zip()
        {
            if (IsZipped)
            {
                return this;
            }

            using (MemoryStream memory = new MemoryStream())
            {
                using (GZipStream tinyStream = new GZipStream(memory, CompressionMode.Compress))
                {
                    using MemoryStream ms = ToMemoryStream();
                    ms.CopyTo(tinyStream);
                }
                byte[] bytes = memory.ToArray();
                InitFromBytes(bytes, IsForWriting, IsEncrypted, true);
            }

            return this;
        }
        /// <summary>
        /// Unzips data in the stream
        /// </summary>
        /// <returns></returns>
        public InMemoryDataStream UnZip()
        {
            if (!IsZipped)
            {
                return this;
            }

            using (MemoryStream ms = ToMemoryStream())
            {
                using MemoryStream memory = new MemoryStream();
                using GZipStream decompres = new GZipStream(ms, CompressionMode.Decompress);
                decompres.CopyTo(memory);
                byte[] b = memory.ToArray();

                InitFromBytes(b, IsForWriting, IsEncrypted, false);
            }

            return this;
        }
        /// <summary>
        /// Encrypts data in the stream
        /// </summary>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        public InMemoryDataStream Encrypt(string password, string salt = "AI Framework")
        {
            if (IsEncrypted)
            {
                return this;
            }

            byte[] key = GenKey(password, salt);
            byte[] dat = AsByteArray();
            dat = EncryptAes(dat, key);
            InitFromBytes(dat, IsForWriting, true, IsZipped);

            return this;
        }
        /// <summary>
        /// Decrypts data in the stream
        /// </summary>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        public InMemoryDataStream Decrypt(string password, string salt = "AI Framework")
        {
            if (!IsEncrypted)
            {
                return this;
            }

            byte[] key = GenKey(password, salt);
            byte[] dat = AsByteArray();

            try
            {
                dat = DecryptAes(dat, key);
                InitFromBytes(dat, IsForWriting, false, IsZipped);
            }
            catch
            {
                throw new ArgumentException("Password is incorrect", nameof(password));
            }

            return this;
        }
        /// <summary>
        /// Returns data as a byte array
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] AsByteArray()
        {
            return _data;
        }
        /// <summary>
        /// Returns data as a memory stream
        /// </summary>
        /// <returns></returns>
        public MemoryStream ToMemoryStream()
        {
            MemoryStream memoryStream = new MemoryStream(_data);
            return memoryStream;
        }
        /// <summary>
        /// Сохранениеs data to the file
        /// </summary>
        /// <param name="path">Путь до файла</param>
        public void Save(string path)
        {
            if (_data.Length == 0)
            {
                throw new InvalidOperationException("Data is empty");
            }

            using FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
            Save(fs);
        }
        /// <summary>
        /// Сохранениеs data to the System.IO.Stream
        /// </summary>
        /// <param name="stream">Поток</param>
        public void Save(Stream stream)
        {
            if (_data.Length == 0)
            {
                throw new InvalidOperationException("Data is empty");
            }

            stream.Write(_data, 0, _data.Length);
        }
        /// <summary>
        /// Returns data as base64 string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Convert.ToBase64String(AsByteArray());
        }
        #endregion

        #region Статические методы инициализации
        /// <summary>
        /// Initialize empty DataStream for writing data
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static InMemoryDataStream Create()
        {
            return new InMemoryDataStream();
        }
        /// <summary>
        /// Inintialize DataStream for reading data from file
        /// </summary>
        /// <param name="path">Путь до файла</param>
        /// <param name="isEncrypted"></param>
        /// <param name="isZipped"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static InMemoryDataStream FromFile(string path, bool isEncrypted = false, bool isZipped = false)
        {
            return new InMemoryDataStream(path, isEncrypted, isZipped);
        }
        /// <summary>
        /// Inintialize DataStream for reading data from System.IO.Stream
        /// </summary>
        /// <param name="stream">Поток</param>
        /// <param name="isEncrypted"></param>
        /// <param name="isZipped"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static InMemoryDataStream FromSystemStream(Stream stream, bool isEncrypted = false, bool isZipped = false)
        {
            return new InMemoryDataStream(stream, isEncrypted, isZipped);
        }
        /// <summary>
        /// Inintialize DataStream for reading data from byte array
        /// </summary>
        /// <param name="data"></param>
        /// <param name="isEncrypted"></param>
        /// <param name="isZipped"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static InMemoryDataStream FromByteArray(byte[] data, bool isEncrypted = false, bool isZipped = false)
        {
            return new InMemoryDataStream(data, isEncrypted, isZipped);
        }
        /// <summary>
        /// Initialize DataStream from base64 string for reading data
        /// </summary>
        /// <param name="strBase64"></param>
        /// <param name="isEncrypted"></param>
        /// <param name="isZipped"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static InMemoryDataStream FromBase64String(string strBase64, bool isEncrypted = false, bool isZipped = false)
        {
            if (strBase64 == null)
            {
                throw new ArgumentNullException(nameof(strBase64));
            }

            byte[] array = Convert.FromBase64String(strBase64);
            return new InMemoryDataStream(array, isEncrypted, isZipped);
        }
        #endregion

        #region Приватные методы
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InitFromBytes(byte[] data, bool isForWriting = false, bool isEncrypted = false, bool isZipped = false)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (data.Length == 0)
            {
                throw new ArgumentException("Data is empty", nameof(data));
            }

            IsForWriting = isForWriting;
            IsForReading = !isForWriting;
            IsEncrypted = isEncrypted;
            IsZipped = isZipped;

            _data = data;
        }
        // Дописывание данных
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteInternal(byte[] array)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (IsForReading)
            {
                throw new InvalidOperationException("Stream is opened for reading");
            }

            if (IsZipped)
            {
                throw new InvalidOperationException("Data is zipped");
            }

            if (IsEncrypted)
            {
                throw new InvalidOperationException("Data is encrypted");
            }

            byte[] newData = new byte[_data.Length + array.Length];
            Array.Copy(_data, newData, _data.Length);
            Array.Copy(array, 0, newData, _position, array.Length);
            _data = newData;
            _position += array.Length;
        }

        // Проверка параметров для чтения
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private byte[] ReadInternal(int count)
        {
            if (count <= 0)
            {
                throw new ArgumentException("Count can't be less or equal zero", nameof(count));
            }

            if (_position >= _data.Length - 1)
            {
                throw new InvalidOperationException("The end of stream was reached");
            }

            if (_position + count > _data.Length)
            {
                throw new ArgumentException("Too large count to read", nameof(count));
            }

            if (IsForWriting)
            {
                throw new InvalidOperationException("Stream is opened for writing");
            }

            if (IsZipped)
            {
                throw new InvalidOperationException("Data is zipped");
            }

            if (IsEncrypted)
            {
                throw new InvalidOperationException("Data is encrypted");
            }

            byte[] array = new byte[count];
            Array.Copy(_data, _position, array, 0, count);
            _position += count;
            return array;
        }

        // Генерация ключа
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private byte[] GenKey(string pass, string salt)
        {
            Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(pass, Encoding.ASCII.GetBytes(salt));
            return rfc.GetBytes(32);
        }

        //Кодер
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte[] EncryptAes(byte[] data, byte[] key)
        {
            string plainText = Convert.ToBase64String(data);

            byte[] encrypted;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = IV;

                ICryptoTransform cryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using MemoryStream msEncrypt = new MemoryStream();
                using CryptoStream csEncrypt = new CryptoStream(msEncrypt, cryptor, CryptoStreamMode.Write);
                using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                {
                    swEncrypt.Write(plainText);
                }
                encrypted = msEncrypt.ToArray();
            }
            return encrypted;
        }

        //Декодер
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte[] DecryptAes(byte[] cipherText, byte[] key)
        {
#pragma warning disable CS8600 // Преобразование литерала, допускающего значение NULL или возможного значения NULL в тип, не допускающий значение NULL.
            string base64 = null;
#pragma warning restore CS8600 // Преобразование литерала, допускающего значение NULL или возможного значения NULL в тип, не допускающий значение NULL.

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = IV;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using MemoryStream msDecrypt = new MemoryStream(cipherText);
                using CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                using StreamReader srDecrypt = new StreamReader(csDecrypt);
                base64 = srDecrypt.ReadToEnd();
            }

            return Convert.FromBase64String(base64);
        }
        #endregion
    }
}