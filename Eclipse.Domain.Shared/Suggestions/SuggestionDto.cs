using Eclipse.Domain.Shared.Base;

namespace Eclipse.Domain.Shared.Suggestions;

public class SuggestionDto : EntityDto
{
    public string Text { get; set; } = string.Empty;

    public long ChatId { get; set; }

    public DateTime CreatedAt { get; set; }
}
