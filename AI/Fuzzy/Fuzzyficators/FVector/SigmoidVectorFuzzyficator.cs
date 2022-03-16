using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.Fuzzy.Fuzzyficators.FVector
{
    /// <summary>
    /// Векторный фаззификатор на базе сигмоиды
    /// </summary>
    public class SigmoidVectorFuzzyficator : IFuzzyficatorVector
    {
        private readonly double _beta = 1;

        /// <summary>
        /// Векторный фаззификатор на базе сигмоиды
        /// </summary>
        /// <param name="beta">Наклон</param>
        public SigmoidVectorFuzzyficator(double beta = 1) 
        {
            _beta = beta;
        }

        /// <summary>
        /// Дефаззификация
        /// </summary>
        /// <param name="valueF">Нечеткое значение</param>
        public Vector DeFuzzyfication(Vector valueF)
        {
            return valueF.Transform(sigmoid_minus_one);
        }


        /// <summary>
        /// Фаззификация
        /// </summary>
        /// <param name="value">Значение</param>
        public Vector Fuzzyfication(Vector value)
        {
            return AI.HightLevelFunctions.ActivationFunctions.Sigmoid(value,_beta);
        }

        double sigmoid_minus_one(double v)
        {
            if (v == 0) return double.MinValue;
            if (v == 1) return double.MaxValue;
            return -Math.Log(1.0 / v - 1)/_beta;
        }
    }
}
