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
    /// Корреляционный анализ, проверка ортогональности
    /// </summary>
    [Serializable]
    public class CorrelationAnalysis
    {
        /// <summary>
        /// Проверка нормализованной ортогональной матрицы
        /// </summary>
        public Matrix CorMatrNorm { get; protected set; }


        /// <summary>
        /// Корреляционный анализ, проверка ортогональности
        /// </summary>
        public CorrelationAnalysis(Matrix matrix)
        {
            Vector[] vectsCol = Matrix.GetColumns(matrix);
            CorMatrNorm = Matrix.GetCorrelationMatrixNorm(vectsCol);
        }

        /// <summary>
        /// Средний коэффициент ортогональности
        /// </summary>
        public double MeanOrtog()
        {
            double mean = Statistics.Statistic.ExpectedValueAbsNotCheckNaN(CorMatrNorm);
            mean = (mean - CorMatrNorm.Height) / ((CorMatrNorm.Height * CorMatrNorm.Height) - CorMatrNorm.Height);
            return 1.0 - mean;
        }

        /// <summary>
        /// Определитель корреляционной матрицы (один из показателей мультиколлинеарности)
        /// </summary>
        public double CorMatrDeterm()
        {
            return CorMatrNorm.Determinant;
        }
    }
}
