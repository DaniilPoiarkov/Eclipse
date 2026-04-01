using Eclipse.Core.Pipelines;

using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace Eclipse.Core.Stores.Cosmos.Stores;

internal sealed class CosmosPipelineStore : IPipelineStore
{
    private readonly IServiceProvider _serviceProvider;

    private readonly ICosmosStore _cosmosStore;

    private readonly ILogger<CosmosPipelineStore> _logger;

    public CosmosPipelineStore(
        IServiceProvider serviceProvider,
        ICosmosStore cosmosStore,
        ILogger<CosmosPipelineStore> logger)
    {
        _serviceProvider = serviceProvider;
        _cosmosStore = cosmosStore;
        _logger = logger;
    }

    public Task<IPipeline?> Get(long chatId, int messageId, CancellationToken cancellationToken = default)
    {
        var query = new QueryDefinition("""
                select * from c
                where c.Discriminator = 'PipelineInfo'
                    and c.ChatId = @ChatId
                    and c.MessageId = @MessageId
                order by c.CreatedAt desc
            """)
            .WithParameter("@ChatId", chatId)
            .WithParameter("@MessageId", messageId);

        return Get(query, cancellationToken);
    }

    public Task<IPipeline?> Get(long chatId, CancellationToken cancellationToken = default)
    {
        var query = new QueryDefinition("""
                select * from c
                where c.Discriminator = 'PipelineInfo'
                    and c.ChatId = @ChatId
                order by c.CreatedAt desc
            """)
            .WithParameter("@ChatId", chatId);

        return Get(query, cancellationToken);
    }

    private async Task<IPipeline?> Get(QueryDefinition query, CancellationToken cancellationToken)
    {
        var pipelineInfo = await _cosmosStore.Get<PipelineInfo>(query, cancellationToken);

        if (pipelineInfo is null)
        {
            return null;
        }

        var pipelineType = Type.GetType(pipelineInfo.PipelineType);

        if (pipelineType is null)
        {
            return null;
        }

        if (_serviceProvider.GetService(pipelineType) is not IPipeline pipeline)
        {
            return null;
        }

        while (pipeline.StagesLeft != pipelineInfo.StagesLeft)
        {
            pipeline.SkipStage();
        }

        return pipeline;
    }

    public async Task Remove(long chatId, IPipeline pipeline, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Entered removing pipeline for chat {ChatId} with {StagesLeft} stages left", chatId, pipeline.StagesLeft);

        var query = new QueryDefinition("""
                select * from c
                where c.Discriminator = @Discriminator
                    and c.ChatId = @ChatId
                    and c.PipelineType = @PipelineType
                    and c.StagesLeft = @StagesLeft
                order by c.CreatedAt desc
            """)
            .WithParameter("@Discriminator", nameof(PipelineInfo))
            .WithParameter("@ChatId", chatId)
            .WithParameter("@PipelineType", pipeline.GetType().AssemblyQualifiedName)
            .WithParameter("@StagesLeft", pipeline.StagesLeft);

        var pipelineInfo = await _cosmosStore.Get<PipelineInfo>(query, cancellationToken);

        if (pipelineInfo is null)
        {
            _logger.LogWarning("Pipeline for chat {ChatId} with {StagesLeft} stages left not found.", chatId, pipeline.StagesLeft);
            return;
        }

        _logger.LogInformation("Removing pipeline id {Id} for chat {ChatId} with {StagesLeft} stages left..", pipelineInfo.Id, chatId, pipeline.StagesLeft);
        await _cosmosStore.Remove(pipelineInfo, cancellationToken);
    }

    public Task Set(long chatId, int messageId, IPipeline value, CancellationToken cancellationToken = default)
    {
        var pipelineInfo = new PipelineInfo(
            Guid.CreateVersion7().ToString(),
            chatId,
            messageId,
            value.StagesLeft,
            value.GetType().AssemblyQualifiedName!,
            DateTime.UtcNow
        );

        return _cosmosStore.Set(pipelineInfo, cancellationToken);
    }

    public Task Set(long chatId, IPipeline value, CancellationToken cancellationToken = default)
    {
        var pipelineInfo = new PipelineInfo(
            Guid.CreateVersion7().ToString(),
            chatId,
            null,
            value.StagesLeft,
            value.GetType().AssemblyQualifiedName!,
            DateTime.UtcNow
        );

        return _cosmosStore.Set(pipelineInfo, cancellationToken);
    }
}

file record PipelineInfo : IStoreModel
{
    [JsonProperty("id")]
    public string Id { get; init; } = string.Empty;
    public long ChatId { get; init; }
    public int? MessageId { get; init; }
    public int StagesLeft { get; init; }
    public string PipelineType { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public string Discriminator { get; init; } = string.Empty;

    public PipelineInfo(string id, long chatId, int? messageId, int stagesLeft, string pipelineType, DateTime createdAt, string discriminator = nameof(PipelineInfo))
    {
        Id = id;
        ChatId = chatId;
        MessageId = messageId;
        StagesLeft = stagesLeft;
        PipelineType = pipelineType;
        CreatedAt = createdAt;
        Discriminator = discriminator;
    }

    public Dictionary<string, object?> ToDictionary()
    {
        return new Dictionary<string, object?>
        {
            ["id"] = $"{Discriminator}|{Id}",
            [nameof(ChatId)] = ChatId,
            [nameof(MessageId)] = MessageId,
            [nameof(StagesLeft)] = StagesLeft,
            [nameof(PipelineType)] = PipelineType,
            [nameof(CreatedAt)] = CreatedAt,
            [nameof(Discriminator)] = Discriminator
        };
    }
}
