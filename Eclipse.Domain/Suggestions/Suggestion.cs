using Eclipse.Domain.Base;

namespace Eclipse.Domain.Suggestions;

public class Suggestion : Entity
{
    public Suggestion(Guid id, string text, long chatId) : base(id)
    {
        Text = text;
        ChatId = chatId;
        CreatedAt = DateTime.UtcNow;
    }

    public Suggestion() { }

    public string Text { get; private set; } = string.Empty;

    public long ChatId { get; private set; }

    public DateTime CreatedAt { get; private set; }
}
