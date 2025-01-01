using Eclipse.Common.Events;
using Eclipse.Domain.Statistics;
using Eclipse.Domain.Users.Events;

namespace Eclipse.Application.Statistics.EventHandlers;

internal sealed class TodoItemFinishedEventHandler : IEventHandler<TodoItemFinishedDomainEvent>
{
    private readonly IUserStatisticsRepository _repository;

    public TodoItemFinishedEventHandler(IUserStatisticsRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(TodoItemFinishedDomainEvent notification, CancellationToken cancellationToken)
    {
        var statistics = await _repository.FindByUserIdAsync(notification.UserId, cancellationToken)
            ?? await _repository.CreateAsync(new UserStatistics(Guid.NewGuid(), notification.UserId), cancellationToken);

        statistics.TodoItemFinished();

        await _repository.UpdateAsync(statistics, cancellationToken);
    }
}
