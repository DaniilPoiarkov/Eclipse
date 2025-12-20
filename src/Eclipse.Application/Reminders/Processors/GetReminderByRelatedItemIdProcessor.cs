using Eclipse.Domain.Reminders;
using Eclipse.Domain.Users;

namespace Eclipse.Application.Reminders.Processors;

internal sealed class GetReminderByRelatedItemIdProcessor : IReminderProcessor
{
    private readonly Guid _relatedItemId;

    public GetReminderByRelatedItemIdProcessor(Guid relatedItemId)
    {
        _relatedItemId = relatedItemId;
    }

    public Reminder? Process(User user)
    {
        return user.Reminders.FirstOrDefault(r => r.RelatedItemId == _relatedItemId);
    }
}
