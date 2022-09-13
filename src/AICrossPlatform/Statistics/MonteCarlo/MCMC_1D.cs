using AI.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI.Statistics.MonteCarlo
{

    /// <summary>
    /// Метод Монте-Карло Марковских Цепей (для одномерной плотности)
    /// </summary>
    [Serializable]
    public class MCMC_1D
    {
        // Логарифм ненормированной ф-ии распределения
        private Func<double, double> _distr_log;
        private Random _random;
        private int _rnd_seed = 0;// Сид генератора случ. чисел
        private bool _use_seed = false;// Использовать ли сид

        /// <summary>
        /// Длительность переходного процесса в отсчетах
        /// </summary>
        public int StepsTrPro { get; set; }

        /// <summary>
        /// Сид генератора случ. чисел
        /// </summary>
        public int Seed 
        {
            get 
            {
               return  _rnd_seed;
            }
            set
            {
                _rnd_seed= value;
                InitRnd();
            }
        }

        /// <summary>
        /// Использовать ли сид
        /// </summary>
        public bool UseSeed
        {
            get
            {
                return _use_seed;
            }
            set
            {
                _use_seed = value;
                InitRnd();
            }
        }

        /// <summary>
        /// Метод Монте-Карло Марковских Цепей (для одномерной плотности)
        /// </summary>
        /// <param name="distr_log">Логарифм ненормированной функции распределения</param>
        /// <param name="stepsTrPro">Длительность переходного процесса в отсчетах</param>
        public MCMC_1D(Func<double, double> distr_log, int stepsTrPro = 400)
        {
            _distr_log = distr_log;
            StepsTrPro = stepsTrPro;
            InitRnd();
        }


        /// <summary>
        /// Вероятность принятия значения (перех. фдро цепи Маркова)
        /// </summary>
        /// <param name="old_value">Старое значение</param>
        /// <param name="new_value">Новое значение</param>
        public double AcceptProb(double old_value, double new_value) 
        {
            return Math.Exp(_distr_log(new_value) - _distr_log(old_value));
        }


        /// <summary>
        /// Генерция выборки
        /// </summary>
        /// <param name="len">Длинна выборки</param>
        /// <param name="start">Начальное значение</param>
        /// <param name="decorelate">Премешивать ли выборку</param>
        /// <param name="min">Минимальное значение</param>
        /// <param name="max">Максимальное значение</param>
        public double[] Generate(int len, double min = 0, double max = 1, double start = 0, bool decorelate = true) 
        {
            double[] data = new double[len];
            double old_value = start;
            double scale = max - min;

            // Выходим в стационарный режим (ждем окончания переходного процесса)
            for (int i = 0; i < StepsTrPro; i++) 
                old_value = NextState(old_value, min, scale);

            // Генерируем выборку
            for (int i = 0; i < len; i++)
            {
                data[i] = NextState(old_value, min, scale);
                old_value = data[i];
            }
            
            if(decorelate) data.Shuffle();

            return data;
          
        }

        // Отдает следующее значение
        private double NextState(double old, double min, double scale) 
        {
            double cand = _random.NextDouble() * scale + min;
            double prob;

            while(cand == old) 
                cand = _random.NextDouble()* scale + min;

            prob = AcceptProb(old, cand);

            return prob > _random.NextDouble() ? cand : old;
        }

        // Инициализация генератора случайных чисел
        private void InitRnd()
        {
            _random = _use_seed? new Random(_rnd_seed): new Random();
        }
    }
}
