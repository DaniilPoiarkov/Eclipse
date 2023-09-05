using Eclipse.Core.Pipelines;

namespace Eclipse.Application.Contracts.Telegram.Stores;

public interface IPipelineStore
{
    PipelineBase? GetOrDefault(PipelineKey key);

    void Set(PipelineBase pipeline, PipelineKey key);

    void Remove(PipelineKey key);
}
