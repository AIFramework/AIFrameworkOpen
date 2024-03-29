﻿using AI.DataStructs.Algebraic;
using AI.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AI.ML.DataSets.Base
{
    /// <summary>
    /// Последовательность данных для обучения рекуррентной сети по принципу «многие ко многим»
    /// </summary>
    [Serializable]
    public class Many2ManyVectorClassifier
    {
        /// <summary>
        /// Метки классов
        /// </summary>
        public List<int> Labels { get; set; }
        /// <summary>
        /// Признаки
        /// </summary>
        public List<Vector> Features { get; set; }

        /// <summary>
        /// Последовательность данных для обучения рекуррентной сети по принципу «многие ко многим»
        /// </summary>
        public Many2ManyVectorClassifier()
        {
        }

        /// <summary>
        /// Последовательность данных для обучения рекуррентной сети по принципу «многие ко многим»
        /// </summary>
        /// <param name="labels">Метки классов</param>
        /// <param name="features">Признаки</param>
        public Many2ManyVectorClassifier(IEnumerable<int> labels, IEnumerable<IAlgebraicStructure<double>> features)
        {
            Labels = labels.ToList();
            Features = features.Select(x => (Vector)x.Data).ToList();
        }

        /// <summary>
        /// Получение набора данных для использования с менеджером нейронной сети
        /// </summary>
        /// <param name="data">Набор данных</param>
        /// <param name="doShuffling">Нужно ли перемешивать данные в случайном порядке</param>
        public Many2ManyVectorClassifierDataset GetDataset(IEnumerable<Many2ManyVectorClassifier> data, bool doShuffling = true)
        {
            Many2ManyVectorClassifier[] array = data.ToArray();

            if (doShuffling)
            {
                array.Shuffle();
            }

            Many2ManyVectorClassifierDataset many2ManyVectorClassifiers = new Many2ManyVectorClassifierDataset();
            many2ManyVectorClassifiers.AddRange(array);
            return many2ManyVectorClassifiers;
        }
    }
}
