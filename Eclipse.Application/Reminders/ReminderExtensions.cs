using Eclipse.Application.Contracts.Reminders;
using Eclipse.Domain.Reminders;

namespace Eclipse.Application.Reminders;

internal static class ReminderExtensions
{
    public static ReminderDto ToDto(this Reminder reminer)
    {
        return new ReminderDto
        {
            Id = reminer.Id,
            UserId = reminer.UserId,
            Text = reminer.Text,
            NotifyAt = reminer.NotifyAt
        };
    }
}
