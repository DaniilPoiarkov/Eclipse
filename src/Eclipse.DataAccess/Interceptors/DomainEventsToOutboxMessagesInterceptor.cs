using Eclipse.Common.Clock;
using Eclipse.Domain.OutboxMessages;
using Eclipse.Domain.Shared.Entities;

using Microsoft.EntityFrameworkCore.Diagnostics;

using Newtonsoft.Json;

namespace Eclipse.DataAccess.Interceptors;

internal sealed class DomainEventsToOutboxMessagesInterceptor : SaveChangesInterceptor
{
    private readonly ITimeProvider _timeProvider;

    public DomainEventsToOutboxMessagesInterceptor(ITimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;

        if (context is null)
        {
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        var outboxMessages = context.ChangeTracker
            .Entries<IHasDomainEvents>()
            .Select(entry => entry.Entity)
            .SelectMany(entity => entity.GetEvents())
            .Select(domainEvent => new OutboxMessage(
                Guid.NewGuid(),
                domainEvent.GetType().Name,
                JsonConvert.SerializeObject(domainEvent, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                }),
                _timeProvider.Now)
            )
            .ToList();

        await context.Set<OutboxMessage>().AddRangeAsync(outboxMessages, cancellationToken);

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
