/*
 * Created by SharpDevelop.
 * User: 01
 * Date: 31.03.2016
 * Time: 18:48
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

namespace AI.ML.Regression
{
    /// <summary>
    /// Regression dataset
    /// </summary>
    [Serializable]
    public class StructRegres
    {

        private List<StructRegr> _classes = new List<StructRegr>();

        /// <summary>
        /// Данные
        /// </summary>
        public List<StructRegr> Classes
        {
            get => _classes;
            set => _classes = value;
        }
    }
}

