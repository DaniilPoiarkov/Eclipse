using Eclipse.Common.EventBus;
using Eclipse.Common.Events;

using Newtonsoft.Json;

using StackExchange.Redis;

namespace Eclipse.Infrastructure.EventBus.Redis;

internal sealed class RedisEventBus : IEventBus
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public RedisEventBus(IConnectionMultiplexer connectionMultiplexer)
    {
        _connectionMultiplexer = connectionMultiplexer;
    }

    public Task Publish<TEvent>(TEvent @event, CancellationToken cancellationToken)
        where TEvent : class, IDomainEvent
    {
        var publisher = _connectionMultiplexer.GetSubscriber();

        var message = JsonConvert.SerializeObject(@event, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
        });

        return publisher.PublishAsync(
            RedisChannel.Literal("domain-events"),
            new RedisValue(message)
        );
    }
}
