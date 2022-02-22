using AI.BackEnds.DSP.NWaves.Filters.Fda;
using AI.BackEnds.DSP.NWaves.Utils;
using System.Runtime.Serialization;

namespace AI.BackEnds.DSP.NWaves.FeatureExtractors.Options
{
    [DataContract]
    public class MfccHtkOptions : MfccOptions
    {
        public MfccHtkOptions(int samplingRate,
                              int featureCount,
                              double frameDuration,
                              double lowFrequency = 0,
                              double highFrequency = 0,
                              int filterbankSize = 24,
                              int fftSize = 0)
        {

            int frameSize = (int)(frameDuration * samplingRate);
            fftSize = fftSize > frameSize ? fftSize : MathUtils.NextPowerOfTwo(frameSize);

            System.Tuple<double, double, double>[] melBands = FilterBanks.MelBands(filterbankSize, samplingRate, lowFrequency, highFrequency);
            FilterBank = FilterBanks.Triangular(fftSize, samplingRate, melBands, null, Scale.HerzToMel);
            FilterBankSize = filterbankSize;
            FeatureCount = featureCount;
            FftSize = fftSize;
            SamplingRate = samplingRate;
            LowFrequency = lowFrequency;
            HighFrequency = highFrequency;
            NonLinearity = NonLinearityType.LogE;
            LogFloor = 1.0f;
        }
    }
}
