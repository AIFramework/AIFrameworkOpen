using System.Linq;
using System.Numerics;

namespace AI.ClassicMath.MatrixUtils.FindFraction;

public class ConversionResult
{
    public string Type { get; set; } // "Integer", "Terminating", "Repeating", "Irrational/Transcendent"
    public string Fraction { get; set; }
    public string Description { get; set; }
    public BigInteger Numerator { get; set; }
    public BigInteger Denominator { get; set; }
}
