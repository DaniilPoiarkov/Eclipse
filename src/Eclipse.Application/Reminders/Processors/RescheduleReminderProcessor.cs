using Eclipse.Application.Contracts.Reminders;
using Eclipse.Domain.Reminders;
using Eclipse.Domain.Users;

namespace Eclipse.Application.Reminders.Processors;

internal sealed class RescheduleReminderProcessor : IReminderProcessor
{
    private readonly Guid _reminderId;

    private readonly RescheduleReminderOptions _options;

    public RescheduleReminderProcessor(Guid reminderId, RescheduleReminderOptions options)
    {
        _reminderId = reminderId;
        _options = options;
    }

    public Reminder? Process(User user)
    {
        return user.RescheduleReminder(_reminderId, _options.NotifyAt);
    }
}
