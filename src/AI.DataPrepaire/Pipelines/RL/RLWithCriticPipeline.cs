using System;
using System.Collections.Generic;

namespace AI.DataPrepaire.Pipelines.RL
{
    /// <summary>
    /// Классификатор с обучением с подкреплением и с использованием критика
    /// </summary>
    [Serializable]
    public class RLWithCriticPipeline<T> : RLWithoutCriticPipeline<T>
    {
        /// <summary>
        /// Критик
        /// </summary>

        public CriticPipeline<T> Critic;

        /// <summary>
        /// Классификатор с обучением с подкреплением и с использованием критика
        /// </summary>
        public RLWithCriticPipeline() { }

        /// <summary>
        /// Классификатор с обучением с подкреплением и с использованием критика
        /// </summary>
        public RLWithCriticPipeline(ObjectClassifierPipeline<T> actor, CriticPipeline<T> critic)
        {
            Actor = actor;
            Critic = critic;
        }

        /// <summary>
        /// Оценка выигрыша в данной партии при помощи критика
        /// </summary>
        /// <param name="states">Состояния</param>
        public virtual double ExpendedReward(T[] states)
        {
            double reward = 0;
            for (int i = 0; i < states.Length; i++)
                reward += Critic.Predict(states[i]);

            return reward;
        }

        /// <summary>
        /// Обучение 
        /// </summary>
        /// <param name="topK"></param>
        public override void Train(int topK = 3)
        {
            CriticTrain(); // Обучение критика

            for (int i = 0; i < RewardData.Count; i++)
            {
                double expendReward = ExpendedReward(RewardData[i].States);
                // Перерасчет реварда с учетом критика
                RewardData[i].Score = (RewardData[i].Score - expendReward) / RewardData[i].States.Length;
            }

            base.Train(topK); // Обучение агента
        }

        /// <summary>
        /// Обучение критика
        /// </summary>
        public virtual void CriticTrain()
        {
            List<T> states = new List<T>();
            List<double> rewards = new List<double>();

            foreach (var item in RewardData)
            {
                states.AddRange(item.States);
                double meanReward = item.Score / item.States.Length;

                for (int i = 0; i < item.States.Length; i++)
                    rewards.Add(meanReward);
            }

            Critic.Train(states, rewards);
        }
    }
}
