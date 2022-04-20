using AI.DataStructs;
using AI.DataStructs.Algebraic;
using AI.ML.DataSets;
using System;
using System.IO;

namespace AI.ML.Classifiers
{
    /// <summary>
    /// Базовый классификатор
    /// </summary>
    /// <typeparam name="T">Тип классификатора</typeparam>
    [Serializable]
    public class BaseClassifier<T> : IClassifier
    {
        /// <summary>
        /// Классификация
        /// </summary>
        /// <param name="inp">Вход</param>
        /// <exception cref="NotImplementedException"></exception>
        public virtual int Classify(Vector inp)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Классификация (вероятности)
        /// </summary>
        /// <param name="inp">Вход</param>
        public virtual Vector ClassifyProbVector(Vector inp)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Обучить
        /// </summary>
        public virtual void Train(Vector[] features, int[] classes)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Обучить
        /// </summary>
        public virtual void Train(VectorIntDataset dataset)
        {
            Vector[] features = new Vector[dataset.Count];
            int[] classes = new int[dataset.Count];

            for (int i = 0; i < features.Length; i++)
            {
                classes[i] = dataset[i].ClassMark;
                features[i] = dataset[i].Features;
            }

            Train(features, classes);
        }

        /// <summary>
        /// Сохранить
        /// </summary>
        /// <param name="path">Путь до файла</param>
        public virtual void Save(string path)
        {
            BinarySerializer.Save(path, this);
        }

        /// <summary>
        /// Сохранить
        /// </summary>
        /// <param name="stream">Поток</param>
        public virtual void Save(Stream stream)
        {
            BinarySerializer.Save(stream, this);
        }

        /// <summary>
        /// Загрузить
        /// </summary>
        /// <param name="path">Путь</param>
        public static T Load(string path)
        {
            return BinarySerializer.Load<T>(path);
        }

        /// <summary>
        /// Загрузить 
        /// </summary>
        /// <param name="stream">Поток</param>
        public static T Load(Stream stream)
        {
            return BinarySerializer.Load<T>(stream);
        }
    }
}
