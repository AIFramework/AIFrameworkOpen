using System.Collections.Generic;

namespace AI.ClassicMath.Calculator.ProcessorLogic;

/// <summary>
/// Инструкция 'while'.
/// </summary>
internal class WhileStatement : Statement
{
    public string Condition { get; }
    public List<Statement> Body { get; }

    public WhileStatement(string condition, List<Statement> body)
    {
        Condition = condition;
        Body = body;
    }

    public override void Execute(Processor processor, ExecutionContext context, List<string> output)
    {
        while (processor.IsTruthy(processor.AdvancedCalculator.Evaluate(Condition, context)))
        {
            foreach (var statement in Body) statement.Execute(processor, context, output);
        }
    }
}