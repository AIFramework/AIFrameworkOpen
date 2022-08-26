using AI.DataStructs.Algebraic;
using System;

namespace AI.ML
{
    [Serializable]
    public class EmbedingMatrix
    {
        public Vector[] Rows { get; private set; }

        public EmbedingMatrix(Vector[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                Rows[i] = data[i].Clone();
            }
        }

        public EmbedingMatrix(int countVectors = 5, int embedingDimention = 5)
        {
            Rows = new Vector[countVectors];
            double pikToPik = 2.0 / Math.Sqrt(embedingDimention);

            for (int i = 0; i < Rows.Length; i++)
            {
                Rows[i] = pikToPik * (Statistics.Statistic.Rand(embedingDimention) - 0.5);
            }
        }
    }
}
