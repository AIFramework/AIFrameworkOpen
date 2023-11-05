using AI.ODE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODETest
{
    class Program
    {
        static void Main()
        {
            Func<double, double, double> f = (x, y) => x * Math.Sqrt(y);

            double x0 = 0;
            double y0 = 1;
            double xEnd = 10;
            double h = 0.1;
            int p = 10;

            var (yCorrected, errorEstimate) = RungeKutta.RungeRombergRK4(f, x0, y0, xEnd, h, p);
            Console.WriteLine("Приближенное решение: " + yCorrected);
            Console.WriteLine("Оценка погрешности: " + errorEstimate);
        }
    }
}
