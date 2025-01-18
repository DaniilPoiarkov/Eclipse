using Eclipse.Application.Contracts.Users;
using Eclipse.Application.Reminders;
using Eclipse.Application.TodoItems;
using Eclipse.Domain.Users;

namespace Eclipse.Application.Users;

internal static class UserExtensions
{
    public static UserDto ToDto(this User value)
    {
        return new UserDto
        {
            Id = value.Id,
            Name = value.Name,
            Surname = value.Surname,
            UserName = value.UserName,
            ChatId = value.ChatId,
            Culture = value.Culture,
            NotificationsEnabled = value.NotificationsEnabled,
            Gmt = value.Gmt,
            CreatedAt = value.CreatedAt,

            Reminders = value.Reminders
                .Select(r => r.ToDto())
                .ToList(),

            TodoItems = value.TodoItems
                .Select(i => i.ToDto())
                .ToList(),
        };
    }

    public static UserSlimDto ToSlimDto(this User value)
    {
        return new UserSlimDto
        {
            Id = value.Id,
            Name = value.Name,
            Surname = value.Surname,
            UserName = value.UserName,
            ChatId = value.ChatId,
            Culture = value.Culture,
            NotificationsEnabled = value.NotificationsEnabled,
            Gmt = value.Gmt,
            CreatedAt = value.CreatedAt,
        };
    }
}
