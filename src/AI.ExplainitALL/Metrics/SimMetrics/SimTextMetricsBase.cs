// ------------------------------
// Оригинальный проект Python:
// https://github.com/Bots-Avatar/ExplainitAll/blob/main/explainitall/metrics/CheckingForHallucinations.py
// -----------------------------------
using System;
using System.Text;

namespace AI.ExplainitALL.Metrics.SimMetrics
{
    /// <summary>
    /// Базовый класс для рассчета схожести предложений
    /// </summary>
    [Serializable]
    public abstract class SimTextMetricsBase<T>
    {
        public abstract T Transform(string text);

        public abstract double Sim(T text1, T text2);

        public double Sim(string text1, string text2) 
        {
            T text1T = Transform(text1);
            T text2T = Transform(text2);
            return Sim(text1T, text2T);
        }
    }
}
