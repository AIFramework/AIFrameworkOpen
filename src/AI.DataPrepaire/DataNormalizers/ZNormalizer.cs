using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AI.DataPrepaire.DataNormalizers
{
    /// <summary>
    /// Z - нормализация (Мат. ожидание = 0, СКО = 1)
    /// </summary>
    [Serializable]
    public class ZNormalizer : Normalizer
    {
        /// <summary>
        /// Среднее
        /// </summary>
        public double[] Mean { get; set; }
        /// <summary>
        /// СКО
        /// </summary>
        public double[] Std { get; set; }

        private double _eps = 1e-200;


        /// <summary>
        /// Обучение преобразователя
        /// </summary>
        /// <param name="data">Набор данных</param>
        public override void Train(IEnumerable<IAlgebraicStructure<double>> data)
        {
            IAlgebraicStructure<double>[] algebraicStructures = (data is IAlgebraicStructure<double>[]) ? data as IAlgebraicStructure<double>[] : data.ToArray();
            Mean = new double[algebraicStructures[0].Shape.Count];
            Std = new double[algebraicStructures[0].Shape.Count];

            for (int i = 0; i < algebraicStructures.Length; i++)
            {
                double[] dat = algebraicStructures[i].Data;

                for (int j = 0; j < Std.Length; j++)
                {
                    Mean[j] += dat[j];
                    Std[j] += dat[j] * dat[j];
                }
            }

            for (int i = 0; i < Std.Length; i++)
            {
                Mean[i] /= algebraicStructures.Length;
                Std[i] = Std[i] == 0 ? _eps : (Std[i] / algebraicStructures.Length) - (Mean[i] * Mean[i]);
                Std[i] = Math.Sqrt(Std[i]);
            }
        }

        /// <summary>
        /// Использование преобразователя (Перезапись значений алгебраической структуры)
        /// </summary>
        public override IAlgebraicStructure<double> Transform(IAlgebraicStructure<double> data)
        {
            if (Mean == null) return data;

            if (data is Vector)
            {
                return ((data as Vector) - Mean) / Std;
            }

            IAlgebraicStructure<double> dat = data;

            for (int i = 0; i < data.Data.Length; i++)
                dat.Data[i] = (data.Data[i] - Mean[i]) / Std[i];

            return dat;

        }

        /// <summary>
        /// Восстановление нормализованных данных (Перезапись значений алгебраической структуры)
        /// </summary>
        /// <param name="normalizeData">Нормализованные данные</param>
        public override IAlgebraicStructure<double> Denormalize(IAlgebraicStructure<double> normalizeData)
        {
            if (Mean == null) return normalizeData;

            if (normalizeData is Vector)
            {
                return ((normalizeData as Vector) * Std) + Mean;
            }

            IAlgebraicStructure<double> dat = normalizeData;

            for (int i = 0; i < normalizeData.Data.Length; i++)
                dat.Data[i] = (normalizeData.Data[i] * Std[i]) + Mean[i];

            return dat;
        }
    }
}
