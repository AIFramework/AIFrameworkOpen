﻿using AI.BackEnds.DSP.NWaves.Windows;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AI.BackEnds.DSP.NWaves.FeatureExtractors.Options
{
    [DataContract]
    public class PlpOptions : FilterbankOptions
    {
        [DataMember]
        public int LpcOrder { get; set; }    // if 0, then it'll be autocalculated as FeatureCount - 1
        [DataMember]
        public double Rasta { get; set; }
        [DataMember]
        public int LifterSize { get; set; }
        [DataMember]
        public double[] CenterFrequencies { get; set; }
        [DataMember]
        public bool IncludeEnergy { get; set; }
        [DataMember]
        public float LogEnergyFloor { get; set; } = float.Epsilon;

        public PlpOptions()
        {
            FilterBankSize = 24;
            Window = WindowTypes.Hamming;
        }

        public override List<string> Errors
        {
            get
            {
                List<string> errors = base.Errors;
                if (FeatureCount <= 0)
                {
                    errors.Add("Positive number of PLP coefficients must be specified");
                }

                return errors;
            }
        }
    }
}
