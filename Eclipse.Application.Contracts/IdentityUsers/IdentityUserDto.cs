using Eclipse.Application.Contracts.Entities;
using Eclipse.Application.Contracts.Reminders;
using Eclipse.Application.Contracts.TodoItems;

namespace Eclipse.Application.Contracts.IdentityUsers;

public class IdentityUserDto : AggregateRootDto
{
    public string Name { get; set; } = string.Empty;

    public string Surname { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public long ChatId { get; set; }

    public string Culture { get; set; } = string.Empty;

    public bool NotificationsEnabled { get; set; }

    public TimeSpan Gmt {  get; set; }

    public IReadOnlyList<ReminderDto> Reminders { get; set; } = new List<ReminderDto>();

    public IReadOnlyList<TodoItemDto> TodoItems { get; set; } = new List<TodoItemDto>();
}
