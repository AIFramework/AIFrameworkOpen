using AI.DataStructs.Algebraic;
using System;

namespace AI.MathUtils.ODE
{
    /// <summary>
    /// Solves an ordinary differential equation using the 4th order Runge-Kutta method.
    /// </summary>
    [Serializable]
    public class RungeKutta
    {

        public Vector Y { get; set; }
        public Vector X { get; set; }

        public Vector ErrorEstimate { get; set; }

        public RungeKutta() { }


        /// <summary>
        /// Solves an ordinary differential equation using the 4th order Runge-Kutta method.
        /// </summary>
        /// <param name="function">The function representing the right-hand side of the differential equation dy/dx = f(x, y).</param>
        /// <param name="initialX">The initial value of x.</param>
        /// <param name="initialY">The initial value of y.</param>
        /// <param name="finalX">The final value of x for which y is required.</param>
        /// <param name="stepSize">The step size for the iteration.</param>
        /// <returns>The approximate value of y at finalX.</returns>
        public static RungeKutta RungeKutta4(Func<double, double, double> function, double initialX, double initialY, double finalX, double stepSize, bool isHalfStep = false)
        {
            if (stepSize <= 0)
            {
                throw new ArgumentException("Step size must be positive.", nameof(stepSize));
            }

            Vector xGrid = new Vector();
            Vector yGrid = new Vector();
            int index = 0;

            double x = initialX;
            double y = initialY;
            while (x < finalX)
            {
                double k1 = stepSize * function(x, y);
                double k2 = stepSize * function(x + 0.5 * stepSize, y + 0.5 * k1);
                double k3 = stepSize * function(x + 0.5 * stepSize, y + 0.5 * k2);
                double k4 = stepSize * function(x + stepSize, y + k3);

                xGrid.Add(x);

                y += (k1 + 2 * k2 + 2 * k3 + k4) / 6;


                if (isHalfStep && index % 2 == 0) { }
                else
                {
                    xGrid.Add(x);
                    yGrid.Add(y);
                }


                index++;

                x += stepSize;
            }

            return new RungeKutta() { X = xGrid, Y = yGrid };
        }


        /// <summary>
        /// Estimates the error of the Runge-Kutta 4 method using the Runge-Romberg rule.
        /// </summary>
        /// <param name="function">The function representing the right-hand side of the differential equation dy/dx = f(x, y).</param>
        /// <param name="initialX">The initial value of x.</param>
        /// <param name="initialY">The initial value of y.</param>
        /// <param name="finalX">The final value of x for which y is required.</param>
        /// <param name="stepSize">The step size for the iteration.</param>
        /// <returns>A tuple containing the corrected value of y and the estimated error.</returns>
        public static RungeKutta RungeRombergRK4(Func<double, double, double> function, double initialX, double initialY, double finalX, double stepSize)
        {
            int order = 4;

            var rk = RungeKutta4(function, initialX, initialY, finalX, stepSize / 2, true);
            Vector y1 = RungeKutta4(function, initialX, initialY, finalX, stepSize).Y;

            y1 = y1.CutAndZero(rk.Y.Count);

            rk.ErrorEstimate = (rk.Y - y1) / (Math.Pow(2, order) - 1);
            rk.Y += rk.ErrorEstimate;
            return rk;
        }
    }
}
