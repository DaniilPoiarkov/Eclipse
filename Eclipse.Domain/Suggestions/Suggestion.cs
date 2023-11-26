using Eclipse.Domain.Shared.Entities;

namespace Eclipse.Domain.Suggestions;

public class Suggestion : Entity
{
    public Suggestion(Guid id, string text, long telegramUserId) : base(id)
    {
        Text = text;
        TelegramUserId = telegramUserId;
        CreatedAt = DateTime.UtcNow;
    }

    private Suggestion() { }

    public string Text { get; private set; } = string.Empty;

    public long TelegramUserId { get; private set; }

    public DateTime CreatedAt { get; private set; }
}
