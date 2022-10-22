/*
 * Created by SharpDevelop.
 * User: 01
 * Date: 11.09.2013
 * Time: 17:23
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace AI.NLP
{
    /// <summary>
    /// Данные вероятностного словаря
    /// </summary>
    [Serializable]
    public class ProbabilityDictionaryData<T>
    {
        /// <summary>
        /// Слово
        /// </summary>
		public T Word { get; set; }
        /// <summary>
        /// Вероятность встретить это слово
        /// </summary>
        public double Probability { get; set; }

    }
}
