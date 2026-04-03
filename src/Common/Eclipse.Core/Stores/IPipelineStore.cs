using Eclipse.Core.Pipelines;

namespace Eclipse.Core.Stores;

public interface IPipelineStore
{
    Task Set(long chatId, int messageId, IPipeline value, CancellationToken cancellationToken = default);

    Task Set(long chatId, IPipeline value, CancellationToken cancellationToken = default);

    Task<IPipeline?> Get(long chatId, int messageId, CancellationToken cancellationToken = default);

    Task<IPipeline?> Get(long chatId, CancellationToken cancellationToken = default);

    Task Remove(long chatId, IPipeline pipeline, CancellationToken cancellationToken = default);
}
