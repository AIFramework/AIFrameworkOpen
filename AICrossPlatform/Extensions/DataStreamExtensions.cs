using AI.DataStructs;
using AI.DataStructs.Algebraic;
using AI.DataStructs.WithComplexElements;
using AI.ML.NeuralNetwork.CoreNNW;
using System.Runtime.CompilerServices;

namespace AI.Extensions
{
    /// <summary>
    /// Работа с массивами байт
    /// </summary>
    public static class DataStreamExtensions
    {
        #region ToDataStream
        /// <summary>
        /// Получение объекта памяти
        /// </summary>
        /// <param name="array">Массив байт</param>
        /// <param name="isEncrypted">Зашифрован ли объект</param>
        /// <param name="isZipped">Сжат ли объект</param>
        /// <returns></returns>
        public static InMemoryDataStream ToDataStream(this byte[] array, bool isEncrypted = false, bool isZipped = false)
        {
            return new InMemoryDataStream(array, isEncrypted, isZipped);
        }

        /// <summary>
        /// Получение объекта памяти
        /// </summary>
        /// <param name="convertable">AI объект конвертируемый в байты</param>
        /// <returns></returns>
        public static InMemoryDataStream ToDataStream(this IByteConvertable convertable)
        {
            return new InMemoryDataStream(convertable.GetBytes());
        }
        #endregion

        #region Запись
        /// <summary>
        /// Writes IByteConvertable to the stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="convertable"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static InMemoryDataStream Write(this InMemoryDataStream stream, IByteConvertable convertable)
        {
            if (convertable == null)
            {
                stream.Write("0");
            }

            stream.Write(convertable.GetBytes());
            return stream;
        }
        /// <summary>
        /// Writes IByteConvertable array to the stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="convertables"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static InMemoryDataStream Write(this InMemoryDataStream stream, IByteConvertable[] convertables)
        {
            stream.Write(convertables.Length);

            for (int i = 0; i < convertables.Length; i++)
            {
                stream.Write(convertables[i]);
            }

            return stream;
        }
        #endregion

