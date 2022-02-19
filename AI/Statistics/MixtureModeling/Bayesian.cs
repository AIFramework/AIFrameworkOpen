﻿using AI.DataStructs.Algebraic;
using AI.Statistics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.Statistics.MixtureModeling
{
    public class Bayesian
    {

        IDistributionWithoutParams[] _distributions;
        Vector _apriori;

        public Bayesian(IEnumerable<IDistributionWithoutParams> distributions, Vector apriori)
        {
            _distributions = distributions.ToArray();
            _apriori = apriori;
        }

        public int[] GetIndicators(Vector inps)
        {
            int[] indicators = new int[inps.Count];
            for (int i = 0; i < inps.Count; i++)
            {
                indicators[i] = LogArgmax1D(inps[i]);
            }
            return indicators;
        }

        /// <summary>
        /// Возвращает индикаторы
        /// </summary>
        /// <param name="inps">Входы</param>
        public int[] GetIndicators(Vector[] inps)
        {
            int[] indicators = new int[inps.Length];
            for (int i = 0; i < inps.Length; i++)
            {
                indicators[i] = LogArgmaxND(inps[i]);
            }
            return indicators;
        }

        /// <summary>
        /// Одномерная плотность вероятности
        /// </summary>
        /// <param name="inp">Вход</param>
        public int LogArgmax1D(double inp)
        {
            Vector logs = (new Vector(_apriori.Count)).TransformByIndex(i => _distributions[i].CulcLogProb(inp) * _apriori[i]);
            return logs.MaxElementIndex();
        }

        /// <summary>
        /// Многомерная плотность вероятности
        /// </summary>
        /// <param name="inp">Вход</param>
        public int LogArgmaxND(Vector inp)
        {
            Vector logs = (new Vector(_apriori.Count)).TransformByIndex(i => _distributions[i].CulcLogProb(inp) * _apriori[i]);
            return logs.MaxElementIndex();
        }

        public static Vector CalcAposteori(Vector conditionalProbabilities, Vector apriori) 
        {
            Vector vector_prob = conditionalProbabilities * apriori;
            double denominator = vector_prob.Sum();
            return vector_prob / denominator;
        }
        /// <summary>
        /// Моделирует argmax{log[f(x|Theta)] + log(w)}
        /// </summary>
        /// <param name="inp"></param>
        /// <param name="distribution">Родительская функция распределения</param>
        /// <param name="param_dist">Параметры распределения</param>
        /// <param name="apriori">Априорное расп</param>
        public static int LogArgmax1D(double inp, IDistribution distribution, Dictionary<string, double>[] param_dist, Vector apriori) 
        {
            Vector logs = new Vector(apriori.Count).TransformByIndex(i => distribution.CulcLogProb(inp, param_dist[i]) * apriori[i]);
            return logs.MaxElementIndex();
        }

        /// <summary>
        /// Многомерная плотность вероятности 
        /// </summary>
        /// <param name="inp">Вход</param>
        public static int LogArgmaxND(Vector inp, IDistribution distribution, Dictionary<string, Vector>[] param_dist, Vector apriori)
        {
            Vector logs = new Vector(apriori.Count).TransformByIndex(i => distribution.CulcLogProb(inp, param_dist[i]) * apriori[i]);
            return logs.MaxElementIndex();
        }

        /// <summary>
        /// Расчет индикаторов
        /// </summary>
        /// <param name="inps"></param>
        /// <param name="distribution"></param>
        /// <param name="param_dist"></param>
        /// <param name="apriori"></param>
        /// <returns></returns>
        public static int[] GetIndicators(Vector inps, IDistribution distribution, Dictionary<string, double>[] param_dist, Vector apriori)
        {
            int[] indicators = new int[inps.Count];
            for (int i = 0; i < inps.Count; i++)
            {
                indicators[i] = LogArgmax1D(inps[i], distribution, param_dist, apriori);
            }
            return indicators;
        }

        /// <summary>
        /// Возвращает индикаторы
        /// </summary>
        /// <param name="inps">Входы</param>
        public static int[] GetIndicators(Vector[] inps, IDistribution distribution, Dictionary<string, Vector>[] param_dist, Vector apriori)
        {
            int[] indicators = new int[inps.Length];
            for (int i = 0; i < inps.Length; i++)
            {
                indicators[i] = LogArgmaxND(inps[i], distribution, param_dist, apriori);
            }
            return indicators;
        }

    }
}
