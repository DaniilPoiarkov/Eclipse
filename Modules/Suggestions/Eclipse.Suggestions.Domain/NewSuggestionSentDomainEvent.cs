using Eclipse.Domain.Events;

namespace Eclipse.Suggestions.Domain;

[Serializable]
public sealed record NewSuggestionSentDomainEvent : IDomainEvent
{
    public Guid SuggestionId { get; }

    public long ChatId { get; }

    public string Text { get; }

    internal NewSuggestionSentDomainEvent(Guid suggestionId, long chatId, string text)
    {
        SuggestionId = suggestionId;
        ChatId = chatId;
        Text = text;
    }
}
