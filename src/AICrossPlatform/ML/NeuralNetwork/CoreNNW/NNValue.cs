using AI.DataStructs;
using AI.DataStructs.Algebraic;
using AI.DataStructs.Shapes;
using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace AI.ML.NeuralNetwork.CoreNNW
{
    /// <summary>
    /// Основной класс для хранения данных о нейронной сети может быть векторои, матрицей или же тензором 3-го ранга.
    /// </summary>
    [Serializable]
    public class NNValue : IByteConvertable
    {
        #region Поля и свойства
        /// <summary>
        /// Массив элементов, данные нейросети
        /// </summary>
        public float[] Data { get; private set; }
        /// <summary>
        /// Массив частных производных
        /// </summary>
        public float[] DifData { get; private set; }
        /// <summary>
        /// Массив кэша оптимизаторов использующих один момент
        /// </summary>
        public float[] StepCache { get; private set; }
        /// <summary>
        /// Массив кэша оптимизаторов использующих два момента, например Адам
        /// </summary>
        public float[] StepCache2 { get; private set; }
        /// <summary>
        /// Форма тензора
        /// </summary>
        public Shape3D Shape { get; set; }
        /// <summary>
        /// Получить элемент по индексу
        /// </summary>
        /// <param name="i">Индекс</param>
        public float this[int i]
        {
            get => Data[i];
            set => Data[i] = value;
        }
        /// <summary>
        /// Получение элемента по индексу высоты и ширины
        /// </summary>
        /// <param name="h">Индекс высоты</param>
        /// <param name="w">Индекс ширины</param>
        public float this[int h, int w]
        {
            get => GetW(h, w);
            set => SetW(h, w, value);
        }
        /// <summary>
        /// Получение элемента по индексу высоты, ширины и глубины
        /// </summary>
        /// <param name="h">Индекс высоты</param>
        /// <param name="w">Индекс ширины</param>
        /// <param name="d">Индекс глубины</param>
        public float this[int h, int w, int d]
        {
            get => GetW(h, w, d);
            set => SetW(h, w, d, value);
        }
        #endregion

        #region Конструкторы
        /// <summary>
        /// Создание основного класса как тензора 3-го ранга
        /// </summary>
        /// <param name="height">Ширина</param>
        /// <param name="width">Ширина</param>
        /// <param name="depth">Глубина</param>
        public NNValue(int height, int width = 1, int depth = 1)
        {
            Shape = new Shape3D(height, width, depth);
            Data = new float[Shape.Volume];
            DifData = new float[Shape.Volume];
            StepCache = new float[Shape.Volume];
            StepCache2 = new float[Shape.Volume];
        }
        /// <summary>
        /// Создание основного класса как тензора 3-го ранга
        /// </summary>
        /// <param name="shape">Форма тензора</param>
        public NNValue(Shape3D shape)
        {
            Shape = shape;
            Data = new float[Shape.Volume];
            DifData = new float[Shape.Volume];
            StepCache = new float[Shape.Volume];
            StepCache2 = new float[Shape.Volume];
        }
        /// <summary>
        /// Create a tensor based on the algebraic strucuture
        /// </summary>
        /// <param name="algebraicStructure"></param>
        public NNValue(IAlgebraicStructure algebraicStructure)
        {
            if (algebraicStructure == null)
            {
                throw new ArgumentNullException(nameof(algebraicStructure));
            }

            if (algebraicStructure.Shape.Rank > 3)
            {
                throw new ArgumentException("Rank of the shape of the given strucuture is greater than 3", nameof(algebraicStructure));
            }

            switch (algebraicStructure.Shape.Rank)
            {
                case 1:
                    Shape = new Shape3D(algebraicStructure.Shape[0]);
                    break;
                case 2:
                    Shape = new Shape3D(algebraicStructure.Shape[1], algebraicStructure.Shape[0]);
                    break;
                case 3:
                    Shape = new Shape3D(algebraicStructure.Shape[1], algebraicStructure.Shape[0], algebraicStructure.Shape[2]);
                    break;
            }

            Data = new float[Shape!.Volume];

            for (int i = 0; i < Shape.Volume; i++)
            {
                Data[i] = (float)algebraicStructure.Data[i];
            }

            DifData = new float[Shape.Volume];
            StepCache = new float[Shape.Volume];
            StepCache2 = new float[Shape.Volume];
        }
        /// <summary>
        /// Create tensor with parameters 1, 1, 1
        /// </summary>
        /// <param name="value">Value</param>
        public NNValue(double value)
        {
            Shape = new Shape3D();
            Data = new float[1];
            Data[0] = (float)value;
            DifData = new float[1];
            StepCache = new float[1];
            StepCache2 = new float[1];
        }
        #endregion

        #region Операторы
        /// <summary>
        /// Division of the tensor of DERIVATIVES by a number
        /// </summary>
        /// <returns></returns>
        public static NNValue operator /(NNValue nNValue, double d)
        {
            NNValue ret = nNValue.Clone();

            for (int i = 0; i < ret.Data.Length; i++)
            {
                ret.DifData[i] = (float)(nNValue.DifData[i] / d);
            }

            return ret;
        }
        #endregion

        /// <summary>
        /// Similar tensor (same shape, filled with zeros)
        /// </summary>
        public NNValue Like0()
        {
            return new NNValue(Shape);
        }
        /// <summary>
        /// Перевод в AI Тензор
        /// </summary>
        public Tensor ToTensor()
        {
            Tensor tensor = new Tensor(Shape);

            for (int i = 0; i < tensor.Shape.Count; i++)
            {
                tensor.Data[i] = Data[i];
            }

            return tensor;
        }
        /// <summary>
        /// Перевод в AI Матрицу
        /// </summary>
        public Matrix ToMatrix()
        {
            Matrix matr = new Matrix(Shape.Height, Shape.Width);

            for (int i = 0; i < matr.Shape.Count; i++)
            {
                matr.Data[i] = Data[i];
            }

            return matr;
        }
        /// <summary>
        /// Перевод в AI Вектор
        /// </summary>
        public Vector ToVector()
        {
            return new Vector(Data);
        }
        /// <summary>
        /// Перевод матрицы в массив строк
        /// </summary>
        /// <returns></returns>
        public string[] ToTxts()
        {
            string[] result = new string[Shape.Count + 4];
            result[0] = "h:" + Shape.Height;
            result[1] = "w:" + Shape.Width;
            result[2] = "d:" + Shape.Depth;
            result[3] = "data:";

            for (int i = 0; i < Shape.Count; i++)
            {
                result[i + 4] = "" + this[i];
            }
            return result;
        }
        /// <summary>
        /// Перевод матрицы в массив строк
        /// </summary>
        /// <returns></returns>
        public string[] ToTxtsNoInfo()
        {
            string[] result = new string[Shape.Count];

            for (int i = 0; i < Shape.Count; i++)
            {
                result[i] = "" + this[i];
            }
            return result;
        }
        /// <summary>
		/// Гауссово распределение
		/// </summary>
		/// <returns>Возвращает норм. распред величину СКО = 1, M = 0</returns>
		public static double Gauss(Random A)
        {
            double a = (2 * A.NextDouble()) - 1,
            b = (2 * A.NextDouble()) - 1,
            s = (a * a) + (b * b);

            if (s == 0 || s > 1)
            {
                return Gauss(A);
            }

            return b * Math.Sqrt(Math.Abs(-2 * Math.Log(s) / s));
        }
        /// <summary>
        /// Клонирование тензора
        /// </summary>
        /// <returns></returns>
        public NNValue Clone()
        {
            NNValue result = new NNValue(Shape);
            int lenght = 4 * Shape.Count;

            Buffer.BlockCopy(Data, 0, result.Data, 0, lenght);
            Buffer.BlockCopy(DifData, 0, result.DifData, 0, lenght);
            Buffer.BlockCopy(StepCache, 0, result.StepCache, 0, lenght);
            Buffer.BlockCopy(StepCache2, 0, result.StepCache2, 0, lenght);

            return result;
        }
        /// <summary>
        /// Сброс производных
        /// </summary>
        public void ResetDw()
        {
            DifData = new float[Shape.Count];
        }
        /// <summary>
        /// Сброс кэша
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ResetStepCache()
        {
            StepCache = new float[Shape.Count];
            StepCache2 = new float[Shape.Count];
        }
        /// <summary>
        /// Транспонирование
        /// </summary>
        /// <param name="m">Матрица</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NNValue Transpose(NNValue m)
        {
            //TODO: Написать реализацию для тензора
            NNValue result = new NNValue(m.Shape.Width, m.Shape.Height);
            for (int r = 0; r < m.Shape.Height; r++)
            {
                for (int c = 0; c < m.Shape.Width; c++)
                {
                    result.SetW(c, r, m.GetW(r, c));
                }
            }
            return result;
        }
        /// <summary>
        /// Заполнение тензора случайными числами
        /// </summary>
        /// <param name="h">Ширина</param>
        /// <param name="w"> Высота</param>
        /// <param name="initParamsStdDev">ско</param>
        /// <param name="rnd">Генератор псевдослуч. чисел</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NNValue Random(int h, int w, double initParamsStdDev, Random rnd)
        {
            NNValue result = new NNValue(h, w);
            for (int i = 0; i < result.Shape.Count; i++)
            {
                result.Data[i] = (float)(Gauss(rnd) * initParamsStdDev);
            }
            return result;
        }
        /// <summary>
        /// Заполнение тензора случайными числами
        /// </summary>
        /// <param name="h">Ширина</param>
        /// <param name="w"> Высота</param>
        /// <param name="d">Глубина</param>
        /// <param name="initParamsStdDev">ско</param>
        /// <param name="rnd">Генератор псевдослуч. чисел</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NNValue Random(int h, int w, int d, double initParamsStdDev, Random rnd)
        {
            NNValue result = new NNValue(h, w, d);
            for (int i = 0; i < result.Data.Length; i++)
            {
                result.Data[i] = (float)(Gauss(rnd) * initParamsStdDev);
            }
            return result;
        }
        /// <summary>
        /// Заполнение тензора случайными числами
        /// </summary>
        /// <param name="h">Ширина</param>
        /// <param name="w"> Высота</param>
        /// <param name="d">Глубина</param>
        /// <param name="initParamsStdDev">ско</param>
        /// <param name="rnd">Генератор псевдослуч. чисел</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NNValue RandomR(int h, int w, int d, double initParamsStdDev, Random rnd)
        {
            NNValue result = new NNValue(h, w, d);
            for (int i = 0; i < result.Data.Length; i++)
            {
                result.Data[i] = (float)(((2 * rnd.NextDouble()) - 1) * initParamsStdDev);
            }
            return result;
        }
        /// <summary>
        /// Создание единичной матрицы
        /// </summary>
        /// <param name="dim">Размерность матрицы</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NNValue Ident(int dim)
        {
            NNValue result = new NNValue(dim, dim);
            for (int i = 0; i < dim; i++)
            {
                result.SetW(i, i, 1.0);
            }
            return result;
        }
        /// <summary>
        /// Создание матрицы заполненной одним числом
        /// </summary>
        /// <param name="h"> Высота</param>
        /// <param name="w">Ширина</param>
        /// <param name="s">Число</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe NNValue Uniform(int h, int w, double s)
        {
            NNValue result = new NNValue(h, w);
            float fs = (float)s;

            fixed (float* rD = result.Data)
            {
                float* pointer = rD;

                for (int i = 0; i < result.Data.Length; i++)
                {
                    *pointer = fs;
                    pointer++;
                }
            }

            return result;
        }

        /// <summary>
        /// Создание тензора заполненного одним значением
        /// </summary>
        /// <param name="shape">Форма тензора</param>
        /// <param name="s">Значение</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe NNValue Uniform(Shape3D shape, double s)
        {
            NNValue result = new NNValue(shape);
            float fs = (float)s;

            fixed (float* rD = result.Data)
            {
                float* pointer = rD;

                for (int i = 0; i < result.Data.Length; i++)
                {
                    *pointer = fs;
                    pointer++;
                }
            }

            return result;
        }
        /// <summary>
        /// Заполнение тензора единицами
        /// </summary>
        /// <param name="h"> Высота</param>
        /// <param name="w">Ширина</param>
        public static NNValue Ones(int h, int w)
        {
            return Uniform(h, w, 1.0);
        }
        /// <summary>
        /// Заполнение тензора единицами
        /// </summary>
        /// <param name="shape"></param>
        public static NNValue Ones(Shape3D shape)
        {
            return Uniform(shape, 1.0);
        }
        /// <summary>
        /// Заполнение тензора единицами
        /// </summary>
        /// <param name="h"> Высота</param>
        public static NNValue Ones(int h)
        {
            return Uniform(h, 1, 1.0);
        }
        /// <summary>
        /// Заполнение тензора -1
        /// </summary>
        /// <param name="h"> Высота</param>
        /// <param name="w">Ширина</param>
        public static NNValue NegativeOnes(int h, int w)
        {
            return Uniform(h, w, -1.0);
        }

        /// <summary>
        /// Изменение формы тензора
        /// </summary>
        /// <param name="value"></param>
        /// <param name="newShape"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Содержит исключение "Изменение формы невозможно, т.к. объемы не совпадают"</exception>
        public static NNValue ReShape(NNValue value, Shape3D newShape)
        {
            if (value.Shape.Count != newShape.Volume)
            {
                throw new InvalidOperationException("Изменение формы невозможно, т.к. объемы не совпадают");
            }

            NNValue valRes = value.Clone();

            valRes.Shape = newShape;

            return valRes;
        }

        /// <summary>
        /// Сохранение тензора в текстовом формате
        /// </summary>
        /// <param name="path">Путь до файла</param>
        public void SaveAsText(string path)
        {
            string[] conent = ToTxts();
            File.WriteAllLines(path, conent);
        }
        /// <summary>
        /// Сохранение без тензора без описания
        /// </summary>
        /// <param name="path">Путь для сохранения</param>
        public void SaveAsTextNoInfo(string path)
        {
            string[] conent = ToTxtsNoInfo();
            File.WriteAllLines(path, conent);
        }
        /// <summary>
        /// Save NNValue to file
        /// </summary>
        /// <param name="path"></param>
        public void Save(string path)
        {
            BinarySerializer.Save(path, this);
        }
        /// <summary>
        /// Save NNValue to stream
        /// </summary>
        /// <param name="stream"></param>
        public void Save(Stream stream)
        {
            BinarySerializer.Save(stream, this);
        }
        /// <summary>
        /// Represents NNValue as an array of bytes
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            InMemoryDataStream stream = InMemoryDataStream.Create().Write(KeyWords.NNValue).Write(Shape.GetDataCopy()).Write(Data);

            if (DifData == null)
            {
                stream.Write('\0');
            }
            else
            {
                stream.Write(DifData);
            }

            if (StepCache == null)
            {
                stream.Write('\0');
            }
            else
            {
                stream.Write(StepCache);
            }

            if (StepCache2 == null)
            {
                stream.Write('\0');
            }
            else
            {
                stream.Write(StepCache2);
            }

            return stream.AsByteArray();
        }
        /// <summary>
        /// Load NNValue from file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static NNValue Load(string path)
        {
            return BinarySerializer.Load<NNValue>(path);
        }
        /// <summary>
        /// Load NNValue from stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static NNValue Load(Stream stream)
        {
            return BinarySerializer.Load<NNValue>(stream);
        }
        /// <summary>
        /// Initialize NNValue from an array of bytes
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static NNValue FromBytes(byte[] data)
        {
            return FromDataStream(InMemoryDataStream.FromByteArray(data));
        }
        /// <summary>
        /// Initialize NNValue from data stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static NNValue FromDataStream(InMemoryDataStream stream)
        {
            stream.SkipIfEqual(KeyWords.NNValue).ReadInts(out int[] shape).ReadFloats(out float[] valData);
            float[] difData = stream.NullIfEqual('\0').ReadFloats();
            float[] stepCache = stream.NullIfEqual('\0').ReadFloats();
            float[] stepCache2 = stream.NullIfEqual('\0').ReadFloats();
            NNValue result = new NNValue(shape[1], shape[0], shape[2])
            {
                Data = valData,
                DifData = difData,
                StepCache = stepCache,
                StepCache2 = stepCache2
            };
            return result;
        }
        
        /// <summary>
        /// Модификация (только использование), убирает массивы кэшей и данные производных
        /// что облегчает модель в 4 раза
        /// </summary>
        public void OnlyUse()
        {
            DifData = null;
            StepCache = null;
            StepCache2 = null;
        }

        /// <summary>
        /// Добавляет массивы кэшей и данные производных, если те были удалены(например вследствии вызова метода OnlyUse() ), что позволяет снова учить сеть
        /// </summary>
        public void InitTrainData()
        {
            if (DifData == null)
            {
                DifData = new float[Shape.Count];
            }

            if (StepCache == null)
            {
                StepCache = new float[Shape.Count];
            }

            if (StepCache2 == null)
            {
                StepCache2 = new float[Shape.Count];
            }
        }

        #region Технические методы
        /// <summary>
        /// Вывод тензора в строку
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToString(AISettings.GetProvider());
        }

        /// <summary>
        /// Вывод тензора в строку
        /// </summary>
        public string ToString(NumberFormatInfo provider)
        {
            StringBuilder sb = new StringBuilder();

            for (int k = 0; k < Shape.Depth; k++)
            {
                sb.Append("Deep #");
                sb.Append(k + 1);
                sb.AppendLine(":");

                for (int i = 0; i < Shape.Height; i++)
                {
                    sb.Append("[");
                    for (int j = 0; j < Shape.Width; j++)
                    {
                        sb.Append(this[i, j, k].ToString(provider));
                        sb.Append(" ");
                    }
                    sb.Length--;
                    sb.AppendLine("]");
                }
            }

            sb.Length -= Environment.NewLine.Length;
            return sb.ToString();
        }
        #endregion

        #region Приватные методы
        private int GetByIndex(int h, int w)
        {
            return (Shape.Width * h) + w;
        }

        private int GetByIndex(int h, int w, int d)
        {
            return (Shape.Width * h) + w + (Shape.Area * d);
        }

        private float GetW(int h, int w)
        {
            return Data[GetByIndex(h, w)];
        }

        private float GetW(int h, int w, int d)
        {
            return Data[GetByIndex(h, w, d)];
        }

        private void SetW(int h, int w, double val)
        {
            Data[GetByIndex(h, w)] = (float)val;
        }

        private void SetW(int h, int w, int d, double val)
        {
            Data[GetByIndex(h, w, d)] = (float)val;
        }
        #endregion
    }
}