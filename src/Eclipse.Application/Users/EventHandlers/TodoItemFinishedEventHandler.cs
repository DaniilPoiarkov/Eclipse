using Eclipse.Domain.Shared.Repositories;
using Eclipse.Domain.Statistics;
using Eclipse.Domain.Users.Events;

using MediatR;

namespace Eclipse.Application.Users.EventHandlers;

internal sealed class TodoItemFinishedEventHandler : INotificationHandler<TodoItemFinishedDomainEvent>
{
    private readonly IRepository<UserStatistics> _repository;

    public TodoItemFinishedEventHandler(IRepository<UserStatistics> repository)
    {
        _repository = repository;
    }

    public async Task Handle(TodoItemFinishedDomainEvent notification, CancellationToken cancellationToken)
    {
        var statistics = (await _repository.GetByExpressionAsync(s => s.UserId == notification.UserId, cancellationToken))
            .FirstOrDefault();

        if (statistics is null)
        {
            statistics = new UserStatistics(Guid.NewGuid(), notification.UserId);
            await _repository.CreateAsync(statistics, cancellationToken);
        }

        statistics.TodoItemFinished();

        await _repository.UpdateAsync(statistics, cancellationToken);
    }
}
