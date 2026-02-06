using Eclipse.Core.Pipelines;
using Eclipse.Core.Stores;

namespace Eclipse.Pipelines.Stores.InMemory;

internal sealed class InMemoryPipelineStore : IPipelineStore
{
    private static readonly Dictionary<long, List<PipelineInfo>> _cahce = [];

    private readonly IServiceProvider _serviceProvider;

    public InMemoryPipelineStore(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task RemoveAll(long chatId, CancellationToken cancellationToken = default)
    {
        _cahce.Remove(chatId);
        return Task.CompletedTask;
    }

    public Task Set(long chatId, PipelineBase pipeline, CancellationToken cancellationToken = default)
    {
        if (!_cahce.TryGetValue(chatId, out var pipelines))
        {
            pipelines = [];
            _cahce[chatId] = pipelines;
        }

        var type = pipeline.GetType();

        if (!pipelines.Exists(p => p.PipelineType == type && p.StagesLeft == pipeline.StagesLeft))
        {
            pipelines.Add(new PipelineInfo(null, pipeline.StagesLeft, type));
        }

        return Task.CompletedTask;
    }

    public Task Remove(long chatId, PipelineBase pipeline, CancellationToken cancellationToken = default)
    {
        var type = pipeline.GetType();

        if (_cahce.TryGetValue(chatId, out var pipelines)
            && pipelines.Find(p => p.PipelineType == type && p.StagesLeft == pipeline.StagesLeft) is PipelineInfo pipelineInfo)
        {
            pipelines.Remove(pipelineInfo);
        }

        return Task.CompletedTask;
    }

    public Task Set(long chatId, int messageId, PipelineBase value, CancellationToken cancellationToken = default)
    {
        if (!_cahce.TryGetValue(chatId, out var pipelines))
        {
            pipelines = [];
            _cahce[chatId] = pipelines;
        }

        var type = value.GetType();

        var current = pipelines.Find(p => p.PipelineType == type);

        if (current is not null)
        {
            pipelines.Remove(current);
        }

        pipelines.Add(new PipelineInfo(messageId, value.StagesLeft, type));

        return Task.CompletedTask;
    }

    public Task<PipelineBase?> Get(long chatId, int messageId, CancellationToken token = default)
    {
        if (!_cahce.TryGetValue(chatId, out var pipelines))
        {
            return Task.FromResult<PipelineBase?>(null);
        }

        var pipelineInfo = pipelines.Find(p => p.MessageId == messageId);

        if (pipelineInfo is null)
        {
            return Task.FromResult<PipelineBase?>(null);
        }

        if (_serviceProvider.GetService(pipelineInfo.PipelineType) is not PipelineBase pipeline)
        {
            return Task.FromResult<PipelineBase?>(null);
        }

        while (pipeline.StagesLeft != pipelineInfo.StagesLeft)
        {
            pipeline.SkipStage();
        }

        return Task.FromResult<PipelineBase?>(pipeline);
    }

    public Task<PipelineBase?> Get(long chatId, CancellationToken cancellationToken = default)
    {
        if (!_cahce.TryGetValue(chatId, out var pipelines))
        {
            return Task.FromResult<PipelineBase?>(null);
        }

        var pipelineInfo = pipelines.SingleOrDefault();

        if (pipelineInfo is null)
        {
            return Task.FromResult<PipelineBase?>(null);
        }

        if (_serviceProvider.GetService(pipelineInfo.PipelineType) is not PipelineBase pipeline)
        {
            return Task.FromResult<PipelineBase?>(null);
        }

        while (pipeline.StagesLeft != pipelineInfo.StagesLeft)
        {
            pipeline.SkipStage();
        }

        return Task.FromResult<PipelineBase?>(pipeline);
    }

    private record PipelineInfo(int? MessageId, int StagesLeft, Type PipelineType);
}
