using AI.DataStructs;
using AI.DataStructs.Algebraic;
using AI.ML.DataSets;
using AI.Statistics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AI.ML.Classifiers
{
    /// <summary>
    /// Корреляционный классификатор
    /// </summary>
    [Serializable]
    public class CorrelationClassifier : IClassifier
    {
        /// <summary>
        /// Классы
        /// </summary>
        public StructClasses Classes { get; set; }

        /// <summary>
        /// Корреляционный классификатор
        /// </summary>
        public CorrelationClassifier()
        {
            Classes = new StructClasses();
        }
        /// <summary>
        /// Корреляционный классификатор
        /// </summary>
        /// <param name="path">Путь до файла</param>
        public CorrelationClassifier(string path)
        {
            Classes = new StructClasses();
            Load(path);
        }
        /// <summary>
        /// Корреляционный классификатор
        /// </summary>
        /// <param name="classifikator">Классы</param>
        public CorrelationClassifier(StructClasses classifikator)
        {
            Classes = classifikator;
        }

        /// <summary>
        /// Добавить класс
        /// </summary>
        /// <param name="features">Вектор признаков</param>
        /// <param name="num">Метка</param>
        public void AddClass(Vector features, int num)
        {
            VectorClass structClass = new VectorClass
            {
                Features = features.Clone(),
                ClassMark = num
            };
            Classes.Add(structClass);
        }


        //TODO: Оптимизировать (убрать сортировку)
        /// <summary>
        /// Распознавание вектора
        /// </summary>
        /// <param name="inp">Вход</param>
        public int Classify(Vector inp)
        {
            for (int i = 0; i < Classes.Count; i++)
            {
                Classes[i].R = Statistic.CorrelationCoefficient(inp, Classes[i].Features); // Вычисление билжайшего центра	
            }
            Classes.Sort((a, b) => a.R.CompareTo(b.R) * -1);
            return Classes[0].ClassMark;
        }
        /// <summary>
        /// Распознавание вектора, представить вектором распределения вероятностей
        /// </summary>
        /// <param name="inp">Вектор входа</param>
        public Vector ClassifyProbVector(Vector inp)
        {
            List<int> indexis = new List<int>();

            for (int i = 0; i < Classes.Count; i++)
            {
                indexis.Add(Classes[i].ClassMark);
            }

            int Max = indexis.Max();

            Vector classes = new Vector(Max + 1)
            {
                [Classify(inp)] = Classes[0].R
            };

            return classes;

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
                throw new InvalidOperationException("The number of Вектор признаковs and the class method do not match");
            }

            for (int i = 0; i < features.Length; i++)
            {
                AddClass(features[i], classes[i]);
            }
        }
        /// <summary>
        /// Обучение классификатора
        /// </summary>
        /// <param name="dataset">Набор данных признаки-метка</param>
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
            BinarySerializer.Save(path, Classes);
        }
        /// <summary>
        /// Сохранить в поток
        /// </summary>
        /// <param name="stream"></param>
        public void Save(Stream stream)
        {
            BinarySerializer.Save(stream, this);
        }
        /// <summary>
        /// Загрузить из файла
        /// </summary>
        /// <param name="path">Путь до файла</param>
        public static CorrelationClassifier Load(string path)
        {
            return BinarySerializer.Load<CorrelationClassifier>(path);
        }
        /// <summary>
        /// Загрузить из потока
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static CorrelationClassifier Load(Stream stream)
        {
            return BinarySerializer.Load<CorrelationClassifier>(stream);
        }
    }
}
