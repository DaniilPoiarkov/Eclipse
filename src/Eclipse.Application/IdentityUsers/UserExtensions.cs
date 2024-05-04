using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Application.Reminders;
using Eclipse.Application.TodoItems;
using Eclipse.Domain.IdentityUsers;

namespace Eclipse.Application.IdentityUsers;

internal static class UserExtensions
{
    public static IdentityUserDto ToDto(this IdentityUser value)
    {
        return new IdentityUserDto
        {
            Id = value.Id,
            Name = value.Name,
            Surname = value.Surname,
            UserName = value.UserName,
            ChatId = value.ChatId,
            Culture = value.Culture,
            NotificationsEnabled = value.NotificationsEnabled,
            Gmt = value.Gmt,

            Reminders = value.Reminders
                .Select(r => r.ToDto())
                .ToList(),

            TodoItems = value.TodoItems
                .Select(i => i.ToDto())
                .ToList(),
        };
    }

    public static IdentityUserSlimDto ToSlimDto(this IdentityUser value)
    {
        return new IdentityUserSlimDto
        {
            Id = value.Id,
            Name = value.Name,
            Surname = value.Surname,
            Username = value.UserName,
            ChatId = value.ChatId,
            Culture = value.Culture,
            NotificationsEnabled = value.NotificationsEnabled
        };
    }
}
