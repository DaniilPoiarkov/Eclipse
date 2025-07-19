namespace Eclipse.Application.Contracts.Users;

[Serializable]
public sealed class UserPartialUpdateDto
{
    public bool NameChanged { get; init; }
    public string? Name { get; init; }

    public bool SurnameChanged { get; init; }
    public string? Surname { get; init; }

    public bool UserNameChanged { get; init; }
    public string? UserName { get; init; }

    public bool CultureChanged { get; init; }
    public string? Culture { get; init; }

    public bool NotificationsEnabledChanged { get; init; }
    public bool NotificationsEnabled { get; init; }

    public bool IsEnabledChanged { get; init; }
    public bool IsEnabled { get; init; }

    public bool GmtChanged { get; init; }
    public TimeOnly Gmt { get; init; }
}
