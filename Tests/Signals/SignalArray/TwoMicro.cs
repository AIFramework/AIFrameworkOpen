using AI;
using AI.DataStructs.Algebraic;
using AI.DataStructs.WithComplexElements;
using AI.DSP.DSPCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalArray;

public class TwoMicro
{
    public static Tuple<double, double, double> GetR1R2DtFFT(Vector sig1, Vector sig2, double sr, double v)
    {
        ComplexVector Sp1 = FFT.CalcFFT(sig1);
        ComplexVector Sp2 = FFT.CalcFFT(sig2);

        Vector a1 = Sp1.MagnitudeVector, a2 = Sp2.MagnitudeVector;
        int idx = (a1 + a2).MaxElementIndex();

        if (idx == 0) idx = 1;

        double phase1 = Math.Atan2(Sp1[idx].Imaginary, Sp1[idx].Real);
        double phase2 = Math.Atan2(Sp2[idx].Imaginary, Sp2[idx].Real);
        double df = phase2 - phase1;

        if (df > Math.PI) df -= 2 * Math.PI;
        if (df < -Math.PI) df += 2 * Math.PI;

        double dt = Sp1.Count * df / (sr * idx * 2 * Math.PI);
        double dr = Math.Abs(dt * v);

        double ratioR_sq = (sig1.Max() * sig1.Max()) / (sig2.Max() * sig2.Max());

        double r1, r2;

        if (ratioR_sq > 1)
        {
            r1 = dr / (Math.Sqrt(ratioR_sq) - 1);
            r2 = r1 * Math.Sqrt(ratioR_sq);
        }
        else
        {
            ratioR_sq = 1 / ratioR_sq;
            r2 = dr / (Math.Sqrt(ratioR_sq) - 1);
            r1 = r2 * Math.Sqrt(ratioR_sq);
        }

        return Tuple.Create(r1, r2, dt);
    }


    public static Tuple<double, double, double> GetR1R2DtCorrelation(Vector sig1, Vector sig2, double sr, double v)
    {
        var corr = Correlation.CrossCorrelation(sig1, sig2);
        int centerIndex = sig2.Count - 1;
        int maxIndex = corr.MaxElementIndex();
        double dt = (maxIndex - centerIndex) / sr;
        double dr = Math.Abs(dt * v);

        double ratioR_sq = (sig1.Max() * sig1.Max()) / (sig2.Max() * sig2.Max());

        double r1, r2;

        if (ratioR_sq > 1)
        {
            r1 = dr / (Math.Sqrt(ratioR_sq) - 1);
            r2 = r1 * Math.Sqrt(ratioR_sq);
        }
        else
        {
            ratioR_sq = 1 / ratioR_sq;
            r2 = dr / (Math.Sqrt(ratioR_sq) - 1);
            r1 = r2 * Math.Sqrt(ratioR_sq);
        }

        return Tuple.Create(r1, r2, dt);
    }
}
