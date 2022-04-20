using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AI.BackEnds.DSP.NWaves.FeatureExtractors.Serializers
{
    /// <summary>
    /// Class for simple CSV serialization of feature vectors
    /// </summary>
    public class CsvFeatureSerializer
    {
        /// <summary>
        /// List of feature vectors for serialization
        /// </summary>
        private readonly IList<float[]> _vectors;

        /// <summary>
        /// List of time markers for serialization
        /// </summary>
        private readonly IList<double> _timeMarkers;

        /// <summary>
        /// List of feature names for serialization
        /// </summary>
        private readonly IList<string> _names;

        /// <summary>
        /// Delimiter
        /// </summary>
        private readonly char _delimiter;

        /// <summary>
        /// Constructor accepting the list of feature vectors
        /// </summary>
        /// <param name="featureVectors">List of feature vectors for serialization</param>
        /// <param name="timeMarkers">List of time markers for serialization</param>
        /// <param name="featureNames">List of feature names for serialization</param>
        /// <param name="delimiter">Delimiter char</param>
        public CsvFeatureSerializer(IList<float[]> featureVectors,
                                    IList<double> timeMarkers = null,
                                    IList<string> featureNames = null,
                                    char delimiter = ',')
        {
            _vectors = featureVectors;
            _timeMarkers = timeMarkers;
            _names = featureNames;
            _delimiter = delimiter;
        }

        /// <summary>
        /// Asynchronous method for feature vectors serialization
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="format"></param>
        /// <param name="timeFormat"></param>
        /// <returns></returns>
        public async Task SerializeAsync(Stream stream, string format = "0.00000", string timeFormat = "0.000")
        {
            string comma = _delimiter.ToString();

            using (StreamWriter writer = new StreamWriter(stream))
            {
                if (_names != null)
                {
                    string names = string.Join(comma, _names);
                    string header = _timeMarkers == null ? $"{names}" : $"time_pos{comma}{names}";
                    await writer.WriteLineAsync(header).ConfigureAwait(false);
                }

                if (_timeMarkers == null)
                {
                    foreach (float[] vector in _vectors)
                    {
                        string line = string.Join(comma, vector.Select(f => f.ToString(format, CultureInfo.InvariantCulture)));

                        await writer.WriteLineAsync(line).ConfigureAwait(false);
                    }
                }
                else
                {
                    for (int i = 0; i < _vectors.Count; i++)
                    {
                        string line = string.Format("{0}{1}{2}",
                                             _timeMarkers[i].ToString(timeFormat, CultureInfo.InvariantCulture),
                                             comma,
                                             string.Join(comma, _vectors[i].Select(f => f.ToString(format, CultureInfo.InvariantCulture))));

                        await writer.WriteLineAsync(line).ConfigureAwait(false);
                    }
                }
            }
        }
    }
}
