namespace Eclipse.Core.Models;

public class TelegramUser
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Username { get; set; } = string.Empty;

    public TelegramUser(long id, string name, string username)
    {
        Id = id;
        Name = name;
        Username = username;
    }
}
