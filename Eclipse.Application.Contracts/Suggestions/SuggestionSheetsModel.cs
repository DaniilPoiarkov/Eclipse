using Eclipse.Application.Contracts.Entities;

namespace Eclipse.Application.Contracts.Suggestions;

[Serializable]
public sealed class SuggestionSheetsModel : EntityDto
{
    public string Text { get; set; } = string.Empty;

    public long TelegramUserId { get; set; }

    public DateTime CreatedAt { get; set; }
}
