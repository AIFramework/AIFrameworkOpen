using AI;
using AI.DataStructs.Algebraic;
using AI.DSP.Multiray;
using SignalArray;
using Environment = AI.DSP.Multiray.Environment;

double[] coordD1 = [1, 3];
double[] coordD2 = [10.4, 3];
double[] coordD3 = [1.2, 3.2];

double[] coordS1 = [-5, -13];

Detector detector1 = new Detector(coordD1);
Detector detector2 = new Detector(coordD2);
Detector detector3 = new Detector(coordD3);

Source source = new SinSource(10000, coordS1);

Environment env = new Environment();


env.Detectors.Add(detector1);
env.Detectors.Add(detector2);
env.Sources.Add(source);

var signals = env.GetSignals();

var r1r2 = TwoMicro.GetR1R2Correlation(signals[0], signals[1], env.SR, env.WaveSpeed);
var r1r2F = TwoMicro.GetR1R2FFT(signals[0], signals[1], env.SR, env.WaveSpeed);

var r1Real = Environment.GetDist(detector1, source);
var r2Real = Environment.GetDist(detector2, source);
var dtReal = Environment.GetDeltaT(detector1, detector2, source, env.WaveSpeed);

Console.WriteLine();
Console.WriteLine();
