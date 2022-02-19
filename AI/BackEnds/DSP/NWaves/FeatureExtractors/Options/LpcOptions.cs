﻿using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AI.BackEnds.DSP.NWaves.FeatureExtractors.Options
{
    [DataContract]
    public class LpcOptions : FeatureExtractorOptions
    {
        [DataMember]
        public int LpcOrder { get; set; }

        public override List<string> Errors
        {
            get
            {
                var errors = base.Errors;
                if (LpcOrder <= 0) errors.Add("Positive order of LPC must be specified");
                return errors;
            }
        }
    }
}
