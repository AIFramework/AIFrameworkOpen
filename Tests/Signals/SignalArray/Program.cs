using AI;
using AI.DataStructs.Algebraic;
using AI.DSP.Multiray;
using SignalArray;
using Environment = AI.DSP.Multiray.Environment;

double[] coordD1 = [1, 3];
double[] coordD2 = [1.4, 3];

double[] coordS1 = [-5, -13];

// Ошибка зависит от ЧД
double sr = 5000;

Detector detector1 = new Detector(coordD1);
Detector detector2 = new Detector(coordD2);

Source source = new SinSource(sr, coordS1);

Environment env = new Environment();


env.Detectors.Add(detector1);
env.Detectors.Add(detector2);
env.Sources.Add(source);

var signals = env.GetSignals();

var r1r2dt = TwoMicro.GetR1R2DtCorrelation(signals[0], signals[1], sr, env.WaveSpeed);
var r1r2F = TwoMicro.GetR1R2DtFFT(signals[0], signals[1], sr, env.WaveSpeed);

var r1Real = Environment.GetDist(detector1, source);
var r2Real = Environment.GetDist(detector2, source);
var dtReal = Environment.GetDeltaT(detector1, detector2, source, env.WaveSpeed);

Console.WriteLine($"Корреляционный метод:\nОшибка (Расстояние): {(r1r2dt.Item1 - r1Real + r1r2dt.Item2 - r2Real) / (r1Real + r2Real)}\n\nОшибка (Задержка) Важно: {(r1r2dt.Item3 - dtReal) / dtReal} сек");

Console.WriteLine($"\n\nФурье метод:\nОшибка (Расстояние): {(r1r2F.Item1 - r1Real + r1r2F.Item2 - r2Real) / (r1Real + r2Real)}\n\nОшибка (Задержка) Важно: {(r1r2F.Item3 - dtReal) / dtReal} сек");

Console.WriteLine();
