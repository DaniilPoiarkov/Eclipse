using Eclipse.Application.Contracts.Base;
using Eclipse.Core.Models;

namespace Eclipse.Application.Contracts.Suggestions;

public class SuggestionAndUserDto : EntityDto
{
    public string Text { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public TelegramUser? User { get; set; }
}
