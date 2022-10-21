using System;

namespace AI.ML.HMM
{
    /// <summary>
    /// Блок для сохранения
    /// </summary>
    [Serializable]
    public class MCFastModel
    {
        /// <summary>
        /// N-грамма
        /// </summary>
		public int[] Model { get; }
        /// <summary>
        /// Вероятность
        /// </summary>
        public double Probability { get; set; }

        /// <summary>
        /// Создание параметров для хранения марковской цепи
        /// </summary>
        /// <param name="model">Модель</param>
        /// <param name="probability">Вероятность</param>
        public MCFastModel(int[] model, double probability)
        {
            Model = model;
            Probability = probability;
        }
    }
}
