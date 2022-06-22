using AI.DataStructs.Algebraic;
using System;

namespace AI.ML.DataEncoding.PositionalEncoding
{
    [Serializable]
    public class PositionEncoderOnDeductionRings : IPositionEncoding
    {
        public int Dim { get; set; }

        public PositionEncoderOnDeductionRings(int dim) 
        {
            Dim = dim;
        }

        public Vector GetCode(int position)
        {
            Vector outp = new Vector(Dim)
            {
                [position % Dim] = 1
            };
            return outp;
        }

        public Vector GetCode(double position)
        {
            return GetCode((int)position);
        }
    }
}
