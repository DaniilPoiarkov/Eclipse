namespace Eclipse.Application.Contracts.Users;

[Serializable]
public sealed class UserPartialUpdateDto
{
    public bool NameChanged { get; set; }
    public string? Name { get; set; }

    public bool SurnameChanged { get; set; }
    public string? Surname { get; set; }

    public bool UserNameChanged { get; set; }
    public string? UserName { get; set; }

    public bool CultureChanged { get; set; }
    public string? Culture { get; set; }

    public bool NotificationsEnabledChanged { get; set; }
    public bool NotificationsEnabled { get; set; }
}
