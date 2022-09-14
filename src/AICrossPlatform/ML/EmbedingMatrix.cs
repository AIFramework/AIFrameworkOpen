using AI.DataStructs.Algebraic;
using System;

namespace AI.ML
{
    /// <summary>
    /// Матрица векторов встраивания
    /// </summary>
    [Serializable]
    public class EmbedingMatrix
    {
        /// <summary>
        /// Векторы встраивания, строки
        /// </summary>
        public Vector[] Rows { get; private set; }

        /// <summary>
        /// Матрица векторов встраивания
        /// </summary>
        public EmbedingMatrix(Vector[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
#pragma warning disable CS8602 // Разыменование вероятной пустой ссылки.
                Rows[i] = data[i].Clone();
#pragma warning restore CS8602 // Разыменование вероятной пустой ссылки.
            }
        }

        /// <summary>
        /// Матрица векторов встраивания
        /// </summary>
        public EmbedingMatrix(int countVectors = 5, int embedingDimention = 5)
        {
            Rows = new Vector[countVectors];
            double pikToPik = 2.0 / Math.Sqrt(embedingDimention);

            for (int i = 0; i < Rows.Length; i++)
            {
                Rows[i] = pikToPik * (Statistics.Statistic.UniformDistribution(embedingDimention) - 0.5);
            }
        }
    }
}
