namespace Eclipse.Domain.Shared.IdentityUsers;

public class IdentityUserCreateDto
{
    public string Name { get; set; } = string.Empty;

    public string Surname { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public long ChatId { get; init; }

    public string Culture { get; set; } = string.Empty;

    public bool NotificationsEnabled { get; set; }
}
