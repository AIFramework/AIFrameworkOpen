using AI.DataStructs.Algebraic;
using System;

namespace AI.HightLevelFunctions;

/// <summary>
/// Функции активации нейронов
/// </summary>
public static class ActivationFunctions
{
    /// <summary>
    /// Softmax (с численной стабильностью)
    /// </summary>
    public static Vector Softmax(Vector inp)
    {
        // Вычитаем максимум для численной стабильности (предотвращение overflow)
        double max = inp.Max();
        Vector shifted = inp - max;
        Vector exp = shifted.Transform(Math.Exp);
        double sum = exp.Sum();
        
        // Защита от деления на ноль (хотя sum exp всегда > 0)
        if (sum < double.Epsilon)
            sum = double.Epsilon;
            
        return exp / sum;
    }

    /// <summary>
    /// Сигмоидальная однополярная активационная функция
    /// </summary>
    /// <param name="Inp">Входной вектор</param>
    /// <param name="betta">Угол наклона</param>
    public static Vector Sigmoid(Vector Inp, double betta = 1)
    {
        return 1.0 / (1 + FunctionsForEachElements.Exp(Inp * (-betta)));
    }
    /// <summary>
    /// Сигмоидальная однополярная активационная функция
    /// </summary>
    /// <param name="x">Входное значение</param>
    /// <param name="betta">Угол наклона</param>
    public static double Sigmoid(double x, double betta = 1)
    {
        return 1.0 / (1 + Math.Exp(x * (-betta)));
    }
    /// <summary>
    /// Сигмоидальная однополярная активационная функция
    /// </summary>
    /// <param name="x">Входное значение</param>
    /// <param name="betta">Угол наклона</param>
    public static double InverseSigmoid(double x, double betta = 1)
    {
        return -Math.Log((1 / x) - 1) / betta;
    }
    /// <summary>
    /// Сигмоидальная однополярная активационная функция
    /// </summary>
    /// <param name="x">Входное значение</param>
    /// <param name="betta">Угол наклона</param>
    public static Vector InverseSigmoid(Vector x, double betta = 1)
    {
        return -FunctionsForEachElements.Ln((1 / x) - 1) / betta;
    }
    /// <summary>
    /// Сигмоидальная биполярная активационная функция
    /// </summary>
    /// <param name="Inp">Входной вектор</param>
    /// <param name="betta">Угол наклона</param>
    public static Vector SigmoidBiplyar(Vector Inp, double betta = 1)
    {
        return FunctionsForEachElements.Tanh(Inp * betta);
    }
    /// <summary>
    /// Пороговая активационная функция
    /// </summary>
    /// <param name="Inp">Входной вектор</param>
    /// <param name="threshold">Порог</param>
    public static Vector Threshold(Vector Inp, double threshold = 0)
    {
        Vector A = new Vector(Inp.Count);
        for (int i = 0; i < Inp.Count; i++)
            if (Inp[i] >= threshold)
                A[i] = 1;
            else
                A[i] = 0;

        return A;
    }
    /// <summary>
    /// Ограничение сверху и снизу
    /// </summary>
    /// <param name="Inp">Входной вектор</param>
    /// <param name="thresholdUp"></param>
    /// <param name="thresholdDoun"></param>
    /// <returns></returns>
    public static Vector Threshold(Vector Inp, double thresholdUp = 1, double thresholdDoun = 0)
    {
        Vector A = new Vector(Inp.Count);
        for (int i = 0; i < Inp.Count; i++)
            if ((Inp[i] >= thresholdDoun) && (Inp[i] <= thresholdUp))
                A[i] = 1;
            else
                A[i] = 0;

        return A;
    }
    /// <summary>
    /// Релу
    /// </summary>
    /// <param name="Inp"></param>
    /// <param name="threshold"></param>
    /// <returns></returns>
    public static Vector Relu(Vector Inp, double threshold = 0)
    {
        Vector A = new Vector(Inp.Count);
        for (int i = 0; i < Inp.Count; i++)
            if (Inp[i] >= threshold)
                A[i] = Inp[i];
            else
                A[i] = 0;

        return A;
    }
    /// <summary>
    /// Активация Релу
    /// </summary>
    /// <param name="Inp">Вход</param>
    /// <param name="thresholdUp">Верхний порог</param>
    /// <param name="thresholdDoun">Нижний порог</param>
    public static Vector Relu(Vector Inp, double thresholdUp, double thresholdDoun = 0)
    {
        Vector A = new Vector(Inp.Count);
        for (int i = 0; i < Inp.Count; i++)
            if ((Inp[i] >= thresholdDoun) && (Inp[i] <= thresholdUp))
                A[i] = Inp[i];
            else
                A[i] = 0;

        return A;
    }
    /// <summary>
    /// Активация Релу
    /// </summary>
    /// <param name="inp">Вход</param>
    /// <param name="thresholdUp">Верхний порог</param>
    /// <param name="thresholdDoun">Нижний порог</param>
    public static Matrix Relu(Matrix inp, double thresholdUp, double thresholdDoun = 0)
    {
        Matrix A = new Matrix(inp.Height, inp.Width);

        for (int i = 0; i < inp.Height; i++)
            for (int j = 0; j < inp.Width; j++)
                if ((inp[i, j] >= thresholdDoun) && (inp[i, j] <= thresholdUp))
                    A[i, j] = inp[i, j];
                else if (inp[i, j] <= thresholdUp)
                    A[i, j] = 0;
                else
                    A[i, j] = 1;
        return A;
    }
    /// <summary>
    /// Сигмоида
    /// </summary>
    /// <param name="Inp"></param>
    /// <param name="betta"></param>
    /// <returns></returns>
    public static Matrix Sigmoid(Matrix Inp, double betta = 1)
    {
        return 1.0 / (1 + FunctionsForEachElements.Exp(Inp * (-betta)));
    }
    /// <summary>
    /// Сигмоида
    /// </summary>
    /// <param name="Inp"></param>
    /// <param name="betta"></param>
    /// <returns></returns>
    public static Matrix SigmoidBiplyar(Matrix Inp, double betta = 1)
    {
        return FunctionsForEachElements.Tanh(Inp * betta);
    }
    /// <summary>
    /// Сигмоида
    /// </summary>
    /// <param name="inp"></param>
    /// <param name="threshold"></param>
    /// <returns></returns>
    public static Matrix Threshold(Matrix inp, double threshold = 0)
    {
        Matrix A = new Matrix(inp.Height, inp.Width);
        int len = A.Shape.Count;

        for (int i = 0; i < len; i++)
            A.Data[i] = (inp.Data[i] >= threshold) ? 1 : 0;

        return A;
    }
    /// <summary>
    /// Сигмоида
    /// </summary>
    /// <param name="tensor">Тензор входных данных</param>
    /// <param name="betta">Коэфициент наклона</param>
    public static Tensor Sigmoid(Tensor tensor, double betta = 1)
    {
        Tensor tensorOut = new Tensor(tensor.Height, tensor.Width, tensor.Depth);
        int len = tensorOut.Shape.Count;

        for (int i = 0; i < len; i++)
            tensorOut.Data[i] = Sigmoid(tensor.Data[i], betta);

        return tensorOut;
    }
    /// <summary>
    /// Логарифм по основанию 10
    /// </summary>
    /// <param name="tensor">Тензор входных данных</param>
    public static Tensor Log10(Tensor tensor)
    {
        Tensor tensorOut = new Tensor(tensor.Height, tensor.Width, tensor.Depth);
        int len = tensorOut.Shape.Count;

        for (int i = 0; i < len; i++)
            tensorOut.Data[i] = Math.Log10(tensor.Data[i]);

        return tensorOut;
    }
    /// <summary>
    /// Активация Релу
    /// </summary>
    /// <param name="Inp">Вход</param>
    /// <param name="threshold">Нижний порог</param>
    public static Matrix Relu(Matrix Inp, double threshold = 0)
    {
        Matrix A = new Matrix(Inp.Height, Inp.Width);

        for (int i = 0; i < Inp.Height; i++)
            for (int j = 0; j < Inp.Width; j++)
                if (Inp[i, j] >= threshold) A[i, j] = Inp[i, j];
                else A[i, j] = 0;
        return A;
    }
    /// <summary>
    /// Активация Релу
    /// </summary>
    /// <param name="Inp">Вход</param>
    /// <param name="threshold">Нижний порог</param>
    public static Vector[] Relu(Vector[] Inp, double threshold = 0)
    {
        Vector[] A = new Vector[Inp.Length];

        for (int i = 0; i < A.Length; i++)
            A[i] = new Vector(Inp[i].Count);

        for (int i = 0; i < Inp.Length; i++)
            for (int j = 0; j < Inp[i].Count; j++)
                if (Inp[i][j] >= threshold) A[i][j] = Inp[i][j];
                else A[i][j] = 0;
        return A;
    }
}