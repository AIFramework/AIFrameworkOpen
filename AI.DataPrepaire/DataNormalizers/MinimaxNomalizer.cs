using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.DataPrepaire.DataNormalizers
{
    /// <summary>
    /// Минимаксная нормализация
    /// </summary>
    [Serializable]
    public class MinimaxNomalizer : INormalizer
    {
        public double[] Min { get; set; }
        public double[] Max { get; set; }
        public double[] MaxMinusMin { get; set; }

        /// <summary>
        /// Обучение преобразователя
        /// </summary>
        /// <param name="data">Набор данных</param>
        public void Train(IEnumerable<IAlgebraicStructure> data)
        {
            IAlgebraicStructure[] algebraicStructures = (data is IAlgebraicStructure[]) ? data as IAlgebraicStructure[] : data.ToArray();
            Min = new double[algebraicStructures[0].Shape.Count];
            Max = new double[algebraicStructures[0].Shape.Count];
            MaxMinusMin = new double[algebraicStructures[0].Shape.Count];

            // Инициализация
            double[] dat = algebraicStructures[0].Data;
            for (int i = 0; i < Min.Length; i++)
            {
                Min[i] = dat[i];
                Max[i] = dat[i];
            }


            for (int i = 1; i < algebraicStructures.Length; i++)
            {
                dat = algebraicStructures[i].Data;

                for (int j = 0; j < Min.Length; j++)
                {
                    if(Min[j] > dat[j]) Min[j] = dat[j];
                    if(Max[j] < dat[j]) Max[j] = dat[j];
                }
            }

            for (int i = 0; i < Min.Length; i++)
            {
                MaxMinusMin[i] = Max[i] - Min[i];
                MaxMinusMin[i] = MaxMinusMin[i] == 0? double.Epsilon : MaxMinusMin[i];
            }
        }


        /// <summary>
        /// Использование преобразователя (Перезапись значений алгебраической структуры)
        /// </summary>
        public IAlgebraicStructure Transform(IAlgebraicStructure data)
        {
            if (data is Vector)
            {
                return ((data as Vector) - Min) / MaxMinusMin;
            }

            IAlgebraicStructure dat = data;

            for (int i = 0; i < data.Data.Length; i++)
                dat.Data[i] = (data.Data[i] - Min[i]) / MaxMinusMin[i];

            return dat;
        }

        /// <summary>
        /// Использование преобразователя (Перезапись значений алгебраической структуры)
        /// </summary>
        public IAlgebraicStructure[] Transform(IEnumerable<IAlgebraicStructure> data)
        {
            IAlgebraicStructure[] algebraicStructures = (data is IAlgebraicStructure[]) ? data as IAlgebraicStructure[] : data.ToArray();

            for (int i = 0; i < algebraicStructures.Length; i++)
                algebraicStructures[i] = Transform(algebraicStructures[i]);

            return algebraicStructures;
        }
    }
}
