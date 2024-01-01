﻿using AI.DataPrepaire.Backends.BertTokenizers;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI.DataPrepaire.Tokenizers.TextTokenizers.HFTokenizers
{
    /// <summary>
    /// Класс BertTokenizer представляет токенизатор для модели BERT.
    /// </summary>
    [Serializable]
    public class BertTokenizer : CasedTokenizer
    {

        /// <summary>
        /// Инициализирует новый экземпляр класса BertTokenizer.
        /// </summary>
        /// <param name="path">Путь к файлу словаря.</param>
        /// <param name="splitWords">Разделять ли по словам</param>
        public BertTokenizer(string path, bool splitWords = true) : base(path, splitWords)
        {
            
        }
    }
}
