using Eclipse.Core.Pipelines;

namespace Eclipse.Pipelines.Stores.InMemory;

internal sealed class InMemoryPipelineStore : IPipelineStore
{
    private static readonly Dictionary<long, List<PipelineBase>> _cahce = [];

    public Task RemoveAll(long chatId, CancellationToken cancellationToken = default)
    {
        _cahce.Remove(chatId);
        return Task.CompletedTask;
    }

    public Task Set(long chatId, PipelineBase value, CancellationToken cancellationToken = default)
    {
        if (!_cahce.TryGetValue(chatId, out var pipelines))
        {
            pipelines = [];
            _cahce[chatId] = pipelines;
        }

        if (!pipelines.Contains(value))
        {
            pipelines.Add(value);
        }

        return Task.CompletedTask;
    }

    public Task<IEnumerable<PipelineBase>> GetAll(long chatId, CancellationToken cancellationToken = default)
    {
        if (_cahce.TryGetValue(chatId, out var pipelines))
        {
            return Task.FromResult(pipelines.AsEnumerable());
        }

        return Task.FromResult(Enumerable.Empty<PipelineBase>());
    }

    public Task Remove(long chatId, PipelineBase pipeline, CancellationToken cancellationToken = default)
    {
        if (_cahce.TryGetValue(chatId, out var pipelines))
        {
            pipelines.Remove(pipeline);
        }

        return Task.CompletedTask;
    }
}
