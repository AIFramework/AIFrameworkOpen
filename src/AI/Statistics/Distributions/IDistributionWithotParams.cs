using AI.DataStructs.Algebraic;

namespace AI.Statistics.Distributions
{
    /// <summary>
    /// Интерфейс функции распределения без параметров
    /// </summary>
    public interface IDistributionWithoutParams
    {
        /// <summary>
        /// Рассчет функции распределения
        /// </summary>
        /// <param name="x">Вход</param>
        double CulcProb(Vector x);

        /// <summary>
        /// Рассчет функции распределения
        /// </summary>
        /// <param name="x">Вход</param>
        double CulcProb(double x);

        /// <summary>
        /// Рассчет логарифма функции распределения
        /// </summary>
        /// <param name="x">Вход</param>
        double CulcLogProb(double x);

        /// <summary>
        /// Рассчет логарифма функции распределения
        /// </summary>
        /// <param name="x">Вход</param>
        double CulcLogProb(Vector x);
    }
}
