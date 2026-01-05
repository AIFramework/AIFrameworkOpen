using AI;
using AI.DataStructs.Algebraic;
using AI.DataStructs.WithComplexElements;
using AI.DSP.DSPCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalArray;

public class TwoMicro
{
    ///// <summary>
    ///// Вычисляет среднеквадратичное значение (RMS) сигнала
    ///// RMS более устойчив к модуляции амплитуды, чем пиковое значение
    ///// </summary>
    //private static double CalculateRMS(Vector signal)
    //{
    //    double sumSquares = 0;
    //    for (int i = 0; i < signal.Count; i++)
    //    {
    //        sumSquares += signal[i] * signal[i];
    //    }
    //    return Math.Sqrt(sumSquares / signal.Count);
    //}
    
    ///// <summary>
    ///// Вычисляет отношение расстояний из амплитуд сигналов
    ///// Использует RMS для устойчивости к модуляции
    ///// </summary>
    //private static double CalculateDistanceRatio(Vector sig1, Vector sig2)
    //{
    //    double rms1 = CalculateRMS(sig1);
    //    double rms2 = CalculateRMS(sig2);
    //    return (rms1 * rms1) / (rms2 * rms2);
    //}
    
    public static Tuple<double, double, double> GetR1R2DtFFT(Vector sig1, Vector sig2, double sr, double v)
    {
        ComplexVector Sp1 = FFT.CalcFFT(sig1);
        ComplexVector Sp2 = FFT.CalcFFT(sig2);

        Vector a1 = Sp1.MagnitudeVector, a2 = Sp2.MagnitudeVector;
        
        // Ищем только в первой половине спектра (положительные частоты)
        // Вторая половина - зеркальное отражение для действительных сигналов
        int halfSize = Sp1.Count / 2;
        Vector a1_half = new Vector(halfSize);
        Vector a2_half = new Vector(halfSize);
        for (int i = 0; i < halfSize; i++)
        {
            a1_half[i] = a1[i];
            a2_half[i] = a2[i];
        }
        
        int idx = (a1_half + a2_half).MaxElementIndex();

        if (idx == 0) idx = 1;

        double phase1 = Math.Atan2(Sp1[idx].Imaginary, Sp1[idx].Real);
        double phase2 = Math.Atan2(Sp2[idx].Imaginary, Sp2[idx].Real);
        double df = phase2 - phase1;

        if (df > Math.PI) df -= 2 * Math.PI;
        if (df < -Math.PI) df += 2 * Math.PI;

        // dt = -df / (2*pi*f) где f = idx * sr / Sp1.Count
        // Отрицательный знак т.к. df < 0 означает sig2 запаздывает (dt > 0)
        double dt = -Sp1.Count * df / (sr * idx * 2 * Math.PI);
        double dr = Math.Abs(dt * v);

        double ratioR_sq = (sig1.Max() * sig1.Max()) / (sig2.Max() * sig2.Max());

        double r1, r2;

        if (ratioR_sq > 1)
        {
            r1 = dr / (Math.Sqrt(ratioR_sq) - 1);
            r2 = r1 * Math.Sqrt(ratioR_sq);
        }
        else
        {
            ratioR_sq = 1 / ratioR_sq;
            r2 = dr / (Math.Sqrt(ratioR_sq) - 1);
            r1 = r2 * Math.Sqrt(ratioR_sq);
        }

        return Tuple.Create(r1, r2, dt);
    }


    //public static Tuple<double, double, double> GetR1R2DtCorrelation(Vector sig1, Vector sig2, double sr, double v)
    //{
    //    var corr = Correlation.CrossCorrelation(sig1, sig2);
    //    int centerIndex = sig2.Count;
        
    //    // ЧЕСТНЫЙ глобальный поиск максимума
    //    int maxIndex = corr.MaxElementIndex();
        
    //    // ВАЖНОЕ ОГРАНИЧЕНИЕ МЕТОДА:
    //    // Для узкополосных периодических сигналов (чистый синус) корреляция имеет
    //    // множество одинаковых локальных максимумов с интервалом T = sr/F0.
    //    // В этом случае глобальный максимум может быть НЕ на правильной задержке,
    //    // а со сдвигом на N*T отсчетов. Это НЕ баг - это фундаментальное свойство метода!
    //    //
    //    // Решение: использовать другие методы для периодических сигналов:
    //    // - FFT-метод (GetR1R2DtFFT) - отлично работает для узкополосных
    //    // - GCC-PHAT (GetR1R2DtGCCPHAT) - универсальный для любых сигналов
    //    //
    //    // Данный метод работает ОТЛИЧНО для:
    //    // - широкополосных сигналов (речь, шум)
    //    // - импульсных сигналов
    //    // - сложных непериодических сигналов
        
