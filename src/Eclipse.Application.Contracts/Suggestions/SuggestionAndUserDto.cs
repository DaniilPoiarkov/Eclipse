using Eclipse.Application.Contracts.Entities;
using Eclipse.Application.Contracts.Users;

namespace Eclipse.Application.Contracts.Suggestions;

[Serializable]
public sealed class SuggestionAndUserDto : EntityDto
{
    public string Text { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public UserSlimDto? User { get; set; }
}
