namespace Eclipse.Domain.Users;

public sealed class CreateUserRequest
{
    public Guid Id { get; set; }

    public string? Name { get; set; }
    public string Surname { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;

    public long ChatId { get; set; }

    public TimeSpan Gmt { get; set; }

    public bool NotificationsEnabled { get; set; }
    public bool NewRegistered { get; set; }
}
