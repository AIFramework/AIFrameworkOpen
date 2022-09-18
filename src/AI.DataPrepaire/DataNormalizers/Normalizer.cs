using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public abstract void Train(IEnumerable<IAlgebraicStructure<double>> data);

        /// <summary>
        /// Использование преобразователя (Перезапись значений алгебраической структуры)
        /// </summary>
        public abstract IAlgebraicStructure<double> Transform(IAlgebraicStructure<double> data);


        /// <summary>
        /// Использование преобразователя (Перезапись значений алгебраической структуры)
        /// </summary>
        public virtual IAlgebraicStructure<double>[] Transform(IEnumerable<IAlgebraicStructure<double>> data)
        {
            IAlgebraicStructure<double>[] algebraicStructures = (data is IAlgebraicStructure<double>[]) ? data as IAlgebraicStructure<double>[] : data.ToArray();

            for (int i = 0; i < algebraicStructures.Length; i++)
                algebraicStructures[i] = Transform(algebraicStructures[i]);

            return algebraicStructures;
        }

        /// <summary>
        /// Восстановление нормализованных данных (Перезапись значений алгебраической структуры)
        /// </summary>
        /// <param name="normalizeData">Нормализованные данные</param>
        public abstract IAlgebraicStructure<double> Denormalize(IAlgebraicStructure<double> normalizeData);

        /// <summary>
        /// Восстановление нормализованных данных (Перезапись значений алгебраической структуры)
        /// </summary>
        /// <param name="normalizeData">Нормализованные данные</param>
        public virtual IAlgebraicStructure<double>[] Denormalize(IEnumerable<IAlgebraicStructure<double>> normalizeData)
        {
            IAlgebraicStructure<double>[] algebraicStructures = (normalizeData is IAlgebraicStructure<double>[]) ? normalizeData as IAlgebraicStructure<double>[] : normalizeData.ToArray();

            for (int i = 0; i < algebraicStructures.Length; i++)
                algebraicStructures[i] = Denormalize(algebraicStructures[i]);

            return algebraicStructures;
        }
    }
}
