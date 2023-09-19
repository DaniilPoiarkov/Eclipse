using Eclipse.Application.Contracts.Telegram.Messages;
using Eclipse.Infrastructure.Cache;

using Telegram.Bot.Types;

namespace Eclipse.Application.Telegram.Messages;

internal class MessageStore : IMessageStore
{
    private readonly ICacheService _cacheService;

    public MessageStore(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public Message? GetMessage(MessageKey key) =>
        _cacheService.GetAndDelete<Message>(CreateKey(key));

    public void SaveMessage(MessageKey key, Message message) =>
        _cacheService.Set(CreateKey(key), message);

    private static CacheKey CreateKey(MessageKey key) =>
        new($"Message-{key.ChatId}");
}
