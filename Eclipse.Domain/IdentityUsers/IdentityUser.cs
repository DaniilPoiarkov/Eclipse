using Eclipse.Domain.Shared.Entities;

using Newtonsoft.Json;

namespace Eclipse.Domain.IdentityUsers;

public class IdentityUser : Entity
{
    [JsonConstructor]
    internal IdentityUser(Guid id, string name, string surname, string username, long chatId, string culture, bool notificationsEnabled)
        : base(id)
    {
        Name = name;
        Surname = surname;
        Username = username;
        ChatId = chatId;
        Culture = culture;
        NotificationsEnabled = notificationsEnabled;
    }

    public string Name { get; set; }

    public string Surname { get; set; }

    public string Username { get; internal set; }

    public long ChatId { get; init; }

    public string Culture { get; private set; }

    public bool NotificationsEnabled { get; private set; }

    public void SetCulture(string culture) => Culture = culture;

    public void SwitchNotifications(bool notificationsEnabled) => NotificationsEnabled = notificationsEnabled;
}
