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
    [Serializable]
    public abstract class Normalizer
    {
        /// <summary>
        /// Обучение преобразователя
        /// </summary>
        /// <param name="data">Набор данных</param>
        public abstract void Train(IEnumerable<IAlgebraicStructure> data);

        /// <summary>
        /// Использование преобразователя (Перезапись значений алгебраической структуры)
        /// </summary>
        public abstract IAlgebraicStructure Transform(IAlgebraicStructure data);


        /// <summary>
        /// Использование преобразователя (Перезапись значений алгебраической структуры)
        /// </summary>
        public virtual IAlgebraicStructure[] Transform(IEnumerable<IAlgebraicStructure> data) 
        {
            IAlgebraicStructure[] algebraicStructures = (data is IAlgebraicStructure[]) ? data as IAlgebraicStructure[] : data.ToArray();

            for (int i = 0; i < algebraicStructures.Length; i++)
                algebraicStructures[i] = Transform(algebraicStructures[i]);

            return algebraicStructures;
        }

        /// <summary>
        /// Восстановление нормализованных данных (Перезапись значений алгебраической структуры)
        /// </summary>
        /// <param name="normalizeData">Нормализованные данные</param>
        public abstract IAlgebraicStructure Denormalize(IAlgebraicStructure normalizeData);

        /// <summary>
        /// Восстановление нормализованных данных (Перезапись значений алгебраической структуры)
        /// </summary>
        /// <param name="normalizeData">Нормализованные данные</param>
        public virtual IAlgebraicStructure[] Denormalize(IEnumerable<IAlgebraicStructure> normalizeData) 
        {
            IAlgebraicStructure[] algebraicStructures = (normalizeData is IAlgebraicStructure[]) ? normalizeData as IAlgebraicStructure[] : normalizeData.ToArray();

            for (int i = 0; i < algebraicStructures.Length; i++)
                algebraicStructures[i] = Denormalize(algebraicStructures[i]);

            return algebraicStructures;
        }
    }
}
