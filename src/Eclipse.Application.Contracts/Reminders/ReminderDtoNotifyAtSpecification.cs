using Eclipse.Common.Specifications;

using System.Linq.Expressions;

namespace Eclipse.Application.Contracts.Reminders;

/// <summary>
/// Filtration via NotifyAt property
/// </summary>
public class ReminderDtoNotifyAtSpecification : Specification<ReminderDto>
{
    private readonly TimeOnly _time;

    public ReminderDtoNotifyAtSpecification(TimeOnly time)
    {
        _time = time;
    }

    public override Expression<Func<ReminderDto, bool>> IsSatisfied()
    {
        return r => r.NotifyAt == _time;
    }
}
