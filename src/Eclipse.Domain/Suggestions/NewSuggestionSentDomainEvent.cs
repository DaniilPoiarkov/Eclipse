using Eclipse.Common.Events;

namespace Eclipse.Domain.Suggestions;

[Serializable]
public sealed record NewSuggestionSentDomainEvent : IDomainEvent
{
    public Guid SuggestionId { get; }

    public long ChatId { get; }

    public string Text { get; }

    public NewSuggestionSentDomainEvent(Guid suggestionId, long chatId, string text)
    {
        SuggestionId = suggestionId;
        ChatId = chatId;
        Text = text;
    }
}
