using AI.DataStructs.Algebraic;
using AI.DSP.Multiray.Sources;
using AI.HightLevelFunctions;
using System.Collections.Generic;

namespace AI.DSP.Multiray
{
    public class SignalEnvironment
    {
        public List<Source> Sources { get; set; } = new List<Source>();

        public List<Detector> Detectors { get; set; } = new List<Detector>();

        public double SR { get; set; } = 10000;

        public double WaveSpeed { get; set; } = 300; // Скорость звука

        public List<Vector> GetSignals() 
        {
            List<Vector> signals = new List<Vector>(Detectors.Count);

            foreach (var detector in Detectors)
                signals.Add(detector.GetSignal(Sources, WaveSpeed));
            
            return signals;
        }

        

        
    }
}
