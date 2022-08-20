using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.DataPrepaire.DataNormalizers
{
    /// <summary>
    /// Z - нормализация (Мат. ожидание = 0, СКО = 1)
    /// </summary>
    [Serializable]
    public class ZNormalizer : INormalizer
    {
        public double[] Mean { get; set; }
        public double[] Std { get; set; }

        public double Eps = 1e-200;


        /// <summary>
        /// Обучение преобразователя
        /// </summary>
        /// <param name="data">Набор данных</param>
        public void Train(IEnumerable<IAlgebraicStructure> data)
        {
            IAlgebraicStructure[] algebraicStructures = (data is IAlgebraicStructure[]) ? data as IAlgebraicStructure[] : data.ToArray();
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
                Std[i] = Std[i] == 0? Eps: Std[i]/algebraicStructures.Length - Mean[i] * Mean[i];
                Std[i] = Math.Sqrt(Std[i]);
            }
        }

        /// <summary>
        /// Использование преобразователя (Перезапись значений алгебраической структуры)
        /// </summary>
        public IAlgebraicStructure Transform(IAlgebraicStructure data)
        {
            if (data is Vector)
            {
                return ((data as Vector) - Mean) / Std;
            }

            IAlgebraicStructure dat = data;

            for (int i = 0; i < data.Data.Length; i++)    
                dat.Data[i] = (data.Data[i] - Mean[i]) / Std[i];

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
