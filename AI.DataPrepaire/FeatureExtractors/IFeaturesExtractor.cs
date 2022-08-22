using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.DataPrepaire.FeatureExtractors
{
    /// <summary>
    /// Извлечение признаков из объектов
    /// </summary>
    public interface IFeaturesExtractor<T>
    {
        /// <summary>
        /// Получение признаков из данных
        /// </summary>
        Vector GetFeatures(T data);

        /// <summary>
        /// Получение признаков из данных
        /// </summary>
        Vector[] GetFeatures(T[] data);

    }
}
