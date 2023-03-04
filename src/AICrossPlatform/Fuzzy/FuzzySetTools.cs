using System;
using System.Collections.Generic;
using System.Linq;

namespace AI.Fuzzy
{
    /// <summary>
    /// Инструменты для работы с нечеткими множествами
    /// </summary>
    [Serializable]
    public static class FuzzySetTools
    {
        /// <summary>
        /// Объединение нечетких множеств
        /// </summary>
        /// <param name="sets"></param>
        /// <returns></returns>
        public static Dictionary<string, double> Merge(IEnumerable<Dictionary<string, double>> sets)
        {
            Dictionary<string, double>[] setArr = (Dictionary<string, double>[]) sets.ToArray().Clone();
            Dictionary<string, double> result = new Dictionary<string, double>();

            foreach (var set in setArr)
                foreach (var setEl in set)
                    if (result.ContainsKey(setEl.Key)) result[setEl.Key] |= (FLV)setEl.Value;  // Слияние с помощью ИЛИ
                    else result.Add(setEl.Key, setEl.Value);

            return result;
        }
    }
}
