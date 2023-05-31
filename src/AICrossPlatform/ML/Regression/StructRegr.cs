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
    ///  Набор данных регрессии item
    /// </summary>
    [Serializable]
    public class StructRegr
    {
        private double _targ = -1; // Имя класса
        private Vector _centGiperSfer = new Vector(3);// Координата центра гиперсферы


        /// <summary>
        /// Значение целевой переменной
        /// </summary>
        public double Target
        {
            get => _targ;
            set => _targ = value;
        }

        /// <summary>
        /// Вектор признаков
        /// </summary>
        public Vector Features
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

