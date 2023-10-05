using Eclipse.Application.Contracts.Base;
using Eclipse.Application.Contracts.Reminders;
using Eclipse.Domain.Reminders;

namespace Eclipse.Application.Reminders;

public class ReminderMapper : IMapper<Reminder, ReminderDto>
{
    public ReminderDto Map(Reminder value)
    {
        return new ReminderDto
        {
            Id = value.Id,
            Text = value.Text,
            NotifyAt = value.NotifyAt,
            UserId = value.UserId,
        };
    }
}
