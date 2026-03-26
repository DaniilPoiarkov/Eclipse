using Eclipse.Core.Pipelines;
using Eclipse.Core.Stores.Cosmos.Containers;

using Microsoft.Azure.Cosmos;

using Newtonsoft.Json;

using System.Net;

namespace Eclipse.Core.Stores.Cosmos.Stores;

internal sealed class CosmosPipelineStore : IPipelineStore
{
    private const int MaxItemsCount = 1;

    private readonly IContainerResolver _containerResolver;

    private readonly IServiceProvider _serviceProvider;

    public CosmosPipelineStore(IContainerResolver containerResolver, IServiceProvider serviceProvider)
    {
        _containerResolver = containerResolver;
        _serviceProvider = serviceProvider;
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
        using var iterator = _containerResolver.Container.GetItemQueryIterator<PipelineInfo>(query, requestOptions: new QueryRequestOptions
        {
            MaxItemCount = MaxItemsCount
        });

        var response = await iterator.ReadNextAsync(cancellationToken);

        if (response.StatusCode != HttpStatusCode.OK)
        {
            return null;
        }

        var pipelineInfo = response.Resource.FirstOrDefault();

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
        var type = pipeline.GetType();

        var query = new QueryDefinition("""
                select * from c
                where c.Discriminator = 'PipelineInfo'
                    and c.ChatId = @ChatId
                    and c.PipelineType = @PipelineType
                    and c.StagesLeft = @StagesLeft
                order by c.CreatedAt desc
            """)
            .WithParameter("@ChatId", chatId)
            .WithParameter("@PipelineType", type.FullName)
            .WithParameter("@StagesLeft", pipeline.StagesLeft);

        using var iterator = _containerResolver.Container.GetItemQueryIterator<PipelineInfo>(query, requestOptions: new QueryRequestOptions
        {
            MaxItemCount = MaxItemsCount
        });

        var response = await iterator.ReadNextAsync(cancellationToken);

        var pipelineInfo = response.Resource.FirstOrDefault();

        if (pipelineInfo is null)
        {
            return;
        }

        await _containerResolver.Container.DeleteItemAsync<PipelineInfo>(
            pipelineInfo.Id.ToString(),
            new PartitionKey(pipelineInfo.Id.ToString()),
            cancellationToken: cancellationToken
        );
    }

    public async Task Set(long chatId, int messageId, IPipeline value, CancellationToken cancellationToken = default)
    {
        var id = Guid.CreateVersion7().ToString();

        var pipelineInfo = new PipelineInfo(
            id,
            chatId,
            messageId,
            value.StagesLeft,
            value.GetType().AssemblyQualifiedName!,
            DateTime.UtcNow
        );

        await _containerResolver.Container.CreateItemAsync(pipelineInfo,
            new PartitionKey(pipelineInfo.Id.ToString()),
            cancellationToken: cancellationToken
        );
    }

    public async Task Set(long chatId, IPipeline value, CancellationToken cancellationToken = default)
    {
        var id = Guid.CreateVersion7().ToString();

        var pipelineInfo = new PipelineInfo(
            id,
            chatId,
            null,
            value.StagesLeft,
            value.GetType().AssemblyQualifiedName!,
            DateTime.UtcNow
        );

        await _containerResolver.Container.CreateItemAsync(pipelineInfo,
            new PartitionKey(pipelineInfo.Id.ToString()),
            cancellationToken: cancellationToken
        );
    }
}

file record PipelineInfo
{
    [JsonProperty("id")]
    public string Id { get; init; }
    public long ChatId { get; init; }
    public int? MessageId { get; init; }
    public int StagesLeft { get; init; }
    public string PipelineType { get; init; }
    public DateTime CreatedAt { get; init; }
    public string Discriminator { get; init; }

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
}
