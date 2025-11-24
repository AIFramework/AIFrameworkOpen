/*
 * Created by SharpDevelop.
 * User: 01
 * Date: 08.04.2015
 * Time: 22:04
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using AI.DataStructs.Algebraic;
using System;

namespace AI.ML.Clustering
{
    /// <summary>
    /// Структура кластера
    /// </summary>
    [Serializable]
    public class Cluster
    {
        private Vector _centr;
        private Vector[] _dataset;


        /// <summary>
        /// Набор данных
        /// </summary>
        public Vector[] Dataset
        {
            get => _dataset;
            set => _dataset = value;
        }

        /// <summary>
        /// Центр кластера
        /// </summary>
        public Vector Centr
        {
            get => _centr;
            set => _centr = value;
        }

    }




}