    //    // Субвыборочная интерполяция пика
    //    double fractionalShift = 0;
    //    if (maxIndex > 0 && maxIndex < corr.Count - 1)
    //    {
    //        double y1 = corr[maxIndex - 1];
    //        double y2 = corr[maxIndex];
    //        double y3 = corr[maxIndex + 1];
            
    //        double denominator = y1 - 2 * y2 + y3;
    //        if (Math.Abs(denominator) > 1e-10)
    //        {
    //            fractionalShift = 0.5 * (y1 - y3) / denominator;
    //        }
    //    }
        
    //    double dt = (centerIndex - maxIndex - fractionalShift) / sr;
    //    double dr = Math.Abs(dt * v);

    //    double ratioR_sq = (sig1.Max() * sig1.Max()) / (sig2.Max() * sig2.Max());

    //    double r1, r2;

    //    if (ratioR_sq > 1)
    //    {
    //        r1 = dr / (Math.Sqrt(ratioR_sq) - 1);
    //        r2 = r1 * Math.Sqrt(ratioR_sq);
    //    }
    //    else
    //    {
    //        ratioR_sq = 1 / ratioR_sq;
    //        r2 = dr / (Math.Sqrt(ratioR_sq) - 1);
    //        r1 = r2 * Math.Sqrt(ratioR_sq);
    //    }

    //    return Tuple.Create(r1, r2, dt);
    //}

    ///// <summary>
    ///// УНИВЕРСАЛЬНЫЙ метод определения задержки
    ///// Автоматически выбирает лучший алгоритм в зависимости от типа сигнала:
    ///// - Узкополосные (синус, тон) -> FFT-фазовый метод
    ///// - Широкополосные (речь, шум) -> GCC-PHAT
    ///// </summary>
    //public static Tuple<double, double, double> GetR1R2DtUniversal(Vector sig1, Vector sig2, double sr, double v)
    //{
    //    // Определяем тип сигнала через анализ спектра
    //    int N = Functions.NextPow2(Math.Max(sig1.Count, sig2.Count));
    //    ComplexVector Sp1 = FFT.CalcFFT(sig1.CutAndZero(N));
        
    //    // Вычисляем спектральную энергию (только положительные частоты)
    //    Vector magnitudes = new Vector(N / 2);
    //    for (int i = 0; i < N / 2; i++)
    //    {
    //        magnitudes[i] = Sp1[i].Magnitude;
    //    }
        
    //    // Находим максимальную магнитуду и считаем значимые пики
    //    double maxMagnitude = magnitudes.Max();
    //    double threshold = maxMagnitude * 0.1; // 10% от максимума
        
    //    int significantPeaks = 0;
    //    double totalEnergy = 0;
    //    double significantEnergy = 0;
        
    //    for (int i = 0; i < magnitudes.Count; i++)
    //    {
    //        double energy = magnitudes[i] * magnitudes[i];
    //        totalEnergy += energy;
            
    //        if (magnitudes[i] > threshold)
    //        {
    //            significantPeaks++;
    //            significantEnergy += energy;
    //        }
    //    }
        
    //    double energyConcentration = significantEnergy / totalEnergy;
        
    //    Console.WriteLine($"[UNIVERSAL] Significant peaks (>10% max): {significantPeaks}");
    //    Console.WriteLine($"[UNIVERSAL] Energy concentration: {energyConcentration:F4}");
        
    //    // Узкополосный сигнал: мало пиков (< 10) и высокая концентрация энергии (>0.8)
    //    if (significantPeaks < 10 && energyConcentration > 0.8)
    //    {
    //        Console.WriteLine($"[UNIVERSAL] Detected narrowband signal -> using FFT phase method\n");
    //        return GetR1R2DtFFT(sig1, sig2, sr, v);
    //    }
    //    else
    //    {
    //        Console.WriteLine($"[UNIVERSAL] Detected wideband signal -> using GCC-PHAT method\n");
    //        return GetR1R2DtGCCPHAT(sig1, sig2, sr, v);
    //    }
    //}
    
