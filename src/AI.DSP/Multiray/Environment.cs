using AI.DataStructs.Algebraic;
using AI.HightLevelFunctions;
using System.Collections.Generic;

namespace AI.DSP.Multiray
{
    public class Environment
    {
        public List<Source> Sources { get; set; } = new List<Source>();

        public List<Detector> Detectors { get; set; } = new List<Detector>();

        public double SR { get; set; } = 10000;

        public double WaveSpeed { get; set; } = 300; // Скорость звука

        public List<Vector> GetSignals() 
        {
            List<Vector> signals = new List<Vector>(Detectors.Count);

            foreach (var detector in Detectors) 
            {
                Vector signal = null;

                foreach (var source in Sources)
                {
                    if(signal == null) 
                        signal = GetSignalOnDetector(detector, source);
                    else
                        signal += GetSignalOnDetector(detector, source);
                }

                signals.Add(signal);
            }

            return signals;
        }

        private Vector GetSignalOnDetector(Detector detector, Source source)
        {
            double d = GetDist(detector, source);
            return source.GetSignal(d, WaveSpeed);
        }

        public static double GetDist(GeometrySignalObject go1, GeometrySignalObject go2) =>
            AnalyticGeometryFunctions.DistanceFromAToB(go1.Coordinates, go2.Coordinates);

        public static double GetDeltaT(GeometrySignalObject go1, GeometrySignalObject go2, GeometrySignalObject ancor, double v) 
        {
            double t1 = GetDist(go1, ancor)/v;
            double t2 = GetDist(go2, ancor)/v;
            return t2 - t1;
        }
    }
}
