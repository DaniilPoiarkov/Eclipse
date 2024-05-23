namespace Eclipse.Application.Contracts.Users;

[Serializable]
public sealed class UserCreateDto
{
    public string Name { get; set; } = string.Empty;

    public string Surname { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public long ChatId { get; init; }

    public string Culture { get; set; } = string.Empty;

    public bool NotificationsEnabled { get; set; }
}
