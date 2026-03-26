using Eclipse.Core.Stores.Cosmos.Containers;

using Microsoft.Azure.Cosmos;

using Newtonsoft.Json;

using Telegram.Bot.Types;

namespace Eclipse.Core.Stores.Cosmos.Stores;

internal sealed class CosmosMessageStore : IMessageStore
{
    private const int MaxItemsCountForLatestMessageQuery = 1;

    private readonly IContainerResolver _containerResolver;

    public CosmosMessageStore(IContainerResolver containerResolver)
    {
        _containerResolver = containerResolver;
    }

    public async Task<Message?> GetLatestBotMessage(long chatId, Type pipelineType, CancellationToken cancellationToken = default)
    {
        var query = new QueryDefinition("""
                select * from c
                where c.Discriminator = 'MessageInfo'
                    and c.chatId = @ChatId
                    and c.PipelineType = @PipelineType
                order by c.Message.Date desc
            """)
            .WithParameter("@ChatId", chatId)
            .WithParameter("@PipelineType", pipelineType.AssemblyQualifiedName);

        using var iterator = _containerResolver.Container.GetItemQueryIterator<MessageInfo>(query, requestOptions: new QueryRequestOptions
        {
            MaxItemCount = MaxItemsCountForLatestMessageQuery
        });

        var messageInfo = await iterator.ReadNextAsync(cancellationToken);

        return messageInfo.Resource.FirstOrDefault()?.Message;
    }

    public async Task RemoveOlderThan(long chatId, DateTime date, CancellationToken cancellationToken = default)
    {
        var query = new QueryDefinition("""
                select * from c
                where c.Discriminator = 'MessageInfo'
                    and c.ChatId = @ChatId
                    and c.Message.Date < @Date
            """)
            .WithParameter("@ChatId", chatId)
            .WithParameter("@Date", date);

        List<string> messageIds = [];

        using var iterator = _containerResolver.Container.GetItemQueryIterator<MessageInfo>(query);

        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync(cancellationToken);
            messageIds.AddRange(response.Resource.Select(mi => mi.Id));
        }

        var removals = messageIds.Select(messageInfoId => _containerResolver.Container.DeleteItemAsync<MessageInfo>(
            messageInfoId,
            new PartitionKey(messageInfoId),
            cancellationToken: cancellationToken
        ));

        await Task.WhenAll(removals);
    }

    public async Task Set(long chatId, Type pipelineType, Message message, CancellationToken cancellationToken = default)
    {
        var id = Guid.CreateVersion7().ToString();

        var messageInfo = new MessageInfo(id, chatId, pipelineType.AssemblyQualifiedName ?? string.Empty, message);

        await _containerResolver.Container.UpsertItemAsync(messageInfo,
            new PartitionKey(messageInfo.Id.ToString()),
            cancellationToken: cancellationToken
        );
    }
}

file sealed record MessageInfo
{
    [JsonProperty("id")]
    public string Id { get; init; }
    public long ChatId { get; init; }
    public string PipelineType { get; init; }
    public Message Message { get; init; }
    public string Discriminator { get; init; }

    public MessageInfo(string cosmosId, long chatId, string pipelineType, Message message, string discriminator = nameof(MessageInfo))
    {
        Id = cosmosId;
        ChatId = chatId;
        PipelineType = pipelineType;
        Message = message;
        Discriminator = discriminator;
    }
}
