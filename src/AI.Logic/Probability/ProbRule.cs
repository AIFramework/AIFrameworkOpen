using AI.Logic.Data;
using System;
using System.Text;

namespace AI.Logic.Probability
{
    /// <summary>
    /// Вероятностное правило
    /// </summary>
    [Serializable]
    public class ProbRule<ImageType, ElementType>
    {
        /// <summary>
        /// Условие срабатывания
        /// </summary>
        public Condition<ImageType, ElementType> ConditionRule { get; set; }

        /// <summary>
        /// Путь для срабатывания
        /// </summary>
        public PathImplication Path {  get; set; }

        /// <summary>
        /// Следствие
        /// </summary>
        public DataMutation<ImageType, ElementType> Then { get; set; }
    }
}
