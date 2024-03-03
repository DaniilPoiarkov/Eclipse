namespace Eclipse.Suggestions.Application.Contracts;

[Serializable]
public sealed class CreateSuggestionRequest
{
    public string Text { get; set; } = string.Empty;

    public long ChatId { get; set; }
}
