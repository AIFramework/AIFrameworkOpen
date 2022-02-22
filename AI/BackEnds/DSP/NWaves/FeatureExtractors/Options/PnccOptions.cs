using AI.BackEnds.DSP.NWaves.Windows;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AI.BackEnds.DSP.NWaves.FeatureExtractors.Options
{
    [DataContract]
    public class PnccOptions : FilterbankOptions
    {
        [DataMember]
        public int Power { get; set; } = 15;
        [DataMember]
        public bool IncludeEnergy { get; set; }
        [DataMember]
        public float LogEnergyFloor { get; set; } = float.Epsilon;

        public PnccOptions()
        {
            LowFrequency = 100;
            HighFrequency = 6800;
            FilterBankSize = 40;
            Window = WindowTypes.Hamming;
        }

        public override List<string> Errors
        {
            get
            {
                List<string> errors = base.Errors;
                if (FeatureCount <= 0)
                {
                    errors.Add("Positive number of PNCC coefficients must be specified");
                }

                return errors;
            }
        }
    }
}
