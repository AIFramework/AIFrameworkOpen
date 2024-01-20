using AI.DataStructs;
using AI.DataStructs.Algebraic;
using AI.ML.DataSets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace AI.ML.Classifiers
{

    /// <summary>
    /// Метод k-ближайших соседей
    /// </summary>
    [Serializable]
    public class KNNCl : IClassifier
    {
        #region Поля и свойства
        /// <summary>
        /// Количество соседей для учёта в алгоритме KNN.
        /// </summary>
        public int K { get; set; } = 4;

        /// <summary>
        /// Ширина окна для метода окна Парзена.
        /// </summary>
        public double H { get; set; } = 1;

        /// <summary>
        /// Указывает, фиксирована ли ширина окна.
        /// </summary>
        public bool IsFixed { get; set; } = false;

        /// <summary>
        /// Указывает, используется ли метод окна Парзена.
        /// </summary>
        public bool IsParsenMethod { get; set; } = false;

        /// <summary>
        /// Функция ядра для окна Парзена.
        /// </summary>
        public Func<double, double> KernelParsenWindow { get; set; }

        /// <summary>
        /// Функция для измерения расстояния между векторами.
        /// </summary>
        public Func<Vector, Vector, double> Dist { get; set; }

        /// <summary>
        /// Набор классов для классификации.
        /// </summary>
        public StructClasses Classes { get; private set; } = new StructClasses();

        private int _count;

        #endregion

        #region Конструкторы

        /// <summary>
        /// Инициализация экземпляра класса KNNCl с настройками по умолчанию.
        /// </summary>
        public KNNCl()
        {
            InitializeDefaults();
        }

        /// <summary>
        /// Инициализация экземпляра класса KNNCl с использованием предоставленного набора данных.
        /// </summary>
        /// <param name="vectorClasses">Набор данных для классификации.</param>
        public KNNCl(VectorIntDataset vectorClasses) : this()
        {
            foreach (VectorClass item in vectorClasses)
            {
                AddClass(item.Features, item.ClassMark);
            }
        }

        /// <summary>
        /// Инициализация экземпляра класса KNNCl с предоставленным набором классов.
        /// </summary>
        /// <param name="classifikator">Набор классов для классификации.</param>
        public KNNCl(StructClasses classifikator) : this()
        {
            Classes = classifikator ?? throw new ArgumentNullException(nameof(classifikator));
        }

        /// <summary>
        /// Устанавливает настройки по умолчанию.
        /// </summary>
        private void InitializeDefaults()
        {
            KernelParsenWindow = RbfK;
            Dist = Distances.BaseDist.SquareEucl;
        }

        #endregion

        /// <summary>
        /// Радиально-базисная функция ядра.
        /// </summary>
        /// <param name="r">Радиальное расстояние.</param>
        /// <returns>Результат радиально-базисной функции.</returns>
        public double RbfK(double r) => Math.Exp(-2 * r * r);

        /// <summary>
        /// Классифицирует входной вектор и возвращает индекс класса с максимальной вероятностью.
        /// </summary>
        /// <param name="inp">Входной вектор для классификации.</param>
        /// <returns>Индекс класса с максимальной вероятностью.</returns>
        public int Classify(Vector inp)
        {
            if (inp == null) throw new ArgumentNullException(nameof(inp));
            return ClassifyProbVector(inp).MaxElementIndex();
        }

        /// <summary>
        /// Добавляет новый класс в набор классов.
        /// </summary>
        /// <param name="features">Вектор признаков класса.</param>
        /// <param name="num">Метка класса.</param>
        public void AddClass(Vector features, int num)
        {
            if (features == null) throw new ArgumentNullException(nameof(features));
            Classes.Add(new VectorClass { Features = features.Clone(), ClassMark = num });
            _count = GetN();
        }
        
        
        


        /// <summary>
        /// Распознаёт вектор и возвращает вектор вероятностей классов.
        /// </summary>
        /// <param name="inp">Входной вектор.</param>
        public Vector ClassifyProbVector(Vector inp)
        {
            Rang(inp);
            Vector classes = new Vector(_count);
            int limit = UpdateLimitAndH();  // Метод для обновления 'limit' и 'H'

            double sum = 0;
            Parallel.For(0, limit, i =>
            {
                var classVector = ToVector(i, IsParsenMethod);
                lock (classes)
                {
                    classes += classVector;
                    sum += classVector.Sum(); // или другая логика в зависимости от 'ToVector'
                }
            });

            classes /= sum;
            return classes;
        }

        /// <summary>
        /// Возвращает вектор распознавания классов с максимальным значением 1 для распознанного класса.
        /// </summary>
        /// <param name="inp">Входной вектор.</param>
        public Vector RecognizeVectorMax(Vector inp)
        {
            Rang(inp);
            Vector classes = new Vector(_count);
            int limit = UpdateLimitAndH(); 

            double max = 0;
            Parallel.For(0, limit, i =>
            {
                var classVector = ToVector(i, IsParsenMethod);
                lock (classes)
                {
                    classes += classVector;
                    max = Math.Max(max, classVector.Max()); // или другая логика в зависимости от 'ToVector'
                }
            });

            classes /= max;
            return classes;
        }

        private int UpdateLimitAndH()
        {
            int limit = IsFixed && IsParsenMethod ? Classes.Count : Math.Min(K, Classes.Count);
            if (!IsFixed || !IsParsenMethod) H = Classes[limit - 1].R;
            return limit;
        }

        /// <summary>
        /// Обучает классификатор.
        /// </summary>
        /// <param name="features">Массив признаков.</param>
        /// <param name="classes">Массив меток классов.</param>
        public void Train(Vector[] features, int[] classes)
        {
            if (features.Length != classes.Length)
                throw new InvalidOperationException("Размерности векторов признаков и классов не совпадают.");

            for (int i = 0; i < features.Length; i++)
                AddClass(features[i], classes[i]);
        }

        /// <summary>
        /// Обучает классификатор на основе набора данных.
        /// </summary>
        /// <param name="dataset">Набор данных.</param>
        public void Train(VectorIntDataset dataset)
        {
            foreach (var item in dataset)
                AddClass(item.Features, item.ClassMark);
        }

        /// <summary>
        /// Сохранить в файл
        /// </summary>
        /// <param name="path">Путь до файла</param>
        public void Save(string path)
        {
            BinarySerializer.Save(path, this);
        }
        /// <summary>
        /// Сохранить в поток
        /// </summary>
        /// <param name="stream">Поток</param>
        public void Save(Stream stream)
        {
            BinarySerializer.Save(stream, this);
        }
        /// <summary>
        /// Загрузить из файла
        /// </summary>
        /// <param name="path">Путь до файла</param>
        /// <returns></returns>
        public static KNNCl Load(string path)
        {
            return BinarySerializer.Load<KNNCl>(path);
        }
        /// <summary>
        /// Загрузить из потока
        /// </summary>
        /// <param name="stream">Поток</param>
        /// <returns></returns>
        public static KNNCl Load(Stream stream)
        {
            return BinarySerializer.Load<KNNCl>(stream);
        }
        /// <summary>
        /// Загрузить из csv (признаки; метка класса)
        /// </summary>
        /// <param name="pathToEtallonClassCsv">Путь до файла</param>
        public static KNNCl GetKNN(string pathToEtallonClassCsv)
        {
            VectorIntDataset vectorClasses = new VectorIntDataset(pathToEtallonClassCsv);
            return new KNNCl(vectorClasses);
        }

        #region Приватные методы
        /// <summary>
        /// Transform to vector
        /// </summary>
        /// <param name="i">Индекс</param>
        /// <param name="isParsenMethod">Используется ли окно Парзена</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Vector ToVector(int i, bool isParsenMethod)
        {
            int index = i < Classes.Count - 1 ? i : Classes.Count - 1;
            int mark = Classes[index].ClassMark;
            Vector outp = new Vector(_count);

            if (isParsenMethod)
            {
                double r = Classes[index].R / H;
                outp[mark] = KernelParsenWindow(r);
            }
            else
            {
                outp[mark] = 1;
            }

            return outp;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Rang(Vector inp)
        {
            Parallel.For(0, Classes.Count, i => {
                Classes[i].R = Dist(inp, Classes[i].Features);
            });

            Classes.Sort((a, b) => a.R.CompareTo(b.R));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetN() => Classes.Select(c => c.ClassMark).Distinct().Count();
        #endregion
    }
}
