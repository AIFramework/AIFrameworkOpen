using AI.DataPrepaire.DataNormalizers;
using AI.DataPrepaire.FeatureExtractors;
using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.DataPrepaire.Pipelines
{
    /// <summary>
    /// Конвейер обработки данных, преобразование объекта в вектор
    /// </summary>
    [Serializable]
    public abstract class Object2VectorPipeline<T>
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
        /// Преобразование вектор -> вектор
        /// </summary>
        public Func<Vector, Vector> Transformer;

        // Простое преобразование
        Vector SimpleTransform(Vector inp) { return inp; }


        /// <summary>
        /// Конвейер обработки данных, преобразование объекта в вектор
        /// </summary>
        public Object2VectorPipeline() 
        {
            Transformer = SimpleTransform;
        }

        /// <summary>
        /// Запуск преобразования
        /// </summary>
        /// <param name="input">Вход</param>
        public virtual Vector Run(T input)
        {
            return Transformer(
                (Vector)Normalizer.Transform(
                    Extractor.GetFeatures(input)));
        }


    }
}
