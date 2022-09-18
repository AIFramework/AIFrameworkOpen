using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AI.DataPrepaire.DataNormalizers
{
    /// <summary>
    /// Минимаксная нормализация
    /// </summary>
    [Serializable]
    public class MinimaxNomalizer : Normalizer
    {
        public double[] Min { get; set; }
        public double[] Max { get; set; }
        public double[] MaxMinusMin { get; set; }


        /// <summary>
        /// Восстановление нормализованных данных (Перезапись значений алгебраической структуры)
        /// </summary>
        /// <param name="normalizeData">Нормализованные данные</param>
        public override IAlgebraicStructure<double> Denormalize(IAlgebraicStructure<double> normalizeData)
        {
            if (normalizeData is Vector)
            {
                return ((normalizeData as Vector) * MaxMinusMin) + Min;
            }

            IAlgebraicStructure<double> dat = normalizeData;

            for (int i = 0; i < normalizeData.Data.Length; i++)
                dat.Data[i] = (normalizeData.Data[i] * MaxMinusMin[i]) + Min[i];

            return dat;
        }

        /// <summary>
        /// Обучение преобразователя
        /// </summary>
        /// <param name="data">Набор данных</param>
        public override void Train(IEnumerable<IAlgebraicStructure<double>> data)
        {
            IAlgebraicStructure<double>[] algebraicStructures = (data is IAlgebraicStructure<double>[]) ? data as IAlgebraicStructure<double>[] : data.ToArray();
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
                    if (Min[j] > dat[j]) Min[j] = dat[j];
                    if (Max[j] < dat[j]) Max[j] = dat[j];
                }
            }

            for (int i = 0; i < Min.Length; i++)
            {
                MaxMinusMin[i] = Max[i] - Min[i];
                MaxMinusMin[i] = MaxMinusMin[i] == 0 ? double.Epsilon : MaxMinusMin[i];
            }
        }


        /// <summary>
        /// Использование преобразователя (Перезапись значений алгебраической структуры)
        /// </summary>
        public override IAlgebraicStructure<double> Transform(IAlgebraicStructure<double> data)
        {
            if (data is Vector)
            {
                return ((data as Vector) - Min) / MaxMinusMin;
            }

            IAlgebraicStructure<double> dat = data;

            for (int i = 0; i < data.Data.Length; i++)
                dat.Data[i] = (data.Data[i] - Min[i]) / MaxMinusMin[i];

            return dat;
        }

    }
}
