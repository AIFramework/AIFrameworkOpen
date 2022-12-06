using AI.DataStructs.Algebraic;
using System;

namespace AI.ML.SeqAnalyze
{
    /// <summary>
    /// Значение для словаря регрессий
    /// </summary>
    [Serializable]
    public class ValueDictRegressor
    {
        /// <summary>
        /// Целевое значение
        /// </summary>
        public Vector TargetValue;

        /// <summary>
        /// Градиент целевого значения
        /// </summary>
        public Vector TargetValueGrad;

        private Vector _oldUpd;

        /// <summary>
        /// Число активаций
        /// </summary>
        public int CountActiv;
        /// <summary>
        /// Размер n-граммы
        /// </summary>
        public short NGram;
        /// <summary>
        /// Коэф. важности
        /// </summary>
        public double Importance => _importance;

        private double _importance = 0;


        /// <summary>
        /// Получить важность n-граммы по компонентам 
        /// </summary>
        public double GetImportance(double mean = 0)
        {
            double k = GetKImportance();
            Vector impor = k * (TargetValue - mean).Transform(Math.Abs);
            return impor.Mean();
        }


        /// <summary>
        /// Получить вектор умноженный на важность элемента
        /// </summary>
        public Vector GetVector()
        {
            //double k = GetKImportance();
            return _importance * TargetValue;
        }

        /// <summary>
        /// Получить коэффициент важности
        /// </summary>
        /// <returns></returns>
        public double GetKImportance() 
        {
            _importance = Math.Sqrt(NGram * CountActiv);
            return _importance;
        }

        /// <summary>
        /// Добавить градиент
        /// </summary>
        public void AddGrad(Vector grad)
        {
            if (TargetValueGrad == null) TargetValueGrad = grad.Clone();
            else TargetValueGrad += grad;
        }

        /// <summary>
        /// Обновление вектора
        /// </summary>
        /// <param name="lr">Скорость обучения</param>
        /// <param name="l1">L1</param>
        /// <param name="l2">L2</param>
        /// <param name="momentum">Момент</param>
        public void Upd(double lr, double l1, double l2, double momentum = 0.8)
        {
            if(_oldUpd == null) _oldUpd = new Vector(TargetValue.Count);
            Vector upd = lr * (TargetValueGrad + l1 + l2 * TargetValue); // Обновление веса
            TargetValue -= upd + momentum * _oldUpd;
            _oldUpd = upd;

            TargetValueGrad *= 0;
        }
    }
}