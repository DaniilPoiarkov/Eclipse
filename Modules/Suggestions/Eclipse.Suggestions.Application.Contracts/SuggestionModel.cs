using Eclipse.Application.Entities;

namespace Eclipse.Suggestions.Application.Contracts;

[Serializable]
public sealed class SuggestionModel : AggregateRootDto
{
    public string Text { get; set; } = string.Empty;

    public long ChatId { get; set; }

    public DateTime CreatedAt { get; set; }
}
