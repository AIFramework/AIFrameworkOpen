/*
 * Created by SharpDevelop.
 * User: 01
 * Date: 31.03.2016
 * Time: 18:48
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using AI.DataStructs.Algebraic;
using System;

namespace AI.ML.Regression
{
    /// <summary>
    /// Структура для хранения регрессионных данных
    /// </summary>
    [Serializable]
    public class StructRegrMulty
    {
        /// <summary>
        /// Целевой вектор
        /// </summary>
        private Vector _targ = new Vector();

        /// <summary>
        /// Координата центра гиперсферы
        /// </summary>
        private Vector _centGiperSfer = new Vector(3);// Координата центра гиперсферы


        /// <summary>
        /// Целевой вектор
        /// </summary>
        public Vector Targets
        {
            get => _targ;
            set => _targ = value;
        }

        /// <summary>
        /// Координата центра гиперсферы
        /// </summary>
        public Vector CentGiperSfer
        {
            get => _centGiperSfer;
            set => _centGiperSfer = value;
        }

        /// <summary>
        /// Расстояние
        /// </summary>
        public double R
        {
            get;
            set;
        }

        /// <summary>
        /// Дополнительные опции
        /// </summary>
        public double[] Params
        {
            get;
            set;
        }

    }
}


