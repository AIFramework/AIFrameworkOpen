using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Events
{
    /// <summary>
    /// Тип элемента отчета об обучении
    /// </summary>
    [Serializable]
    public class ReportElementType : IEquatable<ReportElementType>
    {
        /// <summary>
        /// Начало обучения
        /// </summary>
        public static readonly ReportElementType TrainingStarted = new ReportElementType("TrainingStarted");
        /// <summary>
        /// Окончание обучения
        /// </summary>
        public static readonly ReportElementType TrainingFinished = new ReportElementType("TrainingFinished");
        /// <summary>
        /// Остановка обучения
        /// </summary>
        public static readonly ReportElementType TrainingStopped = new ReportElementType("TrainingStopped");
        /// <summary>
        /// Начало обучения
        /// </summary>
        public static readonly ReportElementType TrainingCancelled = new ReportElementType("TrainingCancelled");
        /// <summary>
        /// Пройденные эпохи
        /// </summary>
        public static readonly ReportElementType EpochPassed = new ReportElementType("EpochPassed");
        /// <summary>
        /// Сохранение чекпоинта
        /// </summary>
        public static readonly ReportElementType CheckPointSaved = new ReportElementType("CheckPointSaved");

        /// <summary>
        /// Значение
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// Создание элемента отчета
        /// </summary>
        /// <param name="value"></param>
        public ReportElementType(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Равенство
        /// </summary>
        public static bool operator ==(ReportElementType left, ReportElementType right)
        {
            return left.Value == right.Value;
        }

        /// <summary>
        /// Не равенство
        /// </summary>
        public static bool operator !=(ReportElementType left, ReportElementType right)
        {
            return left.Value != right.Value;
        }

        #region Технические методы
        /// <summary>
        /// Строковое представление
        /// </summary>
        public override string ToString()
        {
            return $"ReportElementType[{Value}]";
        }
        /// <summary>
        /// Равенство
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj is ReportElementType elementType)
            {
                return Value == elementType.Value;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Равенство
        /// </summary>
        public bool Equals(ReportElementType other)
        {
            return Value == other.Value;
        }
        /// <summary>
        /// Хэш-код
        /// </summary>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
        #endregion
    }
}