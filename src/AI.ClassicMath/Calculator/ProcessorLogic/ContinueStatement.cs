using System.Collections.Generic;
using System.Threading;

namespace AI.ClassicMath.Calculator.ProcessorLogic;

/// <summary>
/// Инструкция 'continue' - переход к следующей итерации цикла
/// </summary>
internal class ContinueStatement : Statement
{
    public override void Execute(Processor processor, ExecutionContext context, List<string> output, CancellationToken cancellationToken = default)
    {
        throw new ContinueException();
    }
}

