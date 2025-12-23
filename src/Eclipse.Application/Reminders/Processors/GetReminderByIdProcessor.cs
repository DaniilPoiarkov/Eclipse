using Eclipse.Domain.Reminders;
using Eclipse.Domain.Users;

namespace Eclipse.Application.Reminders.Processors;

internal sealed class GetReminderByIdProcessor : IReminderProcessor
{
    private readonly Guid _reminderId;

    public GetReminderByIdProcessor(Guid reminderId)
    {
        _reminderId = reminderId;
    }

    public Reminder? Process(User user)
    {
        return user.Reminders.FirstOrDefault(r => r.Id == _reminderId);
    }
}
