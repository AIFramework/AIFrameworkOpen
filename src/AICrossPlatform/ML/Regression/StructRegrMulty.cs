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
    /// Structure for storing regression data
    /// </summary>
    [Serializable]
    public class StructRegrMulty
    {
        /// <summary>
        /// Target vector
        /// </summary>
        private Vector _targ = new Vector(0);

        /// <summary>
        /// Center of the hypersphere
        /// </summary>
        private Vector _centGiperSfer = new Vector();// Координата центра гиперсферы


        /// <summary>
        /// Target vector
        /// </summary>
        public Vector Targets
        {
            get => _targ;
            set => _targ = value;
        }

        /// <summary>
        /// Center of the hypersphere
        /// </summary>
        public Vector CentGiperSfer
        {
            get => _centGiperSfer;
            set => _centGiperSfer = value;
        }

        /// <summary>
        /// Distance
        /// </summary>
        public double R
        {
            get;
            set;
        }

        /// <summary>
        /// Extra options
        /// </summary>
        public double[] Params
        {
            get;
            set;
        }

    }
}


