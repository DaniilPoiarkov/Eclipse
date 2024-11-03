using Eclipse.Domain.Statistics;
using Eclipse.Domain.Users.Events;

using MediatR;

namespace Eclipse.Application.Statistics.EventHandlers;

internal sealed class RemindersReceivedEventHandler : INotificationHandler<RemindersReceivedDomainEvent>
{
    private readonly IUserStatisticsRepository _repository;

    public RemindersReceivedEventHandler(IUserStatisticsRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(RemindersReceivedDomainEvent notification, CancellationToken cancellationToken)
    {
        var statistics = await _repository.FindByUserIdAsync(notification.UserId, cancellationToken)
            ?? await _repository.CreateAsync(new UserStatistics(Guid.NewGuid(), notification.UserId), cancellationToken);

        statistics.ReminderReceived(notification.Count);

        await _repository.UpdateAsync(statistics, cancellationToken);
    }
}
