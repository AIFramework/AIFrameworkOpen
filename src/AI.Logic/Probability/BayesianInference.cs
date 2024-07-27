using System;
using System.Collections.Generic;

namespace AI.Logic.Probability
{
    /// <summary>
    /// Класс для байесовского вывода.
    /// </summary>
    [Serializable]
    public class BayesianInference
    {
        /// <summary>
        /// Вероятностное пространство.
        /// </summary>
        private Dictionary<string, double> probabilities = new Dictionary<string, double>();

        /// <summary>
        /// Устанавливает априорные вероятности.
        /// </summary>
        /// <param name="event">Событие.</param>
        /// <param name="probability">Вероятность события.</param>
        public void SetPrior(string @event, double probability)
        {
            probabilities[@event] = probability;
        }

        /// <summary>
        /// Возвращает априорную вероятность события.
        /// </summary>
        /// <param name="event">Событие.</param>
        /// <returns>Априорная вероятность события.</returns>
        public double GetPrior(string @event)
        {
            return probabilities.ContainsKey(@event) ? probabilities[@event] : 0.0;
        }

        /// <summary>
        /// Обновляет вероятность события на основе новых данных с использованием теоремы Байеса.
        /// </summary>
        /// <param name="likelihoods">Вероятности доказательства при различных гипотезах.</param>
        /// <param name="hypotheses">Гипотезы и их априорные вероятности.</param>
        public void UpdateProbabilities(Dictionary<string, double> likelihoods, Dictionary<string, double> hypotheses)
        {
            double evidenceProbability = 0.0;

            foreach (var hypothesis in hypotheses)
            {
                if (likelihoods.ContainsKey(hypothesis.Key))
                {
                    evidenceProbability += likelihoods[hypothesis.Key] * hypothesis.Value;
                }
            }

            foreach (var hypothesis in hypotheses)
            {
                if (likelihoods.ContainsKey(hypothesis.Key))
                {
                    double prior = hypothesis.Value;
                    double likelihood = likelihoods[hypothesis.Key];
                    double posterior = (likelihood * prior) / evidenceProbability;
                    probabilities[hypothesis.Key] = posterior;
                }
            }
        }

        /// <summary>
        /// Возвращает апостериорную вероятность гипотезы.
        /// </summary>
        /// <param name="hypothesis">Гипотеза.</param>
        /// <returns>Апостериорная вероятность гипотезы.</returns>
        public double GetPosterior(string hypothesis)
        {
            return probabilities.ContainsKey(hypothesis) ? probabilities[hypothesis] : 0.0;
        }
    }
}
