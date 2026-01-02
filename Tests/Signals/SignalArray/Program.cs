using AI;
using AI.DataStructs.Algebraic;
using AI.DSP.Multiray;
using Environment = AI.DSP.Multiray.Environment;

double[] coordD1 = [1, 3];
double[] coordD2 = [1.4, 3];
double[] coordD3 = [1.2, 3.2];

double[] coordS1 = [-5, -13];

Detector detector1 = new Detector(coordD1);
Detector detector2 = new Detector(coordD2);
Detector detector3 = new Detector(coordD3);

Source source = new SinSource(1000, coordS1);

Environment env = new Environment();
env.Detectors.Add(detector1);
env.Detectors.Add(detector2);
env.Detectors.Add(detector3);
env.Sources.Add(source);

var signals = env.GetSignals();
var corr = Matrix.GetCovMatrix(signals.ToArray());
corr /= corr.Std();

Console.WriteLine(corr);
Console.WriteLine();
