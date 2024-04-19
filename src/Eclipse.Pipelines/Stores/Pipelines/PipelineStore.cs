using Eclipse.Core.Pipelines;

namespace Eclipse.Pipelines.Stores.Pipelines;

// TODO: Rework and enhance
internal sealed class PipelineStore : IPipelineStore
{
    private static readonly Dictionary<string, PipelineInfo> _map = [];

    private readonly IServiceProvider _serviceProvider;

    public PipelineStore(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public PipelineBase? GetOrDefault(PipelineKey key)
    {
        if (!_map.TryGetValue(key.ToCacheKey().Key, out var value))
        {
            return null;
        }
        
        if (_serviceProvider.GetService(value.Type) is not PipelineBase pipeline)
        {
            return null;
        }

        while (pipeline.StagesLeft != value.StagesLeft)
        {
            pipeline.SkipStage();
        }

        return pipeline;
    }

    public void Remove(PipelineKey key)
    {
        _map.Remove(key.ToCacheKey().Key);
    }

    public void Set(PipelineKey key, PipelineBase value)
    {
        _map.Add(key.ToCacheKey().Key, new PipelineInfo { Type = value.GetType(), StagesLeft = value.StagesLeft });
    }

    private class PipelineInfo
    {
        public Type Type { get; set; } = null!;

        public int StagesLeft { get; set; }
    }
}
