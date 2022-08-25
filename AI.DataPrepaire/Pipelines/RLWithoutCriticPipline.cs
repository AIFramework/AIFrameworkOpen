﻿using AI.DataPrepaire.Pipelines.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.DataPrepaire.Pipelines
{
    /// <summary>
    /// Конвейер обучения с подкреплением  (без критика)
    /// </summary>
    [Serializable]
    public class RLWithoutCriticPipline<T>
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
        public List<T> States { get; set; }

        /// <summary>
        /// Принятые решения внутри партии
        /// </summary>
        public List<int> Actions { get; set; }

        /// <summary>
        /// Модель обучения с подкреплением  (без критика)
        /// </summary>
        /// <param name="actor"></param>
        public RLWithoutCriticPipline(ObjectClassifierPipeline<T> actor) 
        {
            Actor = actor;
        }

        /// <summary>
        /// Выполнить действие
        /// </summary>
        /// <param name="state">Состояние</param>
        public int GetAction(T state) 
        {
            int action = Actor.StoсhasticClassify(state);

            States.Add(state);
            Actions.Add(action);
        
            return action;
        }

        /// <summary>
        /// Установка реварда / скора (Конец партии)
        /// </summary>
        public void SetReward(double score) 
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
        public void Train(int topK = 3) 
        {
            var dataset = RewardData.TopKToTbl(topK).GetDataset();
            Actor.Train(dataset.ReturnData(), dataset.ReturnClasses());
        }
    }
}
