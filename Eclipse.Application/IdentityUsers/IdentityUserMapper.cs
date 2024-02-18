using Eclipse.Application.Contracts.Base;
using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Application.Contracts.Reminders;
using Eclipse.Application.Contracts.TodoItems;
using Eclipse.Domain.IdentityUsers;

namespace Eclipse.Application.IdentityUsers;

public sealed class IdentityUserMapper : IMapper<IdentityUser, IdentityUserDto>
{
    public IdentityUserDto Map(IdentityUser value)
    {
        return new IdentityUserDto
        {
            Id = value.Id,
            Name = value.Name,
            Surname = value.Surname,
            Username = value.Username,
            ChatId = value.ChatId,
            Culture = value.Culture,
            NotificationsEnabled = value.NotificationsEnabled,
            Gmt = value.Gmt,

            Reminders = value.Reminders
                .Select(r => new ReminderDto
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    Text = r.Text,
                    NotifyAt = r.NotifyAt
                })
                .ToList(),

            TodoItems = value.TodoItems
                .Select(i => new TodoItemDto
                {
                    Id = i.Id,
                    UserId = i.UserId,
                    Text = i.Text,
                    CreatedAt = i.CreatedAt,
                    IsFinished = i.IsFinished,
                    FinishedAt = i.FinishedAt,
                })
                .ToList(),
        };
    }
}
