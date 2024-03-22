namespace Eclipse.Application.Contracts.Suggestions;

[Serializable]
public sealed class CreateSuggestionRequest
{
    public string Text { get; set; } = string.Empty;

    public long TelegramUserId { get; set; }
}
