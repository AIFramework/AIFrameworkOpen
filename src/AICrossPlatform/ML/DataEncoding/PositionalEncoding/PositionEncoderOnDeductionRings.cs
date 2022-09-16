using AI.DataStructs.Algebraic;
using System;

namespace AI.ML.DataEncoding.PositionalEncoding
{
    /// <summary>
    /// Кодирование позиции в вектор на базе колец вычетов
    /// </summary>
    [Serializable]
    public class PositionEncoderOnDeductionRings : IPositionEncoding
    {
        /// <summary>
        /// Модуль кольца - размерность вектора
        /// </summary>
        public int Dim { get; set; }

        /// <summary>
        /// Кодирование позиции в вектор на базе колец вычетов
        /// </summary>
        public PositionEncoderOnDeductionRings(int dim)
        {
            Dim = dim;
        }

        /// <summary>
        /// Код(вектор) позиции 
        /// </summary>
        public Vector GetCode(int position)
        {
            Vector outp = new Vector(Dim)
            {
                [position % Dim] = 1
            };
            return outp;
        }

        /// <summary>
        /// Код(вектор) позиции 
        /// </summary>
        public Vector GetCode(double position)
        {
            return GetCode((int)position);
        }
    }
}
