using AI.DataStructs;
using AI.DataStructs.Algebraic;
using AI.ML.DataSets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AI.ML.Classifiers
{

    /// <summary>
    /// Метод k-ближайших соседей
    /// </summary>
    [Serializable]
    public class KNNCl : IClassifier
    {
        #region Поля и свойства
        private int _count;

        /// <summary>
        /// Число соседей
        /// </summary>
        public int K { get; set; } = 4;
        /// <summary>
        /// Ширина окна
        /// </summary>
        public double H { get; set; } = 1;
        /// <summary>
        /// Фиксирована ли ширина окна
        /// </summary>
        public bool IsFixed { get; set; } = false;
        /// <summary>
        /// Использовать ли метод Парзена
        /// </summary>
        public bool IsParsenMethod { get; set; } = false;
        /// <summary>
        /// Ядро окна Парзена
        /// </summary>
        public Func<double, double> KernelParsenWindow { get; set; }
        /// <summary>
        /// Функция измерения расстояния
        /// </summary>
        public Func<Vector, Vector, double> Dist { get; set; }
        /// <summary>
        /// Набор данных
        /// </summary>
        public StructClasses Classes { get; set; }
        #endregion

        #region Конструкторы
        /// <summary>
        /// Метод k-ближайших соседей
        /// </summary>
        public KNNCl()
        {
            Classes = new StructClasses();
            KernelParsenWindow = RbfK;
            Dist = Distances.BaseDist.SquareEucl;

        }
        /// <summary>
        /// Метод k-ближайших соседей
        /// </summary>
        public KNNCl(VectorIntDataset vectorClasses)
        {
            Classes = new StructClasses();
            KernelParsenWindow = RbfK;
            Dist = Distances.BaseDist.SquareEucl;
            K = 4;
            H = 1;

            foreach (VectorClass item in vectorClasses)
            {
                AddClass(item.Features, item.ClassMark);
            }
        }
        #endregion

        /// <summary>
        /// Радиально-базисная функция окна
        /// </summary>
        public double RbfK(double r)
        {
            return Math.Exp(-2 * r * r);
        }
        /// <summary>
        /// Метод k-ближайших соседей
        /// </summary>
        /// <param name="classifikator">Коллекция классов</param>
        public KNNCl(StructClasses classifikator)
        {
            Classes = classifikator;
        }
        /// <summary>
        /// Распознавание
        /// </summary>
        /// <param name="inp">Вектор входа</param>
        public int Classify(Vector inp)
        {
            return ClassifyProbVector(inp).MaxElementIndex();
        }
        /// <summary>
        /// Добавить класс
        /// </summary>
        /// <param name="features">Вектор признаков</param>
        /// <param name="num">Метка </param>
        public void AddClass(Vector features, int num)
        {
            VectorClass structClass = new VectorClass
            {
                Features = features.Clone(),
                ClassMark = num
            };
            Classes.Add(structClass);
            _count = GetN();
        }
        /// <summary>
        /// Распознавание вектора, представить вектором распределения вероятностей
        /// </summary>
        public Vector ClassifyProbVector(Vector inp)
        {
            if (IsFixed && IsParsenMethod)
            {
                Vector classes = new Vector(_count);
                Rang(inp);

                for (int i = 0; i < Classes.Count; i++)
                {
                    classes += ToVector(i);
                }

                classes /= classes.Sum();


                return classes;
            }

            else
            {
                Vector classes = new Vector(_count);
                Rang(inp);
                int k = Classes.Count < K? Classes.Count : K;
                H = Classes[k - 1].R;

                for (int i = 0; i < K; i++)
                {
                    classes += ToVector(i);
                }

                classes /= classes.Sum();


                return classes;
            }

        }
        /// <summary>
        /// Возвращает вектор, его длина равна количеству классов, на позиции распознанного класса установлено значение 1
        /// </summary>
        /// <param name="inp">Вектор входных данных</param>
        public Vector RecognizeVectorMax(Vector inp)
        {
            if (IsFixed && IsParsenMethod)
            {
                Vector classes = new Vector(_count);
                Rang(inp);

                for (int i = 0; i < Classes.Count; i++)
                {
                    classes += ToVector(i);
                }

                classes /= classes.Max();


                return classes;
            }

            else
            {
                Vector classes = new Vector(_count);
                Rang(inp);
                H = Classes[K - 1].R;

                for (int i = 0; i < K; i++)
                {
                    classes += ToVector(i);
                }

                classes /= classes.Max();


                return classes;
            }

        }
        /// <summary>
        /// Обучение классификатора
        /// </summary>
        /// <param name="features">Признаки</param>
        /// <param name="classes">Метки классов</param>
        public void Train(Vector[] features, int[] classes)
        {
            if (features.Length != classes.Length)
            {
                throw new InvalidOperationException("Число вектров признаков и число меток классов не совпадают");
            }

            for (int i = 0; i < features.Length; i++)
            {
                AddClass(features[i], classes[i]);
            }
        }
        /// <summary>
        /// Обучение на базе набора данных вектор-класс
        /// </summary>
        /// <param name="dataset">Набор данных Vector-int32</param>
        public void Train(VectorIntDataset dataset)
        {
            for (int i = 0; i < dataset.Count; i++)
            {
                AddClass(dataset[i].Features, dataset[i].ClassMark);
            }
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Vector ToVector(int i)
        {
            int index = i < Classes.Count-1?i:Classes.Count-1;
            int mark = Classes[index].ClassMark;
            Vector outp = new Vector(_count);

            if (IsParsenMethod)
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
        // Ранжирование
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Rang(Vector inp)
        {

            for (int i = 0; i < Classes.Count; i++)
            {
                Classes[i].R = Dist(inp, Classes[i].Features); // Вычисление билжайшего центра
            }

            Classes.Sort((a, b) => a.R.CompareTo(b.R));
        }

        // Получение числа классов
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetN()
        {
            List<int> indexis = new List<int>();

            for (int i = 0; i < Classes.Count; i++)
            {
                indexis.Add(Classes[i].ClassMark);
            }

            return indexis.Max() + 1;

        }
        #endregion
    }
}
