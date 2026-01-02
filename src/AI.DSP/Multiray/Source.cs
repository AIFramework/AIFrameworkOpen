using AI.DataStructs.Algebraic;
using System.Text;

namespace AI.DSP.Multiray
{

    public abstract class Source : GeometrySignalObject
    {
        public double T { get; set; } = 1;

        public double SR { get; set; }

        public double KDistDecent = 0.02;

        public Source(double sr = 1000) : base() 
        {
            SR = sr;
        }

        public Source(double sr, params double[] coords) : base(coords) 
        {
            SR = sr;
        } 

        public abstract Vector GetSignal(double dist, double speed);
    }
}
