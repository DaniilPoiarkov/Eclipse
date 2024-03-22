namespace Eclipse.Application.Contracts.IdentityUsers;

[Serializable]
public sealed class IdentityUserUpdateDto
{
    public string? Name { get; set; } = null;

    public string? Surname { get; set; } = null;

    public string? Username { get; set; } = null;

    public long? ChatId { get; init; } = null;

    public string? Culture { get; set; } = null;

    public bool? NotificationsEnabled { get; set; } = null;
}
