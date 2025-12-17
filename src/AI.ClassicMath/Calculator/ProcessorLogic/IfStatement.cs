using System.Collections.Generic;
using System.Threading;

namespace AI.ClassicMath.Calculator.ProcessorLogic;

/// <summary>
/// Инструкция 'if-else'.
/// </summary>
internal class IfStatement : Statement
{
    public string Condition { get; }
    public List<Statement> TrueBranch { get; }
    public List<Statement> FalseBranch { get; }

    public IfStatement(string condition, List<Statement> trueBranch, List<Statement> falseBranch)
    {
        Condition = condition;
        TrueBranch = trueBranch;
        FalseBranch = falseBranch;
    }

    public override void Execute(Processor processor, ExecutionContext context, List<string> output, CancellationToken cancellationToken = default)
    {
        var result = processor.AdvancedCalculator.Evaluate(Condition, context, cancellationToken);
        if (processor.IsTruthy(result))
        {
            foreach (var statement in TrueBranch) statement.Execute(processor, context, output, cancellationToken);
        }
        else if (FalseBranch != null)
        {
            foreach (var statement in FalseBranch) statement.Execute(processor, context, output, cancellationToken);
        }
    }
}