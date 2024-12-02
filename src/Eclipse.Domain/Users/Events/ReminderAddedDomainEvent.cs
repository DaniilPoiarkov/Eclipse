using Eclipse.Common.Events;

namespace Eclipse.Domain.Users.Events;

public sealed class ReminderAddedDomainEvent : IDomainEvent
{
    public Guid ReminderId { get; }
    public Guid UserId { get; }

    public TimeSpan UserGmt { get; }
    public TimeOnly NotifyAt { get; }

    public string Text { get; }
    public string Culture { get; }

    public long ChatId { get; }

    public ReminderAddedDomainEvent(Guid reminderId, Guid userId, TimeSpan userGmt, TimeOnly notifyAt, string text, string culture, long chatId)
    {
        ReminderId = reminderId;
        UserId = userId;
        UserGmt = userGmt;
        NotifyAt = notifyAt;
        Text = text;
        Culture = culture;
        ChatId = chatId;
    }
}
