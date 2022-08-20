using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.DataPrepaire.DataNormalizers
{
    /// <summary>
    /// Нормализатор данных
    /// </summary>
    public interface INormalizer
    {
        /// <summary>
        /// Обучение преобразователя
        /// </summary>
        /// <param name="data">Набор данных</param>
        void Train(IEnumerable<IAlgebraicStructure> data);

        /// <summary>
        /// Использование преобразователя (Перезапись значений алгебраической структуры)
        /// </summary>
        IAlgebraicStructure Transform(IAlgebraicStructure data);


        /// <summary>
        /// Использование преобразователя (Перезапись значений алгебраической структуры)
        /// </summary>
        IAlgebraicStructure[] Transform(IEnumerable<IAlgebraicStructure> data);
    }
}
