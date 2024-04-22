using AI.ExplainitALL.Metrics.SimMetrics;
using System;
using System.Collections.Generic;

namespace AI.ExplainitALL.Metrics.SimAlgs
{
    /// <summary>
    /// Матрица схожестей без перефразировок
    /// </summary>
    [Serializable]
    public class NgramJaccardSim : SimBiEncoderMatrix<HashSet<string>>
    {

        protected NgramJaccardMetric NgramJaccardMetric { get; set; }

        /// <summary>
        /// Матрица схожестей без перефразировок
        /// </summary>
        public NgramJaccardSim(int nGSize = 2, bool spaceDel = true, bool pDel = true)
        {
            NgramJaccardMetric = new NgramJaccardMetric(nGSize, spaceDel, pDel);
        }

        public override double Sim(HashSet<string> set1, HashSet<string> set2) =>
            NgramJaccardMetric.Sim(set1, set2);

        public override HashSet<string> Transform(string text) =>
            NgramJaccardMetric.Transform(text);
    }
}
