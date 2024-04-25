using Eclipse.Common.EventBus;
using Eclipse.Domain.Shared.Entities;

using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Eclipse.DataAccess.Interceptors;

internal sealed class TriggerDomainEventsInterceptor : SaveChangesInterceptor
{
    private readonly IEventBus _eventBus;

    public TriggerDomainEventsInterceptor(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;

        if (context is null)
        {
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        var tasks = context.ChangeTracker
            .Entries<IHasDomainEvents>()
            .Select(entry => entry.Entity)
            .SelectMany(entity => entity.GetEvents())
            .Select(async domainEvent => await _eventBus.Publish(domainEvent, cancellationToken));

        await Task.WhenAll(tasks);

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
