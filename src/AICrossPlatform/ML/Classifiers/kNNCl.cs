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
    /// Classifier (k-nearest neighbors method)
    /// </summary>
    [Serializable]
    public class KNNCl : IClassifier
    {
        #region Поля и свойства
        private int _count;

        /// <summary>
        /// Number of neighbors
        /// </summary>
        public int K { get; set; } = 4;
        /// <summary>
        /// Window width
        /// </summary>
        public double H { get; set; } = 1;
        /// <summary>
        /// Is the width fixed
        /// </summary>
        public bool IsFixed { get; set; } = false;
        /// <summary>
        /// Is the Parzen window in use
        /// </summary>
        public bool IsParsenMethod { get; set; } = false;
        /// <summary>
        /// Parzen window Kernel
        /// </summary>
        public Func<double, double> KernelParsenWindow { get; set; }
        /// <summary>
        /// Distance function
        /// </summary>
        public Func<Vector, Vector, double> Dist { get; set; }
        /// <summary>
        /// Данныеset
        /// </summary>
        public StructClasses Classes { get; set; }
        #endregion

        #region Конструкторы
        /// <summary>
        /// Classifier (Method of k-nearest neighbors)
        /// </summary>
        public KNNCl()
        {
            Classes = new StructClasses();
            KernelParsenWindow = RbfK;
            Dist = Distances.BaseDist.SquareEucl;

        }
        /// <summary>
        /// Classifier (Method of k-nearest neighbors)
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
        /// Radial basis kernel for the Parzen window
        /// </summary>
        public double RbfK(double r)
        {
            return Math.Exp(-2 * r * r);
        }
        /// <summary>
        /// Classifier (Method of k-nearest neighbors)
        /// </summary>
        /// <param name="classifikator">Collection of classes</param>
        public KNNCl(StructClasses classifikator)
        {
            Classes = classifikator;
        }
        /// <summary>
        /// Recognition
        /// </summary>
        /// <param name="inp">Input vector</param>
        public int Classify(Vector inp)
        {
            return ClassifyProbVector(inp).MaxElementIndex();
        }
        /// <summary>
        /// Adding a class
        /// </summary>
        /// <param name="features">Feature vector</param>
        /// <param name="num">Label </param>
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
        /// Recognizing a vector, the result is a vector of probabilities
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
                H = Classes[K - 1].R;

                for (int i = 0; i < K; i++)
                {
                    classes += ToVector(i);
                }

                classes /= classes.Sum();


                return classes;
            }

        }
        /// <summary>
        /// Returns a vector, its length is the number of classes, the number of the required class is 1
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
                throw new InvalidOperationException("The number of feature vectors and the class method do not match");
            }

            for (int i = 0; i < features.Length; i++)
            {
                AddClass(features[i], classes[i]);
            }
        }
        /// <summary>
        /// Обучениеing a classifier based on the vector-label dataset
        /// </summary>
        /// <param name="dataset">Vector-label dataset</param>
        public void Train(VectorIntDataset dataset)
        {
            for (int i = 0; i < dataset.Count; i++)
            {
                AddClass(dataset[i].Features, dataset[i].ClassMark);
            }
        }
        /// <summary>
        /// Save to file
        /// </summary>
        /// <param name="path"></param>
        public void Save(string path)
        {
            BinarySerializer.Save(path, this);
        }
        /// <summary>
        /// Save to stream
        /// </summary>
        /// <param name="stream"></param>
        public void Save(Stream stream)
        {
            BinarySerializer.Save(stream, this);
        }
        /// <summary>
        /// Load from file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static KNNCl Load(string path)
        {
            return BinarySerializer.Load<KNNCl>(path);
        }
        /// <summary>
        /// Load from stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static KNNCl Load(Stream stream)
        {
            return BinarySerializer.Load<KNNCl>(stream);
        }
        /// <summary>
        /// Loading from csv file (features; class label)
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
            int mark = Classes[i].ClassMark;
            Vector outp = new Vector(_count);

            if (IsParsenMethod)
            {
                double r = Classes[i].R / H;
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
