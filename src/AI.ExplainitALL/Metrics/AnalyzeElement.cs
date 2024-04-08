// ------------------------------
// Оригинальный проект Python:
// https://github.com/Bots-Avatar/ExplainitAll/blob/main/explainitall/metrics/CheckingForHallucinations.py
// -----------------------------------

using System;
using System.Collections.Generic;

namespace AI.ExplainitALL.Metrics
{
    /// <summary>
    /// Элемент анализа
    /// </summary>
    [Serializable]
    public struct AnalyzeElement
    {
        public string AnswerBlock { get; set; }
        public List<string> SupportBlocks { get; set; }
        public List<int> SupportBlocksIndexInDoc { get; set; }
    }
}
