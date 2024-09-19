using Eclipse.Common.Events;
using Eclipse.Domain.OutboxMessages;

using Newtonsoft.Json;

namespace Eclipse.Tests.OutboxMessages;

public static class OutboxMessageGenerator
{
    public static IEnumerable<OutboxMessage> Generate(int count, IDomainEvent? domainEvent = null)
    {
        domainEvent ??= new TestOutboxMessageDomainEvent("test");

        return Enumerable.Range(0, count)
            .Select(_ => new OutboxMessage(
                Guid.NewGuid(),
                domainEvent.GetType().Name,
                JsonConvert.SerializeObject(domainEvent, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                }),
                DateTime.UtcNow)
            );
    }
}
