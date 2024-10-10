using System;

namespace AI.Logic.DiscretMath
{
    /// <summary>
    /// Логическая переменная.
    /// </summary>
    /// <typeparam name="T">Тип значения переменной.</typeparam>
    [Serializable]
    public class LogicVar<T>
    {
        /// <summary>
        /// Значение переменной.
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// Значение логической функции принадлежности.
        /// </summary>
        public bool Mu { get; set; }

        /// <summary>
        /// Конструктор по умолчанию.
        /// </summary>
        public LogicVar() { }

        /// <summary>
        /// Конструктор с параметром значения.
        /// </summary>
        /// <param name="value">Значение переменной.</param>
        public LogicVar(T value)
        {
            Value = value;
        }

        /// <summary>
        /// Конструктор с параметрами значения и логической функции принадлежности.
        /// </summary>
        /// <param name="value">Значение переменной.</param>
        /// <param name="mu">Значение логической функции принадлежности.</param>
        public LogicVar(T value, bool mu) : this(value)
        {
            Mu = mu;
        }
    }

   
}

