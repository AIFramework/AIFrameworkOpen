using System;
using System.Collections.Generic;

namespace AI.ClassicMath.Calculator;

[Serializable]
public class FunctionDefinition
{
    public int ArgumentCount { get; set; }
    public Func<object[], object> Delegate { get; set; }

    public string Name { get; set; }

    public DescriptionFunction Description { get; set; }

    public FunctionDefinition() { }

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
    public List<string> AreaList { get; set; }
    public string Exemple { get; set; }
}

