using Eclipse.Core.Context;
using Eclipse.Core.Results;

namespace Eclipse.Core.Stages;

/// <summary>
/// Defines one step of specific <a cref="Pipelines.PipelineBase"></a>
/// </summary>
public interface IStage
{
    Task<IResult> RunAsync(MessageContext context, CancellationToken cancellationToken = default);
}
