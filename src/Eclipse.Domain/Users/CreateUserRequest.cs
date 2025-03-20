namespace Eclipse.Domain.Users;

public sealed class CreateUserRequest
{
    public Guid Id { get; init; }

    public string? Name { get; init; }
    public string Surname { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;

    public long ChatId { get; init; }

    public TimeSpan Gmt { get; init; }

    public bool NotificationsEnabled { get; init; }
    public bool NewRegistered { get; init; }
    public required bool IsEnabled { get; init; }
}
