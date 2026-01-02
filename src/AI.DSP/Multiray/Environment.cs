using AI.DataStructs.Algebraic;
using AI.HightLevelFunctions;
using System.Collections.Generic;

namespace AI.DSP.Multiray
{
    public class Environment
    {
        public List<Source> Sources { get; set; } = new List<Source>();

        public List<Detector> Detectors { get; set; } = new List<Detector>();

        public double SR { get; set; } = 1000;

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
            double d = AnalyticGeometryFunctions.DistanceFromAToB(detector.Coordinates, source.Coordinates);
            return source.GetSignal(d, WaveSpeed);
        } 
    }
}
