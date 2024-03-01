using Eclipse.Domain.Shared.Events;

namespace Eclipse.Domain.Suggestions;

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
