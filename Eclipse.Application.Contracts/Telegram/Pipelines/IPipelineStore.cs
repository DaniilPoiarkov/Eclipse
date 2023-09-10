using Eclipse.Core.Pipelines;

namespace Eclipse.Application.Contracts.Telegram.Pipelines;

public interface IPipelineStore
{
    PipelineBase? GetOrDefault(PipelineKey key);

    void Set(PipelineBase pipeline, PipelineKey key);

    void Remove(PipelineKey key);
}
