using AI.DataStructs.Algebraic;

namespace AI.DSP
{
    /// <summary>
    /// Равнобедренная трапеция (окно убирающее разрыв фазы)
    /// </summary>
    public static class PhaseCorrectingWindow
    {
        /// <summary>
        /// Равнобедренная трапеция
        /// </summary>
        /// <param name="len">Длинна основания</param>
        /// <param name="slope">Угол наклона</param>
        /// <returns></returns>
        public static Vector Trapezoid(int len, double slope = 0.03)
        {
            int up = (int)(slope * len);
            int down = len - up;
            double k = 1.0 / up;
            Vector outp = new Vector(len);

            for (int i = 1; i < len; i++)
            {
                if (i < up)
                outp[i] = k + outp[i - 1];
                else if (i > down)
                outp[i] = outp[i - 1] - k;
                else
                outp[i] = 1;
            }

            return outp;
        }
    }
}
