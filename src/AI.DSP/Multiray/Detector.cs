using AI.DataStructs.Algebraic;
using AI.DSP.Multiray.Sources;
using System.Collections.Generic;

namespace AI.DSP.Multiray
{
    public class Detector : GeometrySignalObject 
    {
        public Detector() : base() 
        { }

        public Detector(params double[] data) : base(data) { }


        public Vector GetSignal(IEnumerable<Source> signals, double waveSpeed) 
        => MultiRayTools.CollectSignals(signals, this, waveSpeed);
    }
}
