using AI.DataStructs.Algebraic;
using AI.DSP.Multiray.Sources;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI.DSP.Multiray.Reflectors
{
    public class Reflector : Source
    {
        public Reflector(params double[] coords) : base(coords) { }
        

        public override Vector GetSignal(double dist, double speed, IEnumerable<Source> sources)
        {
            Vector signal = MultiRayTools.CollectSignals(sources, this, speed);
            // ToDo: добавить изменение фаз в БПФ для получения результата (или сдвиг)
            throw new NotImplementedException();
        }
    }
}
