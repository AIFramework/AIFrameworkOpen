using System;

namespace AI.ClassicMath.Calculator;

[Serializable]
public class FunctionDefinition
{
    public int ArgumentCount { get; }
    public Func<object[], object> Delegate { get; }

    public string Name { get; set; }

    public DescriptionFunction Description { get; set; }

    public FunctionDefinition(int argumentCount, Func<object[], object> @delegate)
    {
        ArgumentCount = argumentCount;
        Delegate = @delegate;
    }
}

[Serializable]
public class DescriptionFunction 
{
    public string Signature { get; set; }
    public string Description { get; set; }
    public string Area { get; set; }
}

