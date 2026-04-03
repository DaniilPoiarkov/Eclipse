using Microsoft.Azure.Cosmos;

using Newtonsoft.Json;

using Telegram.Bot.Types;

namespace Eclipse.Core.Stores.Cosmos.Stores;

internal sealed class CosmosMessageStore : IMessageStore
{
    private readonly ICosmosStore _cosmosStore;

    public CosmosMessageStore(ICosmosStore cosmosStore)
    {
        _cosmosStore = cosmosStore;
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

        var messageInfo = await _cosmosStore.Get<MessageInfo>(query, cancellationToken);

        return messageInfo?.Message;
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

        var messages = await _cosmosStore.GetAll<MessageInfo>(query, cancellationToken);

        var removals = messages.Select(messageInfo => _cosmosStore.Remove(messageInfo, cancellationToken));

        await Task.WhenAll(removals);
    }

    public Task Set(long chatId, Type pipelineType, Message message, CancellationToken cancellationToken = default)
    {
        var messageInfo = new MessageInfo(Guid.CreateVersion7().ToString(), chatId, pipelineType.AssemblyQualifiedName ?? string.Empty, message);

        return _cosmosStore.Set(messageInfo, cancellationToken);
    }
}

file sealed record MessageInfo : IStoreModel
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

    public Dictionary<string, object?> ToDictionary()
    {
        return new Dictionary<string, object?>
        {
            ["id"] = Id,
            [nameof(ChatId)] = ChatId,
            [nameof(PipelineType)] = PipelineType,
            [nameof(Message)] = Message,
            [nameof(Discriminator)] = Discriminator
        };
    }
}
