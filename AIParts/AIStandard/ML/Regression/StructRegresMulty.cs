﻿/*
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
    /// Dataset for regression
    /// </summary>
    [Serializable]
    public class StructRegresMulty
    {

        private List<StructRegrMulty> _classes = new List<StructRegrMulty>();

        /// <summary>
        /// Data
        /// </summary>
        public List<StructRegrMulty> Classes
        {
            get => _classes;
            set => _classes = value;
        }
    }
}


