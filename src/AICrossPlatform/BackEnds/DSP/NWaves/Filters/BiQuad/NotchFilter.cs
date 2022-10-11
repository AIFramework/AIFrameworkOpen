using System;

namespace AI.BackEnds.DSP.NWaves.Filters.BiQuad
{
    /// <summary>
    /// ������������ ����������� ������
    /// The coef "A"re calculated automatically according to 
    /// audio-eq-cookbook by R.Bristow-Johnson and WebAudio API.
    /// </summary>
    [Serializable]
    public class NotchFilter : BiQuadFilter
    {
        /// <summary>
        /// �������
        /// </summary>
        public double Freq { get; protected set; }

        /// <summary>
        /// �����������
        /// </summary>
        public double Q { get; protected set; }

        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="freq"></param>
        /// <param name="q"></param>
        public NotchFilter(double freq, double q = 1)
        {
            SetCoefficients(freq, q);
        }

        /// <summary>
        /// ���������� ������������ �������
        /// </summary>
        /// <param name="freq"></param>
        /// <param name="q"></param>
        private void SetCoefficients(double freq, double q)
        {
            Freq = freq;
            Q = q;

            double omega = 2 * Math.PI * freq;
            double alpha = Math.Sin(omega) / (2 * q);
            double cosw = Math.Cos(omega);

            _b[0] = 1;
            _b[1] = (float)(-2 * cosw);
            _b[2] = 1;

            _a[0] = (float)(1 + alpha);
            _a[1] = (float)(-2 * cosw);
            _a[2] = (float)(1 - alpha);

            Normalize();
        }

        /// <summary>
        /// �������� ��������� ������� (� ����������� ��� ���������)
        /// </summary>
        /// <param name="freq"></param>
        /// <param name="q"></param>
        public void Change(double freq, double q = 1)
        {
            SetCoefficients(freq, q);
        }
    }
}
