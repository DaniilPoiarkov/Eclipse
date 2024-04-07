using Eclipse.Core.Pipelines;

namespace Eclipse.Pipelines.Stores.Pipelines;

// TODO: Find a way to handle states without json or bytes serialization.
//       This will work fine only in one app instance, but it will fail with multiple app instances
internal sealed class PipelineStore : IPipelineStore
{
    private static readonly Dictionary<string, PipelineBase> _map = [];

    public Task<PipelineBase?> GetOrDefaultAsync(PipelineKey key, CancellationToken cancellationToken = default)
    {
        if (_map.TryGetValue(KeyToString(key), out var pipeline))
        {
            return Task.FromResult<PipelineBase?>(pipeline);
        }

        return Task.FromResult<PipelineBase?>(null);
    }

    public Task RemoveAsync(PipelineKey key, CancellationToken cancellationToken = default)
    {
        _map.Remove(KeyToString(key));
        return Task.CompletedTask;
    }

    public Task SetAsync(PipelineKey key, PipelineBase value, CancellationToken cancellationToken = default)
    {
        _map[KeyToString(key)] = value;
        return Task.CompletedTask;
    }

    private static string KeyToString(PipelineKey key) => key.ToCacheKey().Key;
}
