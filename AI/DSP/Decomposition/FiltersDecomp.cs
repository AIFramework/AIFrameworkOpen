using AI.BackEnds.DSP.NWaves.Filters.Fda;
using AI.BackEnds.DSP.NWaves.Windows;
using AI.DataStructs.Algebraic;
using AI.DSP.DSPCore;
using System;
using System.Threading.Tasks;

namespace AI.DSP.Decomposition
{
    public class FiltersDecomp
    {
        private readonly Vector[] _responses;
        private readonly double[] _fs, _fe;

        public FiltersDecomp(double[] fStart, double[] fEnd, int fd, int order = 157)
        {
            if (fStart.Length != fEnd.Length)
            {
                throw new Exception("fStart.Length != fEnd.Length");
            }

            _responses = new Vector[fStart.Length];
            _fs = fStart;
            _fe = fEnd;

            for (int i = 0; i < fStart.Length; i++)
            {
                _responses[i] = FIRSincImpulseResponse(fd, fStart[i], fEnd[i], order);
            }
        }


        public Vector[] Decomosition(Vector signal)
        {
            Vector[] signals = new Vector[_responses.Length];

            Parallel.For(0, _responses.Length, i =>
            {
                signals[i] = RunFirFilter(signal, _responses[i]);
            });

            return signals;
        }

        public Tuple<Vector[], Vector> DecomositionBalanceSTD(Vector signal)
        {
            Vector[] signals = new Vector[_responses.Length];
            Vector std = new Vector(_responses.Length);

            Parallel.For(0, _responses.Length, i =>
            {
                signals[i] = RunFirFilter(signal, _responses[i]);
                std[i] = signals[i].Std();//(_fe[i] - _fs[i]);
                signals[i] /= std[i];
            });

            return new Tuple<Vector[], Vector>(signals, std);
        }


        public Vector SumDecomosition(Vector signal)
        {
            Vector[] s = Decomosition(signal);

            Vector signal_ = new Vector(s[0].Count);

            for (int i = 0; i < s.Length; i++)
            {
                signal_ += s[i];
            }

            return signal_;
        }

        public Tuple<Vector, Vector> SumDecomositionBalanceSTD(Vector signal)
        {
            Tuple<Vector[], Vector> d = DecomositionBalanceSTD(signal);
            Vector[] s = d.Item1;

            Vector signal_ = new Vector(s[0].Count);

            for (int i = 0; i < s.Length; i++)
            {
                signal_ += s[i];
            }

            return new Tuple<Vector, Vector>(signal_, d.Item2);
        }

        public static Vector FIRSincImpulseResponse(int fd, double f1 = 2, double f2 = 4, int order = 37, WindowTypes windowType = WindowTypes.Blackman)
        {
            if (order % 2 == 0)
            {
                order++;
            }

            return DesignFilter.FirWinBp(order, f1 / fd, f2 / fd, windowType);
        }


        public static Vector FIRRemezImpulseResponse(int fd, double f1_stop = 2, double f1_pass = 2.1, double f2_stop = 4.2, double f2_pass = 4, double w_stop = 0.05, double w_pass = 0.95, int order = 37, WindowTypes windowType = WindowTypes.Kaiser)
        {
            if (order % 2 == 0)
            {
                order++;
            }

            return DesignFilter.FirEquirippleBp(order, f1_stop / fd, f1_pass / fd, f2_stop / fd, f2_pass / fd, w_stop, w_pass, w_stop);
        }

        public static Vector RunFirFilter(Vector signal, Vector impulseResponse)
        {
            int orderHalf = impulseResponse.Count / 2;
            return FastConv.FastConvolution(signal, impulseResponse).GetInterval(orderHalf, signal.Count + orderHalf);
        }
    }
}
