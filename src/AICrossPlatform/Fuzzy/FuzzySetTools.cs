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
        public static Dictionary<T, double> Merge<T>(IEnumerable<Dictionary<T, double>> sets)
        {
            Dictionary<T, double>[] setArr = (Dictionary<T, double>[])sets.ToArray().Clone();
            Dictionary<T, double> result = new Dictionary<T, double>();

            foreach (var set in setArr)
                foreach (var setEl in set)
                    if (result.ContainsKey(setEl.Key)) result[setEl.Key] |= (FLV)setEl.Value;  // Слияние с помощью ИЛИ
                    else result.Add(setEl.Key, setEl.Value);

            return result;
        }

        /// <summary>
        /// Объединение 2х множеств
        /// </summary>
        /// <param name="set_1">Множество 1</param>
        /// <param name="set_2">Множество 2</param>
        /// <returns></returns>
        public static Dictionary<T, double> Add<T>(Dictionary<T, double> set_1, Dictionary<T, double> set_2)
        {
            Dictionary<T, double>[] sets = { set_1, set_2 };
            return Merge(sets);
        }

        /// <summary>
        /// Преобразование списка в нечеткое множество
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        public static Dictionary<T, double> Array2FuzzySet<T>(IEnumerable<T> set)
        {

            Dictionary<T, double> ret = new Dictionary<T, double>();

            foreach (var item in set)
                if (!ret.ContainsKey(item))
                    ret.Add(item, 1.0);

            return ret;
        }

        /// <summary>
        /// Сходство на базе модифицированной метрики Дайса
        /// </summary>
        public static double SetDiceSimModification(Dictionary<string, double> fuzzy_set1, Dictionary<string, double> fuzzy_set2)
        {
            double intersection_of_fuzzy_sets = 0;
            double sum_w_1 = 0;
            double sum_w_2 = 0;

            foreach (var item in fuzzy_set1)
            {
                double w = fuzzy_set1[item.Key];
                sum_w_1 += w;

                if (fuzzy_set2.ContainsKey(item.Key))
                    intersection_of_fuzzy_sets += Math.Sqrt(w * fuzzy_set2[item.Key]);
            }

            foreach (var item in fuzzy_set2)
                sum_w_2 += fuzzy_set2[item.Key];

            intersection_of_fuzzy_sets *= 2 / (sum_w_1 + sum_w_2);

            return intersection_of_fuzzy_sets;
        }

        /// <summary>
        /// Сходство на базе модифицированной метрики Дайса
        /// (функция асиммтрична, сравнивает только с ключами основного множества)
        /// </summary>
        public static double SetDiceSimModificationAsymmetric(Dictionary<string, double> main_set, Dictionary<string, double> fuzzy_set2)
        {
            double intersection_of_fuzzy_sets = 0;
            double sum_w_1 = 0;
            double sum_w_2 = 0;

            foreach (var item in main_set)
            {
                double w = main_set[item.Key];
                sum_w_1 += w;

                if (fuzzy_set2.ContainsKey(item.Key))
                    intersection_of_fuzzy_sets += Math.Sqrt(w * fuzzy_set2[item.Key]);
            }

            foreach (var item in main_set)
                if (fuzzy_set2.ContainsKey(item.Key))
                    sum_w_2 += fuzzy_set2[item.Key];

            intersection_of_fuzzy_sets *= 2 / (sum_w_1 + sum_w_2);

            return intersection_of_fuzzy_sets;
        }

        /// <summary>
        /// Все ли объекты множества являются типом T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="set"></param>
        /// <returns></returns>
        public static bool DicObjIsDicT<T>(Dictionary<object, double> set)
        {
            bool is_dic_t = true;

            foreach (var item in set)
                if (!(item.Key is T))
                {
                    is_dic_t = false;
                    break;
                }

            return is_dic_t;
        }

        /// <summary>
        /// Конвертирует все объекты множества в тип T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="set"></param>
        /// <returns></returns>
        public static Dictionary<T, double> SetConvert<T>(Dictionary<object, double> set)
        {
            Dictionary<T, double> new_set = new Dictionary<T, double>();

            foreach (var item in set)
                new_set.Add((T)item.Key, item.Value);

            return new_set;
        }

        /// <summary>
        /// Конвертирует все объекты множества в тип object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="set"></param>
        /// <returns></returns>
        public static Dictionary<object, double> SetConvertToObj<T>(Dictionary<T, double> set)
        {
            Dictionary<object, double> new_set = new Dictionary<object, double>();

            foreach (var item in set)
                new_set.Add(item.Key, item.Value);

            return new_set;
        }

    }
}
