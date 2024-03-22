namespace Eclipse.Core.Models;

[Serializable]
public sealed class TelegramUser
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Surname { get; set; } = string.Empty;

    public string? Username { get; set; } = string.Empty;

    public TelegramUser(long id, string name, string surname, string username)
    {
        Id = id;
        Name = name;
        Surname = surname;
        Username = username;
    }

    public TelegramUser() { }
}
