using Eclipse.Core.Pipelines;

namespace Eclipse.Pipelines.Pipelines.Store;

public interface IPipelineStore
{
    PipelineBase? GetOrDefault(PipelineKey key);

    void Set(PipelineBase pipeline, PipelineKey key);

    void Remove(PipelineKey key);
}
