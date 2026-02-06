using Eclipse.Core.Pipelines;

namespace Eclipse.Core.Stores;

public interface IPipelineStore
{
    Task Set(long chatId, int messageId, PipelineBase value, CancellationToken cancellationToken = default);

    Task Set(long chatId, PipelineBase value, CancellationToken cancellationToken = default);

    Task<PipelineBase?> Get(long chatId, int messageId, CancellationToken cancellationToken = default);

    Task<PipelineBase?> Get(long chatId, CancellationToken cancellationToken = default);

    Task RemoveAll(long chatId, CancellationToken cancellationToken = default);

    Task Remove(long chatId, PipelineBase pipeline, CancellationToken cancellationToken = default);
}