    ///// <summary>
    ///// GCC-PHAT (Generalized Cross-Correlation with PHAse Transform) метод
    ///// Универсальный метод для широкополосных сигналов:
    ///// - речь, шум - работает отлично
    ///// - импульсные - работает отлично
    ///// Устойчив к реверберации и многолучевому распространению
    ///// НЕ ПОДХОДИТ для узкополосных (синус) - используйте FFT-фазовый метод!
    ///// </summary>
    //public static Tuple<double, double, double> GetR1R2DtGCCPHAT(Vector sig1, Vector sig2, double sr, double v)
    //{
    //    // FFT сигналов (дополняем до степени двойки)
    //    int N = Functions.NextPow2(Math.Max(sig1.Count, sig2.Count));
    //    ComplexVector Sp1 = FFT.CalcFFT(sig1.CutAndZero(N));
    //    ComplexVector Sp2 = FFT.CalcFFT(sig2.CutAndZero(N));
        
    //    // Cross-spectrum: Sp1 * conj(Sp2) 
    //    // Если sig2 запаздывает, фаза будет положительной
    //    ComplexVector crossSpectrum = new ComplexVector(N);
    //    for (int i = 0; i < N; i++)
    //    {
    //        var product = Sp1[i] * System.Numerics.Complex.Conjugate(Sp2[i]);
    //        double magnitude = product.Magnitude;
            
    //        // PHAT weighting: нормализация на магнитуду
    //        // Это подавляет доминирующие частоты и делает метод более устойчивым
    //        // Работает ТОЛЬКО для широкополосных сигналов!
    //        if (magnitude > 1e-10)
    //            crossSpectrum[i] = product / magnitude;
    //        else
    //            crossSpectrum[i] = 0;
    //    }
        
    //    // IFFT для получения GCC-PHAT функции
    //    ComplexVector gccPhat = FFT.CalcIFFT(crossSpectrum);
        
    //    // Берем только действительную часть
    //    Vector gccReal = gccPhat.RealVector;
        
    //    // Применяем fftshift: переносим вторую половину в начало
    //    // До shift: [0...N-1], где [0...N/2-1] - положительные задержки, [N/2...N-1] - отрицательные
    //    // После shift: [-N/2...N/2-1], центр в позиции N/2
    //    Vector gccShifted = new Vector(N);
    //    int halfN = N / 2;
    //    for (int i = 0; i < N; i++)
    //    {
    //        gccShifted[i] = gccReal[(i + halfN) % N];
    //    }
        
    //    // Ищем максимум в shifted версии
    //    int maxIndex = gccShifted.MaxElementIndex();
        
    //    // Преобразуем индекс в задержку
    //    // После shift: индекс 0 соответствует задержке -N/2 samples
    //    // индекс halfN соответствует задержке 0 samples
    //    int delayInSamples = maxIndex - halfN;
        
    //    // Субвыборочная интерполяция
    //    double fractionalShift = 0;
    //    if (maxIndex > 0 && maxIndex < N - 1)
    //    {
    //        double y1 = gccShifted[maxIndex - 1];
    //        double y2 = gccShifted[maxIndex];
    //        double y3 = gccShifted[maxIndex + 1];
            
    //        double denominator = y1 - 2 * y2 + y3;
    //        if (Math.Abs(denominator) > 1e-10)
    //        {
    //            fractionalShift = 0.5 * (y1 - y3) / denominator;
    //        }
    //    }
        
    //    // Положительный delay означает sig2 запаздывает
    //    double dt = (delayInSamples + fractionalShift) / sr;
    //    double dr = Math.Abs(dt * v);
        
    //    // Расчет r1, r2 из амплитуд (используем RMS для устойчивости к модуляции)
    //    double ratioR_sq = CalculateDistanceRatio(sig1, sig2);
        
    //    double r1, r2;
        
    //    if (ratioR_sq > 1)
    //    {
    //        r1 = dr / (Math.Sqrt(ratioR_sq) - 1);
    //        r2 = r1 * Math.Sqrt(ratioR_sq);
    //    }
    //    else
    //    {
    //        ratioR_sq = 1 / ratioR_sq;
    //        r2 = dr / (Math.Sqrt(ratioR_sq) - 1);
    //        r1 = r2 * Math.Sqrt(ratioR_sq);
    //    }
        
    //    return Tuple.Create(r1, r2, dt);
    //}
}
