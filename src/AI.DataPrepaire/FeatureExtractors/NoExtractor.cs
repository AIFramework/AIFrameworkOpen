using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.DataPrepaire.FeatureExtractors
{
    /// <summary>
    /// Заглушка для извлечения признаков
    /// </summary>
    [Serializable]
    public class NoExtractor : FeaturesExtractor<Vector>
    {
        /// <summary>
        /// Копирует вектор с входа на выход
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override Vector GetFeatures(Vector data)
        {
            return data;
        }
    }
}
