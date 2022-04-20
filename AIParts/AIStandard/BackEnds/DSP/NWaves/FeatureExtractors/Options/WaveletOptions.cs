using System.Runtime.Serialization;

namespace AI.BackEnds.DSP.NWaves.FeatureExtractors.Options
{
    [DataContract]
    public class WaveletOptions : FeatureExtractorOptions
    {
        [DataMember]
        public string WaveletName { get; set; } = "haar";
        [DataMember]
        public int FwtSize { get; set; }
        [DataMember]
        public int FwtLevel { get; set; }
    }
}
