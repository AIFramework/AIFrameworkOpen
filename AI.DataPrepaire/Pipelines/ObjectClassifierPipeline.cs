using AI.DataPrepaire.DataNormalizers;
using AI.DataPrepaire.FeatureExtractors;
using AI.DataStructs.Algebraic;
using AI.ML.Classifiers;
using AI.ML.DataSets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.DataPrepaire.Pipelines
{
    public abstract class ObjectClassifierPipeline<T>
    {
        /// <summary>
        // Извлечение признаков из данных
        /// </summary>
        public IFeaturesExtractor<T> Extractor { get; set; }

        /// <summary>
        /// Нормализация данных
        /// </summary>
        public INormalizer Normalizer { get; set; }

        /// <summary>
        /// Классификатор
        /// </summary>
        public IClassifier Classifier { get; set; }

        /// <summary>
        /// Конвейер обработки данных, преобразование объекта в вектор
        /// </summary>
        public ObjectClassifierPipeline()
        {
            
        }

        /// <summary>
        /// Запуск классификатора
        /// </summary>
        /// <param name="input">Вход</param>
        public virtual int Classify(T input)
        {
            return Classifier.Classify(
                (Vector)Normalizer.Transform(
                    Extractor.GetFeatures(input)));
        }

        /// <summary>
        /// Запуск классификатора (Возвращает вектор)
        /// </summary>
        /// <param name="input">Вход</param>
        public virtual Vector ClassifyProb(T input)
        {
            return Classifier.ClassifyProbVector(
                (Vector)Normalizer.Transform(
                    Extractor.GetFeatures(input)));
        }


        /// <summary>
        /// Обучение ковейера
        /// </summary>
        /// <param name="datas"></param>
        /// <param name="marks"></param>
        public virtual void Train(T[] datas, int[] marks)
        {
            Vector[] features = new Vector[datas.Length];

            // Извлечение признаков
            for (int i = 0; i < datas.Length; i++)
                features[i] = Extractor.GetFeatures(datas[i]);
            
            // Обучение нормализатора
            Normalizer.Train(features);
            features = (Vector[])Normalizer.Transform(features); // Нормализация

            // Обучение классификатора
            Classifier.Train(features, marks);
        }
    }
}
