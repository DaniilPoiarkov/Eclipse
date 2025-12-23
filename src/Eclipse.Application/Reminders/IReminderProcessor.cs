using Eclipse.Domain.Reminders;
using Eclipse.Domain.Users;

namespace Eclipse.Application.Reminders;

internal interface IReminderProcessor
{
    Reminder? Process(User user);
}
