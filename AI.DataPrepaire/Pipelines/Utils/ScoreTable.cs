using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.DataPrepaire.Pipelines.Utils
{

    /// <summary>
    /// Таблица результатов
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class ScoreTableClassifier<T> : List<ScoreElCl<T>>
    {
        /// <summary>
        /// Таблица результатов
        /// </summary>
        public ScoreTableClassifier() : base() { }

        /// <summary>
        /// Таблица результатов
        /// </summary>
        public ScoreTableClassifier(IEnumerable<ScoreElCl<T>> data) : base(data) { }

        /// <summary>
        /// Таблица результатов
        /// </summary>
        public ScoreTableClassifier(ScoreTableClassifier<T> model) : base(model) { }

        /// <summary>
        /// Таблица результатов
        /// </summary>
        public ScoreTableClassifier(int cap):base(cap) { }

        /// <summary>
        /// Возвращает Top-k
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public ScoreElCl<T>[] TopK(int k = 3)
        {
            ScoreElCl<T>[] scores = new ScoreElCl<T>[k];
            var datOrder = this.OrderByDescending(x => x.Score); // Сортировка по уменьшению скора

            for (int i = 0; i < k; i++)
                scores[i] = this[i];

            return scores;
        }


        /// <summary>
        /// Возвращает Top-k
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public ScoreTableClassifier<T> TopKToTbl(int k = 3)
        {
            return new ScoreTableClassifier<T>(TopK(k));
        }

        /// <summary>
        /// Возвращает датасет
        /// </summary>
        /// <returns></returns>
        public ObjectClassifierPipeline<T>.DatasetForClassifier GetDataset()
        {
            var dataset = new ObjectClassifierPipeline<T>.DatasetForClassifier();

            foreach (var item in this)
            {
                for (int i = 0; i < item.States.Length; i++)
                {
                    dataset.Add(item.States[i], item.Actions[i]);
                }
            }

            return dataset;
        }



    }


    /// <summary>
    /// Партия и ее результат
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class ScoreElCl<T> 
    {
        /// <summary>
        /// Состояния
        /// </summary>
        public T[] States { get; set; }

        /// <summary>
        /// Классы
        /// </summary>
        public int[] Actions { get; set; } 

        /// <summary>
        /// Оценка качества партии (Reward)
        /// </summary>
        public double Score { get; set; }
    }
}
