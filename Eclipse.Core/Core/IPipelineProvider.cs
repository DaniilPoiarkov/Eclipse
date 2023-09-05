using Eclipse.Core.Pipelines;

namespace Eclipse.Core.Core;

public interface IPipelineProvider
{
    PipelineBase Get(string route);
}
