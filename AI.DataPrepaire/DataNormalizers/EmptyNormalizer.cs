using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI.DataPrepaire.DataNormalizers
{
    /// <summary>
    /// Нормализатор который ничего не делает
    /// </summary>
    public class EmptyNormalizer : Normalizer
    {
        public override IAlgebraicStructure Denormalize(IAlgebraicStructure normalizeData)
        {
            return normalizeData;
        }

        public override void Train(IEnumerable<IAlgebraicStructure> data)
        {
        }

        public override IAlgebraicStructure Transform(IAlgebraicStructure data)
        {
            return data;
        }
    }
}
