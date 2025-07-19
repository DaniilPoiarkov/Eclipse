using Eclipse.Application.Contracts.Reminders;
using Eclipse.Domain.Reminders;

namespace Eclipse.Application.Reminders;

internal static class ReminderExtensions
{
    public static ReminderDto ToDto(this Reminder reminder)
    {
        return new ReminderDto
        {
            Id = reminder.Id,
            UserId = reminder.UserId,
            Text = reminder.Text,
            NotifyAt = reminder.NotifyAt
        };
    }
}
