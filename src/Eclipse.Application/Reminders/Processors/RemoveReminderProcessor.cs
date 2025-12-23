using Eclipse.Domain.Reminders;
using Eclipse.Domain.Users;

namespace Eclipse.Application.Reminders.Processors;

internal sealed class RemoveReminderProcessor : IReminderProcessor
{
    private readonly Guid _reminderId;

    public RemoveReminderProcessor(Guid reminderId)
    {
        _reminderId = reminderId;
    }

    public Reminder? Process(User user)
    {
        return user.RemoveReminder(_reminderId);
    }
}
