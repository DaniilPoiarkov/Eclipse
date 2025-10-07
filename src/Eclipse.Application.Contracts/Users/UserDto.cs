using Eclipse.Application.Contracts.Entities;
using Eclipse.Application.Contracts.Reminders;
using Eclipse.Application.Contracts.TodoItems;

namespace Eclipse.Application.Contracts.Users;

[Serializable]
public sealed class UserDto : AggregateRootDto
{
    public required string Name { get; init; }
    public required string Surname { get; init; }
    public required string UserName { get; init; }
    public required string Culture { get; init; }

    public long ChatId { get; init; }

    public bool NotificationsEnabled { get; init; }
    public bool IsEnabled { get; init; }

    public TimeSpan Gmt { get; init; }

    public DateTime CreatedAt { get; init; }

    public IReadOnlyList<ReminderDto> Reminders { get; init; } = [];
    public IReadOnlyList<TodoItemDto> TodoItems { get; init; } = [];
}
