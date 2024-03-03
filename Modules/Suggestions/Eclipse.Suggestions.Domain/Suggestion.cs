using Eclipse.Domain.Entities;

namespace Eclipse.Suggestions.Domain;

public sealed class Suggestion : AggregateRoot
{
    internal Suggestion(Guid id, string text, long chatId, DateTime createdAt) : base(id)
    {
        Text = text;
        ChatId = chatId;
        CreatedAt = createdAt;
    }

    private Suggestion() { }

    public string Text { get; private set; } = string.Empty;

    public long ChatId { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public static Suggestion Create(Guid id, string text, long telegramUserId, DateTime createdAt)
    {
        var suggestion = new Suggestion(id, text, telegramUserId, createdAt);
        suggestion.AddEvent(new NewSuggestionSentDomainEvent(id, telegramUserId, text));
        return suggestion;
    }
}
