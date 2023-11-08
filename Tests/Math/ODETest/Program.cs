using AI.MathUtils.ODE;
using System;

namespace ODETest
{
    class Program
    {
        static void Main()
        {
            Func<double, double, double> f = (x, y) => x * Math.Sqrt(y);

            double x0 = 0;
            double y0 = 1;
            double xEnd = 20;
            double h = 0.1;

            var rk = RungeKutta.RungeRombergRK4(f, x0, y0, xEnd, h);
            Console.WriteLine("Приближенное решение: " + rk.Y);
            Console.WriteLine("Оценка погрешности: " + rk.ErrorEstimate);
        }
    }
}
