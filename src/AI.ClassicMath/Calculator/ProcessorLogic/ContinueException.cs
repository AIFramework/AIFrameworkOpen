using System;

namespace AI.ClassicMath.Calculator.ProcessorLogic;

/// <summary>
/// Исключение для продолжения следующей итерации цикла (continue)
/// </summary>
internal class ContinueException : Exception
{
    public ContinueException() : base("Continue statement executed")
    {
    }
}

