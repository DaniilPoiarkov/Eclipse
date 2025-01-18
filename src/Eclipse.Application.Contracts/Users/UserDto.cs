using Eclipse.Application.Contracts.Entities;
using Eclipse.Application.Contracts.Reminders;
using Eclipse.Application.Contracts.TodoItems;

namespace Eclipse.Application.Contracts.Users;

[Serializable]
public sealed class UserDto : AggregateRootDto
{
    public string Name { get; init; } = string.Empty;

    public string Surname { get; init; } = string.Empty;

    public string UserName { get; init; } = string.Empty;

    public long ChatId { get; init; }

    public string Culture { get; init; } = string.Empty;

    public bool NotificationsEnabled { get; init; }

    public TimeSpan Gmt { get; init; }

    public DateTime CreatedAt { get; init; }

    public IReadOnlyList<ReminderDto> Reminders { get; init; } = [];

    public IReadOnlyList<TodoItemDto> TodoItems { get; init; } = [];
}
