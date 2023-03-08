using AI.DataStructs.Algebraic;
using AI.Fuzzy.Fuzzyficators.FVector;
using AI.ML.MatrixUtils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace AI.Fuzzy
{
    /// <summary>
    /// Заключение
    /// </summary>
    [Serializable]
    public class Conclusion : BaseVar
    {
        /// <summary>
        /// Вес
        /// </summary>
        public double W { get; set; }
    }

    /// <summary>
    /// Активированные множества
    /// </summary>
    [Serializable]
    public class ActivatedFuzzySet : ClassicFuzzySet
    {
        /// <summary>
        /// Истинность
        /// </summary>
        public double TruthDegree { get; set; } = 0;
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class BaseVar 
    {
        /// <summary>
        /// Нечеткая переменная
        /// </summary>
        public FuzzyVariable Variable { get; set; }

        /// <summary>
        /// Нечеткое множество
        /// </summary>
        public ClassicFuzzySet Term { get; set; }
    }

    /// <summary>
    /// Нечеткое условие
    /// </summary>
    [Serializable]
    public class FuzzyCondition : BaseVar
    {
       
    }

    /// <summary>
    /// Классическое нечеткое правило
    /// </summary>
    [Serializable]
    public class FuzzyRuleClassic
    {
        /// <summary>
        /// Нечеткие условия
        /// </summary>
        public FuzzyCondition[] FuzzyConditions { get; set; }

        /// <summary>
        /// Заключения
        /// </summary>
        public Conclusion[] Conclusions { get; set; }
    }


    /// <summary>
    /// Классическое нечеткое множество
    /// </summary>
    [Serializable]
    public class ClassicFuzzySet
    {
        /// <summary>
        /// Функция фаззификации
        /// </summary>
        public Func<double, double> Fuzzyficator { get; set; }
    }

    /// <summary>
    /// Нечеткая переменная
    /// </summary>
    [Serializable]
    public class FuzzyVariable 
    {
        /// <summary>
        /// Нечеткие множества
        /// </summary>
        public HashSet<ClassicFuzzySet> Terms;
    }
}
