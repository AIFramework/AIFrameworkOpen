using AI.DataStructs.Algebraic;

namespace AI.Fuzzy.Fuzzyficators.FVector
{
    /// <summary>
    /// Интерфейс векторного фаззификатора
    /// </summary>
    public interface IFuzzyficatorVector
    {
        /// <summary>
        /// Фаззификация
        /// </summary>
        /// <param name="value">Значение</param>
        Vector Fuzzyfication(Vector value);
        /// <summary>
        /// Дефаззификация
        /// </summary>
        /// <param name="valueF">Нечеткое значение</param>
        Vector DeFuzzyfication(Vector valueF);
    }
}

