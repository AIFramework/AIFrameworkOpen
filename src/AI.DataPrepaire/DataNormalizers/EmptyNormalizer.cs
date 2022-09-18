using AI.DataStructs.Algebraic;
using System.Collections.Generic;

namespace AI.DataPrepaire.DataNormalizers
{
    /// <summary>
    /// Нормализатор который ничего не делает
    /// </summary>
    public class EmptyNormalizer : Normalizer
    {
        public override IAlgebraicStructure<double> Denormalize(IAlgebraicStructure<double> normalizeData)
        {
            return normalizeData;
        }

        public override void Train(IEnumerable<IAlgebraicStructure<double>> data)
        {
        }

        public override IAlgebraicStructure<double> Transform(IAlgebraicStructure<double> data)
        {
            return data;
        }
    }
}
