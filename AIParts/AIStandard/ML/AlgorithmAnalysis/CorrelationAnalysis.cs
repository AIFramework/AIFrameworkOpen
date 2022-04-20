/*
 * Создано в SharpDevelop.
 * Пользователь: admin
 * Дата: 07.07.2018
 * Время: 11:38
 * 
 * Для изменения этого шаблона используйте меню "Инструменты | Параметры | Кодирование | Стандартные заголовки".
 */
using AI.DataStructs.Algebraic;
using System;

namespace AI.ML.AlgorithmAnalysis
{
    /// <summary>
    /// Correlation analysis (orthogonality check)
    /// </summary>
    [Serializable]
    public class CorrelationAnalysis
    {
        /// <summary>
        /// Normalized correlation matrix
        /// </summary>
        public Matrix CorMatrNorm { get; protected set; }


        /// <summary>
        /// Correlation analysis
        /// </summary>
        public CorrelationAnalysis(Matrix matrix)
        {
            Vector[] vectsCol = Matrix.GetColumns(matrix);
            CorMatrNorm = Matrix.GetCorrelationMatrixNorm(vectsCol);
        }

        /// <summary>
        /// Average Orthogonality Coefficient
        /// </summary>
        public double MeanOrtog()
        {
            double mean = Statistics.Statistic.ExpectedValueAbsNotCheckNaN(CorMatrNorm);
            mean = (mean - CorMatrNorm.Height) / (CorMatrNorm.Height * CorMatrNorm.Height - CorMatrNorm.Height);
            return 1.0 - mean;
        }

        /// <summary>
        /// Determinant of the correlation matrix (one of the multicollinearity indicators)
        /// </summary>
        public double CorMatrDeterm()
        {
            return CorMatrNorm.Determinant;
        }
    }
}
