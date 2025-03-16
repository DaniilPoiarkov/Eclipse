using Eclipse.Common.Events;
using Eclipse.Domain.Statistics;
using Eclipse.Domain.Users.Events;

namespace Eclipse.Application.Statistics.ReminderReceived;

internal sealed class RemindersReceivedEventHandler : IEventHandler<RemindersReceivedDomainEvent>
{
    private readonly IUserStatisticsRepository _repository;

    public RemindersReceivedEventHandler(IUserStatisticsRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(RemindersReceivedDomainEvent notification, CancellationToken cancellationToken)
    {
        var statistics = await _repository.FindByUserIdAsync(notification.UserId, cancellationToken)
            ?? await _repository.CreateAsync(new UserStatistics(Guid.CreateVersion7(), notification.UserId), cancellationToken);

        statistics.ReminderReceived();

        await _repository.UpdateAsync(statistics, cancellationToken);
    }
}
