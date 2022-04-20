using AI.DataStructs.Algebraic;

namespace AI.DSP
{
    /// <summary>
    /// Windows for correcting phase discontinuities
    /// </summary>
    public static class PhaseCorrectingWindow
    {

        public static Vector Trapezoid(int len, double slope = 0.03)
        {
            int up = (int)(slope * len);
            int down = len - up;
            double k = 1.0 / up;
            Vector outp = new Vector(len);

            for (int i = 1; i < len; i++)
            {
                if (i < up)
                {
                    outp[i] = k + outp[i - 1];
                }
                else if (i > down)
                {
                    outp[i] = outp[i - 1] - k;
                }
                else
                {
                    outp[i] = 1;
                }
            }

            return outp;
        }
    }
}
