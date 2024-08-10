using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AI.ML.MetricsTools
{
    /// <summary>
    /// Класс ветви kd-дерево
    /// </summary>
    [Serializable]
    public class KDNode
    {
        /// <summary>
        /// Вектор признаков
        /// </summary>
        public Vector DataVector { get; }
        
        /// <summary>
        /// Метка класса
        /// </summary>
        public int Label { get; }
        
        /// <summary>
        /// Ссылка на левую ветку
        /// </summary>
        public KDNode Left { get; set; }
        
        /// <summary>
        /// Ссылка на правую ветку
        /// </summary>
        public KDNode Right { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="features">Признаки</param>
        /// <param name="label">Метка</param>
        public KDNode(IEnumerable<double> features, int label)
        {
            DataVector = features.ToArray();
            Label = label;
            Left = null;
            Right = null;
        }
    }
}
