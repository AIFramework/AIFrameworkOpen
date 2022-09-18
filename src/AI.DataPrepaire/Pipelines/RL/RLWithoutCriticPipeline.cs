using AI.DataPrepaire.Pipelines.Utils;
using System;
using System.Collections.Generic;

namespace AI.DataPrepaire.Pipelines.RL
{
    /// <summary>
    /// Конвейер обучения с подкреплением  (без критика)
    /// </summary>
    [Serializable]
    public class RLWithoutCriticPipeline<T>
    {
        /// <summary>
        /// Актор, алгоритм принятия решений
        /// </summary>
        public ObjectClassifierPipeline<T> Actor { get; set; }

        /// <summary>
        /// Подкрепления и действия
        /// </summary>
        public ScoreTableClassifier<T> RewardData { get; set; } = new ScoreTableClassifier<T>();

        /// <summary>
        /// Состояния внутри партии
        /// </summary>
        public List<T> States { get; set; } = new List<T>();

        /// <summary>
        /// Принятые решения внутри партии
        /// </summary>
        public List<int> Actions { get; set; } = new List<int> { 0 };

        /// <summary>
        /// Модель обучения с подкреплением  (без критика)
        /// </summary>
        /// <param name="actor"></param>
        public RLWithoutCriticPipeline(ObjectClassifierPipeline<T> actor)
        {
            Actor = actor;
        }

        public RLWithoutCriticPipeline() { }

        /// <summary>
        /// Выполнить действие
        /// </summary>
        /// <param name="state">Состояние</param>
        public virtual int GetAction(T state, double conf = 0.0, double t = 1)
        {
            double sep = Actor.random.NextDouble();

            int action = (sep > conf) ? Actor.StoсhasticClassify(state, t) : Actor.Classify(state);

            States.Add(state);
            Actions.Add(action);

            return action;
        }

        /// <summary>
        /// Установка реварда / скора (Конец партии)
        /// </summary>
        public virtual void SetReward(double score)
        {
            ScoreElCl<T> scoreEl = new ScoreElCl<T>();
            scoreEl.Score = score;
            scoreEl.Actions = Actions.ToArray();
            scoreEl.States = States.ToArray();

            Actions = new List<int>();
            States = new List<T>();

            RewardData.Add(scoreEl);
        }

        /// <summary>
        /// Обучение
        /// </summary>
        /// <param name="topK">Топ К лучших партий</param>
        public virtual void Train(int topK = 3)
        {
            var dataset = RewardData.TopKToTbl(topK).GetDataset();
            Actor.Train(dataset.ReturnData(), dataset.ReturnClasses());
        }
    }
}
