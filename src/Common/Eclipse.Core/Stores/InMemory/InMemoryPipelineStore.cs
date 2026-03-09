using Eclipse.Core.Pipelines;

using System.Collections.Concurrent;

namespace Eclipse.Core.Stores.InMemory;

internal sealed class InMemoryPipelineStore : IPipelineStore
{
    private static readonly ConcurrentDictionary<long, List<PipelineInfo>> _cahce = [];

    private readonly IServiceProvider _serviceProvider;

    public InMemoryPipelineStore(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task Set(long chatId, IPipeline pipeline, CancellationToken cancellationToken = default)
    {
        if (!_cahce.TryGetValue(chatId, out var pipelines))
        {
            pipelines = [];
            _cahce[chatId] = pipelines;
        }

        pipelines.Add(new PipelineInfo(null, pipeline.StagesLeft, pipeline.GetType(), DateTime.UtcNow));

        return Task.CompletedTask;
    }

    public Task Remove(long chatId, IPipeline pipeline, CancellationToken cancellationToken = default)
    {
        var type = pipeline.GetType();

        if (_cahce.TryGetValue(chatId, out var pipelines)
            && pipelines.Find(p => p.PipelineType == type && p.StagesLeft == pipeline.StagesLeft) is PipelineInfo pipelineInfo)
        {
            pipelines.Remove(pipelineInfo);
        }

        return Task.CompletedTask;
    }

    public Task Set(long chatId, int messageId, IPipeline value, CancellationToken cancellationToken = default)
    {
        if (!_cahce.TryGetValue(chatId, out var pipelines))
        {
            pipelines = [];
            _cahce[chatId] = pipelines;
        }

        pipelines.Add(new PipelineInfo(messageId, value.StagesLeft, value.GetType(), DateTime.UtcNow));

        return Task.CompletedTask;
    }

    public Task<IPipeline?> Get(long chatId, int messageId, CancellationToken cancellationToken = default)
    {
        if (!_cahce.TryGetValue(chatId, out var pipelines))
        {
            return Task.FromResult<IPipeline?>(null);
        }

        var pipelineInfo = pipelines.Find(p => p.MessageId == messageId);

        if (pipelineInfo is null)
        {
            return Task.FromResult<IPipeline?>(null);
        }

        if (_serviceProvider.GetService(pipelineInfo.PipelineType) is not IPipeline pipeline)
        {
            return Task.FromResult<IPipeline?>(null);
        }

        while (pipeline.StagesLeft != pipelineInfo.StagesLeft)
        {
            pipeline.SkipStage();
        }

        return Task.FromResult<IPipeline?>(pipeline);
    }

    public Task<IPipeline?> Get(long chatId, CancellationToken cancellationToken = default)
    {
        if (!_cahce.TryGetValue(chatId, out var pipelines))
        {
            return Task.FromResult<IPipeline?>(null);
        }

        var pipelineInfo = pipelines.MaxBy(p => p.CreatedAt);

        if (pipelineInfo is null)
        {
            return Task.FromResult<IPipeline?>(null);
        }

        if (_serviceProvider.GetService(pipelineInfo.PipelineType) is not IPipeline pipeline)
        {
            return Task.FromResult<IPipeline?>(null);
        }

        while (pipeline.StagesLeft != pipelineInfo.StagesLeft)
        {
            pipeline.SkipStage();
        }

        return Task.FromResult<IPipeline?>(pipeline);
    }

    private record PipelineInfo(int? MessageId, int StagesLeft, Type PipelineType, DateTime CreatedAt);
}
