using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.ComputerVision.FiltersEachElements
{
    public class SigmoidalFilter: FilterEE
    {
        double _off, _betta;
        
        /// <summary>
        /// Сигмоидальный фильтр
        /// </summary>
        /// <param name="offset">Смещение</param>
        /// <param name="betta">Коэффициент наклона - контраст</param>
        public SigmoidalFilter(double offset = -0.5, double betta = 10) 
        {
            _off = offset;
            _betta = betta;
            Init(FilterFunction, true, true);
        }

        // Функция фильтра
        private double FilterFunction(double inp) 
        {
            return HightLevelFunctions.ActivationFunctions.Sigmoid(inp + _off, _betta);
        }


    }
}
