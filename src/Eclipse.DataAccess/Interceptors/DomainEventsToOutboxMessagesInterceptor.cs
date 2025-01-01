using Eclipse.Common.Caching;
using Eclipse.Common.Clock;
using Eclipse.Domain.OutboxMessages;
using Eclipse.Domain.Shared.Entities;

using Microsoft.EntityFrameworkCore.Diagnostics;

using Newtonsoft.Json;

namespace Eclipse.DataAccess.Interceptors;

internal sealed class DomainEventsToOutboxMessagesInterceptor : SaveChangesInterceptor
{
    private readonly ITimeProvider _timeProvider;

    private readonly ICacheService _cacheService;

    public DomainEventsToOutboxMessagesInterceptor(ITimeProvider timeProvider, ICacheService cacheService)
    {
        _timeProvider = timeProvider;
        _cacheService = cacheService;
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
                domainEvent.GetType().AssemblyQualifiedName!,
                JsonConvert.SerializeObject(domainEvent, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                }),
                _timeProvider.Now)
            )
            .ToList();

        await context.Set<OutboxMessage>().AddRangeAsync(outboxMessages, cancellationToken);

        await _cacheService.DeleteByPrefixAsync(
            typeof(OutboxMessage).AssemblyQualifiedName ?? string.Empty,
            cancellationToken
        );

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
