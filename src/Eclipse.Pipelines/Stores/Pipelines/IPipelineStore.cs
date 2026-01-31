using Eclipse.Core.Pipelines;

using Telegram.Bot.Types;

namespace Eclipse.Pipelines.Stores.Pipelines;

[Obsolete("This PipelineStore is deprecated and will be removed in future versions.", DiagnosticId = "ECLIPSE1006")]
public interface IPipelineStore : IStore<PipelineBase, PipelineKey>
{

}

public interface IPipelineStoreV2
{
    Task<PipelineBase> Get(Update update, CancellationToken cancellationToken = default);

    Task Set(Update update, PipelineBase value, CancellationToken cancellationToken = default);

    Task Remove(Update update, CancellationToken cancellationToken = default);
}
