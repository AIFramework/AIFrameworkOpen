using AI.DataStructs.Algebraic;
using System.Collections.Generic;

namespace AI.DataPrepaire.DataNormalizers
{
    /// <summary>
    /// Нормализатор который ничего не делает
    /// </summary>
    public class EmptyNormalizer : Normalizer
    {
        /// <summary>
        /// Восстановление данных
        /// </summary>
        /// <param name="normalizeData"></param>
        /// <returns></returns>
        public override IAlgebraicStructure<double> Denormalize(IAlgebraicStructure<double> normalizeData)
        {
            return normalizeData;
        }

        /// <summary>
        /// Ничего не делает
        /// </summary>
        public override void Train(IEnumerable<IAlgebraicStructure<double>> data)
        {
        }

        /// <summary>
        /// Преобразование данных
        /// </summary>
        public override IAlgebraicStructure<double> Transform(IAlgebraicStructure<double> data)
        {
            return data;
        }
    }
}
