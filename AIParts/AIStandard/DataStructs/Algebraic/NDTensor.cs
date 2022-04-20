using AI.DataStructs.Shapes;
using AI.Extensions;
using System;
using System.Diagnostics;
using System.IO;

namespace AI.DataStructs.Algebraic
{
    [Serializable]
    [DebuggerDisplay("Shape = {Shape.ToString(),nq}")]
    public class NDTensor : IAlgebraicStructure, IEquatable<NDTensor>, ISavable, IByteConvertable
    {
        #region Поля и свойства
        private readonly int[] _volumes;

        /// <summary>
        /// Tensor data as a flat array
        /// </summary>
        public double[] Data { get; private set; }
        /// <summary>
        /// Tensor shape
        /// </summary>
        public Shape Shape { get; }
        /// <summary>
        /// Returns value by indexes in all dimensions
        /// </summary>
        /// <param name="indexes"></param>
        /// <returns></returns>
        public double this[params int[] indexes]
        {
            get => Data[GetIndex(indexes)];
            set => Data[GetIndex(indexes)] = value;
        }
        #endregion

        #region Конструкторы
        public NDTensor(Shape shape)
        {
            Shape = shape;

            _volumes = new int[shape.Rank - 1];
            _volumes[0] = shape[0];
            int len = 1;

            for (int i = 0; i < shape.Rank; i++)
            {
                len *= shape[i];
            }

            for (int i = 1; i < _volumes.Length; i++)
            {
                _volumes[i] *= shape[i];
            }

            Data = new double[len];
        }

        public NDTensor(int[] shape) : this(new Shape(shape)) { }
        #endregion

        #region Операторы
        public static bool operator ==(NDTensor left, NDTensor right)
        {
            return left.Shape == right.Shape && left.Data.ElementWiseEqual(right.Data);
        }

        public static bool operator !=(NDTensor left, NDTensor right)
        {
            return left.Shape != right.Shape || !left.Data.ElementWiseEqual(right.Data);
        }
        #endregion

        #region Технические методы
#pragma warning disable CS1591 // Отсутствует комментарий XML для открытого видимого типа или члена

        public override bool Equals(object obj)
        {
            if (obj is NDTensor nD)
            {
                return nD == this;
            }
            else
            {
                return false;
            }
        }

        public bool Equals(NDTensor other)
        {
            return this == other;
        }

#pragma warning restore CS1591 // Отсутствует комментарий XML для открытого видимого типа или члена
        #endregion

        #region Сериализация

        #region Сохранение
        /// <summary>
        /// Saves NDTensor to file
        /// </summary>
        /// <param name="path"></param>
        public void Save(string path)
        {
            BinarySerializer.Save(path, this);
        }
        /// <summary>
        /// Saves NDTensor to stream
        /// </summary>
        /// <param name="stream"></param>
        public void Save(Stream stream)
        {
            BinarySerializer.Save(stream, this);
        }
        /// <summary>
        /// Represents NDTensor as an array of bytes
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            return InMemoryDataStream.Create().Write(KeyWords.NDTensor).Write(Shape.GetDataCopy()).Write(Data).AsByteArray();
        }
        #endregion

        #region Загрузка
        /// <summary>
        /// Loads NDTensor from file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static NDTensor Load(string path)
        {
            return BinarySerializer.Load<NDTensor>(path);
        }
        /// <summary>
        /// Loads NDTensor from stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static NDTensor Load(Stream stream)
        {
            return BinarySerializer.Load<NDTensor>(stream);
        }
        /// <summary>
        /// Initialize NDTensor form an array of bytes
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static NDTensor FromBytes(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            return FromDataStream(InMemoryDataStream.FromByteArray(data));
        }
        /// <summary>
        /// Initialize NDTensor form a data stream
        /// </summary>
        /// <param name="dataStream"></param>
        /// <returns></returns>
        public static NDTensor FromDataStream(InMemoryDataStream dataStream)
        {
            if (dataStream == null)
            {
                throw new ArgumentNullException(nameof(dataStream));
            }

            dataStream.SkipIfEqual(KeyWords.NDTensor).ReadInts(out int[] shapeVals).ReadDoubles(out double[] tData);
            NDTensor tensor = new NDTensor(shapeVals)
            {
                Data = tData
            };
            return tensor;
        }
        #endregion

        #endregion

        #region Приватные методы
        private int GetIndex(int[] indexes)
        {
            int ind = indexes[0];

            for (int i = 0; i < indexes.Length; i++)
            {
                ind += indexes[i + 1] * _volumes[i];
            }

            return ind;
        }
        #endregion
    }
}
