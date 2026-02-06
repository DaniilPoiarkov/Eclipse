using Eclipse.Core.Pipelines;

namespace Eclipse.Pipelines.Stores;

public interface IPipelineStore
{
    Task Set(long chatId, PipelineBase value, CancellationToken cancellationToken = default);

    Task RemoveAll(long chatId, CancellationToken cancellationToken = default);

    Task Remove(long chatId, PipelineBase pipeline, CancellationToken cancellationToken = default);

    Task<IEnumerable<PipelineBase>> GetAll(long chatId, CancellationToken cancellationToken = default);
}
