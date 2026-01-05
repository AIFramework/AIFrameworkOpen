using AI;
using AI.DataStructs.Algebraic;
using AI.DSP.Multiray;
using SignalArray;
using Environment = AI.DSP.Multiray.Environment;

double[] coordD1 = [1, 3];
double[] coordD2 = [1.4, 3];
double[] coordS1 = [-5, -13];

// Проверяем на разных частотах дискретизации и типах источников
double[] testSampleRates = [5000, 8000, 10000, 16000];

Console.WriteLine("╔═══════════════════════════════════════════════════════════════════╗");
Console.WriteLine("║  ТЕСТИРОВАНИЕ FFT-ФАЗОВОГО МЕТОДА НА РАЗНЫХ ТИПАХ СИГНАЛОВ       ║");
Console.WriteLine("╚═══════════════════════════════════════════════════════════════════╝\n");

// Тест 1: Обычный синус
Console.WriteLine("═══════════════════════════════════════════════════════════════════");
Console.WriteLine("  ТИП СИГНАЛА: ЧИСТЫЙ СИНУС sin(2πft)");
Console.WriteLine("═══════════════════════════════════════════════════════════════════\n");

foreach (double sr in testSampleRates)
{
    Console.WriteLine($"────────────────────────────────────────────────────");
    Console.WriteLine($"Частота дискретизации: {sr} Гц\n");
    
    Detector detector1 = new Detector(coordD1);
    Detector detector2 = new Detector(coordD2);
    Source source = new SinSource(sr, coordS1);

    Environment env = new Environment();
    env.Detectors.Add(detector1);
    env.Detectors.Add(detector2);
    env.Sources.Add(source);

    var signals = env.GetSignals();
    
    // Используем FFT-фазовый метод - самый надёжный для узкополосных сигналов
    var r1r2Universal = TwoMicro.GetR1R2DtFFT(signals[0], signals[1], sr, env.WaveSpeed);

    var r1Real = Environment.GetDist(detector1, source);
    var r2Real = Environment.GetDist(detector2, source);
    var dtReal = Environment.GetDeltaT(detector1, detector2, source, env.WaveSpeed);

    Console.WriteLine($"Реальные: r1={r1Real:F6}, r2={r2Real:F6}, dt={dtReal:F9}");
    Console.WriteLine($"Результат: r1={r1r2Universal.Item1:F6}, r2={r1r2Universal.Item2:F6}, dt={r1r2Universal.Item3:F9}");
    Console.WriteLine($"Ошибка r1: {(r1r2Universal.Item1 - r1Real) / r1Real * 100:F3}%");
    Console.WriteLine($"Ошибка r2: {(r1r2Universal.Item2 - r2Real) / r2Real * 100:F3}%");
    Console.WriteLine($"Ошибка dt: {(r1r2Universal.Item3 - dtReal) / dtReal * 100:F3}% ⭐ ГЛАВНАЯ МЕТРИКА");
    Console.WriteLine();
}

// Тест 2: Модулированный синус
Console.WriteLine("\n═══════════════════════════════════════════════════════════════════");
Console.WriteLine("  ТИП СИГНАЛА: МОДУЛИРОВАННЫЙ СИНУС sin(2πft) * t");
Console.WriteLine("  ПРИМЕЧАНИЕ: Амплитудная модуляция нарушает предположение 1/r");
Console.WriteLine("  Задержка dt определяется точно, но r1/r2 из амплитуд - неточно");
Console.WriteLine("═══════════════════════════════════════════════════════════════════\n");

foreach (double sr in testSampleRates)
{
    Console.WriteLine($"────────────────────────────────────────────────────");
    Console.WriteLine($"Частота дискретизации: {sr} Гц\n");
    
    Detector detector1 = new Detector(coordD1);
    Detector detector2 = new Detector(coordD2);
    Source source = new ModulatedSinSource(sr, coordS1);

    Environment env = new Environment();
    env.Detectors.Add(detector1);
    env.Detectors.Add(detector2);
    env.Sources.Add(source);

    var signals = env.GetSignals();
    
    // Используем FFT-фазовый метод - самый надёжный для узкополосных сигналов
    var r1r2Universal = TwoMicro.GetR1R2DtFFT(signals[0], signals[1], sr, env.WaveSpeed);

    var r1Real = Environment.GetDist(detector1, source);
    var r2Real = Environment.GetDist(detector2, source);
    var dtReal = Environment.GetDeltaT(detector1, detector2, source, env.WaveSpeed);

    Console.WriteLine($"Реальные: r1={r1Real:F6}, r2={r2Real:F6}, dt={dtReal:F9}");
    Console.WriteLine($"Результат: r1={r1r2Universal.Item1:F6}, r2={r1r2Universal.Item2:F6}, dt={r1r2Universal.Item3:F9}");
    Console.WriteLine($"Ошибка r1: {(r1r2Universal.Item1 - r1Real) / r1Real * 100:F3}%");
    Console.WriteLine($"Ошибка r2: {(r1r2Universal.Item2 - r2Real) / r2Real * 100:F3}%");
    Console.WriteLine($"Ошибка dt: {(r1r2Universal.Item3 - dtReal) / dtReal * 100:F3}% ⭐ ГЛАВНАЯ МЕТРИКА");
    Console.WriteLine();
}

Console.WriteLine("═══════════════════════════════════════════════════════════════════");
