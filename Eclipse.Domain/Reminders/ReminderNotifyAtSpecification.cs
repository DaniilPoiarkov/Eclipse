using Eclipse.Common.Specifications;

using System.Linq.Expressions;

namespace Eclipse.Domain.Reminders;

public class ReminderNotifyAtSpecification : Specification<Reminder>
{
    private readonly TimeOnly _time;

    public ReminderNotifyAtSpecification(TimeOnly time)
    {
        _time = time;
    }

    public override Expression<Func<Reminder, bool>> IsSatisfied()
    {
        return reminder => reminder.NotifyAt == _time;
    }
}
