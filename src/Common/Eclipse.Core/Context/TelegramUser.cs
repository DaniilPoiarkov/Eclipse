namespace Eclipse.Core.Context;

[Serializable]
public sealed class TelegramUser
{
    public long Id { get; }

    public string Name { get; }

    public string Surname { get; }

    public string? UserName { get; }

    public TelegramUser(long id, string name, string surname, string? userName)
    {
        Id = id;
        Name = name;
        Surname = surname;
        UserName = userName;
    }
}
