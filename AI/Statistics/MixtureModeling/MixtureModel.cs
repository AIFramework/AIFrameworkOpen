using AI.DataStructs.Algebraic;
using AI.Statistics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.Statistics.MixtureModeling
{
    public class MixtureModel : IDistributionWithoutParams
    {
        public bool IsOneD { get; private set;}
        Dictionary<string, double>[] _paramDists1D;
        Dictionary<string, Vector>[] _paramDistsND;
        Vector _w;
        IDistribution _perentDistribution;

        public MixtureModel(IDistribution perentDistribution, IEnumerable<Dictionary<string, double>> paramDists, Vector w)
        {
            IsOneD = true;
            _paramDists1D = paramDists.ToArray();
            _perentDistribution = perentDistribution;
            _w = w;
        }

        public MixtureModel(IDistribution perentDistribution, IEnumerable<Dictionary<string, Vector>> paramDists, Vector w)
        {
            IsOneD = false;
            _paramDistsND = paramDists.ToArray();
            _perentDistribution = perentDistribution;
            _w = w;
        }

        public double CulcLogProb(double x)
        {
            throw new NotImplementedException();
        }

        public double CulcLogProb(Vector x)
        {
            throw new NotImplementedException();
        }

        public double CulcProb(Vector x)
        {
            throw new NotImplementedException();
        }

        public double CulcProb(double x)
        {
            throw new NotImplementedException();
        }
    }
}
