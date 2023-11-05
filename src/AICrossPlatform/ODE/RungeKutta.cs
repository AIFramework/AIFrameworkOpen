using System;

namespace AI.ODE
{
    /// <summary>
    /// Solves an ordinary differential equation using the 4th order Runge-Kutta method.
    /// </summary>
    [Serializable]
    public static class RungeKutta
    {
        /// <summary>
        /// Solves an ordinary differential equation using the 4th order Runge-Kutta method.
        /// </summary>
        /// <param name="function">The function representing the right-hand side of the differential equation dy/dx = f(x, y).</param>
        /// <param name="initialX">The initial value of x.</param>
        /// <param name="initialY">The initial value of y.</param>
        /// <param name="finalX">The final value of x for which y is required.</param>
        /// <param name="stepSize">The step size for the iteration.</param>
        /// <returns>The approximate value of y at finalX.</returns>
        public static double RungeKutta4(Func<double, double, double> function, double initialX, double initialY, double finalX, double stepSize)
        {
            if (stepSize <= 0)
            {
                throw new ArgumentException("Step size must be positive.", nameof(stepSize));
            }

            double x = initialX;
            double y = initialY;
            while (x < finalX)
            {
                double k1 = stepSize * function(x, y);
                double k2 = stepSize * function(x + 0.5 * stepSize, y + 0.5 * k1);
                double k3 = stepSize * function(x + 0.5 * stepSize, y + 0.5 * k2);
                double k4 = stepSize * function(x + stepSize, y + k3);
                y += (k1 + 2 * k2 + 2 * k3 + k4) / 6;
                x += stepSize;
            }
            return y;
        }

        /// <summary>
        /// Estimates the error of the Runge-Kutta 4 method using the Runge-Romberg rule.
        /// </summary>
        /// <param name="function">The function representing the right-hand side of the differential equation dy/dx = f(x, y).</param>
        /// <param name="initialX">The initial value of x.</param>
        /// <param name="initialY">The initial value of y.</param>
        /// <param name="finalX">The final value of x for which y is required.</param>
        /// <param name="stepSize">The step size for the iteration.</param>
        /// <param name="order">The order of the Runge-Kutta method (should be 4 for RK4).</param>
        /// <returns>A tuple containing the corrected value of y and the estimated error.</returns>
        public static (double CorrectedY, double ErrorEstimate) RungeRombergRK4(Func<double, double, double> function, double initialX, double initialY, double finalX, double stepSize, int order)
        {
            if (order <= 0)
            {
                throw new ArgumentException("Order must be positive.", nameof(order));
            }

            double y1 = RungeKutta4(function, initialX, initialY, finalX, stepSize);
            double y2 = RungeKutta4(function, initialX, initialY, finalX, stepSize / 2);
            double R = (y2 - y1) / (Math.Pow(2, order) - 1);
            double correctedY = y2 + R;
            return (correctedY, R);
        }
    }
}
