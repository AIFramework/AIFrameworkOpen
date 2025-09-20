using System;

namespace AI.ClassicMath.Calculator;

public readonly struct FunctionDefinition
{
    public int ArgumentCount { get; }
    public Func<object[], object> Delegate { get; }

    public FunctionDefinition(int argumentCount, Func<object[], object> @delegate)
    {
        ArgumentCount = argumentCount;
        Delegate = @delegate;
    }
}

