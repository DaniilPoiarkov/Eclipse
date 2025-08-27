using Eclipse.Domain.Shared.Entities;

namespace Eclipse.Domain.Suggestions;

public sealed class Suggestion : AggregateRoot, IHasCreatedAt
{
    internal Suggestion(Guid id, string text, long telegramUserId, DateTime createdAt) : base(id)
    {
        Text = text;
        TelegramUserId = telegramUserId;
        CreatedAt = createdAt;
    }

    private Suggestion() { }

    public string Text { get; private set; } = string.Empty;

    public long TelegramUserId { get; private set; }

    public DateTime CreatedAt { get; init; }

    public static Suggestion Create(Guid id, string text, long telegramUserId, DateTime createdAt)
    {
        var suggestion = new Suggestion(id, text, telegramUserId, createdAt);
        suggestion.AddEvent(new NewSuggestionSentDomainEvent(id, telegramUserId, text));
        return suggestion;
    }
}