        #region Чтение
        /// <summary>
        /// Reads vector from the stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector ReadVector(this InMemoryDataStream stream)
        {
            return Vector.FromDataStream(stream);
        }
        /// <summary>
        /// Reads vector from the stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static InMemoryDataStream ReadVector(this InMemoryDataStream stream, out Vector result)
        {
            result = Vector.FromDataStream(stream);
            return stream;
        }
        /// <summary>
        /// Reads vector array from the stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector[] ReadVectors(this InMemoryDataStream stream)
        {
            int length = stream.ReadInt();
            Vector[] result = new Vector[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = ReadVector(stream);
            }

            return result;
        }
        /// <summary>
        /// Reads vector array from the stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static InMemoryDataStream ReadVectors(this InMemoryDataStream stream, out Vector[] result)
        {
            result = ReadVectors(stream);
            return stream;
        }
        /// <summary>
        /// Reads matrix from the stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix ReadMatrix(this InMemoryDataStream stream)
        {
            return Matrix.FromDataStream(stream);
        }
        /// <summary>
        /// Reads matrix from the stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static InMemoryDataStream ReadMatrix(this InMemoryDataStream stream, out Matrix result)
        {
            result = Matrix.FromDataStream(stream);
            return stream;
        }
        /// <summary>
        /// Reads matrix array from the stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix[] ReadMatrices(this InMemoryDataStream stream)
        {
            int length = stream.ReadInt();
            Matrix[] result = new Matrix[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = ReadMatrix(stream);
            }

            return result;
        }
        /// <summary>
        /// Reads matrix array from the stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static InMemoryDataStream ReadMatrices(this InMemoryDataStream stream, out Matrix[] result)
        {
            result = ReadMatrices(stream);
            return stream;
        }
        /// <summary>
        /// Reads tensor from the stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Tensor ReadTensor(this InMemoryDataStream stream)
        {
            return Tensor.FromDataStream(stream);
        }
        /// <summary>
        /// Reads tensor from the stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static InMemoryDataStream ReadTensor(this InMemoryDataStream stream, out Tensor result)
        {
            result = Tensor.FromDataStream(stream);
            return stream;
        }
        /// <summary>
        /// Reads tensor array from the stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Tensor[] ReadTensors(this InMemoryDataStream stream)
        {
            int length = stream.ReadInt();
            Tensor[] result = new Tensor[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = ReadTensor(stream);
            }

            return result;
        }
        /// <summary>
        /// Reads tensor array from the stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static InMemoryDataStream ReadTensors(this InMemoryDataStream stream, out Tensor[] result)
        {
            result = ReadTensors(stream);
            return stream;
        }
        /// <summary>
        /// Reads NDTensor from the stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NDTensor ReadNDTensor(this InMemoryDataStream stream)
        {
            return NDTensor.FromDataStream(stream);
        }
        /// <summary>
        /// Reads NDTensor from the stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static InMemoryDataStream ReadNDTensor(this InMemoryDataStream stream, out NDTensor result)
        {
            result = NDTensor.FromDataStream(stream);
            return stream;
        }
        /// <summary>
        /// Reads NDTensor array from the stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NDTensor[] ReadNDTensors(this InMemoryDataStream stream)
        {
            int length = stream.ReadInt();
            NDTensor[] result = new NDTensor[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = ReadNDTensor(stream);
            }

            return result;
        }
        /// <summary>
        /// Reads NDTensor array from the stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static InMemoryDataStream ReadNDTensors(this InMemoryDataStream stream, out NDTensor[] result)
        {
            result = ReadNDTensors(stream);
            return stream;
        }
        /// <summary>
        /// Reads NNValue from the stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NNValue ReadNNValue(this InMemoryDataStream stream)
        {
            return NNValue.FromDataStream(stream);
        }
        /// <summary>
        /// Reads NNValue from the stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static InMemoryDataStream ReadNNValue(this InMemoryDataStream stream, out NNValue result)
        {
            result = NNValue.FromDataStream(stream);
            return stream;
        }
        /// <summary>
        /// Reads NNValue array from the stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NNValue[] ReadNNValues(this InMemoryDataStream stream)
        {
            int length = stream.ReadInt();
            NNValue[] result = new NNValue[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = ReadNNValue(stream);
            }

            return result;
        }
        /// <summary>
        /// Reads NNValue array from the stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static InMemoryDataStream ReadNNValues(this InMemoryDataStream stream, out NNValue[] result)
        {
            result = ReadNNValues(stream);
            return stream;
        }
        /// <summary>
        /// Reads complex vector from the stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ComplexVector ReadComplexVector(this InMemoryDataStream stream)
        {
            return ComplexVector.FromDataStream(stream);
        }
        /// <summary>
        /// Reads complex vector from the stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static InMemoryDataStream ReadComplexVector(this InMemoryDataStream stream, out ComplexVector result)
        {
            result = ComplexVector.FromDataStream(stream);
            return stream;
        }
        /// <summary>
        /// Reads complex vector array from the stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ComplexVector[] ReadComplexVectors(this InMemoryDataStream stream)
        {
            int length = stream.ReadInt();
            ComplexVector[] result = new ComplexVector[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = ReadComplexVector(stream);
            }

            return result;
        }
        /// <summary>
        /// Reads complex vector array from the stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static InMemoryDataStream ReadComplexVectors(this InMemoryDataStream stream, out ComplexVector[] result)
        {
            result = ReadComplexVectors(stream);
            return stream;
        }
        /// <summary>
        /// Reads complex matrix from the stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ComplexMatrix ReadComplexMatrix(this InMemoryDataStream stream)
        {
            return ComplexMatrix.FromDataStream(stream);
        }
        /// <summary>
        /// Reads complex matrix from the stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static InMemoryDataStream ReadComplexMatrix(this InMemoryDataStream stream, out ComplexMatrix result)
        {
            result = ComplexMatrix.FromDataStream(stream);
            return stream;
        }
        /// <summary>
        /// Reads complex matrix array from the stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ComplexMatrix[] ReadComplexMatrices(this InMemoryDataStream stream)
        {
            int length = stream.ReadInt();
            ComplexMatrix[] result = new ComplexMatrix[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = ReadComplexMatrix(stream);
            }

            return result;
        }
        /// <summary>
        /// Reads complex matrix array from the stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static InMemoryDataStream ReadComplexMatrices(this InMemoryDataStream stream, out ComplexMatrix[] result)
        {
            result = ReadComplexMatrices(stream);
            return stream;
        }
        #endregion
    }
}