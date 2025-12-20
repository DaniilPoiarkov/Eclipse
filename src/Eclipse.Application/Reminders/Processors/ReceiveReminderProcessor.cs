using Eclipse.Domain.Reminders;
using Eclipse.Domain.Users;

namespace Eclipse.Application.Reminders.Processors;

internal sealed class ReceiveReminderProcessor : IReminderProcessor
{
    private readonly Guid _reminderId;

    public ReceiveReminderProcessor(Guid reminderId)
    {
        _reminderId = reminderId;
    }

    public Reminder? Process(User user)
    {
        return user.ReceiveReminder(_reminderId);
    }
}
