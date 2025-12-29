using System;

namespace AI.ClassicMath.Calculator.ProcessorLogic;

/// <summary>
/// Исключение для прерывания цикла (break)
/// </summary>
internal class BreakException : Exception
{
    public BreakException() : base("Break statement executed")
    {
    }
}

