using System;
using System.Collections.Generic;

namespace AI.Logic.Fuzzy
{

    /// <summary>
    /// Класс для нечеткого множества.
    /// </summary>
    /// <typeparam name="ElType">Тип элемента.</typeparam>
    /// <typeparam name="ImgType">Тип образа элемента.</typeparam>
    [Serializable]
    public class FuzzySet<ElType, ImgType> : List<FuzzySetElement<ElType, ImgType>>
    {
        
        /// <summary>
        /// Класс для нечеткого множества.
        /// </summary>
        public FuzzySet()
        {
            
        }

        /// <summary>
        /// Класс для нечеткого множества.
        /// </summary>
        public FuzzySet(IEnumerable<FuzzySetElement<ElType, ImgType>> fuzzySet)
        {
            foreach (var item in fuzzySet)
            {
                Add(item);
            }
        }

        /// <summary>
        /// Операция "НЕ" для нечеткого множества.
        /// </summary>
        /// <param name="w">Степень</param>
        /// <returns>Нечеткое множество после операции "НЕ".</returns>
        public FuzzySet<ElType, ImgType> Not(double w = 1)
        {
            var result = new FuzzySet<ElType, ImgType>();
            double root = 1.0 / w;

            foreach (var element in this)
            {
                result.Add(new FuzzySetElement<ElType, ImgType>
                {
                    ElementValue = element.ElementValue,
                    Image = element.Image,
                    Mu = Math.Pow(1.0 - Math.Pow(element.Mu, w), root)
                });
            }

            return result;
        }

    }

}
