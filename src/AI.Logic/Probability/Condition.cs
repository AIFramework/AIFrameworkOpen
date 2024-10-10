using AI.Logic.Fuzzy;
using System.Collections.Generic;

namespace AI.Logic.Probability
{
    /// <summary>
    /// Условие
    /// </summary>
    /// <typeparam name="ImageType"></typeparam>
    /// <typeparam name="ElementType"></typeparam>
    public class Condition<ImageType, ElementType> : List<FuzzySetGenerator<ImageType, ElementType>> 
    {
        /// <summary>
        /// Тип операции для проверки условия
        /// </summary>
        public CondOperations OperationType { get; set; } = CondOperations.And;
    }
}
