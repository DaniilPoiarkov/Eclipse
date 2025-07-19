using Eclipse.Application.Contracts.Entities;

namespace Eclipse.Application.Contracts.Users;

[Serializable]
public sealed class UserSlimDto : AggregateRootDto
{
    public string Name { get; init; } = string.Empty;

    public string Surname { get; init; } = string.Empty;

    public string UserName { get; init; } = string.Empty;

    public long ChatId { get; init; }

    public string Culture { get; init; } = string.Empty;

    public bool NotificationsEnabled { get; init; }

    public bool IsEnabled { get; init; }

    public TimeSpan Gmt { get; init; }

    public DateTime? CreatedAt { get; init; }
}
