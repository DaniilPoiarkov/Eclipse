using Eclipse.Core.Pipelines;

namespace Eclipse.Pipelines.Stores.Pipelines;

internal sealed class PipelineStore : IPipelineStore
{
    private static readonly Dictionary<string, Type> _map = [];

    private readonly IServiceProvider _serviceProvider;

    public PipelineStore(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public PipelineBase? GetOrDefault(PipelineKey key)
    {
        if (!_map.TryGetValue(key.ToCacheKey().Key, out var type))
        {
            return null;
        }
        
        _ = 1;

        var pipeline = _serviceProvider.GetService(type) as PipelineBase;

        return pipeline;
    }

    public void Remove(PipelineKey key)
    {
        _map.Remove(key.ToCacheKey().Key);
    }

    public void Set(PipelineKey key, PipelineBase value)
    {
        _map.Add(key.ToCacheKey().Key, value.GetType());
    }
}
