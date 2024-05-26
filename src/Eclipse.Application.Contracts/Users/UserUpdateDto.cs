namespace Eclipse.Application.Contracts.Users;

[Serializable]
public sealed class UserUpdateDto
{
    public string? Name { get; set; } = null;

    public string? Surname { get; set; } = null;

    public string? UserName { get; set; } = null;

    public long? ChatId { get; init; } = null;

    public string? Culture { get; set; } = null;

    public bool? NotificationsEnabled { get; set; } = null;
}
