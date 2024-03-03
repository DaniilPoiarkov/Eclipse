namespace Eclipse.Suggestions.IntegrationEvents;

[Serializable]
public sealed record NewSuggestionSentIntegrationEvent
{
    public Guid SuggestionId { get; }

    public long ChatId { get; }

    public string Text { get; }

    public NewSuggestionSentIntegrationEvent(Guid suggestionId, long chatId, string text)
    {
        SuggestionId = suggestionId;
        ChatId = chatId;
        Text = text;
    }
}
