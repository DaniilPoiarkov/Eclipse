using Eclipse.Domain.Shared.Entities;

namespace Eclipse.Domain.Reminders;

public sealed class Reminder : AggregateRoot
{
    public Guid UserId { get; private set; }

    public Guid? RelatedItemId { get; private set; }

    public string Text { get; private set; } = string.Empty;

    public TimeOnly NotifyAt { get; private set; }

    internal Reminder(Guid id, Guid userId, Guid? relatedItemId, string text, TimeOnly notifyAt) : base(id)
    {
        UserId = userId;
        RelatedItemId = relatedItemId;
        Text = text;
        NotifyAt = notifyAt;
    }

    private Reminder() { }

    public void Link(Guid relatedItemId)
    {
        RelatedItemId = relatedItemId;
    }

    public void Reschedule(TimeOnly notifyAt)
    {
        NotifyAt = notifyAt;
        AddEvent(new ReminderRescheduledDomainEvent(UserId, Id, notifyAt));
    }
}
